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
            TokenTable table = lexicalAnalyzer.analyze("Input1.txt", out ident);
            lexicalAnalyzer.printResults("Tokens.txt");
            Console.WriteLine("Lexical analysis complete.");

            SyntaxAnalyzer syntaxAnalyzer = new SyntaxAnalyzer(table, ident, lexicalAnalyzer);
            syntaxAnalyzer.analyze();
            SyntaxTree tree = syntaxAnalyzer.getTree();
            tree.printItselfToTheFile("Tree.txt");
            Console.WriteLine("Syntax analysis complete.");

            Generator generator = new Generator();
            generator.generateExe(tree);
            Console.ReadLine();
        }
    }
}
