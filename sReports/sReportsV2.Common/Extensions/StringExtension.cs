using sReportsV2.Common.Enums;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

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

        public static string CapitalizeFirstLetter(this string text)
        {
            string retText = string.Empty;
            if (!string.IsNullOrEmpty(text) && text.Length > 0)
            {
                if (text.Length == 1)
                {
                    retText = char.ToUpper(text[0]).ToString();
                }
                else
                {
                    retText = char.ToUpper(text[0]) + text.Substring(1).ToLower();
                }

            }
            return retText;
        }

        public static string CapitalizeOnlyFirstLetter(this string text)
        {
            string retText = string.Empty;
            if (!string.IsNullOrEmpty(text) && text.Length > 0)
            {
                string firstLetterUp = char.ToUpper(text[0]).ToString();
                retText = firstLetterUp + text.Substring(1);
            }
            return retText;
        }

        public static string TrimInput(this string inputText)
        {
            return string.IsNullOrEmpty(inputText) ? string.Empty : inputText.Trim();
        }

        public static bool IsSpecialValue(this string value)
        {
            Enum.TryParse(value, out FieldSpecialValues parsedValue);
            return parsedValue == FieldSpecialValues.NE;
        }

        public static bool IncludesSpecialValue(this string value)
        {
            value = Ensure.IsNotNull(value, nameof(value));
            return value.Contains(((int)FieldSpecialValues.NE).ToString());
        }

        public static string ResetIfSpecialValue(this string value)
        {
            if (value.IsSpecialValue())
            {
                return string.Empty;
            }
            else
            {
                return value;
            }
        }

        public static bool ShouldSetSpecialValue(this string value, bool isRequired)
        {
            return (string.IsNullOrEmpty(value) && isRequired) || value.IsSpecialValue();
        }

        public static string GetFileNameFromUri(this string uri)
        {
            char nameSeparator = '_';
            string fileName = "";
            if (!string.IsNullOrWhiteSpace(uri))
            {
                string[] guidAndFileName = GetResourceNameFromUri(uri).Split(nameSeparator);
                fileName = string.Join("", guidAndFileName.Skip(1)); // The first is always the GUID
            }
            return fileName;
        }

        public static string GetResourceNameFromUri(this string uri)
        {
            char resourceSeparator = '/';
            return !string.IsNullOrWhiteSpace(uri) ? WebUtility.UrlDecode(uri.Split(resourceSeparator).LastOrDefault()) : "";
        }

        public static string PrepareForMongoStrictTextSearch(this string s)
        {
            return "\"" + s + "\"";
        }

        public static string SanitizeForCsvExport(this string s)
        {
            // Csv specifications suggests of replacing quotes with double quotes
            if (s.Contains("\""))
            {
                s = s.Replace("\"", "\"\"");
            }
            return s;
        }
    }
}