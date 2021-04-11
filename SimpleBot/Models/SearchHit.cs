using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBot.Models
{
    public class SearchHit
    {
        public SearchHit()
        {
            this.PropertyBag = new Dictionary<string, object>();
        }
        public string Key { get; set; }
        public string Title { get; set; }
        public string PictureUrl { get; set; }
        public string Description { get; set; }
        public IDictionary<string, object> PropertyBag { get; set; }
    }
}
