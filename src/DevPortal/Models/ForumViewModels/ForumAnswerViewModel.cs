using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.ForumViewModels
{
    public class ForumQuestionPostViewModel : ForumPostViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
       
        public DateTime LastPost { get; set; }

        public int Views { get; set; }

        public int Answers { get; set; }

        public string[] Tags { get; set; }

    }
}
