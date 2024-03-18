using System.Text.RegularExpressions;
using System.Text;
using dynamic_regex.Models;

namespace dynamic_regex
{
    public class DynamicRegexGenerator
    {
        public string GenerateRegex(bool allowNumeric, bool allowAlpha, int minLength, int maxLength, string allowedSpecialChars = "")
        {
            StringBuilder pattern = new StringBuilder();

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
                var specialCharGroups = allowedSpecialChars.Split(',');
                foreach (var group in specialCharGroups)
                {
                    pattern.Append(Regex.Escape(group));
                }
            }

            pattern.Append(']');

            pattern.AppendFormat("{{{0},{1}}}", minLength, maxLength);

            pattern.Append('$');

            return pattern.ToString();
        }

        public GenerateRegexResponse ValidateString(string input, string regexPattern)
        {
            Regex regex = new(regexPattern);

            var response = new GenerateRegexResponse();

            if (regex.IsMatch(input))
            {
                response.Message = "Match found.";
                response.IsValid = true;
                return response;
            }
            else
            {
                if (string.IsNullOrEmpty(input))
                {
                    response.Message = "Input is empty.";
                    return response;
                }

                if (!regex.IsMatch(input))
                {
                    response.Message = "No part of the input matches the pattern.";
                    return response;
                }

                int expectedLength = new Regex(regexPattern).Matches("AnyStringToGetLength").Count;
                if (input.Length < expectedLength)
                {
                    response.Message = $"Input is too short. Expected at least {expectedLength} characters.";
                    return response;
                }
                else if (input.Length > expectedLength)
                {
                    response.Message = $"Input is too long. Expected no more than {expectedLength} characters.";
                    return response;
                }

                response.Message = "Input does not match the pattern.";
                return response;
            }
        }
    }
}
