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
    /// <summary>
    /// Processes code of a single method.
    /// </summary>
    class GeneratorCode
    {
        DTreeNode<string> node;
        ILGenerator ilg;
        // Dictionary of local variables with it's names.
        Dictionary<string, LocalBuilder> locals = new Dictionary<string, LocalBuilder>();
        bool wasError = false;
        // Exit label of the last if statement or while loop.
        Label lastAfterLabel;

        /// <summary>
        /// Constructor of generator.
        /// </summary>
        /// <param name="node">Entry node of the method.</param>
        /// <param name="ilg">ILGenerator, used in the assembly.</param>
        public GeneratorCode(DTreeNode<string> node, ILGenerator ilg)
        {
            this.node = node;
            this.ilg = ilg;
        }

        /// <summary>
        /// Starts the code generation process for the method.
        /// </summary>
        /// <param name="isEntry">Is the method the entry point of assembly.</param>
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

        /// <summary>
        /// Processes the next independent command in the code.
        /// </summary>
        /// <param name="node">SyntaxTree node of the command.</param>
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

        /// <summary>
        /// Handles the operator as the next command.
        /// </summary>
        /// <param name="node">SyntaxTree node of the operator.</param>
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

        /// <summary>
        /// Handles the keyword as the next command.
        /// </summary>
        /// <param name="node">SyntaxTree node of the keyword.</param>
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

        /// <summary>
        /// Handles if expression.
        /// </summary>
        /// <param name="node">SyntaxTree node of the if statement.</param>
        protected void handleIf(DTreeNode<string> node)
        {
            // Prepare needed lables.
            Label tempAfterLabel = lastAfterLabel;
            Label afterLabel = ilg.DefineLabel();
            lastAfterLabel = afterLabel;
            Label ifLabel = ilg.DefineLabel();
            Label elseLabel = ilg.DefineLabel();
            bool isElse = node.Nodes.Count == 4 ? true : false;

            // Make statement comparison, write jumps.
            handleComparison(node.Nodes[0], ifLabel, elseLabel, isElse);
            // Set the label of the main if body.
            if (node.Nodes.Count > 2)
            {
                ilg.MarkLabel(ifLabel);
                next(node.Nodes[1]);
            }
            // Set the label of the else if body.
            if (node.Nodes.Count > 3)
            {
                ilg.MarkLabel(elseLabel);
                next(node.Nodes[2]);
            }
            // Set the exit point of the if statement.
            else
            {
                ilg.MarkLabel(afterLabel);
                next(node.Nodes[2]);
            }
            lastAfterLabel = tempAfterLabel;
        }

        /// <summary>
        /// Handles while loop.
        /// </summary>
        /// <param name="node">SyntaxTree node of the while loop.</param>
        protected void handleWhile(DTreeNode<string> node)
        {
            // Prepare needed labels.
            Label tempAfterLabel = lastAfterLabel;
            Label afterLabel = ilg.DefineLabel();
            lastAfterLabel = afterLabel;
            Label whileLabel = ilg.DefineLabel();
            Label bodyLabel = ilg.DefineLabel();
            bool isElse = node.Nodes.Count == 4 ? true : false;

            // Mark statement label.
            ilg.MarkLabel(whileLabel);
            handleComparison(node.Nodes[0], bodyLabel, afterLabel, false);
            // Mark loop body label.
            ilg.MarkLabel(bodyLabel);
            next(node.Nodes[1]);
            ilg.Emit(OpCodes.Br, whileLabel);
            // Mark loop exit label.
            ilg.MarkLabel(afterLabel);
            next(node.Nodes[2]);
            lastAfterLabel = tempAfterLabel;
        }

        /// <summary>
        /// Gets the values of compare statement into the stack
        /// and compares them.
        /// </summary>
        /// <param name="node">SyntaxTree node of compare operator.</param>
        /// <param name="ifLabel">Label that will be executed, if statement
        /// is true.</param>
        /// <param name="elseLabel">Label that will be executed if statement
        /// is false.</param>
        /// <param name="isElse">Only for if statement. Is there the else 
        /// code branch. Otherwise, shoud be false.</param>
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

        /// <summary>
        /// Handles equals operator.
        /// </summary>
        /// <param name="ifLabel">Label that will be executed, if statement
        /// is true.</param>
        /// <param name="elseLabel">Label that will be executed if statement
        /// is false.</param>
        /// <param name="isElse">Only for if statement. Is there the else 
        /// code branch. Otherwise, shoud be false.</param>
        protected void handleEqual(Label ifLabel, Label elseLabel, bool isElse)
        {
            ilg.Emit(OpCodes.Beq, ifLabel);
            if
                (isElse) ilg.Emit(OpCodes.Bne_Un, elseLabel);
            else
                ilg.Emit(OpCodes.Br, lastAfterLabel);
        }

        /// <summary>
        /// Handles not equal operator.
        /// </summary>
        /// <param name="ifLabel">Label that will be executed, if statement
        /// is true.</param>
        /// <param name="elseLabel">Label that will be executed if statement
        /// is false.</param>
        /// <param name="isElse">Only for if statement. Is there the else 
        /// code branch. Otherwise, shoud be false.</param>
        protected void handleNotEqual(Label ifLabel, Label elseLabel, bool isElse)
        {
            ilg.Emit(OpCodes.Bne_Un, ifLabel);
            if (isElse) 
                ilg.Emit(OpCodes.Beq, elseLabel);
            else
                ilg.Emit(OpCodes.Br, lastAfterLabel);
        }

        /// <summary>
        /// Handles bigger operator.
        /// </summary>
        /// <param name="ifLabel">Label that will be executed, if statement
        /// is true.</param>
        /// <param name="elseLabel">Label that will be executed if statement
        /// is false.</param>
        /// <param name="isElse">Only for if statement. Is there the else 
        /// code branch. Otherwise, shoud be false.</param>
        protected void handleBigger(Label ifLabel, Label elseLabel, bool isElse)
        {
            ilg.Emit(OpCodes.Bgt, ifLabel);
            if 
                (isElse) ilg.Emit(OpCodes.Ble, elseLabel);
            else
                ilg.Emit(OpCodes.Br, lastAfterLabel);
        }

        /// <summary>
        /// Handles less operator.
        /// </summary>
        /// <param name="ifLabel">Label that will be executed, if statement
        /// is true.</param>
        /// <param name="elseLabel"></param>
        /// <param name="isElse">Only for if statement. Is there the else 
        /// code branch. Otherwise, shoud be false.</param>
        protected void handleLess(Label ifLabel, Label elseLabel, bool isElse)
        {
            ilg.Emit(OpCodes.Blt, ifLabel);
            if 
                (isElse) ilg.Emit(OpCodes.Bge, elseLabel);
            else
                ilg.Emit(OpCodes.Br, lastAfterLabel);
        }

        /// <summary>
        /// Handles System keyword. !!!On any System... calls 
        /// System.Console.WriteLine(...);
        /// </summary>
        /// <param name="node">SyntaxTree node of System keyword.</param>
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

        /// <summary>
        /// Handles identifier. Get LocalBuilder or creates it.
        /// </summary>
        /// <param name="node">SyntaxTree node of identifier.</param>
        /// <returns>LocalBUilder instance that represents the identifier.</returns>
        protected LocalBuilder handleIdentifier(DTreeNode<string> node)
        {
            int type = 0;
            string s = LexemTypeHelper.parse(ref type, node.Value);
            LocalBuilder lBuilder;
            if (!locals.TryGetValue(s, out lBuilder))
            {
                if (node.Nodes.Count == 0)
                {
                    // ERROR! TODO
                }
                lBuilder = ilg.DeclareLocal(ILHelper.getILType(LexemTypeHelper.getParsedValue(node.Nodes[0].Value)));
                locals.Add(s, lBuilder);
            }
            return lBuilder;
        }

        /// <summary>
        /// Handles assignment operator.
        /// </summary>
        /// <param name="node">SyntaxTree node of operator.</param>
        protected void handleAssignment(DTreeNode<string> node)
        {
            int type = 0;
            string name = LexemTypeHelper.parse(ref type, node.Nodes[0].Value);
            getValue(node.Nodes[1]);
            ilg.Emit(OpCodes.Stloc, handleIdentifier(node.Nodes[0]));

            if (node.Nodes.Count > 2)
            {
                next(node.Nodes[2]);
            }
        }

        /// <summary>
        /// Gets value after a single mathematic operation or identifier.
        /// </summary>
        /// <param name="node">SyntaxTree node of operation or identifier.</param>
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

        /// <summary>
        /// Gets int value from node.
        /// </summary>
        /// <param name="node">SyntaxTree node.</param>
        protected void getIntValue(DTreeNode<string> node)
        {
            int value;
            Int32.TryParse(LexemTypeHelper.getParsedValue(node.Value), out value);
            ilg.Emit(OpCodes.Ldc_I4, value);
        }

        /// <summary>
        /// Gets double value from node.
        /// </summary>
        /// <param name="node">SyntaxTree node.</param>
        protected void getDoubleValue(DTreeNode<string> node)
        {
            double value;
            Double.TryParse(LexemTypeHelper.getParsedValue(node.Value), out value);
            ilg.Emit(OpCodes.Ldc_R4, value);
        }

        /// <summary>
        /// Gets identifier value.
        /// </summary>
        /// <param name="node">SyntaxTree node of identifier.</param>
        protected void getIdentifierValue(DTreeNode<string> node)
        {
            string name = LexemTypeHelper.getParsedValue(node.Value);
            LocalBuilder lBuilder;
            if (locals.TryGetValue(name, out lBuilder))
            {
                ilg.Emit(OpCodes.Ldloc, lBuilder);
            }
        }

        /// <summary>
        /// Handles the plus operation.
        /// </summary>
        /// <param name="node">SyntaxTree node of plus operation.</param>
        protected void handlePlus(DTreeNode<string> node)
        {
            getValue(node.Nodes[0]);
            getValue(node.Nodes[1]);
            ilg.Emit(OpCodes.Add);
        }

        /// <summary>
        /// Handles the minus operation.
        /// </summary>
        /// <param name="node">SyntaxTree node of minus operation.</param>
        protected void handleMinus(DTreeNode<string> node)
        {
            getValue(node.Nodes[0]);
            getValue(node.Nodes[1]);
            ilg.Emit(OpCodes.Sub);
        }

        /// <summary>
        /// Handles the multiply operation.
        /// </summary>
        /// <param name="node">SyntaxTree node of multiply operation.</param>
        protected void handleMultiply(DTreeNode<string> node)
        {
            getValue(node.Nodes[0]);
            getValue(node.Nodes[1]);
            ilg.Emit(OpCodes.Mul);
        }
    }
}
