using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxAnalysis;
using SyntaxAnalysis.PH.DataTree;

namespace SemanticAnalysis
{
    /// <summary>
    /// Processes syntax tree to find some semantic errors: expressions type
    /// mismatch, and duplicate identifiers.
    /// </summary>
    public class SemanticAnalyzer
    {
        protected SyntaxTree tree;
        protected Dictionary<string, string> identifiers;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="tree">Syntax tree to analyze.</param>
        public SemanticAnalyzer(SyntaxTree tree)
        {
            this.tree = tree;
            identifiers = new Dictionary<string, string>();
        }

        /// <summary>
        /// Starts analyze process.
        /// </summary>
        public void analyze()
        {
            analyzeNode(tree.getRootNode());
            Console.WriteLine("Semantic analysis complete.");
        }

        /// <summary>
        /// Analyzes a single syntaxTree node to decide what to do next.
        /// </summary>
        /// <param name="parentNode">Current syntaxTree node to precess.</param>
        protected void analyzeNode(DTreeNode<string> parentNode)
        {
            int type = 0;
            string s = LexemTypeHelper.parse(ref type, parentNode.Value);
            string identType;
            switch(type) {
                case 1:
                    if (s == "=" || s == "+")
                        checkAssignment(parentNode);
                    break;
                case 3:
                    if (identifiers.TryGetValue(s, out identType))
                        checkDeclaring(parentNode);
                    else
                        identifiers.Add(s, LexemTypeHelper.getParsedValue(parentNode.Nodes[0].Value));
                    break;
            }

            foreach (DTreeNode<string> node in parentNode.Nodes)
            {
                analyzeNode(node);
            }
        }

        /// <summary>
        /// Checks, if identifier is already declared.
        /// </summary>
        /// <param name="parentNode">Current syntaxTree node to process.</param>
        protected void checkDeclaring(DTreeNode<string> parentNode)
        {
            if (parentNode.Nodes.Count > 0)
            {
                int type = 0;
                string name = LexemTypeHelper.parse(ref type, parentNode.Nodes[0].Value);
                if (type == 2 && (name == "int" || name == "double" || name == "String"))  {
                    identifierError(parentNode);
                }
            }
        }

        /// <summary>
        /// Duplicate identifier error occured.
        /// </summary>
        /// <param name="node">SyntaxTree node with duplicated identifier.</param>
        protected void identifierError(DTreeNode<string> node)
        {
            Console.WriteLine("Error - duplicate identifier: " + 
                    LexemTypeHelper.getParsedValue(node.Value) + ".");
            Console.ReadLine();
            Environment.Exit(-2);
        }

        /// <summary>
        /// Checks if in math expression or assignment types match each other.
        /// </summary>
        /// <param name="parentNode">Current syntaxTree node to analyze.</param>
        /// <returns>Type of used expression in all childNodes.</returns>
        protected Type checkAssignment(DTreeNode<string> parentNode)
        {
            Type type1 = getChildType(parentNode.Nodes[0]);
            Type type2 = getChildType(parentNode.Nodes[1]);
            if (type1 != type2)
            {
                typeError(parentNode);
            }
            return type1;
        }

        /// <summary>
        /// Gets type of parts of math expression or assignment from a
        /// single child node.
        /// </summary>
        /// <param name="parentNode">Curent syntaxTree node to process.</param>
        /// <returns>Type of child node.</returns>
        protected Type getChildType(DTreeNode<string> parentNode)
        {
            int type = 0;
            string s = LexemTypeHelper.parse(ref type, parentNode.Value);
            switch (type)
            {
                case 1:
                    if (s == "=" || s == "+" || s == "*" || (s == "-" && parentNode.Nodes.Count != 1))
                        return checkAssignment(parentNode);
                    else 
                        return getChildType(parentNode.Nodes[0]);
                case 3:
                    return getIdentifierType(parentNode, s);
                case 4:
                    return typeof(double);
                case 5:
                    return typeof(int);
                default:
                    undefError();
                    return typeof(int);
            }
        }

        /// <summary>
        /// Gets type of identifier (variable).
        /// </summary>
        /// <param name="parentNode">Current syntaxTree node to process 
        /// (holds a name of variable).</param>
        /// <param name="s">Possible, name of variable.</param>
        /// <returns></returns>
        protected Type getIdentifierType(DTreeNode<string> parentNode, string s)
        {
            string identType;
            if (!identifiers.TryGetValue(s, out identType) && parentNode.Nodes.Count == 1)
            {
                identType = LexemTypeHelper.getParsedValue(parentNode.Nodes[0].Value);
            }
            switch (identType)
            {
                case "int":
                    return typeof(int);
                case "double":
                    return typeof(double);
                case "String":
                    return typeof(string);
                default:
                    undefError();
                    return typeof(int);
            }
        }

        /// <summary>
        /// Type mismatch error has occured.
        /// </summary>
        /// <param name="node">SyntaxTree node, near that error has occured.</param>
        protected void typeError(DTreeNode<string> node)
        {
            Console.WriteLine("Error - type mismatch, near:");
            int numberNodesToPrint = 3;
            tree.printSomeNodes(node, ref numberNodesToPrint);
            Console.ReadLine();
            Environment.Exit(-3);
        }

        /// <summary>
        /// Undefined error has occured (i haven't discovered situations,
        /// where it can occure, but in possible).
        /// </summary>
        protected void undefError()
        {
            Console.WriteLine("Undefined semantic error.");
            Console.ReadLine();
            Environment.Exit(-4);
        }
    }
}
