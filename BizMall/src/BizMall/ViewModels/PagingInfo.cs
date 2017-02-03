using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleList.Models.CommonModels
{
    public class PagingInfo
    {
        public string hashtag { get; set; } 

        public string searchstring { get; set; }

        public string CategoryEnTitleForLink { get; set; }

        public int TotalItems { get; set; }

        public int ItemsPerPage { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage); }
        }
    }
}
