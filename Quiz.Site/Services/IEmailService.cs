using Quiz.Site.Models;

namespace Quiz.Site.Services
{
    public interface IEmailBodyService
    {
        Task<string> RenderRazorEmail<T>(T viewModel) where T : EmailViewModelBase;
    }
}
