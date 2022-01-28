using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Helperland.Models 
{
    public partial class ContactUs
    {
        public int ContactUsId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string SubjectType { get; set; }
        public string Subject { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public string UploadFileName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public int? Status { get; set; }
        public int? Priority { get; set; }
        public int? AssignedToUser { get; set; }
        public bool IsDeleted { get; set; }
    }
}
