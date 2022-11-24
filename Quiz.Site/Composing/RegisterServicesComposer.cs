using Quiz.Site.Services;
using SixLabors.Fonts;
using Umbraco.Cms.Core.Composing;

namespace Quiz.Site.Composing;

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
        builder.Services.AddTransient<IBadgeService, BadgeService>();
        builder.Services.AddTransient<INotificationRepository, NotificationRepository>();
        builder.Services.AddTransient<IReadNotificationRepository, ReadNotificationRepository>();
        builder.Services.AddTransient<IhCaptchaService, hCaptchaService>();
        builder.Services.AddTransient<IEmailBodyService, EmailBodyService>();
        builder.Services.AddTransient<ILeaderboardService, LeaderboardService>();

        ConfigureDynamicImageService(builder);            
    }

    private static void ConfigureDynamicImageService(IUmbracoBuilder builder)
    {
        builder.Services
            .AddSingleton<IFontCollection>(sp => {
                FontCollection collection = new();
                
                var host = sp.GetRequiredService<IWebHostEnvironment>();
                var path = host.MapPathWebRoot("/assets/fonts/OpenSans-Regular.ttf");
                collection.Add(path);
                return collection;
            });

        builder.Services.AddSingleton<IDynamicImageService, DynamicImageService>();
    }
}
