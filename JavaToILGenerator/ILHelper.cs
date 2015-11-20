using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaToILGenerator
{
    static class ILHelper
    {
        public static Type getILType(string s)
        {
            switch (s)
            {
                case "void": return typeof(void);
                case "int": return typeof(int);
                case "double": return typeof(double);
                case "String": return typeof(String);
                default: return null;
            }
        }
    }
}
