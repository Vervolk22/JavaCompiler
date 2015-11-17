using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxAnalysis;
using SyntaxAnalysis.PH.DataTree;
using System.Reflection;
using System.Reflection.Emit;

namespace JavaToILGenerator
{
    class GeneratorMethods
    {
        AssemblyBuilder ab;
        DTreeNode<string> node;
        TypeBuilder tBuilder;

        public GeneratorMethods(AssemblyBuilder ab, DTreeNode<string> node, TypeBuilder tBuilder)
        {
            this.ab = ab;
            this.node = node;
            this.tBuilder = tBuilder;
        }

        public void generate()
        {
            int num = 0;
            for (num = 0; num < node.Nodes.Count; num++)
            {
                if (node.Nodes[num].Value == "declares:args") break;
            }
            Type[] paramTypes = getArguments(node.Nodes[num]);
            Type returnType = getRetType(node, num - 1);
            int attrNum = (LexemTypeHelper.getParsedValue(node.Nodes[num - 1].Value) 
                    == "[]") ? num - 3 : num - 2;
            MethodAttributes attr = getAttributes(node, attrNum);
            MethodBuilder mBuilder = tBuilder.DefineMethod(
                    LexemTypeHelper.getParsedValue(node.Value), attr, returnType, 
                    paramTypes);


        }

        protected Type[] getArguments(DTreeNode<string> node)
        {
            Console.WriteLine("Arguments:");

            Type[] array = new Type[node.Nodes.Count];
            for (int i = 0; i < node.Nodes.Count; i++) 
            {
                array[i] = getArgType(node.Nodes[i]);
            }
            return array;
        }

        protected Type getRetType(DTreeNode<string> node, int num)
        {
            Type type;
            if (LexemTypeHelper.getParsedValue(node.Nodes[num].Value) == "[]")
            {
                switch (LexemTypeHelper.getParsedValue(node.Nodes[num - 1].Value))
                {
                    case "int": type = typeof(int[]); break;
                    case "double": type = typeof(int[]); break;
                    case "String": type = typeof(int[]); break;
                    default: type = null; break;
                }
            }
            else
            {
                switch (LexemTypeHelper.getParsedValue(node.Nodes[num].Value))
                {
                    case "void": type = typeof(void); break;
                    case "int": type = typeof(int); break;
                    case "double": type = typeof(double); break;
                    case "String": type = typeof(String); break;
                    default: type = null; break;
                }
            }
            return type;
        }

        protected MethodAttributes getAttributes(DTreeNode<string> node, int endPos)
        {
            MethodAttributes attr = 0;
            for (int i = 0; i <= endPos; i++)
            {
                switch (LexemTypeHelper.getParsedValue(node.Nodes[i].Value))
                {
                    case "public": attr = attr | MethodAttributes.Public; break;
                    case "static": attr = attr | MethodAttributes.Static; break;
                    default: break;
                }
            }
            return attr;
        }

        protected Type getArgType(DTreeNode<string> node)
        {
            Type type;
            switch (LexemTypeHelper.getParsedValue(node.Nodes[0].Value))
            {
                case "int":
                    if (node.Nodes.Count == 1) type = typeof(int);
                    else type = typeof(int[]);break;
                case "double":
                    if (node.Nodes.Count == 1) type = typeof(double);
                    else type = typeof(double[]); break;
                case "String":
                    if (node.Nodes.Count == 1) type = typeof(String);
                    else type = typeof(String[]); break;
                default: type = null; break;
            }
            return type;
        }
    }
}
