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
using System.Text;
using System.Web.Routing;

namespace ClassifiedAdsApp.Controllers
{
    public class AdsViewAdvertsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult FillSubCategories(int category)
        {
            List<AdsViewSubCategory> objSub = new List<AdsViewSubCategory>();
            objSub = db.AdsViewSubCategory.Where(c => c.CategoryId == category).ToList();
            SelectList objSubCategory = new SelectList(objSub, "SubCategoryId", "Description", 0);
            return Json(objSubCategory);
        }

        // GET: AdsViewAdverts
        [AllowAnonymous]
        public ActionResult Index()
        {
            var allAdverts = db.AdsViewAdverts.Where(c => c.Status == 1);
            if (allAdverts != null)
            {
                return View(allAdverts.ToList());
            }
            else
            {
                return View("There are no active ads.");
            }
        }

        public ActionResult MyAdverts(ApplicationUser user)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var loggedUser = User.Identity.GetUserId();

                try
                {
                    var myAdverts = db.AdsViewAdverts.Where(c => c.User == loggedUser && c.Status == 1);
                    if (myAdverts != null)
                    {
                        return View(myAdverts);
                    }
                    else
                    {
                        return View("You don't have any active ads.");
                    }
                    
                }
                catch (Exception Ex)
                {
                    return View(Ex.InnerException.ToString());
                }
            }
            
            //return View();
        }

        public ActionResult MyPendingAdverts(ApplicationUser user)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var loggedUser = User.Identity.GetUserId();

                try
                {
                    var myAdverts = db.AdsViewAdverts.Where(c => c.User == loggedUser && c.Status == 0);
                    if (myAdverts != null)
                    {
                        return View(myAdverts);
                    }
                    else
                    {
                        return View("You don't have any pending ads.");
                    }

                }
                catch (Exception Ex)
                {
                    return View(Ex.InnerException.ToString());
                }
            }
        }

        // GET: AdsViewAdverts/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BigViewModel bigViewModel = new BigViewModel();
            bigViewModel.AdsViewAdverts = db.AdsViewAdverts.Find(id);
            bigViewModel.AdsViewContact = db.AdsViewContacts.FirstOrDefault(c => c.Advert.Id == id);
            bigViewModel.AdsViewFile = db.AdsViewFile.FirstOrDefault(c => c.AdId == id);

            return View(bigViewModel);
        }

        public int GetAdId(string code)
        {
            int myAdvert = Convert.ToInt32(db.AdsViewAdverts.Where(c => c.ConfirmationCode == code).Select(c => c.Id).Single());

            return myAdvert;
        }

        public ActionResult UpdateAdStatus(int? id)
        {
            if (id != null )
            {   
                var result = db.AdsViewAdverts.Find(id);
                if (result != null)
                {
                    result.Status = 1;
                    db.SaveChanges();
                }
            }

            return RedirectToAction(nameof(MyAdverts));
        }

        private string SendEmailConfirmationTokenAsync(string userID, int AdId, string subject)
        {
            string code = UserManager.GenerateEmailConfirmationToken(userID);
            var callbackUrl = Url.Action(nameof(ConfirmAd), "AdsViewAdverts",
               new { ad = AdId, userId = userID, code = code }, protocol: Request.Url.Scheme);
            UserManager.SendEmail(userID, subject,
               "Please confirm your ad by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return callbackUrl;            
        }

        // GET: /Account/ConfirmEmail
        public async Task<ActionResult> ConfirmAd(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? nameof(ConfirmAd) : "Error");
        }
        //code = "+j0fwKv6mkCOMLoyr3sEe88qgPYyAh6mniG+AyijeyGDLWWfhQnj9CBqBQJn7LAF9BPra4qXyJiRlmgkjbScdljOSKwsGDt8UtROO71kv/Jj9tjWbnZY2ElQhlMDYzQ9HnOgSn9VvY60uOwu/TxXHJ4b2V522OaWfp96R24eY4ESx9cKjJ8Z8z/e9Nz44Ybn"
        // GET: AdsViewAdverts/Create
        public ActionResult Create()
        {
            ViewBag.CategoryList = db.AdsViewCategory;
            ViewBag.AdvertTypeList = db.AdsViewAdvertType;
            ViewBag.LocationList = db.AdsViewLocations;
            ViewBag.AdSubCategoryList = db.AdsViewSubCategory;

            return View();
        }

        [Authorize]
        // POST: AdsViewAdverts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id")] FormCollection adsViewAdverts)
        //public ActionResult Create(FormCollection adsViewAdverts)
        public ActionResult Create([Bind(Include = "Id")] FormCollection adsViewAdverts, HttpPostedFileBase upload)
        {
            var authUser = User.Identity.GetUserId();
            Guid confirmation = Guid.NewGuid();
            bool contactInformationFailure = false;
            bool uploadedFileFailure = false;

            // Get the current date.
            DateTime thisDay = DateTime.Today;

            AdsViewAdverts advert = new AdsViewAdverts();
            
            advert.CategoryId = Int32.Parse(adsViewAdverts["Category"]);
            advert.Status = 0;
            advert.DateLastModified = thisDay;
            advert.DatePublished = Convert.ToDateTime(adsViewAdverts["DatePublished"]);
            advert.ExpiryDate = Convert.ToDateTime(adsViewAdverts["ExpiryDate"]);
            advert.Title = adsViewAdverts["Title"];
            advert.Description = adsViewAdverts["Description"];
            advert.Price = Convert.ToDecimal(adsViewAdverts["Price"]);
            advert.DateAdvertAdded = thisDay;
            advert.DateOfDeleteAdvert = Convert.ToDateTime("31/12/2016 12:00:00 AM");
            advert.ConfirmationCode = Guid.NewGuid().ToString().Substring(0, 9);
            advert.AdvertTypeId = Int32.Parse(adsViewAdverts["AdvertType"]);
            advert.LocationId = Int32.Parse(adsViewAdverts["Location"]);
            advert.SubCategoryId = Int32.Parse(adsViewAdverts["SubCategory"]);            
            advert.User = authUser;

            if (ModelState.IsValid)
            {
                db.AdsViewAdverts.Add(advert);

                try
                {
                    db.SaveChanges();

                    int adId = GetAdId(advert.ConfirmationCode.ToString());

                    if (adId != 0)
                    {
                        AdsViewContact contact = new AdsViewContact();
                        AdsViewFile attachment = new AdsViewFile();

                        contact.Id = adId;
                        contact.ContactName = adsViewAdverts["ContactName"];
                        contact.ContactPhoneNumber = adsViewAdverts["ContactPhoneNumber"];
                        contact.ContactPhysicalAddress = adsViewAdverts["ContactPhysicalAddress"];
                        contact.ContactEmail = adsViewAdverts["ContactEmail"];
                        contact.WebSite = adsViewAdverts["WebSite"];

                        db.AdsViewContacts.Add(contact);

                        if (upload != null && upload.ContentLength > 0)
                        {
                            attachment.AdId = adId;
                            attachment.Filename = Path.GetFileName(upload.FileName);
                            attachment.FileType = FileType.Avatar;
                            attachment.ContentType = upload.ContentType;

                            string[] allowedExtentions = GetAllowedExtension();

                            foreach (var extention in allowedExtentions)
                            {
                                if (attachment.ContentType.Contains(extention.ToLower()))
                                {
                                    using (var reader = new BinaryReader(upload.InputStream))
                                    {
                                        attachment.Content = reader.ReadBytes(upload.ContentLength);

                                        db.AdsViewFile.Add(attachment);
                                    }

                                    break;
                                }
                                else
                                {
                                    uploadedFileFailure = true;
                                }
                            }
                        };

                        db.SaveChanges();

                        string callbackUrl = SendEmailConfirmationTokenAsync(authUser, adId, "Confirm your ad.");

                        return RedirectToAction(nameof(CreateConfirmation), new { uploadedFile = uploadedFileFailure, contactInformation = contactInformationFailure });
                        //return RedirectToAction(nameof(CreateConfirmation), new RouteValueDictionary( new { controller = this, action = "CreateConfirmation", uploadedFile = uploadedFileFailure, contactInformation = contactInformationFailure }));
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            ViewBag.Message("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public ActionResult CreateConfirmation(bool uploadedFile = false, bool contactInformation = false)
        {
            StringBuilder message = new StringBuilder();
            message.Append("Check your email and confirm your ad. You must confirm the ad before it will be visible for viewing/management. ");

            if (uploadedFile)
            {
                message.Append("Note that an error occurred when you attempted to upload your supporting documentation  The likely cause is the file is in an invalid file format. Please edit your ad and upload the file in its correct format. ");
            }

            if(contactInformation)
            {
                message.Append("Note that an error occurred when you attempted to save your contact information. Once confirmed, please edit your ad and save your contact information.</p>");
            }

            ViewBag.Message = message;

            return View();
        }

        public string[] GetAllowedExtension()
        {
            var allowedExtensions = new string[] { "jpeg", "jpg", "tiff", "png", "gif", "pdf", "doc", "docx" };

            return allowedExtensions;
        }

        //public string RandomString()
        //{
        //    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        //    var stringChars = new char[10];
        //    var random = new Random();

        //    for (int i = 0; i < stringChars.Length; i++)
        //    {
        //        stringChars[i] = chars[random.Next(chars.Length)];
        //    }

        //    var finalString = new String(stringChars);

        //    return finalString;
        //}

        // GET: AdsViewAdverts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BigViewModel bigViewModel = new BigViewModel();
            bigViewModel.AdsViewAdverts = db.AdsViewAdverts.Find(id);
            bigViewModel.AdsViewContact = db.AdsViewContacts.FirstOrDefault(c => c.Advert.Id == id);
            bigViewModel.AdsViewFile = db.AdsViewFile.FirstOrDefault(c => c.AdId == id);

            return View(bigViewModel);
        }

        // POST: AdsViewAdverts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id")] FormCollection adsViewAdverts, HttpPostedFileBase uploads)
        {
            //ViewBag.CategoryList = db.AdsViewCategory;
            //ViewBag.AdvertTypeList = db.AdsViewAdvertType;
            //ViewBag.LocationList = db.AdsViewLocations;
            //ViewBag.AdSubCategoryList = db.AdsViewSubCategory;

            if (ModelState.IsValid)
            {
                //db.Entry(adsViewAdverts).State = EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: AdsViewAdverts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var loggedUser = User.Identity.GetUserId();

            try
            {
                var myAdverts = db.AdsViewAdverts.Where(c => c.User == loggedUser && c.Status == 0);

                if (myAdverts != null)
                {
                    AdsViewAdverts advert = db.AdsViewAdverts.Find(id);

                    if (advert == null)
                    {
                        return HttpNotFound();
                    }

                    return View(advert);
                }
            }
            catch (Exception ex)
            {

            }

            return View();
            
        }

        // POST: AdsViewAdverts/Delete/5
        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AdsViewContact adsViewContact = db.AdsViewContacts.FirstOrDefault(c => c.Advert.Id == id);
            if (adsViewContact != null)
            {
                db.AdsViewContacts.Remove(adsViewContact);
            }
            
            AdsViewFile adsViewFile = db.AdsViewFile.FirstOrDefault(c => c.AdId == id);
            if (adsViewFile != null)
            {
                db.AdsViewFile.Remove(adsViewFile);
            }
            
            AdsViewAdverts adsViewAdverts = db.AdsViewAdverts.Find(id);
            if (adsViewAdverts != null)
            {
                db.AdsViewAdverts.Remove(adsViewAdverts);
            }            

            db.SaveChanges();

            return RedirectToAction(nameof(MyAdverts));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
