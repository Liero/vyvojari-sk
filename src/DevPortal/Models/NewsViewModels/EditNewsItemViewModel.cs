using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.NewsViewModels
{
    public class EditNewsItemViewModel : CreateNewsItemViewModel
    {
        public Guid Id { get; set; }
    }
}
