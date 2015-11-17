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
    class GeneratorTypes
    {
        protected AssemblyBuilder ab;
        DTreeNode<string> node;
        ModuleBuilder mBuilder;

        public GeneratorTypes(AssemblyBuilder ab, DTreeNode<string> node, ModuleBuilder mBuilder)
        {
            this.ab = ab;
            this.node = node;
            this.mBuilder = mBuilder;
        }

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

        protected void generateMethods(DTreeNode<string> node, TypeBuilder tBuilder)
        {
            foreach (DTreeNode<string> childNode in node.Nodes)
            {
                new GeneratorMethods(ab, childNode, tBuilder).generate();
            }
        }


        protected void generateFields(DTreeNode<string> node, TypeBuilder mBuilder)
        {

        }
    }
}
