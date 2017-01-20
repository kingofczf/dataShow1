
using dataShow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;



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
        public List<TOP> MediaTop5()//信息源点采集，人数排行表
        {
            List<TOP> obj = MediaSum();
            sortTop(obj);
            return obj;
        }
        [WebMethod]
        public List<TOP> AreaTop5()//地点排行表，不需要考虑区域因素
        {
           List<TOP> obj =  AreaSum();
            sortTop(obj);
            return obj;
        }
        [WebMethod]
        public List<SUMTIMES> SumStayTimeTopSource()//总停留时间排行榜,按照信息源统计
        {
            List<SUMTIMES> list1 = new List<SUMTIMES>();           
            DataShowEntities db = new DataShowEntities();
            List<SOURCE> tmp = new List<SOURCE>();
            int sum = 0;
            int no;
            for(int i=0;i<GetSourceDetail().Count;i++)//统计TIMES中的停留
            {
                SUMTIMES data = new SUMTIMES();
                no = GetSourceDetail()[i].sourceID;
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
        [WebMethod ]
        public List<TOP> SumStayTimeTopArea()//总停留时间排行榜,按照区域统计
        {
            List<TOP> list1 = new List<TOP>();         
            DataShowEntities db = new DataShowEntities();
            List<SOURCE> tmp = new List<SOURCE>();
            var q = (from p in db.SourceDetail
                     group p by p.sourceLocation into g
                     select new { g.Key, num = g.Count() }).ToList();
            int sum = 0;
            for (int i = 0; i <q.Count; i++)//统计TIMES中的停留
            {
               string tmpNo = q.ElementAt(i).Key;
                TOP data = new TOP();
                List<int> s = (from p in db.SourceDetail where p.sourceLocation == tmpNo select p.sourceID).ToList();
                for (int j = 0; j < s.Count; j++)
                {
                    int tmpNo1 = s.ElementAt(j);
                    tmp = (from p in db.SOURCE where p.sourceID == tmpNo1  select p).ToList();

                    for (int k = 0; k < tmp.Count; k++)
                    {
                        sum += tmp.ElementAt(k).stayTime;
                    }

                }
                data.sum = sum;
                data.name = q[i].Key;
                list1.Add(data);

            }
            sortTop(list1);
            return list1;
        }

        [WebMethod]
        public List<SUMTIMES> AvgStayTimeTopSource()//平均停留时间，按照信息源统计
        {
            List<SUMTIMES> list1 = SumStayTimeTopSource();
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
        public List<TOP> AvgStayTimeTopArea()//平均停留时间，按照区域统计
        {
            List<TOP> list1 = SumStayTimeTopArea();
            DataShowEntities db = new DataShowEntities();
            var q = (from p in db.SourceDetail
                     group p by p.sourceLocation into g
                     select new { g.Key, num = g.Count() }).ToList();
            int num = 0;
            for (int i = 0; i < list1.Count; i++)
            {
                string tmpNo = q[i].Key;
                List<int> s = (from p in db.SourceDetail where p.sourceLocation == tmpNo select p.sourceID).ToList();
                for (int j = 0; j < s.Count; j++)
                {
                    int no = s[j];
                    num += (from p in db.SOURCE where p.sourceID == no select p).Count();

                }
                list1[i].sum = list1[i].sum / num;
            }
            return list1;
        }
        [WebMethod]
        public List<TOP> MediaSum()//媒体总人数,按照信息源点统计
        {
            DataShowEntities db = new DataShowEntities();
            List<TOP> list1 = new List<TOP>();           
            List<SourceDetail> gsd = GetSourceDetail();
            int no;
            for (int i = 0; i < GetSourceDetail().Count; i++)
            {
                TOP tmp = new TOP();
                no = gsd.ElementAt(i).sourceID;
                int data = (from p in db.SOURCE where p.sourceID==no group p by p.macAddress into p select p).ToList().Count;
                tmp.sum = data;
                tmp.name = no.ToString();
                list1.Add(tmp);
            }
                return list1;
        }
        [WebMethod]
        public List<TOP> AreaSum()//地点总人数,按照区域统计
        {
            DataShowEntities db = new DataShowEntities();
            List<TOP> list1 = new List<TOP>();
            int data = 0;
            var q = (from p in db.SourceDetail
                     group p by p.sourceLocation into g
                     select new { g.Key, num = g.Count() }).ToList();
            List<int> id = new List<int>();
            for (int i = 0; i < q.Count; i++)
            {
                TOP tmp = new TOP();
                string no = q.ElementAt(i).Key;
                id = (from p in db.SourceDetail where p.sourceLocation == no select p.sourceID).ToList();
                for (int j = 0; j < id.Count; j++)
                {
                    int tmpNo = id[j];
                    int n = (from p in db.SOURCE where p.sourceID == tmpNo group p by p.macAddress into p select p.Key).ToList().Count;
                    data += n;
                }
                tmp.name = no;
                tmp.sum = data;
                list1.Add(tmp);
            }
                return list1;
        }
        [WebMethod]    
        public List<LoginTimes> LoginTimeSumSource()//登录次数信息源统计
        {
            List<LoginTimes> list1 = new List<LoginTimes>();
            DataShowEntities db = new DataShowEntities();
            //按照次数统计，为1，2，3.。。
          
            for(int i=0;i<GetSourceDetail().Count;i++)
            {
                LoginTimes tmp = new LoginTimes();
                tmp.name = GetSourceDetail()[i].sourceID.ToString();
                int tmpNo = GetSourceDetail()[i].sourceID;
               var n =  (from p in db.SOURCE where p.sourceID==tmpNo
                        group p by p.num into q select new { q.Key, sum = q.Count() }).ToList();
                for(int j=0;j<n.Count;j++)
                {
                    tmp.times = n.ElementAt(j).Key;
                    tmp.num = n.ElementAt(j).sum;
                }
                list1.Add(tmp);//存储格式为信息源/登录次数/总共多少人登录这个次数，去重时针对登录次数，前两个数据相同的情况下删除
            }
            LoginNumCheck(list1);
            return list1;
        }
        [WebMethod]
        public List<LoginTimes> LoginTimeArea()//登录次数区域统计
        {
            List<LoginTimes> list1 = new List<LoginTimes>();            
            DataShowEntities db = new DataShowEntities();
            var q = (from p in db.SourceDetail
                     group p by p.sourceLocation into g
                     select new { g.Key, num = g.Count() }).ToList();
            for (int i = 0; i < q.Count; i++)
            {
                string tmpNo =q[i].Key;
                List<int> s = (from p in db.SourceDetail where p.sourceLocation == tmpNo select p.sourceID).ToList();
                for (int j = 0; j < s.Count; j++)
                {
                    LoginTimes tmp = new LoginTimes();
                    int tmpNo1 = s.ElementAt(j);
                    var n = (from p in db.SOURCE
                            where p.sourceID == tmpNo1
                            group p by p.num into info
                            select new { info.Key, sum = info.Count() }).ToList();
                    for (int k = 0; k < n.Count; k++)
                    {
                        tmp.times = n.ElementAt(k).Key;
                        tmp.num = n.ElementAt(k).sum;
                        tmp.name = q[i].Key;
                    }
                    list1.Add(tmp);//存储格式为信息源/登录次数/总共多少人登录这个次数，去重时针对登录次数，前两个数据相同的情况下删除
                }
                      
            }
            LoginNumCheck(list1);
            return list1;
        }
        /**************登录次数去重函数*******************/
        public void LoginNumCheck(List<LoginTimes> obj)
        {
            //比如为两次，则一次的数据包括了第二次登录，则在第j-1次中减去第j次的数量,从小到大减
            for(int i=0;i<obj.Count;i++)
            {
                for(int j=i+1;j<obj.Count;j++)
                {
                    if(obj[i].name==obj[j].name&&obj[i].times==obj[j].times+1)
                    {
                        obj[i].num -= obj[j].num;
                    }
                }
            }
        }
        /*****TOP排序函数****/
        public void sortTop(List<TOP> obj)
        {

            TOP tmp = new TOP();
            for(int i =0;i<obj.Count;i++)//使用冒泡排序法
            {
                for(int j = i+1;j<obj.Count;j++)
                {
                    if(obj.ElementAt(i).sum<obj.ElementAt(j).sum)
                    {
                        tmp = obj.ElementAt(i);
                        obj[i] = obj[j];
                        obj[j] = tmp;                       
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
                for (int j = i+1; j < obj.Count; j++)
                {
                    if (obj.ElementAt(i).sum < obj.ElementAt(j).sum)
                    {
                        tmp = obj.ElementAt(i);
                        obj[i] = obj[j];
                        obj[j] = tmp;
                    }
                }
            }
        }

    }
}
