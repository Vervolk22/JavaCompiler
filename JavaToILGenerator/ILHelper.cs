using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaToILGenerator
{
    /// <summary>
    /// Contains additional helper methods for code generation.
    /// </summary>
    static class ILHelper
    {
        /// <summary>
        /// Returns Type by the string with type name.
        /// </summary>
        /// <param name="s">String, representating the type name (\"void\",
        ///  \"int\", \"double\", \"String\").</param>
        /// <returns>Type that corresponds to input string, null otherwise.</returns>
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
