using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dataShow.Models;
using System.Data;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Web.Services;
using dataShow.ServiceReference;

namespace dataShow.Controllers
{

    
    
    public class IndexController : Controller
    {
        
        public static List<int> sn = new List<int>();

        // GET: Index
        public ActionResult Index()
        {
            
            return View();
        }

  
          // GET: Index/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Index/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: Index/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
   
        // GET: Index/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Index/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Index/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Index/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

     public List<TOP> MediaTop5()
        {

            TOP tmp = new TOP();
         
            List<TOP> list1 = new List<TOP>();
            for(int i = 0;i<5;i++)
            {
                tmp.name = ;
                tmp.sum =;
            }

            return list1;
        }
 




        //操作数据库
  
   
      

    }
}
   

