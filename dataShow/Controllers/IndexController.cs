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


namespace dataShow.Controllers
{
    public class IndexController : Controller
    {
        public static List<int> sn = new List<int>();

        // GET: Index
        public ActionResult Index()
        {     
            var sourceDetail = new List<SourceDetail>(FindSD());
            var source = new List<SOURCE>(FindS());
            var mac = new List<MAC>(FindM());
            var time = new List<TIMES>(FindT());
            var mtop = new List<TOP>(FindMTop());
            var ltop = new List<TOP>(FindLTop());
            var stime =new List<SUMTIMES>();
            var atime = new List<SUMTIMES>();
            dealTime(stime);
            dealTime(atime);
            ViewData["sourceDetail"] = sourceDetail;
            ViewData["source"] =source ;
            ViewData["time"] = time;
            ViewData["mac"] = mac;
            //统计人数排行
            ViewData["MediaTop"] = mtop;
            ViewData["LocationTop"] = ltop;
            //计算停留时间
            ViewData["SumTime"] = stime;
            ViewData["AvgTime"] = atime;//按照信息源填信息

            return View();
        }

        public void dealTime(List<SUMTIMES> t)
        {
            for (int i = 0; i < showS().Count; i++)//按照信号源为点统计，得到结果为对应点和对应的总停留时间
            {
                SUMTIMES s = new SUMTIMES();
                SUMTIMES a = new SUMTIMES();
                s.sourceID = showS().ElementAt(i).sourceID;
                s.sum = SumTime(s.sourceID);
                int mn = SumMac(s.sourceID);
                a.sourceID = s.sourceID;
                a.sum = s.sum / mn;
                t.Add(s);
            }
            sortTime(t);
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

     
        public static List<SourceDetail> FindSD()
        {
            List<SourceDetail> obj = null;
            DataShowEntities db = new DataShowEntities();
            obj = (from p in db.SourceDetail select p).ToList();
            return obj;
        }
        
        public static List<MAC> FindM()
        {
            List<MAC> obj = null;
            DataShowEntities db = new DataShowEntities();
            obj = (from p in db.MAC select p).ToList();
            return obj;
        }

        public static List<TIMES> FindT()
        {
            List<TIMES> obj = null;
            DataShowEntities db = new DataShowEntities();
            obj = (from p in db.TIMES select p).ToList();
            return obj;
        }

        public static List<SOURCE> FindS()
        {
            List<SOURCE> obj = new List<SOURCE>();
            DataShowEntities db = new DataShowEntities();
            obj = (from p in db.SOURCE select p).ToList();
            return obj;
        }
        //操作数据库
        public static List<int> SumSource()
        {
 
            DataShowEntities db = new DataShowEntities();
            List<int> list1 = (from p in db.SourceDetail select p.sourceID).ToList();
            return list1;
        }
        public static List<TOP> FindMTop()
        {
            List<SourceDetail> tmp = showS();
            List<TOP> obj = new List<TOP>();           
            DataShowEntities db = new DataShowEntities();
            for (int i = 0; i < showS().Count; i++)
            {
                TOP s = new TOP();
                int d = tmp[i].sourceID;  
                s.sum = (from p in db.SOURCE where p.sourceID == d select p).Count();
                s.name = tmp[i].sourceID.ToString();
                obj.Add(s);
            }           
            sort(obj);   
            return obj;
        }
        public static void sort(List<TOP> t)
        {
            TOP tmp = new TOP();
            for (int i = 0; i < t.Count; i++)
            {
                for (int j = i; j < t.Count; j++)
                {
                    if (t[i].sum < t[j].sum)
                    {
                        tmp = t[i];
                        t[i] = t[j];
                        t[j] = tmp;
                    }
                }

            }
        }
        public static  void sortTime(List<SUMTIMES> t)
        {
            List<SUMTIMES> showTime = new List<SUMTIMES>();
            for (int i = 0; i < t.Count; i++)
            {
                if (t[i].sourceID == (showTime.Count + 1))
                {
                    SUMTIMES tmp = new SUMTIMES();
                    tmp.sourceID = t[i].sourceID;
                    tmp.sum = t[i].sum;
                    showTime.Add(tmp);
                }
            }
            t = showTime;
        }
        public static List<TOP> FindLTop()
        {
            List<TOP> obj = new List<TOP>(SumSource().Count);
            TOP s = new TOP();
            DataShowEntities db = new DataShowEntities();
            for (int i = 0; i < SumSource().Count; i++)
            {
                int d = SumSource().ElementAt(i);
                s.sum = (from p in db.SOURCE where p.sourceID == d select p).Count();
                s.name = SumSource().ElementAt(i).ToString();
                obj.Add(s);
            }
           sort(obj);
            return obj;
        }
        public static int SumTime(int sourceID)//统计总停留时间，按照信息源分开
        {
            int num = 0;
            DataShowEntities db = new DataShowEntities();
           List<SOURCE> tmp  = (from p in db.SOURCE where p.sourceID == sourceID select p).ToList();//统计数量
            for(int i  =0;i<tmp.Count;i++)
            {
                int d = tmp.ElementAt(i).stayTime;
                num += d;
            }
            return num;
        }
        public static int SumMac()//统计所有数据源的流量,显示日流量
        {
            DataShowEntities db = new DataShowEntities();
            int num = 0;
            List<string> list1 = (from p in db.MAC select p.macaddress).Distinct().ToList();
            num = list1.Count;
            return num;
        }//显示到目前为止所有信息源的总流量(日结)

        public static int SumMac(int si)//显示到当前为止某个信息源的总流量,si为信息源
        {
            DataShowEntities db = new DataShowEntities();
            int num = 0;
            List<string> list1 = (from p in db.SOURCE
                                  where p.sourceID == si
                                  select p.macAddress).Distinct().ToList();//显示一号
            num = list1.Count;
            return num;
        }
        public static int SumMac(DateTime bt, DateTime et)//显示一段时间内所有信息源的总流量
        {
            int num = 0;
            DataShowEntities db = new DataShowEntities();
            List<string> list1 = (from p in db.SOURCE
                                  where p.datatime >= bt
              && p.datatime <= et
                                  select p.macAddress).Distinct().ToList();
            num = list1.Count;
            return num;
        }
        public static int SumMac(int b, int e)//显示某段信息源集合的总流量
        {   
            int num = 0;
            DataShowEntities db = new DataShowEntities();
            List<SOURCE> list1 = (from p in db.SOURCE where p.sourceID >= b && p.sourceID <= e select p).ToList();//查询两个点
            num = list1.Count;
            return num;
        }
        public static int SumMac(int si, DateTime bt, DateTime et)//显示一段时间内某个信息源的总流量
        {
            int num=0;
            DataShowEntities db = new DataShowEntities();
            List<string> list1 = (from p in db.MAC where p.sourceID == si && p.datatime >= bt
                                  && p.datatime <= et select p.macaddress).Distinct().ToList();
            num = list1.Count;
            return num;
        }
        public static List<TIMES> ShowMacTimes(string m)
        {

            DataShowEntities db = new DataShowEntities();
            List<TIMES> list1 = (from p in db.TIMES where p.macAddress == m select p).ToList();
            return list1;
        } //显示某个MAC的访问时间
         public static List<SOURCE> ShowMacSource(string m)//显示某个MAC的访问地点
        {
            DataShowEntities db = new DataShowEntities();
            List<SOURCE> list1 = (from p in db.SOURCE where p.macAddress == m select p).ToList();
            return list1;
        }
        public static List<SourceDetail> showS()
        {
            DataShowEntities db = new DataShowEntities();
            List<SourceDetail> list1 = (from p in db.SourceDetail select p).ToList();
            return list1;
        }


    }
}
   

