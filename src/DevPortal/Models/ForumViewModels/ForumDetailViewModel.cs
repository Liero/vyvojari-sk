using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.ForumViewModels
{
    public class ForumDetailViewModel
    {
        public ForumQuestionPostViewModel Question { get; set; }

        public List<ForumPostViewModel> Answers { get; set; }

        public NewAnswerViewModel NewAnswer {get; set;}
    }
}
