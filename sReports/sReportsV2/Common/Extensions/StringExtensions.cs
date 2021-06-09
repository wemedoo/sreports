using sReportsV2.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace sReportsV2.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceNonAlphaCharactersWithDash(this string text)
        {
            char[] arr = text.Select(c => (char.IsLetterOrDigit(c) || c == '-') ? c : '-').ToArray();
            return new string(arr);
        }
        public static string RemoveDiacritics(this string s)
        {
            s = Ensure.IsNotNull(s, nameof(s));

            String normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        public static string GetFieldSetId(this string text)
        {
            text = Ensure.IsNotNull(text, nameof(text));

            return text.Split('-')[0];
        }
        public static string GetFieldSetCounter(this string text)
        {
            text = Ensure.IsNotNull(text, nameof(text));

            return text.Split('-')[1];
        }
        public static string GetFieldId(this string text)
        {
            text = Ensure.IsNotNull(text, nameof(text));

            return text.Split('-')[2];
        }
        public static string GetFieldCounter(this string text)
        {
            text = Ensure.IsNotNull(text, nameof(text));

            return text.Split('-')[3];
        }
    }
}