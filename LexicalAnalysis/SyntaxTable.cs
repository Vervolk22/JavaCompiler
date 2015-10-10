using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace LexicalAnalysis
{
    public static class SyntaxTable
    {
        readonly static ReadOnlyCollection<String> spaces;
        readonly static ReadOnlyCollection<String> operators;
        readonly static ReadOnlyCollection<String> keywords;
        readonly static ReadOnlyCollection<Char> allowedSymbols;
        readonly static ReadOnlyCollection<Char> numbers;

        /// <summary>
        /// initialize all the lists
        /// </summary>
        static SyntaxTable()
        {
            List<String> t = new List<String>() {
                Convert.ToString(Convert.ToChar(32)), 
                Convert.ToString(Convert.ToChar(9)),
                Convert.ToString(Convert.ToChar(10)), 
                Convert.ToString(Convert.ToChar(13)), };
            spaces = t.AsReadOnly();
            t = new List<String>() {"class", "int", "double", "while", "if", 
                "String", "public", "static", "void", "System", "out",
                "println", "else" };
            keywords = t.AsReadOnly();
            t = new List<String>() { "+", "-", "*", "=", ".", "!", "<", 
                ">", "(", ")", ";", "{", "}", "!=", "()", "[", "]", "\"",};
            operators = t.AsReadOnly();
            List<Char> tc = new List<Char>() {'a', 'b', 'c', 'd', 'e', 'f',
                'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
                's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D',
                'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
                'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1',
                '2', '3', '4', '5', '6', '7', '8', '9', '.', };
            allowedSymbols = tc.AsReadOnly();
            tc = new List<Char>() {'0', '1', '2', '3', '4', '5', '6', '7', 
                '8', '9', '.', };
            numbers = tc.AsReadOnly();
        }

        /// <summary>
        /// checks if string represents a number
        /// </summary>
        /// <param name="str">string to check</param>
        /// <returns>result</returns>
        public static bool isNumber(String str)
        {
            foreach (char ch in str)
            {
                if (ch != '.' && (ch < '0' || ch > '9'))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// checks if the number contains a point(or points)
        /// </summary>
        /// <param name="str">string to check</param>
        /// <returns>0, if there are no poits, 1 - if 1 point was found,
        /// -1 - if there are more than 1 point</returns>
        public static int containsPoint(String str)
        {
            int pointNumber = 0;
            foreach (char ch in str)
            {
                if (ch == '.') pointNumber++;
            }
            if (pointNumber > 1) return -1;
            else
                if (pointNumber == 1) return 1;
                else
                    return 0;
        }

        /// <summary>
        /// return string value of operator with num position
        /// </summary>
        /// <param name="num">position of the operator</param>
        /// <returns>String value of the operator</returns>
        public static String operation(int num)
        {
            return operators[num];
        }

        /// <summary>
        /// return string value of keyword with num position
        /// </summary>
        /// <param name="num">position of the keyword</param>
        /// <returns>String value of the keyword</returns>
        public static String keyword(int num)
        {
            return keywords[num];
        }

        public static bool isSymbolAllowed(Char ch)
        {
            return allowedSymbols.Contains(ch);
        }

        public static bool isSpaceSymbol(String str)
        {
            return spaces.Contains(str);
        }

        public static int isOperation(String str)
        {
            return checkContains(operators, str);
        }

        public static bool isOperationAllowed(String str)
        {
            return operators.Contains(str);
        }

        public static int isKeyword(String str)
        {
            return checkContains(keywords, str);
        }

        private static int checkContains(ReadOnlyCollection<String> coll, String str)
        {
            if (!coll.Contains(str))
            {
                return -1;
            }
            else
            {
                return coll.IndexOf(str);
            }
        }
    }
}
