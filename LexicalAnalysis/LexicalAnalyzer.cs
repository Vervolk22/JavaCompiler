using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LexicalAnalysis
{
    public class LexicalAnalyzer
    {
        String current = "", next = "", text = "";
        TokenTable token = new TokenTable();
        IdentifiersTable table = new IdentifiersTable();
        int i;

        /// <summary>
        /// Analyzes code and creates token and identifier tables
        /// </summary>
        /// <param name="path">file to analize</param>
        public TokenTable analyze(String path, out IdentifiersTable ident)
        {
            text = File.ReadAllText(path);
            // analyzing by each character
            for (i = 0; i < text.Length; i++)
            {
                next = Convert.ToString(text[i]);
                // space symbol behavior
                if (SyntaxTable.isSpaceSymbol(next))
                {
                    processCurrent();
                    continue;
                }

                // number check
                if (SyntaxTable.isNumber(current))
                {
                    if (SyntaxTable.isNumber(current + next)) 
                    {
                        current += next;
                        continue;
                    }
                    else
                    {
                        processCurrent();
                        current = next;
                        continue;
                    }
                }

                // operator behavior
                if (SyntaxTable.isOperation(next) > -1)
                {
                    // complex operation behavior
                    if (!SyntaxTable.isOperationAllowed(current + next))
                    {
                        processCurrent();
                        current = next;
                        continue;
                    }
                    else
                    {
                        current += next;
                        continue;
                    }
                }
                // check if symbol is valid
                if (!SyntaxTable.isSymbolAllowed(Convert.ToChar(next)))
                {
                    error(i);
                    ident = null;
                    return null;
                }

                // check for preceding operator
                if (current.Length == 1 &&
                    (SyntaxTable.isOperation(Convert.ToString(current[0])) > -1))
                {
                    processCurrent();
                    current = next;
                    continue;
                }
                current += next;
            }
            if (current != "") processCurrent();
            ident = table;
            return token;
        }

        /// <summary>
        /// Lexem was found, create token
        /// </summary>
        private void processCurrent()
        {
            if (current == "")
            {
                return;
            }
            else
            {
                int code;
                if (SyntaxTable.containsPoint(current) == -1) error(i);
                if (current == ".")
                {
                    token.addToken(1, SyntaxTable.isOperation(current));
                    current = "";
                    return;
                }
                if (SyntaxTable.isNumber(current))
                {
                    if (SyntaxTable.containsPoint(current) == 1)
                    {
                        code = table.newIdentifier(current);
                        token.addToken(4, code);
                        current = "";
                        return;
                    }
                    else
                    {
                        code = table.newIdentifier(current);
                        token.addToken(5, code);
                        current = "";
                        return;
                    }
                }
                code = SyntaxTable.isOperation(current);
                if (code > -1)
                {
                    token.addToken(1, code);
                    current = "";
                    return;
                }
                code = SyntaxTable.isKeyword(current);
                if (code > -1) {
                    token.addToken(2, code);
                    current = "";
                    return;
                }
                else
                {
                    code = table.newIdentifier(current);
                    token.addToken(3, code);
                    current = "";
                    return;
                }
            }
        }

        /*private bool ckeckIdentifier() {

        }*/

        /// <summary>
        /// just for test purposes, prints token table
        /// </summary>
        public void printResults()
        {
            token.initialize();
            int type = 0, value = 0;
            while (token.next(ref type, ref value))
            {
                switch (type)
                {
                    case 1:
                        {
                            Console.WriteLine("Operator: " + SyntaxTable.operation(value));
                            break;
                        }
                    case 2:
                        {
                            Console.WriteLine("Keyword: " + SyntaxTable.keyword(value));
                            break;
                        }
                    case 3:
                        {
                            Console.WriteLine("Identificator: " + table.identifier(value));
                            break;
                        }
                    case 4:
                        {
                            Console.WriteLine("Floating point constant: " + table.identifier(value));
                            break;
                        }
                    case 5:
                        {
                            Console.WriteLine("Integer constant: " + table.identifier(value));
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// in the code was syntax error
        /// </summary>
        /// <param name="i">number of the character, 
        /// where the error was found</param>
        private void error(int i)
        {
            int num = 0;
            Console.WriteLine("Illegal character: " +
                (text.Take(i).Count(ch => (num++ > -1) && ch == '\r' && 
                ((num = 0)) > -1) + 1) + " line " + num + " position");
        }

        public int tokensCount()
        {
            return token.tokensCount();
        }
    }
}
