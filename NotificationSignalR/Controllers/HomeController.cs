using NotificationCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NotificationSignalR.Controllers
{
    public class HomeController : Controller
    {
        private readonly NotificationService _notificationService = new NotificationService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public JsonResult NotifyClients(string user, string message, string sendToUserId)
        {
            try
            {
                if (string.IsNullOrEmpty(sendToUserId))
                {
                    _notificationService.NotifyAllClients(user, message);
                }
                else
                {
                    Guid.TryParse(sendToUserId, out Guid targetUserId);
                    _notificationService.NotifyUser(targetUserId, user, message);
                }
                return Json(new { success = true, message = "Notification sent successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult NotifyUser(Guid targetUserId, string sender, string message)
        {
            try
            {
                _notificationService.NotifyUser(targetUserId, sender, message);
                return Json(new { success = true, message = "Notification sent successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}