using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexicalAnalysis;
using SyntaxAnalysis.PH.DataTree;

namespace SyntaxAnalysis
{
    public class SyntaxAnalyzer
    {
        private SyntaxTree tree = new SyntaxTree();
        private TokenTable token;
        private IdentifiersTable ident;
        private LexicalAnalyzer lAnalyzer;

        private int count;

        /// <summary>
        /// Gets token and identifier tables from lexical analyzer.
        /// </summary>
        /// <param name="token">Token table.</param>
        /// <param name="ident">Identifiers table.</param>
        public SyntaxAnalyzer(TokenTable token, IdentifiersTable ident, LexicalAnalyzer lAnalyzer)
        {
            this.token = token;
            this.ident = ident;
            this.lAnalyzer = lAnalyzer;
            count = this.token.tokensCount();
        }

        /// <summary>
        /// Starts the syntax analyze process and building syntax tree
        /// </summary>
        public void analyze()
        {
            int currentPos = 0;
            // search if there are a couple of classes
            while (currentPos < count - 1)
            {
                // search for "class"
                int pos = findFirstOccurence(2, 0, currentPos);
                if (pos == -1)
                {
                    break;
                } else
                {
                    processor_Class(currentPos);
                    currentPos++;
                }
            }

        }

        /// <summary>
        /// Process the code segment belongs to class.
        /// </summary>
        /// <param name="start">Number of the first token, that belongs to class.</param>
        /// <param name="pos"></param>
        private void processor_Class(int pos)
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
            findMatchingBraces(curlyStart, 1, ref curlyFinish);

            // adding class occurence in the tree
            DTreeNode<string> node = tree.getRootNode();
            token.token(pos + 1, ref type, ref value);
            node = node.Nodes.Add(LexemTypeHelper.getTypedValue(3, ident.identifier(value)));

            // go to class content handler
            processor_findMethodsInClass(curlyStart, curlyFinish, node);
        }

        /// <summary>
        /// Finds all methods within a class.
        /// </summary>
        /// <param name="start">Open curly brace of a class.</param>
        /// <param name="finish">Close curly brace of a class.</param>
        /// <param name="parentNode">Tree node of the wrapping class.</param>
        private void processor_findMethodsInClass(int start, int finish, DTreeNode<string> parentNode)
        {
            int curlyFinish = 0, curPos = start;
            DTreeNode<string> curNode = tree.addNode(parentNode, LexemTypeHelper.getTypedValue(6, "methods"));
            while ((curPos = findFirstIdentOccurence(curPos)) != -1)
            {
                findMatchingBraces(curPos + 1, 1, ref curlyFinish);
                processor_method(start + 1, curPos, curlyFinish, curNode);
                curPos = curlyFinish;
            }
        }

        /// <summary>
        /// Processes a single method of a class.
        /// </summary>
        /// <param name="start">Token, that starts a method (it's name or modifiers).</param>
        /// <param name="namePos">Method name' position.</param>
        /// <param name="finish">Position of the close curly brace.</param>
        /// <param name="parentNode">Tree node "methods" of a wrapping class.</param>
        private void processor_method(int start, int namePos, int finish, DTreeNode<string> parentNode)
        {
            DTreeNode<string> curNode = tree.addNode(parentNode, LexemTypeHelper.getTypedValue(3, valueOf(namePos)));
            if (valueOf(namePos) == "main")
            {
                tree.setEntryPoint(curNode);
            }
            for (int i = start; i < namePos; i++)
            {
                tree.addNode(curNode, LexemTypeHelper.getTypedValue(2, valueOf(i)));
            }
            DTreeNode<string> args = tree.addNode(curNode, LexemTypeHelper.getTypedValue(6, "args"));
            int closeBrace = 0;
            findMatchingBraces(namePos, 2, ref closeBrace);
            processor_args(args, namePos + 1, closeBrace);

            processor_code(closeBrace + 2, finish, curNode);
        }

        /// <summary>
        /// Analyzes variable declaration or using as left part of assignment operator.
        /// </summary>
        /// <param name="startPos">Start position of sequence.</param>
        /// <param name="finishPos">Position of a name of variable.</param>
        /// <param name="parentNode">Tree node that is a parent of current expression.</param>
        private DTreeNode<string> processor_variable(int startPos, int finishPos, DTreeNode<string> parentNode)
        {
            DTreeNode<string> node = null;
            int type = 0, value = 0;
            token.token(finishPos, ref type, ref value);
            if (type != 3)
            {
                error(startPos);
            }
            else
            {
                node = parentNode.Nodes.Add(LexemTypeHelper.getTypedValue(3, valueOf(finishPos)));
                for (int i = startPos; i < finishPos; i++)
                {
                    node.Nodes.Add(LexemTypeHelper.getTypedValue(2, valueOf(i)));
                }
            }
            return node;
        }

        /// <summary>
        /// Analyzes expression.
        /// </summary>
        /// <param name="startPos">Position, where expression starts.</param>
        /// <param name="assigmentPos">Position, where assigment sign is.</param>
        /// <param name="finishPos">Position, were expression ends.</param>
        private DTreeNode<string> processor_expression(int startPos, int assigmentPos, int finishPos, DTreeNode<string> parentNode)
        {
            DTreeNode<string> node = parentNode.Nodes.Add(LexemTypeHelper.getTypedValue(1, valueOf(assigmentPos)));
            processor_variable(startPos, assigmentPos - 1, node);
            processor_ariphmetic(assigmentPos + 1, finishPos - 1, node);
            return node;
        }

        /// <summary>
        /// Analyzes ariphmetic expression.
        /// </summary>
        /// <param name="startPos">Start position of an expression.</param>
        /// <param name="finishPos">End position of an expression.</param>
        /// <param name="parentNode">Tree parent node of an expression.</param>
        private void processor_ariphmetic(int startPos, int finishPos, DTreeNode<string> parentNode)
        {
            // Search for (...)+(...)       not only +, but + - *
            int pos = findAriphmeticOutsideBraces(startPos, finishPos);
            if (pos != -1)
            {
                DTreeNode<string> node = parentNode.Nodes.Add(LexemTypeHelper.getTypedValue(1, valueOf(pos)));
                processor_ariphmetic(startPos, pos - 1, node);
                processor_ariphmetic(pos + 1, finishPos, node);
                return;
            }

            // Search for (...)
            int openPos, closePos = 0;
            openPos = findMatchingBraces(startPos, 2, ref closePos);
            if (openPos == startPos && closePos == finishPos)
            {
                processor_ariphmetic(startPos + 1, finishPos - 1, parentNode);
                return;
            }

            // Search for identifiers
            int type = 0, value = 0;
            token.token(startPos, ref type, ref value);
            if (startPos == finishPos && type > 2)
            {
                parentNode.Nodes.Add(lAnalyzer.tokenStringBuilder(type, value));
                return;
            }
            else
            {
                error(startPos);
            }
        }

        /// <summary>
        /// Analyzes basic statement (1 compare sign).
        /// </summary>
        /// <param name="startPos">Start position of a statement.</param>
        /// <param name="finishPos">End position of a statement.</param>
        /// <param name="parentNode">Tree parent node of a statement.</param>
        /// <returns></returns>
        private DTreeNode<string> processor_statement(int startPos, int finishPos, DTreeNode<string> parentNode)
        {
            DTreeNode<string> node = parentNode.Nodes.Add(LexemTypeHelper.getTypedValue(1, valueOf(startPos + 1)));
            int type = 0, value = 0;
            token.token(startPos, ref type, ref value);
            node.Nodes.Add(lAnalyzer.tokenStringBuilder(type, value));
            token.token(finishPos, ref type, ref value);
            node.Nodes.Add(lAnalyzer.tokenStringBuilder(type, value));
            return node;
        }

        /// <summary>
        /// Analyzes while cycle.
        /// </summary>
        /// <param name="startPos">Start position of a cycle.</param>
        /// <param name="finishPos">End position of a cycle.</param>
        /// <param name="parentNode">Tree parent node of a cycle.</param>
        /// <returns></returns>
        private DTreeNode<string> processor_while(int startPos, int finishPos, DTreeNode<string> parentNode)
        {
            int braceStart, braceFinish = 0;
            braceStart = findMatchingBraces(startPos, 2, ref braceFinish);
            DTreeNode<string> node = parentNode.Nodes.Add(LexemTypeHelper.getTypedValue(2, valueOf(startPos)));
            processor_statement(braceStart + 1, braceFinish - 1, node);

            braceStart = findMatchingBraces(startPos, 1, ref braceFinish);
            processor_code(braceStart + 1, braceFinish, node);
            return node;
        }

        /// <summary>
        /// Analyzes System namespace command.
        /// </summary>
        /// <param name="startPos">Start position of a command.</param>
        /// <param name="finishPos">End position of a command.</param>
        /// <param name="parentNode">Tree parent node of a command.</param>
        /// <returns></returns>
        private DTreeNode<string> processor_system(int startPos, int finishPos, DTreeNode<string> parentNode)
        {
            parentNode = parentNode.Nodes.Add(LexemTypeHelper.getTypedValue(2, valueOf(startPos)));
            int i = startPos + 2;
            DTreeNode<string> node2 = parentNode;
            while (i < finishPos)
            {
                node2 = node2.Nodes.Add(LexemTypeHelper.getTypedValue(2, valueOf(i)));
                i += 2;
            }
            return parentNode;
        }

        /// <summary>
        /// Analyzes method's code.
        /// </summary>
        /// <param name="curlyStart">Position of open curly brace.</param>
        /// <param name="curlyFinish">Position of close curly brace.</param>
        /// <param name="parentNode">Tree node of the method.</param>
        private void processor_code(int curlyStart, int curlyFinish, DTreeNode<string> parentNode)
        {
            bool perform = true;
            int pos1, pos2, pos3, currentPos = curlyStart;
            DTreeNode<string> node = parentNode;

            while (perform)
            {
                pos1 = findFirstOccurence(1, 10, currentPos);
                pos2 = findFirstOccurence(1, 12, currentPos);

                if (pos1 == -1 || (pos2 != -1 && pos2 < pos1)) pos1 = pos2;
                if (pos1 == -1 || pos1 >= curlyFinish) 
                {
                    perform = false;
                    continue;
                }
                // Check if several divide characters arranged in succession.
                if (currentPos == pos1)
                {
                    currentPos++;
                    continue;
                }
                
                // "while" check
                if (valueOf(currentPos) == "while")
                {
                    findMatchingBraces(currentPos, 1, ref pos2);
                    node = processor_while(currentPos, pos2, node);
                    currentPos = pos2 + 1;
                    continue;
                }
                // "system" check
                if (valueOf(currentPos) == "System")
                {
                    node = processor_system(currentPos, pos1, node);
                    currentPos = pos1 + 1;
                    continue;
                }
                // "expression check"
                pos3 = findFirstOccurence(1, 3, currentPos);
                if (pos3 != -1 && pos3 < pos1)
                {
                    node = processor_expression(currentPos, pos3, pos1, node);
                    currentPos = pos1 + 1;
                    continue;
                }

                node = processor_variable(currentPos, pos1, node);
            }
        }

        /// <summary>
        /// Processes argument list of a method.
        /// </summary>
        /// <param name="parentNode">Args parent node to add arguments.</param>
        /// <param name="curlyStart">Position of open round brace.</param>
        /// <param name="curlyFinish">Position of close round brace.</param>
        private void processor_args(DTreeNode<string> parentNode, int braceStart, int braceFinish)
        {
            int pos, prevPos = braceStart + 1;
            DTreeNode<string> node;
            while ((pos = findFirstIdentOccurence(prevPos)) != -1 && pos < braceFinish)
            {
                node = parentNode.Nodes.Add(LexemTypeHelper.getTypedValue(3, valueOf(pos)));
                for (int i = prevPos; i < pos; i++)
                {
                    node.Nodes.Add(LexemTypeHelper.getTypedValue(2, valueOf(i)));
                }
                prevPos = pos + 2;
            }
        }

        /// <summary>
        /// Indicates that error in syntax analysis was encountered, and
        ///     prints approximate position of it.
        /// </summary>
        /// <param name="position">Token position, where problem was encountered.</param>
        private void error(int position)
        {
            System.Console.WriteLine("Incorrect syntax near:");
            for (int i = 0; i < 3; i++)
            {
                if (position + i < count)
                {
                    System.Console.WriteLine(valueOf(position + i));
                }
            }
            System.Console.ReadLine();
            Environment.Exit(-1);
        }

        /// <summary>
        /// Returns string value of a token by it's type and value.
        /// </summary>
        /// <param name="type">Type of token to find string value.</param>
        /// <param name="value">\"Value\" of token to find string value.</param>
        /// <returns>Actual token's string value.</returns>
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
        /// Returns string value of a token by it's position.
        /// </summary>
        /// <param name="tokenPosition">Position of a token.</param>
        /// <returns>Actual token's string value.</returns>
        private string valueOf(int tokenPosition)
        {
            int type = 0, value = 0;
            token.token(tokenPosition, ref type, ref value);
            return valueOf(type, value);
        }

        /// <summary>
        /// Method finds the first occurence of needed token
        /// </summary>
        /// <param name="type">Type of token to find</param>
        /// <param name="value">Value of actual token type to find</param>
        /// <param name="currentPos">Position, where to star t search</param>
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

        private int findFirstOccurence(int type, int value, int startPos, int finishPos)
        {
            int curType = 0, curValue = 0;
            token.token(startPos, ref curType, ref curValue);
            while (startPos <= finishPos && !(curType == type && curValue == value))
            {
                startPos++;
                token.token(startPos, ref curType, ref curValue);
            }
            if (curType == type && curValue == value)
            {
                return startPos;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Finds needed ariphmetic operator to next division of expression outside any braces,
        ///     within actual segment of tokens.
        /// </summary>
        /// <param name="startPos">Start position of a segment.</param>
        /// <param name="finishPos">Finish position of a segment.</param>
        /// <returns></returns>
        private int findAriphmeticOutsideBraces(int startPos, int finishPos)
        {
            int pos1, pos2;
            int braceOpen, braceClose = 0;

            while (startPos < finishPos)
            {
                braceOpen = findMatchingBraces(startPos, 2, ref braceClose);
                if (braceOpen == -1)
                {
                    braceOpen = finishPos;
                    braceClose = braceOpen;
                }

                // Search for '+'
                pos1 = findFirstOccurence(1, 0, startPos, braceOpen);
                // Search for '-'
                pos2 = findFirstOccurence(1, 1, startPos, braceOpen);
                if (pos1 == -1 || (pos2 < pos1 && pos2 != -1)) pos1 = pos2;
                //if (pos1 == -1 || pos1 >= curlyFinish)
                if (pos1 != -1 && pos1 < finishPos)
                {
                    return pos1;
                }

                // Search for '*'
                pos1 = findFirstOccurence(1, 2, startPos, braceOpen);
                if (pos1 != -1 && pos1 < finishPos)
                {
                    return pos1;
                }

                startPos = braceClose + 1;
            }

            return -1;
        }

        /// <summary>
        /// Method finds the first occurence of needed token
        /// </summary>
        /// <param name="type">Type of token to find</param>
        /// <param name="value">Value of actual token type to find</param>
        /// <param name="startPos">Position, where to start search</param>
        /// <returns>Position of finded token, or -1 if there is not needed token</returns>
        private int findFirstIdentOccurence(int startPos)
        {
            int curType = 0, curValue = 0;
            token.token(startPos, ref curType, ref curValue);
            while (startPos < count - 1 && !(curType == 3))
            {
                startPos++;
                token.token(startPos, ref curType, ref curValue);
            }
            if (curType == 3)
            {
                return startPos;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Finds a matching pair of curly braces within the tokens list
        /// </summary>
        /// <param name="bracesType">Type of braces: 1 - curly, 2 - round, 3 - square.</param>
        /// <param name="openPos">the open curly brace position to find 
        ///     corresponding close bracket</param>
        /// <param name="closePos">position of finded closing brace.</param>
        /// <returns>Actual position of an open brace.</returns>
        private int findMatchingBraces(int openPos, int bracesType, ref int closePos)
        {
            int curlyCounter = 0, currentPos = openPos;
            int value1, value2;
            int actualOpenPos = 0;
            switch (bracesType)
            {
                case 1:
                    value1 = 11;
                    break;
                case 2:
                    value1 = 8;
                    break;
                case 3:
                    value1 = 15;
                    break;
                default:
                    value1 = 0;
                    break;
            }
            value2 = value1 + 1;

            int type = 0, value = 0;
            bool flag = false;
            while (curlyCounter != 0 || !flag)
            {
                // tokens have ended
                if (currentPos >= token.tokensCount()) break;
                token.token(currentPos, ref type, ref value);
                if (type == 1 && value == value1)
                {
                    curlyCounter++;
                    if (!flag)
                    {
                        flag = true;
                        actualOpenPos = currentPos;
                    }
                }
                if (type == 1 && value == value2) curlyCounter--;
                currentPos++;
            }
            currentPos--;
            if (curlyCounter == 0)
            {
                closePos = currentPos;
            }
            return actualOpenPos;
        }

        public SyntaxTree getTree()
        {
            return tree;
        }
    }
}
