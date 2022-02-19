using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Helperland.Models
{
    public class ForgotPassword
    {
        [Required(ErrorMessage = "New password is required", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Both password does not match")]
        public string ConfirmNewPassword { get; set; }

        [Required]
        public string ResetCode { get; set; }

    }
}