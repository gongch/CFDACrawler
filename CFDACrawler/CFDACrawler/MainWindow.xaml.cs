using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using mshtml;
using System.Windows.Threading;

namespace CFDACrawler
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = TimeSpan.FromMilliseconds(100);
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            var now = Environment.TickCount;
            if (now - lastSucceededTick > 2000)
            {
                NavigateTo(urlPrefix + index);
            }
        }

        private FileStream fs;
        private StreamWriter sw;
        private string urlPrefix;
        private int index=1;
        private int failedCount = 0;
        private int lastSucceededTick;
        private DispatcherTimer _timer;

        private void BtnImported_OnClick(object sender, RoutedEventArgs e)
        {
            fs = new FileStream("files1.csv",FileMode.Create);
            sw = new StreamWriter(fs,Encoding.GetEncoding("GB2312"));
            sw.WriteLine("产品名称,注册证编号,注册人名称,注册人住所,生产地址,代理人名称,代理人住所,型号、规格,结构及组成,适用范围,生产国或地区（英文）,其他内容,备注,批准日期,有效期至,生产厂商名称（中文）,产品名称（中文）,产品标准,生产国或地区（中文）,售后服务机构,变更日期,主要组成成分（体外诊断试剂）,预期用途（体外诊断试剂）,产品储存条件及有效期（体外诊断试剂）,审批部门,变更情况,网址");
            urlPrefix = "http://app1.sfda.gov.cn/datasearch/face3/content.jsp?tableId=27&tableName=TABLE27&tableView=%E8%BF%9B%E5%8F%A3%E5%99%A8%E6%A2%B0&Id=";

            wbBrowser.LoadCompleted += WbBrowser_LoadCompleted;
            lastSucceededTick = Environment.TickCount;
            NavigateTo(urlPrefix+index);
            _timer.Start();
        }

      
        private void NavigateTo(string url)
        {
            wbBrowser.Navigate(new Uri(url));
        }

        private void WbBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            var html = wbBrowser.Document as IHTMLDocument2;

            int[] dataIndex = new int[] {7,10,13,16,19,22,25,28,31,34,37,43,46,49,52,55,58,61,64,67,70,73,76,79,82,85};
            IHTMLElementCollection items = (IHTMLElementCollection)html.all.tags("TABLE");
            foreach (IHTMLElement item in items)
            {
                var htmls =item.innerHTML.Split('\n');
                if (htmls.Length != 92)
                {
                    continue;
                }
                foreach (var i in dataIndex)
                {
                    var s = htmls[i].Substring(32,htmls[i].Length-33-10).Replace(',',' ');//.Replace("<TD bgColor=#eaeaea width=\"83%\">", "").Replace("</TD></TR>","");
                    sw.Write(s+",");
                }
                sw.WriteLine(urlPrefix + index);
                index++;
                lastSucceededTick = Environment.TickCount;
                failedCount = 0;
                NavigateTo(urlPrefix + index);
                Console.WriteLine(urlPrefix + index);
                return;
            }
            failedCount++;
            if (failedCount > 5)
            {
                Console.WriteLine("Get "+(urlPrefix + index)+" failed");
                index++;
            }
            NavigateTo(urlPrefix + index);
        }
        private void WbBrowser_OwnMakerCompleted(object sender, NavigationEventArgs e)
        {
            var html = wbBrowser.Document as IHTMLDocument2;

            int[] dataIndex = new int[] { 7,10,13,16,19,22,25,28,31,34,37,40,43,46,52,55,58,61,64,67,70,73};
            IHTMLElementCollection items = (IHTMLElementCollection)html.all.tags("TABLE");
            foreach (IHTMLElement item in items)
            {
                var htmls = item.innerHTML.Split('\n');
                if (htmls.Length != 80)
                {
                    continue;
                }
                foreach (var i in dataIndex)
                {
                    var s = htmls[i].Substring(32, htmls[i].Length - 33 - 10).Replace(',', ' ');//.Replace("<TD bgColor=#eaeaea width=\"83%\">", "").Replace("</TD></TR>","");
                    sw.Write(s + ",");
                }
                sw.WriteLine(urlPrefix + index);
                index++;
                lastSucceededTick = Environment.TickCount;
                failedCount = 0;
                NavigateTo(urlPrefix + index);
                Console.WriteLine(urlPrefix + index);
                return;
            }
            failedCount++;
            if (failedCount > 5)
            {
                Console.WriteLine("Get " + (urlPrefix + index) + " failed");
                index++;
            }
            NavigateTo(urlPrefix + index);
        }

        private void BtnCFDA_OnClick(object sender, RoutedEventArgs e)
        {
            fs = new FileStream("files2.csv", FileMode.Create);
            sw = new StreamWriter(fs, Encoding.GetEncoding("GB2312"));
            sw.WriteLine("注册证编号,注册人名称,注册人住所,生产地址,代理人名称,代理人住所,产品名称,型号、规格,结构及组成,适用范围,其他内容,备注,批准日期,有效期至,产品标准,变更日期,邮编,主要组成成分（体外诊断试剂）,预期用途（体外诊断试剂）,产品储存条件及有效期（体外诊断试剂）,审批部门,变更情况,网址");
            urlPrefix = "http://app1.sfda.gov.cn/datasearch/face3/content.jsp?tableId=26&tableName=TABLE26&tableView=%E5%9B%BD%E4%BA%A7%E5%99%A8%E6%A2%B0&Id=";

            wbBrowser.LoadCompleted += WbBrowser_OwnMakerCompleted;
            lastSucceededTick = Environment.TickCount;
            NavigateTo(urlPrefix + index);
            _timer.Start();
        }
    }
  
}
