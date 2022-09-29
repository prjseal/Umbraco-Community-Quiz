using Konstrukt.Configuration.Actions;
using Konstrukt.Extensions;
using Quiz.Site.Actions;
using Quiz.Site.Enums;
using Quiz.Site.Models;

namespace Quiz.Site
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="webHostEnvironment">The web hosting environment.</param>
        /// <param name="config">The configuration.</param>
        /// <remarks>
        /// Only a few services are possible to be injected here https://github.com/dotnet/aspnetcore/issues/9337.
        /// </remarks>
        public Startup(IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            _env = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddUmbraco(_env, _config)
                .AddBackOffice()
                .AddWebsite()
                .AddKonstrukt(cfg => {

                    cfg.AddSectionAfter("media", "Quiz", sectionConfig => sectionConfig
                        .Tree(treeConfig => treeConfig
                            .AddCollection<Question>(x => x.Id, "Question", "Questions", "A question entity", "icon-help-alt", "icon-help-alt", collectionConfig => collectionConfig
                            .AddCard("Pending", "icon-help-alt", p => p.Status == ((int)QuestionStatus.Pending).ToString(), cardConfig => {
                                cardConfig.SetColor("blue");
                            })
                                .AddSearchableProperty(p => p.QuestionText)
                                .SetNameProperty(p => p.QuestionText)
                                .AddAction<ChangeStatusAction>(actionConfig => actionConfig
                                    .SetVisibility(x => x.ActionType == KonstruktActionType.Bulk
                                        || x.ActionType == KonstruktActionType.Row))
                                .ListView(listViewConfig => listViewConfig
                                    .AddField(p => p.CorrectAnswer).SetHeading("Correct Answer")
                                    .AddField(p => p.DateCreated).SetHeading("Date Created")
                                )
                                .AddDataView("All", p => true)
                                .AddDataView("Pending", p => p.Status == ((int)QuestionStatus.Pending).ToString())
                                .AddDataView("Approved", p => p.Status == ((int)QuestionStatus.Approved).ToString())
                                .AddDataView("Incorrect", p => p.Status == ((int)QuestionStatus.Incorrect).ToString())
                                .AddDataView("Used", p => p.Status == ((int)QuestionStatus.Used).ToString())
                                .AddDataView("Deleted", p => p.Status == ((int)QuestionStatus.Deleted).ToString())
                                .AddDataView("Unknown", p => p.Status == ((int)QuestionStatus.Unknown).ToString())
                                .Editor(editorConfig => editorConfig
                                    .AddTab("General", tabConfig => tabConfig
                                        .AddFieldset("General", fieldsetConfig => fieldsetConfig
                                            .AddField(p => p.QuestionText).SetDataType("Textarea").MakeRequired()
                                            .AddField(p => p.CorrectAnswer).MakeRequired()
                                            .AddField(p => p.WrongAnswer1).MakeRequired()
                                            .AddField(p => p.WrongAnswer2).MakeRequired()
                                            .AddField(p => p.WrongAnswer3).MakeRequired()
                                            .AddField(p => p.MoreInfoLink)
                                            .AddField(p => p.Status).SetDataType("[DataList] Question Status")
                                            .AddField(p => p.AuthorMemberId).SetDataType("Member Picker")
                                        )
                                    )
                                )
                            )
                            .AddCollection<QuizResult>(x => x.Id, "Quiz Result", "Quiz Results", "A quiz result entity", "icon-check", "icon-check", collectionConfig => collectionConfig
                            .AddCard("Perfect Scores", "icon-check", p => p.Score == p.Total, cardConfig => {
                                cardConfig.SetColor("blue");
                            })
                                .SetNameProperty(p => p.Name)
                                .ListView(listViewConfig => listViewConfig
                                    .AddField(p => p.Score).SetHeading("Score")
                                    .AddField(p => p.Total).SetHeading("Total")
                                )
                                .AddDataView("All", p => true)
                                .AddDataView("Perfect", p => p.Score == p.Total)
                                .AddDataView("Inbetweeners", p => p.Score < p.Total && p.Score > 0)
                                .AddDataView("Zero", p => p.Score == 0)
                                .Editor(editorConfig => editorConfig
                                    .AddTab("General", tabConfig => tabConfig
                                        .AddFieldset("General", fieldsetConfig => fieldsetConfig
                                            .AddField(p => p.Score).MakeRequired()
                                            .AddField(p => p.Total).MakeRequired()
                                            .AddField(p => p.MemberId).MakeRequired()
                                            .AddField(p => p.QuizId).MakeRequired()
                                            .AddField(p => p.DateCreated).SetDefaultValue(DateTime.UtcNow).MakeReadOnly()
                                        )
                                    )
                                )
                            )
                        )
                    );

                })
                .AddComposers()
                .Build();
        }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The web hosting environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseUmbraco()
                .WithMiddleware(u => {
                    u.UseBackOffice();
                    u.UseWebsite();
                })
                .WithEndpoints(u => {
                    u.UseInstallerEndpoints();
                    u.UseBackOfficeEndpoints();
                    u.UseWebsiteEndpoints();
                });
        }
    }
}
