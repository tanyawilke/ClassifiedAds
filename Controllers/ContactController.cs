using System.Threading.Tasks;
using System.Web.Mvc;
using ClassifiedAdsApp.Models;
using Microsoft.AspNet.Identity;
using System.Net.Http;
using System.Net.Mail;
using System.Configuration;
using System;
using System.Net.Mime;
using System.Text;

namespace ClassifiedAdsApp.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        [AllowAnonymous]
        public ActionResult Contact()
        {
            var contactModel = new Contact { };
            return View(contactModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Contact(FormCollection formCollection)
        {
            var contactModel = new Contact { };
            
            if (ModelState.IsValid)
            {
                MailMessage mailMsg = new MailMessage();

                // To
                mailMsg.To.Add(new MailAddress("tanya.wilke@gmail.com", "To FreeAds"));

                // From
                mailMsg.From = new MailAddress(formCollection["email"].ToString(), "From");

                // Subject and multipart/alternative Body
                mailMsg.Subject = "FreeAds enquiry";
                StringBuilder mailBody = new StringBuilder();
                mailBody.Append("<p>Name and surname: " + formCollection["FirstName"] + " " + formCollection["Surname"]);
                mailBody.Append("<p>Email address: " + formCollection["Email"]);
                mailBody.Append("<p>Telephone: " + formCollection["Phone"]);
                mailBody.Append("<p>Enquiry:<br/>" + formCollection["Body"]);
                string html = mailBody.ToString();
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(mailBody.ToString(), null, MediaTypeNames.Text.Plain));
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                // Init SmtpClient and send
                SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["mailAccount"], ConfigurationManager.AppSettings["mailPassword"]);
                smtpClient.Credentials = credentials;

                try
                {
                    smtpClient.Send(mailMsg);

                    return View("ThankYou");
                }
                catch (Exception ex)
                {
                    return View("Error: " + ex.Message);
                }
            }

            return View();
        }

        public ActionResult ThankYou()
        {
            return View();
        }
    }
}