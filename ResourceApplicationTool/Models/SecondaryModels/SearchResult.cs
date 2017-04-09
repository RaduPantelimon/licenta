using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResourceApplicationTool.Models.SecondaryModels
{
    public class SearchResult
    {
        public SearchResult(string _name, string _url, string _type) {
            name = _name;
            url = _url;
            type = _type;
        }
        public string name { get; set; }
        public string url { get; set; }
        public string type { get; set; }
    }
}