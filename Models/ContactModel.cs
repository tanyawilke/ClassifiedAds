using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClassifiedAdsApp.Models
{
    [MetadataType(typeof(ContactMetadata))]
    public class Contact
    {
        [DisplayName("First name")]
        [Required(ErrorMessage = "A first name is required."), StringLength(60, MinimumLength = 3)]
        public string FirstName { get; set; }

        [DisplayName("Surname")]
        [Required(ErrorMessage = "A surname is required."), StringLength(60, MinimumLength = 3)]
        public string Surname { get; set; }

        [DisplayName("Email address")]
        [Required(ErrorMessage = "A valid email address is required."), StringLength(60, MinimumLength = 3)]
        public string Email { get; set; }

        [DisplayName("Question / Enquiry")]
        [Required(ErrorMessage = "Please state your question or enquiry."), StringLength(1000, MinimumLength = 3)]
        public string Body { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }

        [DisplayName("Telephone number")]
        public string Phone { get; set; }
    }

    public class ContactMetadata
    {
    }
}