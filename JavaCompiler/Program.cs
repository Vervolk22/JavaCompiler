using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexicalAnalysis;
using SyntaxAnalysis;
using JavaToILGenerator;

namespace JavaCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            LexicalAnalyzer lexicalAnalyzer = new LexicalAnalyzer();
            IdentifiersTable ident;
            TokenTable table = lexicalAnalyzer.analyze("Input8.txt", out ident);
            lexicalAnalyzer.printResults("Tokens.txt");

            SyntaxAnalyzer syntaxAnalyzer = new SyntaxAnalyzer(table, ident, lexicalAnalyzer);
            syntaxAnalyzer.analyze();
            SyntaxTree tree = syntaxAnalyzer.getTree();
            tree.printItselfToTheFile("Tree.txt");

            Generator generator = new Generator();
            generator.generateExe(tree);
            Console.ReadLine();
        }
    }
}
