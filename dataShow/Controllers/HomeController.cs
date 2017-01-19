using dataShow.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Script.Services;


namespace dataShow.Controllers
{
 
    
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
         
              
            return View();

        }

      
        public string update()
        {
            TOP top = new TOP();
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(top);

            return json;

        }
        [HttpPost]
        public string sourcename()
        {

            using (DataShowEntities db = new DataShowEntities())
            {
                List<SourceDetail> sdlist = new List<SourceDetail>();
                var query = from s in db.SourceDetail
                            select new
                            {
                                SourceID = s.sourceID,
                                Location = s.sourceLocation
                            };
                foreach (var item in query)
                {
                    SourceDetail sd = new SourceDetail { sourceID = item.SourceID, sourceLocation = item.Location };
                    sdlist.Add(sd);
                }
                return JsonConvert.SerializeObject(sdlist);

            }

        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult helloworld(SourceDetail obj)
        {
            return Json(obj,JsonRequestBehavior.AllowGet);
        }

        public ActionResult source()
        {
            List<SourceDetail> obj = new List<SourceDetail>();
            DataShowEntities db = new DataShowEntities();
            obj = (from p in db.SourceDetail select p).ToList();
            return Json(obj);
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }

}