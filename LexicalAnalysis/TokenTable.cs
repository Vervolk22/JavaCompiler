using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis
{
    public class TokenTable
    {
        List<String> tokens = new List<String>();
        int num = 0, length = 0;
        String[] s;

        /// <summary>
        /// adds new token to the list
        /// </summary>
        /// <param name="type">type of token</param>
        /// <param name="num">number of token</param>
        public void addToken(int type, int num) 
        {
            tokens.Add(type.ToString() + ":" + num.ToString());
        }

        /// <summary>
        /// reset the iterator counter
        /// </summary>
        public void initialize()
        {
            num = 0;
        }

        public void initialize(int num)
        {
            this.num = num;
        }

        /// <summary>
        /// get the next token
        /// </summary>
        /// <param name="type">ref int value to receive token type</param>
        /// <param name="value">ref int value to receive token number</param>
        /// <returns>returns if actual value was returned by ref values</returns>
        public bool next(ref int type, ref int value) 
        {
            length = tokens.Count;
            if (num < length)
            {
                s = tokens[num].Split(':');
                type = Convert.ToInt32(s[0]);
                value = Convert.ToInt32(s[1]);
                num++;
                return true;
            }
            return false;
        }

        public void token(int position, ref int type, ref int value) {
            s = tokens[position].Split(':');
            type = Convert.ToInt32(s[0]);
            value = Convert.ToInt32(s[1]);
        }

        public int tokensCount()
        {
            return tokens.Count;
        }
    }
}
