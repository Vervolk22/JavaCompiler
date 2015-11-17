using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalysis
{
    public static class LexemTypeHelper
    {
        public static string getTypedValue(int type, string value)
        {
            switch (type)
            {
                case 1: return "operator:" + value;
                case 2: return "keyword:" + value;
                case 3: return "identifier:" + value;
                case 4: return "floatconstant:" + value;
                case 5: return "intconstant:" + value;
                case 6: return "declares:" + value;
                default: return value;
            }
        }

        public static string getParsedValue(string value)
        {
            string[] array = value.Split(':');
            if (array.Length > 1)
            {
                return value.Split(':')[1];
            }
            else
            {
                return null;
            }
        }

        public static string parse(ref int type, string value)
        {
            string[] array = value.Split(':');
            type = getType(array[0]);
            if (array.Length > 1)
            {
                return value.Split(':')[1];
            }
            else
            {
                return null;
            }
        }

        public static int getParsedType(string value)
        {
            string[] array = value.Split(':');
            return getType(array[0]);
        }

        private static int getType(string type)
        {
            switch (type)
            {
                case "operator:": return 1;
                case "keyword:": return 2;
                case "identifier:": return 3;
                case "floatconstant": return 4;
                case "intconstant:": return 5;
                case "declares:": return 6;
                default: return 0;
            }
        }
    }
}
