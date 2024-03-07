using System.Text.RegularExpressions;
using System.Text;

namespace dynamic_regex
{
    public class DynamicRegexGenerator
    {
        public string GenerateRegex(bool allowNumeric, bool allowAlpha, int minLength, int maxLength, string allowedSpecialChars = "", string notAllowedSpecialChars = "")
        {
            StringBuilder pattern = new();

            pattern.Append('^');

            if (allowNumeric && allowAlpha)
            {
                pattern.Append("[a-zA-Z0-9");
            }
            else if (allowNumeric)
            {
                pattern.Append("[0-9");
            }
            else if (allowAlpha)
            {
                pattern.Append("[a-zA-Z");
            }

            if (!string.IsNullOrEmpty(allowedSpecialChars))
            {
                foreach (char c in allowedSpecialChars)
                {
                    pattern.Append(Regex.Escape(c.ToString()));
                }
            }

            pattern.Append(']');

            if (!string.IsNullOrEmpty(notAllowedSpecialChars))
            {
                pattern.Append("[^");
                foreach (char c in notAllowedSpecialChars)
                {
                    pattern.Append(Regex.Escape(c.ToString()));
                }
                pattern.Append(']');
            }

            pattern.AppendFormat("{{{0},{1}}}", minLength, maxLength);

            pattern.Append('$');

            return pattern.ToString();
        }

        public bool ValidateString(string input, string regexPattern)
        {
            Regex regex = new(regexPattern);
            return regex.IsMatch(input);
        }
    }
}
