using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Services;


namespace dataShow.Controllers
{
 
    
    public class HomeController : Controller
    {

        public ActionResult Index()
        {

            var person = new List<string>
            {
                "lala",
                "20",
                "g"
            };
            ViewData["Person"] = person;
            return View();

        }

       
        public string update()
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject("lalala");

            return json;

        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}