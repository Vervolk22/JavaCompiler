using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis
{
    public class IdentifiersTable
    {
        List<String> list = new List<String>();

        /// <summary>
        /// adds new identifier
        /// </summary>
        /// <param name="str">string value of identifier</param>
        /// <returns>position of new identifier</returns>
        public int newIdentifier(String str)
        {
            if (!list.Contains(str))
            {
                list.Add(str);
                return list.Count - 1;
            }
            else
            {
                return list.FindIndex(s => s == str);
            }
        }

        /// <summary>
        /// returns the string value of identifier with the num position
        /// </summary>
        /// <param name="num">position of identifier</param>
        /// <returns>string value of identifier</returns>
        public String identifier(int num)
        {
            return list[num];
        }
    }
}
