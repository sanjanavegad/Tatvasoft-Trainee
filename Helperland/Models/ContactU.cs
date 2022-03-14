using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Helperland.Models
{
    public partial class ContactU
    {
        public int ContactUsId { get; set; }

        [Required]
        [MaxLength (50)]
        public string Name { get; set; }

        [Required]
        [MaxLength (200)]
        public string Email { get; set; }

        [MaxLength(500)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Message { get; set; }

        [MaxLength(100)]
        public string UploadFileName { get; set; }

        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public string FileName { get; set; }

        
    }
}
