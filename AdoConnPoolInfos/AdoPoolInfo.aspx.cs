using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AdoConnPoolInfos
{
    //https://msdn.microsoft.com/zh-tw/library/ms254503(v=vs.110).aspx
    public partial class AdoPoolInfo : System.Web.UI.Page
    {
   
        private static string connStr = @"Application Name=RMtest123;Server={你的server};Database={db};User Id={useraccount};Password={password}
;Min Pool Size=50;Max Pool Size=200;";


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if(!Page.IsPostBack) SetUpPerformanceCounters();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            WritePerformanceCounters();
        }

        /// <summary>
        /// 存放 PerformanceCounter
        /// </summary>
        public List<PerformanceCounter> PerfCounters {
            get
            {
                return Application["PerfCounters"] as List<PerformanceCounter>;
            }
            set
            {
                Application["PerfCounters"] = value;
            }

        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int GetCurrentProcessId();

        /// <summary>
        /// 取得程式的 instanceName 
        /// </summary>
        /// <returns></returns>
        private string GetInstanceName()
        {
            //This works for Winforms apps.
            //string instanceName =
            //    System.Reflection.Assembly.GetEntryAssembly().GetName().Name;


            // For ASP.NET applications your instanceName will be your CurrentDomain's 
            // FriendlyName. Replace the line above that sets the instanceName with this:
            var instanceName = AppDomain.CurrentDomain.FriendlyName.ToString().Replace('(', '[')
            .Replace(')', ']').Replace('#', '_').Replace('/', '_').Replace('\\', '_');

            string pid = GetCurrentProcessId().ToString();
            instanceName = instanceName + "[" + pid + "]";
            Console.WriteLine("Instance Name: {0}", instanceName);
            Console.WriteLine("---------------------------");
            Response.Write($"Instance Name:{instanceName} ");
            return instanceName;
        }


        /// <summary>
        /// 設定要查看的 PerformanceCounter
        /// </summary>
        private void SetUpPerformanceCounters()
        {
            if (PerfCounters == null)
            {
                PerfCounters = new List<PerformanceCounter>();
                string instanceName = GetInstanceName();
                Type apc = typeof(ADO_Net_Performance_Counters);
                foreach (string s in Enum.GetNames(apc))
                {
                    var pc = new PerformanceCounter();
                    pc.CategoryName = ".NET Data Provider for SqlServer";
                    pc.CounterName = s;
                    pc.InstanceName = instanceName;
                    PerfCounters.Add(pc);
                }
            }
                
        }


        /// <summary>
        /// 顯示要查看的 PerformanceCounter
        /// </summary>
        private void WritePerformanceCounters()
        {
            try
            {
                Response.Write("<hr>");
                foreach (PerformanceCounter p in this.PerfCounters)
                {
                    Response.Write($"{p.CounterName} = {p.NextValue()}, RawValue:{p.RawValue}<br/>");
                }
                Response.Write("<hr>");
            }catch(Exception)
            {
                Response.Write("目前沒有db連接的資訊，所以無法顯示 PerformanceCounter 資訊");
            }
            
        }

        /// <summary>
        /// 將未close的connection放在session之中
        /// </summary>
        public List<SqlConnection> Connections
        {
            get
            {
                if (Session["Connections"] == null)
                    Session["Connections"] = new List<SqlConnection>();

                return Session["Connections"] as List<SqlConnection>;
            }
            set { Session["Connections"] = value; }
        }


        /// <summary>
        /// SqlConnection 使用 using 去包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnUsingConn_OnClick(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                using (var connection1 = new SqlConnection(connStr))
                {
                    connection1.Open();
                    //停個 0.5 秒
                    System.Threading.Thread.Sleep(500);
                }
                    
            }

        }

        /// <summary>
        /// 一直 open sqlConnection 不 Close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCreateConn_OnClick(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                var connection1 = new SqlConnection(connStr);
                connection1.Open();
                Connections.Add(connection1);
            }
        }

      
        /// <summary>
        /// 呼叫 Connection 的 ClearAllPools Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClearAllPools_OnClick(object sender, EventArgs e)
        {
                SqlConnection.ClearAllPools();
        }

        /// <summary>
        /// 清單
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClearPool_OnClick(object sender, EventArgs e)
        {
            var connection1 = new SqlConnection(connStr);
            SqlConnection.ClearPool(connection1);
        }

        protected void btnCloseConns_OnClick(object sender, EventArgs e)
        {
            foreach (var c in Connections)
            {
                c.Close();
            }
            Connections.Clear();
        }


        private enum ADO_Net_Performance_Counters
        {
            //NumberOfActiveConnectionPools,
            //NumberOfReclaimedConnections,
            //HardConnectsPerSecond,
            //HardDisconnectsPerSecond,
            //NumberOfActiveConnectionPoolGroups,
            //NumberOfInactiveConnectionPoolGroups,
            NumberOfInactiveConnectionPools,
            //NumberOfNonPooledConnections,
            NumberOfPooledConnections,
            //NumberOfStasisConnections,
            // The following performance counters are more expensive to track.
            // Enable ConnectionPoolPerformanceCounterDetail in your config file.
            //     SoftConnectsPerSecond
            //     SoftDisconnectsPerSecond
                 NumberOfActiveConnections,
                 NumberOfFreeConnections
        }

        
        protected void btnGetCounterInfo_OnClick(object sender, EventArgs e)
        {
            WritePerformanceCounters();
        }
    }
}