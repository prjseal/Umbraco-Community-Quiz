using Konstrukt.Configuration.Actions;
using Konstrukt.Extensions;
using Quiz.Site.Actions;
using Quiz.Site.Enums;
using Quiz.Site.Models;


public static class UmbracoBuilderExtensions
{
    public static IUmbracoBuilder AddKonstrukt(this IUmbracoBuilder builder)
    {
        builder.AddKonstrukt(cfg => {

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
                        .AddDataView("All", p => p.Status == ((int)QuestionStatus.Pending).ToString()
                                            || p.Status == ((int)QuestionStatus.Approved).ToString()
                                            || p.Status == ((int)QuestionStatus.Incorrect).ToString()
                                            || p.Status == ((int)QuestionStatus.Used).ToString()
                                            || p.Status == ((int)QuestionStatus.Deleted).ToString()
                                            || p.Status == ((int)QuestionStatus.Unknown).ToString())
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
                    .AddCollection<QuizResult>(x => x.Id, "Quiz Result", "Quiz Results", "A quiz result entity", 
                        "icon-calculator", "icon-calculator", collectionConfig => collectionConfig
                        .AddCard("Perfect Scores", "icon-check", p => p.Score == p.Total, cardConfig => {
                            cardConfig.SetColor("blue");
                        })
                        .SetNameProperty(p => p.Name)
                        .ListView(listViewConfig => listViewConfig
                            .AddField(p => p.Score).SetHeading("Score")
                            .AddField(p => p.Total).SetHeading("Total")
                            .AddField(p => p.DateCreated).SetHeading("Date Created")
                        )
                        .AddDataView("All", p => p.Score == p.Total
                                            || (p.Score < p.Total && p.Score > 0)
                                            || p.Score == 0)
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
                    .AddCollection<Notification>(x => x.Id, "Notification", "Notifications", "A notification entity", "icon-flag", "icon-flag", collectionConfig => collectionConfig
                        .SetNameProperty(p => p.Message)
                        .ListView(listViewConfig => listViewConfig
                            .AddField(p => p.Id).SetHeading("Id")
                            .AddField(p => p.Message).SetHeading("Message")
                            .AddField(p => p.MemberId).SetHeading("Member Id")
                            .AddField(p => p.DateCreated).SetHeading("Date Created")
                        )
                        .Editor(editorConfig => editorConfig
                            .AddTab("General", tabConfig => tabConfig
                                .AddFieldset("General", fieldsetConfig => fieldsetConfig
                                    .AddField(p => p.Id).MakeReadOnly()
                                    .AddField(p => p.Message).MakeRequired()
                                    .AddField(p => p.MemberId).MakeRequired()
                                    .AddField(p => p.BadgeId).MakeRequired()
                                    .AddField(p => p.DateCreated).SetDefaultValue(DateTime.UtcNow).MakeReadOnly()
                                )
                            )
                        )
                    )
                    .AddCollection<ReadNotification>(x => x.Id, "Read Notification", "Read Notifications", "A read notification entity", "icon-flag", "icon-flag-alt", collectionConfig => collectionConfig
                        .ListView(listViewConfig => listViewConfig
                            .AddField(p => p.Id).SetHeading("Id")
                            .AddField(p => p.NotificationId).SetHeading("Notification Id")
                            .AddField(p => p.DateCreated).SetHeading("Date Created")
                        )
                        .Editor(editorConfig => editorConfig
                            .AddTab("General", tabConfig => tabConfig
                                .AddFieldset("General", fieldsetConfig => fieldsetConfig
                                    .AddField(p => p.Id).MakeReadOnly()
                                    .AddField(p => p.NotificationId).MakeRequired()
                                    .AddField(p => p.DateCreated).SetDefaultValue(DateTime.UtcNow).MakeReadOnly()
                                )
                            )
                        )
                    )
                )
            );

        });

        return builder;
    }
}