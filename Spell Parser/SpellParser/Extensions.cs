using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellParser
{
    public static class Extensions
    {
        public static string After(this string input, char delimiter)
        {
            var tokenized = input.Split(delimiter);
            if (tokenized.Length < 2)
                return tokenized[0];
            return string.Join(delimiter.ToString(), tokenized.Skip(1));
        }

        public static string Last(this string input, char delimiter)
        {
            var tokenized = input.Split(delimiter);
            if (tokenized.Length < 2)
                return tokenized[0];
            return tokenized.Last();
        }
    }
}
