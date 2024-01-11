using System;
using System.Linq;
using System.Text;

namespace Essentials
{
    public static class StringUtils
    {
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                var sb = new StringBuilder();
                foreach (var t in hashBytes)
                {
                    sb.Append(t.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        public static string Truncate(this string input, int length, int lenientLength,
            string truncatedIndicator = " …")
        {
            if (input == null || input.Length <= lenientLength) return input;
            return $"{input.Substring(0, Math.Min(input.Length, length)).Trim()}{truncatedIndicator}";
        }

        public static string TruncateAtWord(this string input, int length, string truncatedIndicator = " …")
        {
            if (input == null || input.Length < length) return input;
            var iNextSpace = input.LastIndexOf(" ", length, StringComparison.Ordinal);
            return $"{input.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim()}{truncatedIndicator}";
        }

        public static string Repeat(string s, int count)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < count; i++)
            {
                builder.Append(s);
            }

            return builder.ToString();
        }

        public static string Capitalize(this string s)
        {
            return string.Join(" ", string.Join("-", s.Split('-').Select(f => f.Length > 1
                ? f[..1].ToUpperInvariant() + f[1..]
                : f.ToUpperInvariant()
            )).Split(' ').Select(f => f.Length > 1
                ? f[..1].ToUpperInvariant() + f[1..]
                : f.ToUpperInvariant()
            ));
        }
    }
}