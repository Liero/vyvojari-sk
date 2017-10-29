using DevPortal.QueryStack.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.ForumViewModels
{
    public class ForumDetailViewModel
    {
        public ForumThread Thread { get; set; }

        public NewAnswerViewModel NewAnswer {get; set;}
    }
}
