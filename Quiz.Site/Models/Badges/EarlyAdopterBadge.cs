namespace Quiz.Site.Models.Badges;

public class EarlyAdopterBadge : IBadge
{
    public string UniqueUmbracoName => "Early Adopter";

    public bool AwardCondition => Condition();
    
    private readonly static DateTime EarlyAdopterThreshold = new DateTime(2022, 11, 5);

    private bool Condition()
    {
        return DateTime.Now.Date < EarlyAdopterThreshold.Date;
    }
}