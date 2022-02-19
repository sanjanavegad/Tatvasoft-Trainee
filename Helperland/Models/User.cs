using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Helperland.Models
{
    public partial class User
    {
        public User()
        {
            FavoriteAndBlockedTargetUsers = new HashSet<FavoriteAndBlocked>();
            FavoriteAndBlockedUsers = new HashSet<FavoriteAndBlocked>();
            RatingRatingFromNavigations = new HashSet<Rating>();
            RatingRatingToNavigations = new HashSet<Rating>();
            ServiceRequestServiceProviders = new HashSet<ServiceRequest>();
            ServiceRequestUsers = new HashSet<ServiceRequest>();
            UserAddresses = new HashSet<UserAddress>();
        }

        public int UserId { get; set; }

        [Required(ErrorMessage = "Please enter your first name")]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name")]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        [MaxLength(100)]
        [EmailAddress(ErrorMessage ="Please Enter valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your password password")]
        [DataType(DataType.Password)]
        [RegularExpression("([a-z]|[A-Z]|[0-9]|[\\W]){4}[a-zA-Z0-9\\W]{4,16}", ErrorMessage = "Password must contain at least 1 capital letter, 1 small letter, 1 number and one special character")]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match")]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }

        

        [Required(ErrorMessage = "Please enter your mobile number")]
        [MaxLength(20)]
        public string Mobile { get; set; }

        public int UserTypeId { get; set; }

        public int? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string UserProfilePicture { get; set; }
        public bool IsRegisteredUser { get; set; }
        public string PaymentGatewayUserRef { get; set; }

        [Required(ErrorMessage = "Please enter ZipCode")]
        [MaxLength(10)]
        public string ZipCode { get; set; }
        public bool WorksWithPets { get; set; }
        public int? LanguageId { get; set; }
        public int? NationalityId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int? Status { get; set; }
        public string BankTokenId { get; set; }
        public string TaxNo { get; set; }

        public string ResetPasswordCode { get; set; }

        public virtual ICollection<FavoriteAndBlocked> FavoriteAndBlockedTargetUsers { get; set; }
        public virtual ICollection<FavoriteAndBlocked> FavoriteAndBlockedUsers { get; set; }
        public virtual ICollection<Rating> RatingRatingFromNavigations { get; set; }
        public virtual ICollection<Rating> RatingRatingToNavigations { get; set; }
        public virtual ICollection<ServiceRequest> ServiceRequestServiceProviders { get; set; }
        public virtual ICollection<ServiceRequest> ServiceRequestUsers { get; set; }
        public virtual ICollection<UserAddress> UserAddresses { get; set; }
    }
}