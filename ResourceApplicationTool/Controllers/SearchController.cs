using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Net.Http;
using ResourceApplicationTool.Models;
using ResourceApplicationTool.Models.SecondaryModels;
using Newtonsoft.Json;
using System.Text;
using ResourceApplicationTool.Utils;
using System.Net;

namespace ResourceApplicationTool.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Index(string query)
        {

            ViewBag.query = query;
            return View();
        }

        // GET: Search
        public JsonResult GetPartial(string query)
        {
            List<SearchResult> dummyResults = null;

            if (!String.IsNullOrEmpty(query) && query.Length >= 2 )
            {
                dummyResults = new List<SearchResult>{ new SearchResult("Albert Camus", "/Employees/Details/14", " Employee"),
                                                   new SearchResult("Albanezu Michi", "/Employees/Details/14", " Employee")};
            }
            else
            {
                dummyResults = new List<SearchResult>{ new SearchResult("Albert Camus", "/Employees/Details/14", " Employee"),
                                                   new SearchResult("Jean-Paul Sartre", "/Employees/Details/15", "Employee") };
            }

            return this.Json(dummyResults, JsonRequestBehavior.AllowGet);
        }
    }
}