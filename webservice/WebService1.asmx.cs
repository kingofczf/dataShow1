
using dataShow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Serialization;
using webservice;

namespace webservice
{
    /// <summary>
    /// WebService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    [Serializable]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public List<SOURCE> GetSource()
        {
            DataShowEntities db = new DataShowEntities();
            List<SOURCE> list1 = (from p in db.SOURCE select p).ToList();
            return list1;
        }
        [WebMethod]
        public List<SourceDetail>  GetSourceDetail()
        {
            DataShowEntities db = new DataShowEntities();
            List<SourceDetail> list1 = (from p in db.SourceDetail select p).ToList();
            return list1;
        }
        [WebMethod]
        public List<TIMES> GetTimes()
        {
            DataShowEntities db = new DataShowEntities();
            List<TIMES> list1 = (from p in db.TIMES select p).ToList();
            return list1;
        }
        [WebMethod]
        public List<MAC> GetMac()
        {
            DataShowEntities db = new DataShowEntities();
            List<MAC> list1 = (from p in db.MAC select p).ToList();
            return list1;
        }
        [WebMethod]
        public List<TOP> MediaTop5()
        {
            List<TOP> list1 = new List<TOP>();

            return list1;
        }
        [WebMethod]
        public List<TOP> AreaTop5()//地点排行表，不需要考虑区域因素
        {
            List<TOP> list1 = new List<TOP>();
            TOP data = new TOP();
            DataShowEntities db = new DataShowEntities();
            List<SOURCE> tmp = new List<SOURCE>();
            for(int i=0;i<GetSourceDetail().Count;i++)
            { int no = GetSourceDetail().ElementAt(i).sourceID;
                tmp = (from p in db.SOURCE where p.sourceID == no select p).ToList();
                data.name = no.ToString();
                data.sum = tmp.Count;
                list1.Add(data);
            }
            sortTop(list1);                 
            return list1;
        }
        [WebMethod]
        public List<SUMTIMES> SumStayTimeTop()//总停留时间排行榜,按照信息源统计
        {
            List<SUMTIMES> list1 = new List<SUMTIMES>();
            SUMTIMES data = new SUMTIMES();
            DataShowEntities db = new DataShowEntities();
            List<SOURCE> tmp = new List<SOURCE>();
            int sum = 0;
            for(int i=0;i<GetSourceDetail().Count;i++)//统计TIMES中的停留
            {
                int no = GetSourceDetail().ElementAt(i).sourceID;
                tmp = (from p in db.SOURCE where p.sourceID ==no select p).ToList();
                for(int j =0;j<tmp.Count;j++)
                {
                    sum += tmp.ElementAt(j).stayTime;
                }
                data.sum = sum;
                data.sourceID = no;
                list1.Add(data);

            }
            sortTimes(list1);
                return list1 ;
        }
        [WebMethod]
        public List<SUMTIMES> AvgStayTimeTop()//平均停留时间，按照信息源统计
        {
            List<SUMTIMES> list1 = SumStayTimeTop();
            DataShowEntities db = new DataShowEntities();
            for(int i= 0;i<list1.Count;i++)
            {
                int no = list1[i].sourceID;
                int num = (from p in db.SOURCE where p.sourceID == no select p).Count();
                list1[i].sum = list1[i].sum / num;
            }
            return list1;
        }
        [WebMethod]
        public List<TOP> MediaSum()//媒体总人数,按照信息源点统计
        {
            DataShowEntities db = new DataShowEntities();
            List<TOP> list1 = new List<TOP>();
            TOP tmp = new TOP();
            for (int i = 0; i < GetSourceDetail().Count; i++)
            {
                int no = GetSourceDetail().ElementAt(i).sourceID;
                int data = (from p in db.SOURCE where p.sourceID==no group p by p.macAddress into p select p).ToList().Count;
                tmp.sum = data;
                tmp.name = no.ToString();
                list1.Add(tmp);
            }
                return list1;
        }
        [WebMethod]
        public List<TOP> AreaSum()//地点总人数,按照信息源所在位置统计
        {
            DataShowEntities db = new DataShowEntities();
            List<TOP> list1 = new List<TOP>();
            TOP tmp = new TOP();
            int data = 0;
            var q = (from p in db.SourceDetail
                     group p by p.sourceLocation into g
                     select new { g.Key, num = g.Count() }).ToList();
            List<int> id = new List<int>();
            for (int i = 0; i < q.Count; i++)
            {
                string no = q.ElementAt(i).Key;
                id = (from p in db.SourceDetail where p.sourceLocation == no select p.sourceID).ToList();
                for(int j=0;j<id.Count;j++)
                 data+=(from p in db.SOURCE where p.sourceID==id[j]  group p by p.macAddress into p select p).ToList().Count;
                tmp.name = no;
                tmp.sum = data;
                list1.Add(tmp);
            }
                return list1;
        }
        /*************当天登录人数,统计当前日期************/
        public int LoginNum()
        {
            DataShowEntities db = new DataShowEntities();
            int data = (from p in db.MAC where p.datatime == DateTime.Now select p).ToList().Count;
            return data;
        }
        /******************历史登录人数 ***********************************/

        /*****TOP排序函数****/
        public void sortTop(List<TOP> obj)
        {

            TOP tmp = new TOP();
            for(int i =0;i<obj.Count;i++)//使用冒泡排序法
            {
                for(int j = i;j<obj.Count;j++)
                {
                    if(obj.ElementAt(j).sum<obj.ElementAt(j+1).sum)
                    {
                        tmp = obj.ElementAt(j);
                        obj[j] = obj[j + 1];
                        obj[j + 1] = tmp;                       
                    }
                }
            }
        }
        /*******SUMTIMES排序函数********/
        public void sortTimes(List<SUMTIMES> obj)
        {
            SUMTIMES tmp = new SUMTIMES();
            for (int i = 0; i < obj.Count; i++)//使用冒泡排序法
            {
                for (int j = i; j < obj.Count; j++)
                {
                    if (obj.ElementAt(j).sum < obj.ElementAt(j + 1).sum)
                    {
                        tmp = obj.ElementAt(j);
                        obj[j] = obj[j + 1];
                        obj[j + 1] = tmp;
                    }
                }
            }
        }

    }
}
