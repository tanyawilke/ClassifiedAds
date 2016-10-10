using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ClassifiedAdsApp.Models
{
    public class AdsViewAdverts
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        //public IEnumerable<SelectListItem> Categories { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual AdsViewCategory Category { get; set; }
        //{
        //    get {
        //        //IEnumerable<SelectListItem> items = db.AdsViewCategory.Select(c => new SelectListItem
        //        //{
        //        //    Value = c.CategoryId.ToString(),
        //        //    Text = c.Description

        //        //});

        //        //return (AdsViewCategory)items;
        //    }

        //    set { }
        //}

        [Required]
        [Display(Name = "Sub category")]
        public int SubCategoryId { get; set; }

        [ForeignKey(nameof(SubCategoryId))]
        public virtual AdsViewSubCategory SubCategory { get; set; }
        
        public int Status { get; set; }
        
        [Display(Name = "Last modified date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? DateLastModified { get; set; }

        [Required]
        [Display(Name = "Publish date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime DatePublished { get; set; }

        [Required]
        [Display(Name = "Expiry date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Advert type")]
        public int AdvertTypeId { get; set; }

        [ForeignKey(nameof(AdvertTypeId))]
        public virtual AdsViewAdvertType AdvertType { get; set; }

        [Required]
        [MaxLength(600)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public AdsViewAdverts()
        {
            Price = Convert.ToDecimal(0.00);
        }

        [Range(0, 100), DataType(DataType.Currency)]
        public decimal Price { get; set; }        
        
        [Display(Name = "Date advert added")]
        public DateTime? DateAdvertAdded { get; set; }

        [Required]
        [Display(Name = "Location")]
        public int LocationId { get; set; }

        [ForeignKey(nameof(LocationId))]
        public virtual AdsViewLocations Location { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfDeleteAdvert { get; set; }

        [Required]
        public string User { get; set; }

        [ForeignKey(nameof(User))]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [MaxLength(10)]
        public string ConfirmationCode { get; set; }
        
        //public AdsViewFile AdsViewFileId { get; set; }
        //public AdsViewContact ContactId { get; set; }
    }

    public class AdsViewContact
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactId { get; set; }
        [Required]
        public int Id { get; set; }

        [ForeignKey(nameof(Id))]
        public virtual AdsViewAdverts Advert { get; set; }
        [MaxLength(50)]
        [DataType(DataType.Text)]
        [Display(Name = "Name of contact")]
        public string ContactName { get; set; }
        [MaxLength(50)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone number of contact")]
        public string ContactPhoneNumber { get; set; }
        [MaxLength(100)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Contact email address")]
        public string ContactEmail { get; set; }
        [MaxLength(300)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Physical address of contact")]
        public string ContactPhysicalAddress { get; set; }
        [MaxLength(100)]
        [DataType(DataType.Url)]
        [Display(Name = "Web site")]
        public string WebSite { get; set; }
    }

    public class AdsViewAdvertType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Display(Name = "Advert type")]
        public int AdvertTypeId { get; set; }
        [Required]
        [MinLength(1), MaxLength(50)]
        [DataType(DataType.Text)]
        public string Description { get; set; }
    }

    public class AdsViewSubCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Display(Name = "Sub Category")]
        public int SubCategoryId { get; set; }
        [Required]
        [MinLength(1), MaxLength(50)]
        [DataType(DataType.Text)]
        public string Description { get; set; }
        [Required]
        public int Status { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }

    public class AdsViewFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int FileId { get; set; }
        [StringLength(400)]
        public string Filename { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public FileType FileType { get; set; }
        public int AdId { get; set; }
        [ForeignKey(nameof(AdId))]
        public virtual AdsViewAdverts AdViewAdverts { get; set; }
    }

    public enum FileType
    {
        Avatar = 1, Photo
    }

    public class AdsViewLocations
    {
        [Required]
        [MinLength(1), MaxLength(50)]
        [DataType(DataType.Text)]
        public string Description { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Display(Name = "Advert location")]
        public int LocationId { get; set; }
    }

    public class AdsViewCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [Required]
        [MinLength(1), MaxLength(50)]
        [DataType(DataType.Text)]
        public string Description { get; set; }
        [Required]
        public int Status { get; set; }
    }

    //public virtual ICollection<ProjectResource> ProjectResources { get; protected set; }
    //public override String ToString() { return (this.Name); }
}