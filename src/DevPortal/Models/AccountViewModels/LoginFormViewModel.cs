using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.AccountViewModels
{
    public class LoginFormViewModel
    {
        [Required]
        [Display(Name = "User Name or Email")]
        public string UsernameOrEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
