using System.Text.RegularExpressions;
using System.Text;
using dynamic_regex.Models;

namespace dynamic_regex
{
    public class DynamicRegexGenerator
    {
        public string GenerateRegex(bool allowNumeric, bool allowAlpha, int minLength, int maxLength, string allowedSpecialChars = "", string prefix = "", string suffix = "", int? maxNumericLength = null, int? maxAlphaLength = null)
        {
            StringBuilder pattern = new StringBuilder();

            pattern.Append('^');

            if (!string.IsNullOrEmpty(prefix))
            {
                pattern.Append(Regex.Escape(prefix));
            }

            pattern.Append('(');

            if (allowNumeric)
            {
                if (maxNumericLength.HasValue)
                {
                    pattern.Append("(\\d{1," + maxNumericLength.Value + "})");
                }
                else
                {
                    pattern.Append("\\d*"); 
                }
            }

            if (allowAlpha)
            {
                if (maxAlphaLength.HasValue)
                {
                    pattern.Append("([a-zA-Z]{1," + maxAlphaLength.Value + "})");
                }
                else
                {
                    pattern.Append("[a-zA-Z]*");
                }
            }

            if (!string.IsNullOrEmpty(allowedSpecialChars))
            {
                var specialCharGroups = allowedSpecialChars.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var group in specialCharGroups)
                {
                    var cleanGroup = group.Trim(new char[] { '[', ']' });
                    pattern.Append(Regex.Escape(cleanGroup));
                }
            }

            pattern.Append(')');

            pattern.AppendFormat("{{{0},{1}}}", minLength, maxLength);

            if (!string.IsNullOrEmpty(suffix))
            {
                pattern.Append(Regex.Escape(suffix));
            }

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
