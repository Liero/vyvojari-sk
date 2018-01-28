using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.ManageViewModels
{
    public class IndexViewModel
    {
        [DisplayName("Meno / Prezývka")]
        public string Username { get; set; }

        [DisplayName("Krátky popis")]
        public string ShortDescription { get; set; }

        [DisplayName("O mne")]
        public string AboutMe { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Telefónne číslo")]
        public string PhoneNumber { get; set; }

        public string StatusMessage { get; set; }

        public string AvatarUrl { get; set; }
    }
}
