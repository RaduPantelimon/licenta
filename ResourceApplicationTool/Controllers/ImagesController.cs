using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ResourceApplicationTool.Models;
using System.Web.UI;

namespace ResourceApplicationTool.Controllers
{
    public class ImagesController : Controller
    {
        private RATV3Entities db = new RATV3Entities();

        // GET: Images

        [Route("Images/{imgName}")]
        [OutputCache(
    Duration = 3600,
    VaryByParam = "productId",
    Location = OutputCacheLocation.Client)]
        public ActionResult ViewImage(string imgName)
        {
            List<File> fields = db.Files.Where(x => x.FileID.ToString().Length > 0).ToList();
            File fileToRetrieve = db.Files.Where(x => (x.FileID.ToString() == imgName)).FirstOrDefault();

            //if getting the file by ID did not work, we'll try to get it by its Title
            if (fileToRetrieve == null)
            {
                fileToRetrieve = db.Files.Where(x => (x.FileNumber == imgName)).FirstOrDefault();
            }
            if (fileToRetrieve != null) return File(fileToRetrieve.ItemImage, fileToRetrieve.FileNumber);

            return new EmptyResult();
        }
        [OutputCache(
    Duration = 3600,
    VaryByParam = "productId",
    Location = OutputCacheLocation.Client)]
        public ActionResult GetImg(int id)
        {
            File fileToRetrieve = db.Files.First();
            return File(fileToRetrieve.ItemImage, fileToRetrieve.FileNumber);
        }

        public ActionResult GetImgForEmployee(int? id)
        {
            if(id.HasValue)
            {
                Employee emp = db.Employees.Include(x => x.File).Where(x => x.EmployeeID == id).FirstOrDefault();

                if (emp != null && emp.File != null)
                {
                    return File(emp.File.ItemImage, emp.File.FileNumber);
                }
            }

            //if we havent found the employee or his picture, return default
            string path = Server.MapPath("/Content/Pictures/default-profile-picture.png");
            return base.File(path, "image/png");

            /*
            
                MVC: 7.6 milliseconds per photo
                Direct: 6.7 milliseconds per photo
                Note: this is the average time of a request. The average was calculated by making thousands of requests on the local machine, so the totals should not include network latency or bandwidth issues.
             
             */
        }
    }
}



