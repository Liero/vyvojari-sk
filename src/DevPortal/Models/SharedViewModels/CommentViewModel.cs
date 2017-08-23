using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.SharedViewModels
{
    public class CommentViewModel
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
    }
}
