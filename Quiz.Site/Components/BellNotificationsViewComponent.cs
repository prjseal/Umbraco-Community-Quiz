using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Services;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "BellNotifications")]
    public class BellNotificationsViewComponent : ViewComponent
    {
        private readonly INotificationRepository _notificationRepository;

        public BellNotificationsViewComponent(INotificationRepository notificationRepository)
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
