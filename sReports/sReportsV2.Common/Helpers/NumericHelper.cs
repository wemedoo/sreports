using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Common.Helpers
{
    public static class NumericHelper
    {
        public static int GetDecimalsNumber(double number)
        {
            int decimalPartNumber;
            string[] numberParts = number.ToString().Split('.');

            if (numberParts.Length == 2)
            {
                decimalPartNumber = numberParts[1].Length;
            }
            else
            {
                decimalPartNumber = 0;
            }

            return decimalPartNumber;
        }
    }
}
