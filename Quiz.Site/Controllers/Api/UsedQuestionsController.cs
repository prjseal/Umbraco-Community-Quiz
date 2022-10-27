using Quiz.Site.Models;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.PublishedModels;
using Quiz.Site.Services;

namespace Quiz.Site.Controllers.Api;


[PluginController("UsedQuestionsContentApp")]
public class UsedQuestionsController : UmbracoAuthorizedApiController
{
    private readonly IUmbracoContextFactory umbracoContextFactory;
 
    private readonly IQuestionRepository questionRepository;


    public UsedQuestionsController(IUmbracoContextFactory umbracoContextFactory, IQuestionRepository questionRepository)
    {
        this.umbracoContextFactory = umbracoContextFactory;
        this.questionRepository = questionRepository;
    }

    public List<Question> GetQuestions(int currentNodeId)
    {
        List<Question> selectedQuestions = new();

        using (var umbracoContextReference = umbracoContextFactory.EnsureUmbracoContext())
        {
            var content = umbracoContextReference.UmbracoContext?.Content?.GetById(currentNodeId);
            
            if(content is QuizPage quizPage && quizPage.Questions != null)
            {
                var arrayOfIds  = quizPage.Questions.Select(int.Parse).ToArray();
                if (arrayOfIds != null && arrayOfIds.Any())
                {
                   selectedQuestions = questionRepository.GetByIds(arrayOfIds);

                }
            }
        }

        return selectedQuestions;

    }
}
