using sReportsV2.Common.ValidationEnums;
using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace sReportsV2.Common.Helpers
{
    public class PasswordHelper
    {
        private static readonly string specialCharacters = "!@#$%^&*+-";
        public static string AdditionalPasswordChecking(string password)
        {
            StringBuilder stringBuilder = new StringBuilder("");
            foreach (PasswordValidationRule validationRule in Enum.GetValues(typeof(PasswordValidationRule)))
            {
                switch (validationRule)
                {
                    case PasswordValidationRule.MinLength:
                        int minPasswordLength;
                        minPasswordLength = Int32.TryParse(ConfigurationManager.AppSettings["MinPasswordLength"], out minPasswordLength) ? minPasswordLength : 4;
                        if (password.Length < minPasswordLength)
                        {
                            string errorMessage = string.Format("Your password should be longer than {0}.", minPasswordLength);
                            stringBuilder.Append(errorMessage);
                            stringBuilder.Append("|");
                        }
                        break;
                    case PasswordValidationRule.RequireDigit:
                        if (!password.Any(char.IsDigit))
                        {
                            string errorMessage = "Your password should have at least one digit.";
                            stringBuilder.Append(errorMessage);
                            stringBuilder.Append("|");
                        }
                        break;
                    case PasswordValidationRule.RequireMixedCase:
                        if (!(password.Any(char.IsUpper) && password.Any(char.IsLower)))
                        {
                            string errorMessage = "Your password should have both upper and lower cases.";
                            stringBuilder.Append(errorMessage);
                            stringBuilder.Append("|");
                        }
                        break;
                    case PasswordValidationRule.RequireSpecialCharacter:
                        if (password.IndexOfAny(specialCharacters.ToCharArray()) == -1)
                        {
                            string errorMessage = string.Format("Your password should have at least one special character: [{0}].", specialCharacters);
                            stringBuilder.Append(errorMessage);
                            stringBuilder.Append("|");
                        }
                        break;
                    default:
                        break;
                }
            }
            return stringBuilder.ToString();
        }

        public static string Hash(string pass, string salt)
        {
            string stringDataToHash = pass + salt;
            // Create a new instance of the hash crypto service provider.
            HashAlgorithm hashAlg = new SHA256CryptoServiceProvider();
            // Convert the data to hash to an array of Bytes.
            byte[] bytValue = System.Text.Encoding.UTF8.GetBytes(stringDataToHash);
            // Compute the Hash. This returns an array of Bytes.
            byte[] bytHash = hashAlg.ComputeHash(bytValue);
            // Optionally, represent the hash value as a base64-encoded string, 
            // For example, if you need to display the value or transmit it over a network.
            string base64 = Convert.ToBase64String(bytHash);
            return base64;
        }

        public static string CreateSalt(int size)
        {
            // Generate a cryptographic random number using the cryptographic 
            // service provider
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            // Return a Base64 string representation of the random number
            return Convert.ToBase64String(buff);
        }

        public static Tuple<string, string> CreateHashedPassword(int length, string salt)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }

            return Tuple.Create<string, string>(res.ToString(), Hash(res.ToString(), salt));
        }
    }
}
