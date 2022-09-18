using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quiz.Site.Models;
using Quiz.Site.Services;
using System;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Models.Email;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Cms.Web.Common.Security;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Extensions;

namespace Quiz.Site.Controllers.Surface
{
    public class QuestionSurfaceController : SurfaceController
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly ILogger<RegisterSurfaceController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly GlobalSettings _globalSettings;

        public QuestionSurfaceController(
            //these are required by the base controller
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            //these are dependencies we've added
            IQuestionRepository questionRepository,
            ILogger<RegisterSurfaceController> logger,
            IEmailSender emailSender,
            IOptions<GlobalSettings> globalSettings
            ) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _globalSettings = globalSettings?.Value ?? throw new ArgumentNullException(nameof(globalSettings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(QuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            Question question = new Question()
            {
                Id = model.Id,
                CreatedBy = "Paul Seal",
                QuestionText = model.QuestionText,
                CorrectAnswer = model.CorrectAnswer,
                WrongAnswer1 = model.WrongAnswer1,
                WrongAnswer2 = model.WrongAnswer2,
                WrongAnswer3 = model.WrongAnswer3,
                MoreInfoLink = model.MoreInfoLink
            };

            _questionRepository.Create(question);

            return RedirectToCurrentUmbracoPage();
        }

        public async Task<bool> SendAlreadyRegisteredEmail(RegisterViewModel model)
        {
            try
            {
                var body =
                    "Somebody tried to register an account using your email address, but you are already registered.";

                var fromAddress = _globalSettings.Smtp.From;

                var subject = string.Format("Already Registered");
                var message = new EmailMessage(fromAddress, model.Email, subject, body, false);
                await _emailSender.SendAsync(message, emailType: "AlreadyRegistered");

                _logger.LogInformation("Already Registered Email Sent Successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error When Trying To Send Already Registered Email");
                return false;
            }
        }
    }
}
