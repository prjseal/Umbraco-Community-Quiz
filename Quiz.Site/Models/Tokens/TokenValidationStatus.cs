namespace Quiz.Site.Models.Tokens
{
    public enum TokenValidationStatus
    {
        Expired,
        WrongUser,
        WrongPurpose,
        WrongGuid,
        UnableToParse
    }
}