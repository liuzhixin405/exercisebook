using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 模拟双色球
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.btnStop.Enabled = false;
        }
        #region
        /// <summary>
        /// 红球集合
        /// </summary>
        private string[] RedNums =
        {
            "01","02","03","04","05","06","07","08","09","10",
            "11","12","13","14","15","16","17","18","19","20",
            "21","22","23","24","25","26","27","28","29","30",
            "31","32","33"
        };

        /// <summary>
        /// 蓝球集合
        /// </summary>
        private string[] BlueNums =
        {
            "01","02","03","04","05","06","07","08","09","10",
            "11","12","13","14","15","16"
        };
        private bool isGoon = true;
        #endregion
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                #region 初始化
                this.btnStart.Text = "摇号开始...";
                this.btnStart.Enabled = false;
                this.isGoon = true;
                this.lblRed1.Text = "00";
                this.lblRed2.Text = "00";
                this.lblRed3.Text = "00";
                this.lblRed4.Text = "00";
                this.lblRed5.Text = "00";
                this.lblRed6.Text = "00"; 
                #endregion
                Thread.Sleep(1000);
                this.btnStop.Enabled = true;

                List<Task> taskList = new List<Task>();
                TaskFactory taskFactory = new TaskFactory();

                foreach (Control c in gboSSQ.Controls)
                {
                    #region 重复
                    //if( c is Label && c.Name.Contains("Blue"))
                    //{
                    //    Label lbl = (Label)c;
                    //    taskList.Add(taskFactory.StartNew(()=>{
                    //        while (isGoon)
                    //        {
                    //            string text = this.GetNum(lbl);
                    //            this.UpdateLbl(lbl, text);
                    //            Console.WriteLine($"当前多线程id{Thread.CurrentThread.ManagedThreadId}");
                    //        }

                    //    }));
                    //}else if(c is Label && c.Name.Contains("Red"))
                    //{
                    //    Label lbl = (Label)c;
                    //    taskList.Add(taskFactory.StartNew(() => {
                    //        while (isGoon)
                    //        {
                    //            string text = this.GetNum(lbl);
                    //            this.UpdateLbl(lbl, text);
                    //            Console.WriteLine($"当前多线程id{Thread.CurrentThread.ManagedThreadId}");
                    //        }

                    //    }));
                    //} 
                    #endregion
                    Label lbl = (Label)c;
                    taskList.Add(taskFactory.StartNew(() =>
                    {
                        while (isGoon)
                        {
                            string text = this.GetNum(lbl);
                            this.UpdateLbl(lbl, text);
                            Console.WriteLine($"当前多线程id{Thread.CurrentThread.ManagedThreadId}");
                        }

                    }));
                }
                taskFactory.ContinueWhenAll(taskList.ToArray(), t =>
                {
                    MessageShow();
                });
             }
            catch
            {

            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.btnStop.Enabled = false;
            this.btnStart.Enabled = true;
            btnStart.Text = "开始";
            this.isGoon = false;
        }

        private void MessageShow()
        {
            MessageBox.Show($"本期双色球开奖结果是 {lblRed1.Text} {lblRed2.Text} {lblRed3.Text} {lblRed4.Text} {lblRed5.Text} {lblRed6.Text} " +
                $"蓝球 {lblBlue.Text}");
        }

        private static object GetNum_Lock = new object();

        private string GetNum(Label label)
        {
            Thread.Sleep(1000);
            if (label.Name.Equals("lblBule"))
            {
                return this.BlueNums[new Random().Next(0, this.BlueNums.Length - 1)];
            }
            else
            {
                string text = this.RedNums[new Random().Next(0, this.RedNums.Length - 1)];
                lock (GetNum_Lock)
                {
                    IEnumerable<string> usedNumList = this.GetUsedNums();
                    while (usedNumList.Contains(text))
                    {
                        text = this.RedNums[new Random().Next(0, this.RedNums.Length - 1)];
                    }
                    return text;
                }
            }
        }

        private IEnumerable<string> GetUsedNums()
        {
            
            foreach(Control c in this.gboSSQ.Controls)
            {
                if(c is Label && c.Name.Contains("Red"))
                {
                    yield return c.Text;
                }
            }
        }

        private void UpdateLbl(Label lbl,string text)
        {
            if (lbl.InvokeRequired)
            {
                this.Invoke(new Action(()=> {
                    lbl.Text = text;
                    Console.WriteLine($"当前UpdateLbl线程id{Thread.CurrentThread.ManagedThreadId}");

                }));
            }
            else
            {
                lbl.Text = text;
            }
        }
    }
}
