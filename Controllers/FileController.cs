using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.HtmlControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ClassifiedAdsApp.Models;
using System.Threading.Tasks;

namespace ClassifiedAdsApp.Controllers
{
    public class FileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: File
        [AllowAnonymous]
        public ActionResult Index(int id)
        {
            var retrieveFile = db.AdsViewFile.FirstOrDefault(c => c.FileId == id);
            
            return File(retrieveFile.Content, retrieveFile.ContentType);
        }
    }
}