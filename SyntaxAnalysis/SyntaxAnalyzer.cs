using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexicalAnalysis;

namespace SyntaxAnalysis
{
    public class SyntaxAnalyzer
    {
        private SyntaxTree tree = new SyntaxTree();
        private TokenTable token;
        private IdentifiersTable ident;
        private LinkedListNode<string> node;

        private int count;

        public SyntaxAnalyzer(TokenTable token, IdentifiersTable ident)
        {
            this.token = token;
            this.ident = ident;
            count = this.token.tokensCount();
        }

        public void analyze()
        {
            int currentPos = 0;
            int lastUsedPos = 0;
            // search if there are a couple of classes
            while (currentPos < count - 1)
            {
                // search for "class"
                int pos = findFirstOccurence(2, 0, currentPos);
                if (pos == -1)
                {
                    error(currentPos);
                } else
                {
                    processor_Class(ref lastUsedPos, ref currentPos);
                    currentPos++;
                }
            }

        }

        private void processor_Class(ref int start, ref int pos)
        {
            int curlyStart = 0, curlyFinish = 0;
            int finded = 0;
            int type = 0, value = 0;
            int currentPos = pos;

            // finding open curly brace of a class
            finded = findFirstOccurence(1, 11, currentPos);
            if (finded == -1)
            {
                error(currentPos);
            }
            else
            {
                curlyStart = finded;
                currentPos++;
            }
            // finding closing curly brace
            findMatchingCurlyBraces(curlyStart, ref curlyFinish);

            // adding class occurence in the tree
            node = tree.getHeadNode();
            token.token(start + 1, ref type, ref value);
            tree.addNode(node, ident.identifier(value));

            // go to class content handler
            processor_findMethodsInClass(curlyStart, curlyFinish);
        }

        private void processor_findMethodsInClass(int start, int finish)
        {
            int curlyStart = 0, curlyFinish = 0;
        }

        private void processor_method()
        {

        }

        private void processor_variable()
        {

        }

        private void processor_expression()
        {

        }

        private void processor_statement()
        {

        }

        private void processor_while()
        {

        }

        private void processor_code()
        {

        }

        private void error(int position)
        {
            int type = 0, value = 0;
            System.Console.WriteLine("Incorrect syntax near:");
            for (int i = 0; i < 3; i++)
            {
                if (position + i < count)
                {
                    token.token(position + i, ref type, ref value);
                    System.Console.WriteLine(valueOf(type, value));
                }
            }
            System.Console.ReadLine();
            Environment.Exit(-1);
        }

        /// <summary>
        /// returns string value of a token
        /// </summary>
        /// <param name="type">Type of token to find string value.</param>
        /// <param name="value">\"Value\" of token to </param>
        /// <returns></returns>
        private string valueOf(int type, int value)
        {
            switch (type)
            {
                case 1:
                    return SyntaxTable.operation(value);
                case 2:
                    return SyntaxTable.keyword(value);
                case 3:
                case 4:
                case 5:
                    return ident.identifier(value);
                default:
                    return "";
            }
        }

        /// <summary>
        /// Method finds the first occurence of needed token
        /// </summary>
        /// <param name="type">Type of token to find</param>
        /// <param name="value">Value of actual token type to find</param>
        /// <param name="currentPos">Position, where to start search</param>
        /// <returns>Position of finded token, or -1 if there is not needed token</returns>
        private int findFirstOccurence(int type, int value, int currentPos)
        {
            int curType = 0, curValue = 0;
            token.token(currentPos, ref curType, ref curValue);
            while (currentPos < count - 1 && !(curType == type && curValue == value))
            {
                currentPos++;
                token.token(currentPos, ref curType, ref curValue);
            }
            if (curType == type && curValue == value)
            {
                return currentPos;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Finds a matching pair of curly braces within the tokens list
        /// </summary>
        /// <param name="openPos">the open curly brace position to find 
        ///     corresponding close bracket</param>
        /// <param name="closePos">position of finded closing brace</param>
        private void findMatchingCurlyBraces(int openPos, ref int closePos)
        {
            int curlyCounter = 0, currentPos = openPos;
            int type = 0, value = 0;
            while (curlyCounter != 0)
            {
                currentPos++;
                // tokens have ended
                if (currentPos >= token.tokensCount()) break;
                token.token(currentPos, ref type, ref value);
                if (type == 1 && value == 11) curlyCounter++;
                if (type == 1 && value == 12) curlyCounter--;
            }
            if (curlyCounter == 0)
            {
                closePos = currentPos;
            }
            else
            {
                error(openPos);
            }
        }
    }
}
