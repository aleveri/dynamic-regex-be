namespace dynamic_regex.Models
{
    public class GenerateRegexRequest
    {
        public bool AllowNumeric { get; set; }
        public bool AllowAlphaNumeric { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public string AllowedSpecialChars { get; set; }
        public string NotAllowedSpecialChars { get; set; }
    }
}
