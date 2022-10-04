using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;
using Quiz.Site.Services;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "ToastNotifications")]
    public class ToastNotificationsViewComponent : ViewComponent
    {
        private readonly INotificationRepository _notificationRepository;

        public ToastNotificationsViewComponent(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public IViewComponentResult Invoke(Member? memberModel)
        {
            var latestNotifications = _notificationRepository.GetAllByMemberId(memberModel.Id);
            return View(latestNotifications);
        }
    }
}
