using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SyntaxAnalysis;
using SyntaxAnalysis.PH.DataTree;
using System.Reflection;
using System.Reflection.Emit;

namespace JavaToILGenerator
{
    class GeneratorCode
    {
        DTreeNode<string> node;
        ILGenerator ilg;
        Dictionary<string, LocalBuilder> locals = new Dictionary<string, LocalBuilder>();
        bool wasError = false;
        Label lastAfterLabel;

        public GeneratorCode(DTreeNode<string> node, ILGenerator ilg)
        {
            this.node = node;
            this.ilg = ilg;
        }

        public void generate(bool isEntry)
        {
            next(node);
            if (isEntry)
            {
                //Console.ReadLine();
                Type type = typeof(Console);
                MethodInfo mi = type.GetMethod("ReadLine");
                ilg.EmitCall(OpCodes.Call, typeof(Console).GetMethod("ReadLine", System.Type.EmptyTypes), null);
                //var methodInfo = type
                ilg.EmitCall(OpCodes.Call, mi, null);
                //ilg.Emit(OpCodes.Starg);
                //ilg.Emit(OpCodes.Stloc);
            }
            
            ilg.EmitWriteLine("TTTT");
            ilg.Emit(OpCodes.Ret);
        }

        protected void next(DTreeNode<string> node)
        {
            int type = 0;
            string value = LexemTypeHelper.parse(ref type, node.Value);
            switch (type)
            {
                case 1: handeOperator(node); break;
                case 2: handleKeyword(node); break;
                case 3: handleIdentifier(node); break;
                default: break;
            }
        }

        protected void handeOperator(DTreeNode<string> node)
        {
            switch (LexemTypeHelper.getParsedValue(node.Value))
            {
                case "=": handleAssignment(node); break;
                /*case "+": handlePlus(node); break;
                case "-": handleMinus(node); break;
                case "*": handleMultiply(node); break;
                case "<": handleLess(node); break;
                case ">": handleBigger(node);break;
                case "==": handleEqual(node); break;
                case "!=": handleNotEqual(node); break;*/
                default: break;
            }
        }

        protected void handleKeyword(DTreeNode<string> node)
        {
            string name = LexemTypeHelper.getParsedValue(node.Value);
            switch (name)
            {
                case "while":
                    handleWhile(node);
                    break;
                case "if":
                    handleIf(node);
                    break;
                case "System":
                    handleSystem(node);
                    break;
            }
        }

        protected void handleIf(DTreeNode<string> node)
        {
            Label tempAfterLabel = lastAfterLabel;
            Label afterLabel = ilg.DefineLabel();
            lastAfterLabel = afterLabel;
            Label ifLabel = ilg.DefineLabel();
            Label elseLabel = ilg.DefineLabel();
            bool isElse = node.Nodes.Count == 4 ? true : false;

            handleComparison(node.Nodes[0], ifLabel, elseLabel, isElse);
            if (node.Nodes.Count > 2)
            {
                ilg.MarkLabel(ifLabel);
                next(node.Nodes[1]);
            }
            if (node.Nodes.Count > 3)
            {
                ilg.MarkLabel(elseLabel);
                next(node.Nodes[2]);
            }
            else
            {
                ilg.MarkLabel(afterLabel);
                next(node.Nodes[2]);
            }
            lastAfterLabel = tempAfterLabel;
        }

        protected void handleWhile(DTreeNode<string> node)
        {
            Label tempAfterLabel = lastAfterLabel;
            Label afterLabel = ilg.DefineLabel();
            lastAfterLabel = afterLabel;
            Label whileLabel = ilg.DefineLabel();
            Label bodyLabel = ilg.DefineLabel();
            bool isElse = node.Nodes.Count == 4 ? true : false;

            ilg.MarkLabel(whileLabel);
            handleComparison(node.Nodes[0], bodyLabel, afterLabel, false);
            ilg.MarkLabel(bodyLabel);
            next(node.Nodes[1]);
            ilg.Emit(OpCodes.Br, whileLabel);
            ilg.MarkLabel(afterLabel);
            next(node.Nodes[2]);
            lastAfterLabel = tempAfterLabel;
        }

        protected void handleComparison(DTreeNode<string> node, Label ifLabel, Label elseLabel, bool isElse)
        {
            getValue(node.Nodes[0]);
            getValue(node.Nodes[1]);
            //ilg.EmitWriteLine(handleIdentifier(node.Nodes[1]));
            //ilg.EmitWriteLine(handleIdentifier(node.Nodes[0]));

            switch (LexemTypeHelper.getParsedValue(node.Value))
            {
                case "==": handleEqual(ifLabel, elseLabel, isElse); break;
                case "!=": handleNotEqual(ifLabel, elseLabel, isElse); break;
                case ">": handleBigger(ifLabel, elseLabel, isElse); break;
                case "<": handleLess(ifLabel, elseLabel, isElse); break;
            }
        }

        protected void handleEqual(Label ifLabel, Label elseLabel, bool isElse)
        {
            ilg.Emit(OpCodes.Beq, ifLabel);
            if
                (isElse) ilg.Emit(OpCodes.Bne_Un, elseLabel);
            else
                ilg.Emit(OpCodes.Br, lastAfterLabel);
        }

        protected void handleNotEqual(Label ifLabel, Label elseLabel, bool isElse)
        {
            ilg.Emit(OpCodes.Bne_Un, ifLabel);
            if (isElse) 
                ilg.Emit(OpCodes.Beq, elseLabel);
            else
                ilg.Emit(OpCodes.Br, lastAfterLabel);
        }

        protected void handleBigger(Label ifLabel, Label elseLabel, bool isElse)
        {
            ilg.Emit(OpCodes.Bgt, ifLabel);
            if 
                (isElse) ilg.Emit(OpCodes.Ble, elseLabel);
            else
                ilg.Emit(OpCodes.Br, lastAfterLabel);
        }

        protected void handleLess(Label ifLabel, Label elseLabel, bool isElse)
        {
            ilg.Emit(OpCodes.Blt, ifLabel);
            if 
                (isElse) ilg.Emit(OpCodes.Bge, elseLabel);
            else
                ilg.Emit(OpCodes.Br, lastAfterLabel);
        }

        protected void handleSystem(DTreeNode<string> node)
        {
            if (node.Nodes.Count > 0)
            {
                handleSystem(node.Nodes[0]);
            }
            else
            {
                ilg.EmitWriteLine(handleIdentifier(node));
            }
            if (LexemTypeHelper.getParsedValue(node.Value) == "System" &&
                node.Nodes.Count > 1)
                next(node.Nodes[1]);
        }

        protected LocalBuilder handleIdentifier(DTreeNode<string> node)
        {
            int type = 0;
            string s = LexemTypeHelper.parse(ref type, node.Value);
            LocalBuilder lBuilder;
            if (!locals.TryGetValue(s, out lBuilder))
            {
                if (node.Nodes.Count == 0)
                {
                    // ERROR!
                }
                lBuilder = ilg.DeclareLocal(ILHelper.getILType(LexemTypeHelper.getParsedValue(node.Nodes[0].Value)));
                locals.Add(s, lBuilder);
            }
            return lBuilder;
            /*switch (type)
            {
                case 2:
                    lBuilder = ilg.DeclareLocal(ILHelper.getILType(s));
                    locals.Add(node.Nodes[0].Value, lBuilder);
                    return lBuilder;
                case 3:
                    locals.TryGetValue(s, out lBuilder);
                    return lBuilder;
                default:
                    return null;
            }*/
        }

        protected void handleAssignment(DTreeNode<string> node)
        {
            int type = 0;
            string name = LexemTypeHelper.parse(ref type, node.Nodes[0].Value);
            getValue(node.Nodes[1]);
            /*if (o.GetType() == typeof(Double))
                ilg.Emit(OpCodes.Ldc_R4, (double)o);
            else if (o.GetType() == typeof(int))
                ilg.Emit(OpCodes.Ldc_I4, (int)o);
            else
                ilg.Emit(OpCodes.Ldloc, (LocalBuilder)o);*/
            ilg.Emit(OpCodes.Stloc, handleIdentifier(node.Nodes[0]));

            if (node.Nodes.Count > 2)
            {
                next(node.Nodes[2]);
            }
        }

        protected void getValue(DTreeNode<string> node)
        {
            switch (LexemTypeHelper.getParsedType(node.Value)) 
            { 
                case 1:
                    switch (LexemTypeHelper.getParsedValue(node.Value))
                    {
                        case "+": handlePlus(node); break;
                        case "-": handleMinus(node); break;
                        case "*": handleMultiply(node); break;
                    }
                    break;
                case 3:
                    getIdentifierValue(node);
                    break;
                case 4:
                    getDoubleValue(node);
                    break;
                case 5:
                    getIntValue(node);
                    break;
            }
        }

        protected void getIntValue(DTreeNode<string> node)
        {
            int value;
            Int32.TryParse(LexemTypeHelper.getParsedValue(node.Value), out value);
            ilg.Emit(OpCodes.Ldc_I4, value);
        }

        protected void getDoubleValue(DTreeNode<string> node)
        {
            double value;
            Double.TryParse(LexemTypeHelper.getParsedValue(node.Value), out value);
            ilg.Emit(OpCodes.Ldc_R4, value);
        }

        protected void getIdentifierValue(DTreeNode<string> node)
        {
            string name = LexemTypeHelper.getParsedValue(node.Value);
            LocalBuilder lBuilder;
            if (locals.TryGetValue(name, out lBuilder))
            {
                ilg.Emit(OpCodes.Ldloc, lBuilder);
            }

            /*if (!locals.TryGetValue(name, out lBuilder))
            {
                lBuilder = ilg.DeclareLocal(ILHelper.getILType(
                        LexemTypeHelper.getParsedValue(node.Nodes[0].Value)));
                locals.Add(name, lBuilder);
            }
            return lBuilder;*/
        }

        protected void handlePlus(DTreeNode<string> node)
        {
            getValue(node.Nodes[0]);
            getValue(node.Nodes[1]);
            ilg.Emit(OpCodes.Add);
        }

        protected void handleMinus(DTreeNode<string> node)
        {
            getValue(node.Nodes[0]);
            getValue(node.Nodes[1]);
            ilg.Emit(OpCodes.Sub);
        }

        protected void handleMultiply(DTreeNode<string> node)
        {
            getValue(node.Nodes[0]);
            getValue(node.Nodes[1]);
            ilg.Emit(OpCodes.Mul);
        }
    }
}
