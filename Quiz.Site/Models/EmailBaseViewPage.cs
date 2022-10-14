using Microsoft.AspNetCore.Mvc.Razor;

namespace Quiz.Site.Models
{
    public abstract class EmailBaseViewPage<TModel> : RazorPage<TModel> where TModel : EmailViewModelBase
    {
    }
}