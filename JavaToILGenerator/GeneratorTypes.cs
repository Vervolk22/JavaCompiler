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
    /// Generates new type.
    /// </summary>
    class GeneratorTypes
    {
        protected AssemblyBuilder ab;
        DTreeNode<string> node;
        ModuleBuilder mBuilder;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ab">AssemblyBuilder.</param>
        /// <param name="node">Starting SyntaxTree node of a type.</param>
        /// <param name="mBuilder">ModuleBuilder.</param>
        public GeneratorTypes(AssemblyBuilder ab, DTreeNode<string> node, ModuleBuilder mBuilder)
        {
            this.ab = ab;
            this.node = node;
            this.mBuilder = mBuilder;
        }

        /// <summary>
        /// Starts the generation process.
        /// </summary>
        public void generate()
        {
            TypeBuilder tBuilder = mBuilder.DefineType(LexemTypeHelper.getParsedValue(node.Value));
            foreach (DTreeNode<string> childNode in node.Nodes)
            {
                switch (childNode.Value)
                {
                    case "declares:methods": generateMethods(childNode, tBuilder); break;
                    case "declares:fields": generateFields(childNode, tBuilder); break;
                    default: continue;
                }
            }
            tBuilder.CreateType();
        }

        /// <summary>
        /// Calls method generation for every method's declaration.
        /// </summary>
        /// <param name="node">Node that declares methods.</param>
        /// <param name="tBuilder">TypeBuilder of the type.</param>
        protected void generateMethods(DTreeNode<string> node, TypeBuilder tBuilder)
        {
            foreach (DTreeNode<string> childNode in node.Nodes)
            {
                new GeneratorMethods(ab, childNode, tBuilder).generate();
            }
        }

        /// <summary>
        /// Generates fields for a type. TODO (Now does nothing).
        /// </summary>
        /// <param name="node">SyntaxTree node that declares fields.</param>
        /// <param name="tBuilder">TypeBuilder of the type.</param>
        protected void generateFields(DTreeNode<string> node, TypeBuilder tBuilder)
        {
            foreach (DTreeNode<string> childNode in node.Nodes)
            {
                //new GeneratorMethods(ab, childNode, tBuilder).generate();
            }
        }
    }
}
