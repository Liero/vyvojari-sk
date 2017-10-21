using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.BlogViewModels
{
    public class CreateViewModel
    {
        public string Title
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public string Tags
        {
            get;
            set;
        }

        public CreateViewModel()
        {

        }
    }
}
