using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioViet_NQL_De_2.Helper
{
    public class Convert
    {
        public  string ConvertBoolArrayToString(bool[] boolArray)
        {
            if (boolArray == null || boolArray.Length != 24)
            {
                throw new ArgumentException("Input array must be a non-null bool array with 24 elements.");
            }

            return string.Join("", boolArray.Select(b => b ? '1' : '0'));
        }
       
        public bool[] ConvertStringToBoolArray(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length != 24)
            {
                throw new ArgumentException("Input string must be non-null and have exactly 24 characters.");
            }

            return input.Select(c => c == '1').ToArray();
        }

    }
}
