using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Quiz.Site.Services;

namespace Quiz.Site.Composing
{
    public class RegisterServicesComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<IAccountService, AccountService>();
            builder.Services.AddTransient<IDataTypeValueService, DataTypeValueService>();
            builder.Services.AddTransient<IMediaUploadService, MediaUploadService>();
            builder.Services.AddTransient<IQuestionRepository, QuestionRepository>();
            builder.Services.AddTransient<IQuizResultRepository, QuizResultRepository>();
            builder.Services.AddTransient<IQuestionService, QuestionService>();
            builder.Services.AddTransient<IQuizResultService, QuizResultService>();
        }
    }
}
