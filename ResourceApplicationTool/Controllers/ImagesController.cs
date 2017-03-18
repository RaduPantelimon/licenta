using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ResourceApplicationTool.Models;

namespace ResourceApplicationTool.Controllers
{
    public class ImagesController : Controller
    {
        private RATV3Entities db = new RATV3Entities();

        // GET: Images

        [Route("Images/{imgName}")]
        public ActionResult ViewImage(string imgName)
        {
            List<File> fields = db.Files.Where(x => x.FileID.ToString().Length > 0).ToList();
            File fileToRetrieve = db.Files.Where(x => (x.FileNumber == imgName)).FirstOrDefault();
            if(fileToRetrieve != null) return File(fileToRetrieve.ItemImage, fileToRetrieve.FileNumber);

            return new EmptyResult();
        }
        public ActionResult GetImg(int id)
        {
            File fileToRetrieve = db.Files.First();
            return File(fileToRetrieve.ItemImage, fileToRetrieve.FileNumber);
        }
    }
}