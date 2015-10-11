using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexicalAnalysis;
using SyntaxAnalysis;

namespace JavaCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            LexicalAnalyzer lexicalAnalyzer = new LexicalAnalyzer();
            IdentifiersTable ident;
            TokenTable table = lexicalAnalyzer.analyze("Input1.txt", out ident);
            lexicalAnalyzer.printResults("Tokens.txt");

            SyntaxAnalyzer syntaxAnalyzer = new SyntaxAnalyzer(table, ident);
            syntaxAnalyzer.analyze();
            Console.WriteLine("Lexical analysis complete.");

            SyntaxTree tree = syntaxAnalyzer.getTree();
            tree.printItselfToTheFile("Tree.txt");

            Console.WriteLine("Syntax analysis complete.");
            Console.ReadLine();
        }
    }
}
