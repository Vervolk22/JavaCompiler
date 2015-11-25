using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxAnalysis;
using SyntaxAnalysis.PH.DataTree;

namespace SemanticAnalysis
{
    public class SemanticAnalyzer
    {
        protected SyntaxTree tree;
        protected HashSet<string> identifiersSet;

        public SemanticAnalyzer(SyntaxTree tree)
        {
            this.tree = tree;
            identifiersSet = new HashSet<string>();
        }

        public void analyze()
        {
            analyzeNode(tree.getRootNode());
        }

        protected void analyzeNode(DTreeNode<string> parentNode)
        {
            int type = 0;
            string s = LexemTypeHelper.parse(ref type, parentNode.Value);
            if (type == 3)
            {
                if (identifiersSet.Contains(s))
                    checkDeclaring(parentNode);
                else
                    identifiersSet.Add(s);
            }

            foreach (DTreeNode<string> node in parentNode.Nodes)
            {
                analyzeNode(node);
            }
        }

        protected void checkDeclaring(DTreeNode<string> parentNode)
        {
            if (parentNode.Nodes.Count == 1)
            {
                int type = 0;
                string name = LexemTypeHelper.parse(ref type, parentNode.Nodes[0].Value);
                if (type == 2 && (name == "int" || name == "double" || name == "String"))  {
                    identifierError(parentNode);
                }
            }
        }

        protected void identifierError(DTreeNode<string> node)
        {
            Console.WriteLine("Error - duplicate identifier: " + LexemTypeHelper.getParsedValue(node.Value));
            Console.ReadLine();
            Environment.Exit(-2);
        }
    }
}
