using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Quiz.Site.Models;

namespace Quiz.Site.Services
{
    public class EmailBodyService : IEmailBodyService
    {
        private readonly ILogger<EmailBodyService> logger;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IRazorViewEngine viewEngine;
        private readonly ITempDataProvider tempDataProvider;

        public EmailBodyService(ILogger<EmailBodyService> logger, IHttpContextAccessor httpContextAccessor, 
            IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider)
        {
            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;
            this.viewEngine = viewEngine;
            this.tempDataProvider = tempDataProvider;
        }
        public async Task<string> RenderRazorEmail<T>(T viewModel) where T : EmailViewModelBase
        {
            string messageBody;
            try
            {
                messageBody = await RenderViewWithModel(viewModel.ViewPath, viewModel);
                return messageBody;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating message body");
                return string.Empty;
            }
        }

        
        private async Task<string> RenderViewWithModel(string viewName, object viewModel)
        {
            RouteData routeData = new RouteData();

            var actionContext = new ActionContext(httpContextAccessor.HttpContext, routeData, new ActionDescriptor());

            ViewEngineResult razorViewResult = GetViewEngineResult(actionContext, viewName);

            var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = viewModel };

            using var stringWriter = new StringWriter();
            var viewContext = 
                new ViewContext(actionContext, 
                razorViewResult.View, 
                viewDataDictionary, 
                new TempDataDictionary(actionContext.HttpContext, tempDataProvider), stringWriter, new HtmlHelperOptions());
            await razorViewResult.View.RenderAsync(viewContext);

            return stringWriter.ToString();
        }
        
        private ViewEngineResult GetViewEngineResult(ActionContext actionContext, string viewName)
        {
            ViewEngineResult viewEngineResult = viewEngine.GetView(viewName, viewName, false);

            if (viewEngineResult.View == null)
                throw new FileNotFoundException("Could not find the View file. Searched locations:\r\n" + string.Join("\t\n", viewEngineResult.SearchedLocations));

            return viewEngineResult;
        }

    }
}
