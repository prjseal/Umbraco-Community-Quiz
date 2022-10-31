namespace Quiz.Site.Models.Tokens
{
    public class TokenValidationModel
    {
        public bool Validated { get { return Errors.Count == 0; } }
        public readonly List<TokenValidationStatus> Errors = new List<TokenValidationStatus>();
    }
}