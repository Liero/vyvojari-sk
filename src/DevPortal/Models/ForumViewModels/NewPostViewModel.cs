using DevPortal.Web.Models.SharedViewModels;
using System.ComponentModel.DataAnnotations;

namespace DevPortal.Web.Models.ForumViewModels
{
    public class NewPostViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required, MinLength(1, ErrorMessage = "Specify at least one tag")]
        public string Tags { get; set; } = "";

        [Required, StringLength(int.MaxValue, MinimumLength = 30, ErrorMessage = "Be more specific")]
        public string Content { get; set; }
    }
}