namespace Quiz.Site.Models.Badges;

public interface IBadge
{
    public string UniqueUmbracoName { get; }
    
    public bool AwardCondition { get; }
}