using ispProject.Models;
using ispProject.Security;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ispProject.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            try
            {
                using (HITProjectData_Fall17Entities1 db = new HITProjectData_Fall17Entities1())
                {
                    user account = db.users.Find(UserAccount.GetUserID());
                    if (account.role.title == "Disabled")
                    {
                        ViewBag.isDisabled = true;
                    }
                }
            }
            catch (Exception e) {
                //Add an error message here later
                Console.WriteLine(e.Message);
            }
            return View();
        }

        public ActionResult tempError()
        {
            return View();
        }
    }
}