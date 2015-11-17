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
    class GeneratorCode
    {
        DTreeNode<string> node;
        ILGenerator ilg;
        Dictionary<string, LocalBuilder> locals = new Dictionary<string, LocalBuilder>();

        public GeneratorCode(DTreeNode<string> node, ILGenerator ilg)
        {
            this.node = node;
            this.ilg = ilg;
        }

        public void generate()
        {
            next(node);
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

        }

        protected void handleKeyword(DTreeNode<string> node)
        {

        }

        protected void handleIdentifier(DTreeNode<string> node)
        {
            string name = LexemTypeHelper.getParsedValue(node.Value);
            LocalBuilder lBuilder;
            if (!locals.TryGetValue(name, out lBuilder))
            {
                lBuilder = ilg.DeclareLocal();
                locals.Add(name, lBuilder);
            }
        }
    }
}
