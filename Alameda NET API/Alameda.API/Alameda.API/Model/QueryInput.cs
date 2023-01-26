namespace Alameda.API.Model
{
    public class QueryInput
    {
        public string Input { get; set; } = string.Empty;
        public string Query { get; set; } = string.Empty;
        public Boolean MatchCase { get; set; } = false;
        public Boolean MatchWhole { get; set; } = false;
        public Boolean IgnorePunctuation { get; set; } = false;
    }

    public enum TokenType
    {
        String,
        Operator
    }
}
