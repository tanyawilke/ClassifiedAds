using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassifiedAdsApp.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult PrivacyPolicy()
        {
            ViewBag.Message = "";

            return View();
        }

        [AllowAnonymous]
        public ActionResult TermsUse()
        {
            ViewBag.Message = "";

            return View();
        }
    }
}