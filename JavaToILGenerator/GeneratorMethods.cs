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
    /// <summary>
    /// Processes a signle method.
    /// </summary>
    class GeneratorMethods
    {
        AssemblyBuilder ab;
        DTreeNode<string> node;
        TypeBuilder tBuilder;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ab">AssemblyBuilder used in assembly.</param>
        /// <param name="node">First SyntaxTree node of method.</param>
        /// <param name="tBuilder">TypeBuilder instance of parent type.</param>
        public GeneratorMethods(AssemblyBuilder ab, DTreeNode<string> node, TypeBuilder tBuilder)
        {
            this.ab = ab;
            this.node = node;
            this.tBuilder = tBuilder;
        }

        /// <summary>
        /// Launches method generation process.
        /// </summary>
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
            if (LexemTypeHelper.getParsedValue(node.Value) == "main")
            {
                ab.SetEntryPoint(mBuilder);
            }

            ILGenerator ilGenerator = mBuilder.GetILGenerator();
            new GeneratorCode(node.Nodes[node.Nodes.Count - 1], ilGenerator).generate(false);
        }

        /// <summary>
        /// Gets array of arguments of method.
        /// </summary>
        /// <param name="node">SyntaxTree node of first method's argument.</param>
        /// <returns>Array of arguments.</returns>
        protected Type[] getArguments(DTreeNode<string> node)
        {
            Type[] array = new Type[node.Nodes.Count];
            for (int i = 0; i < node.Nodes.Count; i++) 
            {
                array[i] = getArgType(node.Nodes[i]);
            }
            return array;
        }

        /// <summary>
        /// Gets return type of the method.
        /// </summary>
        /// <param name="node">SyntaxTree node of the method's name.</param>
        /// <param name="num">Possible position in SyntaxTree of return type.
        /// It's because return type can be array and consists of 2 nodes.</param>
        /// <returns>Return type of the method.</returns>
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
                type = ILHelper.getILType(LexemTypeHelper.getParsedValue(node.Nodes[num].Value));
            }
            return type;
        }

        /// <summary>
        /// Gets method's attributes.
        /// </summary>
        /// <param name="node">SyntaxTree node of the declaration of attributes.</param>
        /// <param name="endPos">End position of arguments.</param>
        /// <returns>Int-based MethodAttributes value.</returns>
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

        /// <summary>
        /// Gets one of the possible variable types.
        /// </summary>
        /// <param name="node">SyntexTree node of variable name.</param>
        /// <returns></returns>
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
