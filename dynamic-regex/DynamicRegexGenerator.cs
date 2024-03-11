using System.Text.RegularExpressions;
using System.Text;

namespace dynamic_regex
{
    public class DynamicRegexGenerator
    {
        public string GenerateRegex(bool allowNumeric, bool allowAlpha, int minLength, int maxLength, string allowedSpecialChars = "")
        {
            StringBuilder pattern = new StringBuilder();

            pattern.Append('^'); // Start of string

            if (allowNumeric && allowAlpha)
            {
                pattern.Append("[a-zA-Z0-9"); // Include both alphabetic and numeric characters
            }
            else if (allowNumeric)
            {
                pattern.Append("[0-9"); // Include only numeric characters
            }
            else if (allowAlpha)
            {
                pattern.Append("[a-zA-Z"); // Include only alphabetic characters
            }

            if (!string.IsNullOrEmpty(allowedSpecialChars))
            {
                foreach (char c in allowedSpecialChars)
                {
                    pattern.Append(Regex.Escape(c.ToString())); // Append allowed special characters, escaped
                }
            }

            pattern.Append(']'); // Close the character class

            pattern.AppendFormat("{{{0},{1}}}", minLength, maxLength); // Specify the length range

            pattern.Append('$'); // End of string

            return pattern.ToString();
        }

        public bool ValidateString(string input, string regexPattern)
        {
            Regex regex = new(regexPattern);
            return regex.IsMatch(input);
        }
    }
}
