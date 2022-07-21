using Microsoft.Win32;
using System;
using System.Collections;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Modbus;
using WpfApp1.utils;
using System.Collections.Generic;
using ZedGraph;
using System.Data.SQLite;
using System.Net.NetworkInformation;
using System.IO;
using System.Text;

namespace WpfApp1
{
    /// <summary>
    /// OperaMonitor.xaml 的交互逻辑
    /// </summary>
    ///
    public partial class OperaMonitor : Window
    {
        public OperaMonitor()
        {
            InitializeComponent();
            //加载背景图片
            ImageBrush b = new ImageBrush();
            b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/img/background1.jpg"));
            b.Stretch = Stretch.Fill;
            this.Background = b;
            //定时器刷新时间
            updateTimer.Tick += new EventHandler(getData);
            updateTimer.Interval = new TimeSpan(0, 0, 0, 1);
            updateTimer.Start();
            //定时器刷新实时数据
            readDataTimer.Tick += new EventHandler(getRealData);
            readDataTimer.Interval = new TimeSpan(0, 0, 0, 1);
            //定时器实时更新整机工作状态
            workStatusTimer.Elapsed += new System.Timers.ElapsedEventHandler(readWorkStatus);
            //一直执行
            //workStatusTimer.Enabled = true;
            //初始化数据库信息
            initalSQL();
            //初始化IP信息
            InitIpAndPort();
            //初始化加载XML文件
            InitXml();

            Thread heartbeat = new Thread(() =>
            {


                while (true)
                {


                    if (textConnection == "断开")
                    {
                        //Console.WriteLine("进入心跳包");
                        //Console.WriteLine(ModbusTcp.Instance.IsConnected);
                        if (!ModbusTcp.Instance.IsConnected)
                        {
                            if (upgrade1==3)
                            {
                                ModbusTcp.Instance.close();
                                ModbusTcp.Instance.ReadInputRegisters4(slaveIdText, Convert.ToInt32(addrTableR["心跳计数"]) - 1, 1, new byte[2]);
                            }
                            
                            ModbusTcp mo = ModbusTcp.Instance;
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                //刷新数据
                                //this.status.Fill = Brushes.Red;
                            }));
                            flagHeartCount++;
                        }
                        else
                        {
                            //ModbusTcp.Instance2.close();
                            if (ModbusTcp.Instance2.ReadInputRegisters5(slaveIdText, Convert.ToInt32(addrTableR["心跳计数"]) - 1, 1, new byte[2]) != 0)
                            {
                                ModbusTcp.Instance2.close();
                                //Console.WriteLine("进入心跳包5555555555555555555555555555555555555555555555555555555555555555555");
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    //刷新数据
                                    //this.status.Fill = Brushes.Red;
                                }));
                                flagHeartCount++;
                            }
                            else
                            {
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    //刷新数据
                                    this.status.Fill = Brushes.SpringGreen;
                                }));
                                flagHeartCount = 0;
                            }
                        }
                        if (flagHeartCount > 2)
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                this.status.Fill = Brushes.Red;
                            }));
                        }
                        if (flagHeartCount > 120)
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                //刷新数据
                                this.connect.Content = "连接";
                                textConnection = "连接";
                                this.ipAddress.IsEnabled = true;
                                this.portContent.IsEnabled = true;
                                this.slaveId2.IsEnabled = true;
                            }));

                            if (readDataTimer.IsEnabled)
                            {
                                //关闭定时器
                                readDataTimer.Stop();
                                ModbusTcp.Instance.close();
                            }
                            flagHeartCount = 0;
                        }
                    }
                    //Console.WriteLine("打印心跳------------------------------");
                    Thread.Sleep(1000);

                }
            });
            heartbeat.IsBackground = true;
            heartbeat.Start();


        }
        #region 系统变量及公用方法

        //实例化定时方法
        private static DispatcherTimer updateTimer = new DispatcherTimer();
        private static DispatcherTimer readDataTimer = new DispatcherTimer();
        //private static DispatcherTimer workStatusTimer = new DispatcherTimer();
        private static System.Timers.Timer workStatusTimer = new System.Timers.Timer(1000);
        //递归找寻元素
        public T GetVisualChild<T>(DependencyObject parent, Func<T, bool> predicate) where T : Visual
        {
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                DependencyObject v = (DependencyObject)VisualTreeHelper.GetChild(parent, i);
                T child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v, predicate);
                    if (child != null)
                    {
                        return child;
                    }
                }
                else
                {
                    if (predicate(child))
                    {
                        return child;
                    }
                }
            }

            return null;
        }
        //定义存储config中的路径集合
        Dictionary<string, string> pathTable = new Dictionary<string, string>();
        //定义存储config中的RW型地址集合
        Dictionary<string, string> addrTableRW = new Dictionary<string, string>();
        //定义存储appendix中bit数据集合
        Dictionary<string, Dictionary<int, string>> appendArray = new Dictionary<string, Dictionary<int, string>>();
        Dictionary<string, string> addrTableR = new Dictionary<string, string>();
        SortedDictionary<string, string> CAN1Sort = new SortedDictionary<string, string>();
        SortedDictionary<string, string> CAN2Sort = new SortedDictionary<string, string>();
        SortedDictionary<string, string> CAN3Sort = new SortedDictionary<string, string>();
        //整机工作状态
        private int workAddress;
        //创建字典用于保存数据库名称
        Dictionary<string, string> sqlList = new Dictionary<string, string>();
        //定义IP和端口号保存路径
        public static string ipAndPortPath = AppDomain.CurrentDomain.BaseDirectory + "ipConfig" +"\\" + "ipConfig.txt";
        public static string xmlPath = AppDomain.CurrentDomain.BaseDirectory + "xmlFile" + "\\" + "myConfig.xml";
        //定义导入状态
        bool importFlag = false;
        //定义是否建立连接
        bool isConnection = false;
        #region 加载数据
        //根节点
        private XmlElement xe;
        //根节点元素
        private XmlNode xn;
        //根节点元素的子节点
        private XmlNodeList xnl;
        //定义加载数据方法
        private void LoadData(string xmlName)
        {
            this.listBox.Items.Clear();
            pathTable.Clear();
            addrTableRW.Clear();
            appendArray.Clear();
            CAN2Sort.Clear();
            CAN1Sort.Clear();
            CAN3Sort.Clear();
            addrTableR.Clear();
            int idCount = 1;
            try
            {
                XmlReading.Load(xmlName);
                //获取根节点
                xe = XmlReading.GetXmlDocumentRoot();
                //将根元素转换为根节点元素
                xn = XmlReading.ExchangeNodeElement(xe);
                //获取根节点元素的子节点
                xnl = xn.ChildNodes;
                if (xnl.Count > 0)
                {
                    for (int i = 0; i < xnl.Count; i++)
                    {
                        XmlElement firstNode = (XmlElement)xnl[i]; //读取第i项属性值
                        XmlNodeList cofigNode = firstNode.GetElementsByTagName("config");
                        XmlNodeList fxnl = firstNode.ChildNodes;
                        //XmlNodeList fxnl = firstNode.ChildNodes;
                        if (fxnl.Count > 0)
                        {
                            for (int j = 0; j < fxnl.Count; j++)
                            {
                                XmlElement secondNode = (XmlElement)fxnl[j];
                                if (firstNode.Name.Equals("config"))
                                {
                                    //string pointName = secondNode.Name;
                                    string key = secondNode.GetAttribute("function_name");
                                    string value = secondNode.GetAttribute("path");
                                    //hashtable.Add(secondNode.InnerText, secondNode.GetAttribute("path"));
                                    if (key.Equals("固件在线升级"))
                                    {
                                        addrTableR.Add("CAN1数量", secondNode.GetAttribute("CAN1_DEV"));
                                        addrTableR.Add("CAN2数量", secondNode.GetAttribute("CAN2_DEV"));
                                    }
                                    if(key != null && key != "" && value != null && value != "")                                    
                                        pathTable.Add(key, value);
                                    XmlNodeList fxn2 = secondNode.ChildNodes;
                                    if (fxn2.Count > 0)
                                    {
                                        for (int k = 0; k < fxn2.Count; k++)
                                        {
                                            XmlElement pointNode = (XmlElement)fxn2[k];
                                            if (pointNode.GetAttribute("type").Equals("RW"))
                                            {
                                                Console.WriteLine(pointNode.GetAttribute("DataName"));
                                                addrTableRW.Add(pointNode.GetAttribute("DataName"), pointNode.GetAttribute("DataAddress"));

                                            }else if (pointNode.GetAttribute("DataName").Equals("整机工作状态"))
                                            {
                                                workAddress = Convert.ToInt32(pointNode.GetAttribute("DataAddress"));
                                            }  
                                            else
                                            {
                                                Console.WriteLine(pointNode.GetAttribute("DataName"));
                                                if (pointNode.GetAttribute("DataName").Contains("CAN1_"))
                                                {
                                                    CAN1Sort.Add(pointNode.GetAttribute("DataName"), pointNode.GetAttribute("DataAddress"));
                                                }
                                                if (pointNode.GetAttribute("DataName").Contains("CAN2_"))
                                                {
                                                    CAN2Sort.Add(pointNode.GetAttribute("DataName"), pointNode.GetAttribute("DataAddress"));
                                                }
                                                if (pointNode.GetAttribute("DataName").Contains("COM升级"))
                                                {
                                                    //idCount++ +
                                                    CAN3Sort.Add(pointNode.GetAttribute("DataName"), pointNode.GetAttribute("DataAddress"));
                                                }

                                                addrTableR.Add(pointNode.GetAttribute("DataName"), pointNode.GetAttribute("DataAddress"));
                                            }
                                        }

                                    }
                                }
                                else if (firstNode.Name.Equals("protocol"))
                                {
                                    string UnitName = secondNode.GetAttribute("unit");
                                    if (UnitName != null)
                                    {
                                        if (!this.listBox.Items.Contains("全部"))
                                            this.listBox.Items.Add("全部");
                                        switch (UnitName)
                                        {
                                            case "0":
                                                if (!this.listBox.Items.Contains("总设备单元"))
                                                    this.listBox.Items.Add("总设备单元");
                                                break;
                                            case "1":
                                                if (!this.listBox.Items.Contains("设备单元1"))
                                                    this.listBox.Items.Add("设备单元1");
                                                break;
                                            case "2":
                                                if (!this.listBox.Items.Contains("设备单元2"))
                                                    this.listBox.Items.Add("设备单元2");
                                                break;
                                            case "3":
                                                if (!this.listBox.Items.Contains("设备单元3"))
                                                    this.listBox.Items.Add("设备单元3");
                                                break;
                                            case "4":
                                                if (!this.listBox.Items.Contains("设备单元4"))
                                                    this.listBox.Items.Add("设备单元4");
                                                break;
                                            default:
                                                if (!this.listBox.Items.Contains("全部"))
                                                    this.listBox.Items.Add("全部");
                                                break;
                                        }
                                    }
                                }
                                else if (firstNode.Name.Equals("appendix"))
                                {
                                    string nodeName = secondNode.Name;
                                    string keyName = secondNode.GetAttribute("DataName");
                                    XmlNodeList xnl = secondNode.ChildNodes;
                                    Dictionary<int, string> element = new Dictionary<int, string>();
                                    foreach(XmlElement third in xnl)
                                    {
                                        int index = 0;
                                        int key = 0;
                                        string value = null;
                                        if (nodeName.Equals("bit"))
                                        {
                                            index = Convert.ToInt32(third.GetAttribute("BitId"));
                                            key = (int)Math.Pow(2, index);
                                            value = third.GetAttribute("BitName");
                                        }
                                        if (nodeName.Equals("discrete"))
                                        {
                                            key = Convert.ToInt32(third.GetAttribute("Var"));
                                            value = third.GetAttribute("VarName");
                                        }
                                        element.Add(key,value);
                                    }
                                    appendArray.Add(keyName, element);
                                    Console.WriteLine("附录");
                                }
                                else
                                {
                                    MessageBox.Show("配置文件不正确，请重新导入");
                                    return;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("配置文件不正确，请重新导入");
                            return;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("配置文件不正确，请重新导入");
                    return;
                }
                //默认选中第一项
                listBox.SelectedIndex = 0;
                //将导入状态置位
                importFlag = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //定义初始化数据库集合方法
        private void initalSQL()
        {
            sqlList.Add("db_his_data", "HIS_DATA_INFO");
            sqlList.Add("db_his_Eday", "HISEDAY");
            sqlList.Add("db_his_Emonth", "HISEMONTH");
            sqlList.Add("db_his_Eyear", "HISEYEAR");
            sqlList.Add("db_his_Pday", "HISPDAYINF");
            sqlList.Add("db_his_record", "HISRECORD");
        }
        //获取系统时间
        public void getData(object sender, EventArgs e)
        {
            string timeData = DateTime.Now.Second.ToString();
            //RealShow();
            SysTime.Content = DateTime.Now.ToString("F");
        }
        #endregion
        //退出系统方法
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("您确定要退出程序吗？", "确认退出", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel) == MessageBoxResult.OK)
            {
                Environment.Exit(0);
            }
        }
        private static int flagHeartCount = 0;
        public void runHeartBeat() {
            
            
        
        }
        private void InitIpAndPort()
        {
            if (File.Exists(ipAndPortPath))
            {
                FileStream fs = new FileStream(ipAndPortPath, FileMode.Open, FileAccess.Read);
                StreamReader m_streamReader = new StreamReader(fs);
                //使用StreamReader类来读取文件
                m_streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                //从数据流中读取每一行，直到文件的最后一行
                string result = "";
                string strLine = m_streamReader.ReadLine();
                while (strLine != null)
                {
                    result += strLine + "\n";
                    strLine = m_streamReader.ReadLine();
                }
                if(result != "")
                {
                    //将字符串转换为数组
                    string[] str = result.Split(',');
                    if(str.Length >= 3)
                    {
                        ipAddress.Text = str[0].Trim();
                        portContent.Text = str[1].Trim();
                        slaveId2.Text = str[2].Trim();
                    }

                }
                //关闭此StreamReader对象
                m_streamReader.Close();
            }
            

        }
        private void InitXml()
        {
            if (File.Exists(xmlPath))
            {
                LoadData(xmlPath);
            }
        }
        private int workOrigin = 0;
        //读取整机状态
        private void readWorkStatus(object sender, EventArgs e)
        {
            if (textConnection == "断开")
            {
                byte[] bytes = new byte[1024];
                ModbusTcp.Instance.ReadInputRegisters6(slaveIdText, workAddress - 1, 1, bytes);
                int result = bytes[1] | (bytes[0] << 8);
                if (result >= 0 && result < 400 && (workOrigin < 0 || workOrigin >= 400))
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        //刷新
                        workStatus.Fill = Brushes.Yellow;
                    }));

                }
                else if (result >= 400 && result < 500 && (workOrigin < 400 || workOrigin >= 500))
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        //刷新
                        workStatus.Fill = Brushes.ForestGreen;
                    }));

                }
                else if (result >= 500 && result < 1000 && (workOrigin < 500 || workOrigin >= 1000))
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        //刷新
                        workStatus.Fill = Brushes.Red;
                    }));

                }
                else
                {
                    return;
                }
                workOrigin = result;
                Thread.Sleep(200);
            }
            
        }
        #endregion

    /*
   #########################################################################################
   ##############################################实时信息及参数设置模块###############################
   #########################################################################################
   */

     #region 实时信息及参数设置
    /*全局变量定义*/
    //从机地址
    private const ushort slaveId = 1;
        //定义实时信息类作为数据源使用
        //public class Informations : List<Information> { }
        private ObservableCollection<Information> Informations = new ObservableCollection<Information>();
        //定义设置信息类作为数据源使用
        private ObservableCollection<SettingInfo> SettingInfos = new ObservableCollection<SettingInfo>();
        //定义设置状态类作为数据源使用
        private ObservableCollection<IsState> iss = new ObservableCollection<IsState>();
        //定义存放实时信息参数，用于读取实时信息
        RealInfo realInfo = new RealInfo();
        //实时信息集合
        private Collection<RealInfo> RealInfos = new Collection<RealInfo>();
        //设置信息集合
        private Collection<SetInfo> SetInfos = new Collection<SetInfo>();
        private bool countHeart;
        //设置信息回读      
        private void callBack_Click(object sender, RoutedEventArgs e)
        {
            paramsetValue();
        }
        //参数设置
        private void setting_Click(object sender, RoutedEventArgs e)
        {
            if (this.connect.Content.ToString().Trim() == "连接")
            {
                MessageBox.Show("通信未连接......");
                return;
            }
            iss.Clear();
            setState ss = new setState();
            int a = checkList.Count;
           /* int count = setGrid.SelectedItems.Count;
            DataRowView[] drv = new DataRowView[count];*/
            setGrid.UpdateLayout();
             /*           for (int i = 0; i < setGrid.Items.Count; i++)
             {
                DataGridRow neddrow = (DataGridRow)setGrid.ItemContainerGenerator.ContainerFromIndex(i);
                if (neddrow != null)
                {
                    //获取该行的某列
                    SettingInfo di = (SettingInfo)neddrow.Item;
                    
             */
            for(int i = 0; i < checkList.Count; i++) {
                SettingInfo di = (SettingInfo)checkList[i];
                bool isSelected = di.Selected;
                string address = di.Address;
                string name = di.Name;
                string value = di.Value;
                string dataDecimal = di.DataDecimal;
                string scopes = di.Scope;
                string dataLen = di.datalen;
                double scope1 = 0;
                double scope2 = 0;
                /*                if (isSelected)
                {*/
                try
                {

                    string[] scopeArr = scopes.Split('~');
                    if (scopeArr[0].IndexOf("x") > -1)
                    {
                        scope1 = Convert.ToInt32(scopeArr[0], 16);
                        scope2 = Convert.ToInt32(scopeArr[1], 16);
                    }
                    else
                    {
                        scope1 = Convert.ToDouble(scopeArr[0]);
                        scope2 = Convert.ToDouble(scopeArr[1]);
                    }

                    if (value.Equals("") && value == null)
                    {
                        IsState isState = new IsState();
                        isState.name = name;
                        isState.value = "不能设置空值！";
                        iss.Add(isState);
                    }
                    else
                    {
                        if (dataLen == "2")
                        {
                            int dvalue = 0;
                            double dvalue1 = 0;
                            if (value.IndexOf("0x") > -1 || value.IndexOf("0X") > -1)
                            {
                                dvalue = (int)(Convert.ToInt32(value, 16) / Convert.ToDouble(dataDecimal));
                                dvalue1 = Convert.ToInt32(value, 16);
                            }
                            else
                            {
                                dvalue = (int)(Convert.ToDouble(value) / Convert.ToDouble(dataDecimal));
                                dvalue1 = Convert.ToDouble(value);
                            }

                            if (dvalue1 >= scope1 && dvalue1 <= scope2)
                            {

                                int flagS = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)(Convert.ToInt32(address) - 1), 1, new byte[] { (byte)(dvalue >> 8), (byte)(dvalue & 255) });
                                if (flagS != 0)
                                {
                                    flagS = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)(Convert.ToInt32(address) - 1), 1, new byte[] { (byte)(dvalue >> 8), (byte)(dvalue & 255) });
                                }

                                if (flagS == 0)
                                {
                                    IsState isState = new IsState();
                                    isState.name = name;
                                    isState.value = "设置成功！";
                                    iss.Add(isState);
                                }
                                else
                                {
                                    IsState isState = new IsState();
                                    isState.name = name;
                                    isState.value = "设置失败！";
                                    iss.Add(isState);
                                }
                            }
                            else
                            {
                                IsState isState = new IsState();
                                isState.name = name;
                                isState.value = "设置值超出范围！";
                                iss.Add(isState);
                            }
                        }
                        else
                        {
                            int dvalue = 0;
                            double dvalue1 = 0;
                            if (value.IndexOf("0x") > -1 || value.IndexOf("0X") > -1)
                            {
                                dvalue = (int)(Convert.ToInt32(value, 16) / Convert.ToDouble(dataDecimal));
                                dvalue1 = Convert.ToInt32(value, 16);
                            }
                            else
                            {
                                dvalue = (int)(Convert.ToDouble(value) / Convert.ToDouble(dataDecimal));
                                dvalue1 = Convert.ToDouble(value);
                            }

                            if (dvalue1 >= scope1 && dvalue1 <= scope2)
                            {
                                int flagS = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)(Convert.ToInt32(address) - 1), 2, new byte[] { (byte)((dvalue >> 8) & 0xFF), (byte)(dvalue & 0xFF), (byte)((dvalue >> 24) & 0xFF), (byte)((dvalue >> 16) & 0xFF) });
                                if (flagS != 0)
                                {
                                    ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)(Convert.ToInt32(address) - 1), 2, new byte[] { (byte)((dvalue >> 8) & 0xFF), (byte)(dvalue & 0xFF), (byte)((dvalue >> 24) & 0xFF), (byte)((dvalue >> 16) & 0xFF) });
                                }

                                if (flagS == 0)
                                {
                                    IsState isState = new IsState();
                                    isState.name = name;
                                    isState.value = "设置成功！";
                                    iss.Add(isState);
                                }
                                else
                                {
                                    IsState isState = new IsState();
                                    isState.name = name;
                                    isState.value = "设置失败！";
                                    iss.Add(isState);
                                }
                            }
                            else
                            {
                                IsState isState = new IsState();
                                isState.name = name;
                                isState.value = "设置值超出范围！";
                                iss.Add(isState);
                            }
                        }



                    }
                }
                catch (FormatException ex)
                {
                    MessageBox.Show("请输入正确的十进制格式数据");
                }
                finally
                {
                }
                //}
                //}
            }
            checkList.Clear();
            SettingInfo oldItem = (SettingInfo)setGrid.SelectedItem;
            ObservableCollection<SettingInfo> tmpParts = (ObservableCollection<SettingInfo>)setGrid.ItemsSource;
            try
            {
                if (tmpParts != null)
                {
                    foreach (SettingInfo dataItem in tmpParts)
                    {
                        dataItem.Selected = false;
                    }
                }
                setGrid.ItemsSource = tmpParts;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            CheckBox chb = GetVisualChild<CheckBox>(setGrid, v => v.Name == "Set_AllSelect");
            if (null != chb)
            {
                chb.IsChecked = false;
            }
            if (iss.Count != 0)
            {
                //弹窗
                ss.resultGrid.ItemsSource = iss;
                ss.ShowDialog();
            }
            paramsetValue();
        }
        //listBox选项改变触发方法
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //默认选择第一项
            string index = "-1";
            if(listBox.SelectedItem != null)
            {
                try
                {
                    string nodeNmae = listBox.SelectedItem.ToString();
                    //转换根据用户选择内容确定索引值
                    switch (nodeNmae)
                    {
                        case "全部":
                            index = "-1";
                            break;
                        case "总设备单元":
                            index = "0";
                            break;
                        case "设备单元1":
                            index = "1";
                            break;
                        case "设备单元2":
                            index = "2";
                            break;
                        case "设备单元3":
                            index = "3";
                            break;
                        case "设备单元4":
                            index = "4";
                            break;
                        default:
                            index = "-1";
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                if(this.tabControl.SelectedIndex == 0)
                {
                    //调用实时信息显示方法
                    RealShow(index);
                    if (!readDataTimer.IsEnabled && this.connect.Content.Equals("断开"))
                    {
                        //开启定时器
                        //readDataTimer.Start();
                    }

                }
                else
                {
                    ParamSet(index);
                }
                    
            }
            
        }

        private void equipMoni_Click(object sender, RoutedEventArgs e)
        {
            upgrade1 = 1;
            wave.Visibility = Visibility.Collapsed;
            upgrade.Visibility = Visibility.Collapsed;
            history.Visibility = Visibility.Collapsed;
            monitor.Visibility = Visibility.Visible;
            if (!readDataTimer.IsEnabled && this.connect.Content.ToString().Trim() == "断开")
            {
                //开启定时器
                readDataTimer.Start();
            }
        }

        /**
         * 加载实时信息方法
         * @Param index 单元索引，识别当前单元
         **/
        private void RealShow(string index)
        {
            //Informations infos = new Informations();
            Informations.Clear();
            RealInfos.Clear();
            try
            {
                XmlNodeList tableList = XmlReading.getList("table");
                for (int i = 0; i < tableList.Count; i++)
                {
                    XmlElement tableNode = (XmlElement)tableList[i];
                    if (tableNode.GetAttribute("unit").Equals(index))
                    {
                        //register节点
                        XmlNodeList regList = tableList[i].ChildNodes;
                        for (int j = 0; j < regList.Count; j++)
                        {
                            XmlElement regNode = (XmlElement)regList[j];
                            string type = regNode.GetAttribute("type");
                            if (type.Equals("R"))
                            {
                                string regNum = regNode.GetAttribute("regNum");
                                string startAddress = regNode.GetAttribute("startAddress");
                                XmlNodeList pointNodeList = regNode.ChildNodes;
                                string[] address = new string[pointNodeList.Count];
                                string[] len = new string[pointNodeList.Count];
                                string[] sign = new string[pointNodeList.Count];
                                string[] dec = new string[pointNodeList.Count];
                                for (int k = 0; k < pointNodeList.Count; k++)
                                {
                                    string DataName = ((XmlElement)pointNodeList[k]).GetAttribute("DataName");
                                    string DataAddress = ((XmlElement)pointNodeList[k]).GetAttribute("DataAddress");
                                    string DataLen = ((XmlElement)pointNodeList[k]).GetAttribute("DataLen");
                                    string DataInfo = ((XmlElement)pointNodeList[k]).GetAttribute("DataInfo");
                                    string DataType = ((XmlElement)pointNodeList[k]).GetAttribute("DataType");
                                    string DataSign = ((XmlElement)pointNodeList[k]).GetAttribute("DataSign");
                                    string DataDecimal = ((XmlElement)pointNodeList[k]).GetAttribute("DataDecimal");
                                    string DataUnit = ((XmlElement)pointNodeList[k]).GetAttribute("DataUnit");
                                    string DataMin = ((XmlElement)pointNodeList[k]).GetAttribute("DataMin");
                                    string DataMax = ((XmlElement)pointNodeList[k]).GetAttribute("DataMax");
                                    address[k] = DataAddress;
                                    len[k] = DataLen;
                                    sign[k] = DataSign;
                                    dec[k] = DataDecimal;
                                    if (DataUnit == null || DataUnit == "")
                                        DataUnit = "--";
                                    Information info = new Information();
                                    info.Name = DataName;
                                    info.Value = "0";
                                    info.Unit = DataUnit;
                                    info.DataMin = DataMin;
                                    info.DataMax = DataMax;
                                    Informations.Add(info);
                                }
                                RealInfo realInfo = new RealInfo();
                                realInfo.RegNum = regNum;
                                realInfo.StartAddress = startAddress;
                                realInfo.Address = address;
                                realInfo.DataLen = len;
                                realInfo.DataSign = sign;
                                realInfo.DataDecimal = dec;
                                RealInfos.Add(realInfo);
                            }
                        }
                    }
                    else if(index.Equals("-1"))
                    {
                        //register节点
                        XmlNodeList regList = tableList[i].ChildNodes;
                        for (int j = 0; j < regList.Count; j++)
                        {
                            XmlElement regNode = (XmlElement)regList[j];
                            string type = regNode.GetAttribute("type");
                            if (type.Equals("R"))
                            {
                                string regNum = regNode.GetAttribute("regNum");
                                string startAddress = regNode.GetAttribute("startAddress");
                                XmlNodeList pointNodeList = regNode.ChildNodes;
                                string[] address = new string[pointNodeList.Count];
                                string[] len = new string[pointNodeList.Count];
                                string[] sign = new string[pointNodeList.Count];
                                string[] dec = new string[pointNodeList.Count];
                                for (int k = 0; k < pointNodeList.Count; k++)
                                {
                                    string DataName = ((XmlElement)pointNodeList[k]).GetAttribute("DataName");
                                    string DataAddress = ((XmlElement)pointNodeList[k]).GetAttribute("DataAddress");
                                    string DataLen = ((XmlElement)pointNodeList[k]).GetAttribute("DataLen");
                                    string DataInfo = ((XmlElement)pointNodeList[k]).GetAttribute("DataInfo");
                                    string DataType = ((XmlElement)pointNodeList[k]).GetAttribute("DataType");
                                    string DataSign = ((XmlElement)pointNodeList[k]).GetAttribute("DataSign");
                                    string DataDecimal = ((XmlElement)pointNodeList[k]).GetAttribute("DataDecimal");
                                    string DataUnit = ((XmlElement)pointNodeList[k]).GetAttribute("DataUnit");
                                    string DataMin = ((XmlElement)pointNodeList[k]).GetAttribute("DataMin");
                                    string DataMax = ((XmlElement)pointNodeList[k]).GetAttribute("DataMax");
                                    address[k] = DataAddress;
                                    len[k] = DataLen;
                                    sign[k] = DataSign;
                                    dec[k] = DataDecimal;
                                    if (DataUnit == null || DataUnit == "")
                                        DataUnit = "--";
                                    Information info = new Information();
                                    info.Name = DataName;
                                    info.Value = "0";
                                    info.Unit = DataUnit;
                                    info.DataMin = DataMin;
                                    info.DataMax = DataMax;
                                    Informations.Add(info);
                                }
                                RealInfo realInfo = new RealInfo();
                                realInfo.RegNum = regNum;
                                realInfo.StartAddress = startAddress;
                                realInfo.Address = address;
                                realInfo.DataLen = len;
                                realInfo.DataSign = sign;
                                realInfo.DataDecimal = dec;
                                RealInfos.Add(realInfo);
                            }
                        }
                    }
                    realGrid.ItemsSource = null;
                    realGrid.ItemsSource = Informations;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        //判断实时数据查询线程任务是否执行完成
        bool IsRealContinue = true;
        //定义新线程用于执行实时数据查询与刷新
        Thread realThread = null;
        System.Threading.Tasks.Task realTask = null;
        //刷新实时数据，单开线程
        //刷新实时数据，单开线程
        private void getRealData(object sender, EventArgs e)
        {
            try
            {
                //Console.WriteLine(IsRealContinue);
                if (IsRealContinue)                     //如果上个线程任务(查询与界面刷新完成后才会开启新线程)
                {
/*                    if (realThread != null)
                    {
                        realThread.Abort();
                        realThread = null;
                        if (realThread != null)
                            MessageBox.Show("线程未销毁");
                    }

                    realThread = new Thread(() =>
                    {
                        int countz = 0;
                        IsRealContinue = false;
                        for (int j = 0; j < RealInfos.Count; j++)
                        {
                            
                            byte[] bytDate = new byte[Convert.ToInt32(RealInfos[j].RegNum) * 2];
                            if (ModbusTcp.Instance.ReadInputRegisters4(slaveIdText, Convert.ToInt32(RealInfos[j].StartAddress) - 1, Convert.ToInt32(RealInfos[j].RegNum), bytDate) != 0)
                            {
                                try
                                {
                                    ModbusTcp.Instance.close();
                                    Thread.Sleep(500);
                                    if (textConnection=="连接")
                                    {
                                        break;
                                    }
                                }
                                catch (Exception)
                                {

                                    Console.WriteLine("出现一个tcp连接错误");
                                }
                            }
                            else
                            {
                                try
                                {
                                    short v = 0;
                                    double dValue = 0;
                                    uint value1 = 0;
                                    int length = Convert.ToInt32(RealInfos[j].RegNum);
                                    int count = 0;

                                    for (int i = 0; i < length; i++)
                                    {
                                        int nextAddress = 0;
                                        int interval = 0;
                                        if(count != 0 && Convert.ToInt32(RealInfos[j].Address[count - 1]) + Convert.ToInt32(RealInfos[j].DataLen[count - 1]) / 2 != Convert.ToInt32(RealInfos[j].Address[count]))
                                        {
                                            interval = Convert.ToInt32(RealInfos[j].Address[count]) - (Convert.ToInt32(RealInfos[j].Address[count - 1]) + Convert.ToInt32(RealInfos[j].DataLen[count - 1]) / 2);
                                        }
                                        if(interval != 0)
                                        {
                                            int A = 0;
                                        }
                                        i += interval;
                                        if (count == RealInfos[j].DataLen.Length)
                                        {
                                            break;
                                        }
                                        if (RealInfos[j].DataLen[count] == "2")
                                        {
                                            if (RealInfos[j].DataSign[count] == "U")
                                            {
                                                ushort uv = (ushort)(bytDate[i * 2 + 1] | (bytDate[i * 2] << 8));
                                                dValue = uv * Convert.ToDouble(RealInfos[j].DataDecimal[count]);
                                                string name = Informations[countz].Name;
                                                if (appendArray.ContainsKey(name))
                                                {
                                                    Dictionary<int, string> element = new Dictionary<int, string>();
                                                    appendArray.TryGetValue(name, out element);
                                                    int key = (int)dValue;
                                                    string value = null;
                                                    element.TryGetValue(key, out value);
                                                    if (value != null && value != "")
                                                    {
                                                        Informations[countz].Value = value;
                                                    }
                                                    else
                                                    {
                                                        Informations[countz].Value = dValue + "";
                                                    }       
                                                }
                                                else
                                                {
                                                    if (Informations[countz].DataMin.IndexOf("x") > -1 && Informations[countz].DataMax.IndexOf("x") > -1)
                                                    {
                                                        Informations[countz].Value = "0x" + (((int)dValue).ToString("x")) + "";
                                                    }
                                                    else
                                                    {
                                                        Informations[countz].Value = dValue + "";
                                                    }
                                                }

                                                //Informations[countz].Value = dValue + "";
                                            }
                                            else
                                            {
                                                v = (short)(bytDate[i * 2 + 1] | (bytDate[i * 2] << 8));
                                                dValue = v * Convert.ToDouble(RealInfos[j].DataDecimal[count]);
                                                string name = Informations[countz].Name;
                                                if (appendArray.ContainsKey(name))
                                                {
                                                    Dictionary<int, string> element = new Dictionary<int, string>();
                                                    appendArray.TryGetValue(name, out element);
                                                    int key = (int)dValue;
                                                    string value = null;
                                                    element.TryGetValue(key, out value);
                                                    if (value != null && value != "")
                                                    {
                                                        Informations[countz].Value = value;
                                                    }
                                                    else
                                                    {
                                                        Informations[countz].Value = dValue + "";
                                                    }
                                                }
                                                else
                                                {
                                                    if (Informations[countz].DataMin.IndexOf("x") > -1 && Informations[countz].DataMax.IndexOf("x") > -1)
                                                    {
                                                        Informations[countz].Value = "0x" + (((int)dValue).ToString("x")) + "";
                                                    }
                                                    else
                                                    {
                                                        Informations[countz].Value = dValue + "";
                                                    }
                                                }
                                                //Informations[countz].Value = dValue + "";
                                            }
                                        }
                                        else
                                        {
                                            if (RealInfos[j].DataSign[count] == "U")
                                            {
                                                value1 = (uint)(ConvertTo32(bytDate[i * 2 + 2], bytDate[i * 2 + 3], bytDate[i * 2 + 0], bytDate[i * 2 + 1]));
                                                dValue = value1 * Convert.ToDouble(RealInfos[j].DataDecimal[count]);
                                                string name = Informations[countz].Name;
                                                if (appendArray.ContainsKey(name))
                                                {
                                                    Dictionary<int, string> element = new Dictionary<int, string>();
                                                    appendArray.TryGetValue(name, out element);
                                                    int key = (int)dValue;
                                                    string value = null;
                                                    element.TryGetValue(key, out value);
                                                    if (value != null && value != "")
                                                    {
                                                        Informations[countz].Value = value;
                                                    }
                                                    else
                                                    {
                                                        Informations[countz].Value = dValue + "";
                                                    }
                                                }
                                                else
                                                {
                                                    if (Informations[countz].DataMin.IndexOf("x") > -1 && Informations[countz].DataMax.IndexOf("x") > -1)
                                                    {
                                                        Informations[countz].Value = "0x" + (((uint)dValue).ToString("x")) + "";
                                                    }
                                                    else
                                                    {
                                                        Informations[countz].Value = dValue + "";
                                                    }
                                                }
                                                //Informations[countz].Value = dValue + "";
                                            }
                                            else
                                            {
                                                int value11 = (ConvertTos32(bytDate[i * 2 + 2], bytDate[i * 2 + 3], bytDate[i * 2 + 0], bytDate[i * 2 + 1]));
                                                dValue = value11 * Convert.ToDouble(RealInfos[j].DataDecimal[count]);
                                                string name = Informations[countz].Name;
                                                if (appendArray.ContainsKey(name))
                                                {
                                                    Dictionary<int, string> element = new Dictionary<int, string>();
                                                    appendArray.TryGetValue(name, out element);
                                                    int key = (int)dValue;
                                                    string value = null;
                                                    element.TryGetValue(key, out value);
                                                    if (value != null && value != "")
                                                    {
                                                        Informations[countz].Value = value;
                                                    }
                                                    else
                                                    {
                                                        Informations[countz].Value = dValue + "";
                                                    }
                                                }
                                                else
                                                {
                                                    if (Informations[countz].DataMin.IndexOf("x") > -1 && Informations[countz].DataMax.IndexOf("x") > -1)
                                                    {
                                                        Informations[countz].Value = "0x" + (((int)dValue).ToString("x8")) + "";
                                                    }
                                                    else
                                                    {
                                                        Informations[countz].Value = dValue + "";
                                                    }
                                                }
                                                //Informations[countz].Value = dValue + "";
                                            }
                                            i++;
                                        }

                                        count++;
                                        countz++;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                    //throw;
                                }
                                
                            }
                            Thread.Sleep(190);
                        }
                        IsRealContinue = true;
                        *//*realGrid.ItemsSource = null;
                        realGrid.ItemsSource = Informations;*//*
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            //刷新数据
                            realGrid.Items.Refresh();
                        }));
                        
                    });
                    realThread.Start();*/
/*                     if (!realTask.IsCompleted)
                    {
                        MessageBox.Show("线程未销毁");
                        return;
                    }*/
                    // 处理任务的子线程
                    realTask = new System.Threading.Tasks.Task(() =>
                    {
                        int countz = 0;
                        IsRealContinue = false;
                        for (int j = 0; j < RealInfos.Count; j++)
                        {

                            byte[] bytDate = new byte[Convert.ToInt32(RealInfos[j].RegNum) * 2];
                            if (ModbusTcp.Instance.ReadInputRegisters4(slaveIdText, Convert.ToInt32(RealInfos[j].StartAddress) - 1, Convert.ToInt32(RealInfos[j].RegNum), bytDate) != 0)
                            {
                                try
                                {
                                    ModbusTcp.Instance.close();
                                    Thread.Sleep(500);
                                    if (textConnection == "连接")
                                    {
                                        break;
                                    }
                                }
                                catch (Exception)
                                {

                                    Console.WriteLine("出现一个tcp连接错误");
                                }
                            }
                            else
                            {
                                try
                                {
                                    short v = 0;
                                    double dValue = 0;
                                    uint value1 = 0;
                                    int length = Convert.ToInt32(RealInfos[j].RegNum);
                                    int count = 0;

                                    for (int i = 0; i < length; i++)
                                    {
                                        int nextAddress = 0;
                                        int interval = 0;
                                        if (count != 0 && Convert.ToInt32(RealInfos[j].Address[count - 1]) + Convert.ToInt32(RealInfos[j].DataLen[count - 1]) / 2 != Convert.ToInt32(RealInfos[j].Address[count]))
                                        {
                                            interval = Convert.ToInt32(RealInfos[j].Address[count]) - (Convert.ToInt32(RealInfos[j].Address[count - 1]) + Convert.ToInt32(RealInfos[j].DataLen[count - 1]) / 2);
                                        }
                                        if (interval != 0)
                                        {
                                            int A = 0;
                                        }
                                        i += interval;
                                        if (count == RealInfos[j].DataLen.Length)
                                        {
                                            break;
                                        }
                                        if (RealInfos[j].DataLen[count] == "2")
                                        {
                                            if (RealInfos[j].DataSign[count] == "U")
                                            {
                                                ushort uv = (ushort)(bytDate[i * 2 + 1] | (bytDate[i * 2] << 8));
                                                dValue = uv * Convert.ToDouble(RealInfos[j].DataDecimal[count]);
                                                string name = Informations[countz].Name;
                                                if (appendArray.ContainsKey(name))
                                                {
                                                    Dictionary<int, string> element = new Dictionary<int, string>();
                                                    appendArray.TryGetValue(name, out element);
                                                    int key = (int)dValue;
                                                    string value = null;
                                                    element.TryGetValue(key, out value);
                                                    if (value != null && value != "")
                                                    {
                                                        Informations[countz].Value = value;
                                                    }
                                                    else
                                                    {
                                                        Informations[countz].Value = dValue + "";
                                                    }
                                                }
                                                else
                                                {
                                                    if (Informations[countz].DataMin.IndexOf("x") > -1 && Informations[countz].DataMax.IndexOf("x") > -1)
                                                    {
                                                        Informations[countz].Value = "0x" + (((int)dValue).ToString("x")) + "";
                                                    }
                                                    else
                                                    {
                                                        Informations[countz].Value = dValue + "";
                                                    }
                                                }

                                                //Informations[countz].Value = dValue + "";
                                            }
                                            else
                                            {
                                                v = (short)(bytDate[i * 2 + 1] | (bytDate[i * 2] << 8));
                                                dValue = v * Convert.ToDouble(RealInfos[j].DataDecimal[count]);
                                                string name = Informations[countz].Name;
                                                if (appendArray.ContainsKey(name))
                                                {
                                                    Dictionary<int, string> element = new Dictionary<int, string>();
                                                    appendArray.TryGetValue(name, out element);
                                                    int key = (int)dValue;
                                                    string value = null;
                                                    element.TryGetValue(key, out value);
                                                    if (value != null && value != "")
                                                    {
                                                        Informations[countz].Value = value;
                                                    }
                                                    else
                                                    {
                                                        Informations[countz].Value = dValue + "";
                                                    }
                                                }
                                                else
                                                {
                                                    if (Informations[countz].DataMin.IndexOf("x") > -1 && Informations[countz].DataMax.IndexOf("x") > -1)
                                                    {
                                                        Informations[countz].Value = "0x" + (((int)dValue).ToString("x")) + "";
                                                    }
                                                    else
                                                    {
                                                        Informations[countz].Value = dValue + "";
                                                    }
                                                }
                                                //Informations[countz].Value = dValue + "";
                                            }
                                        }
                                        else
                                        {
                                            if (RealInfos[j].DataSign[count] == "U")
                                            {
                                                value1 = (uint)(ConvertTo32(bytDate[i * 2 + 2], bytDate[i * 2 + 3], bytDate[i * 2 + 0], bytDate[i * 2 + 1]));
                                                dValue = value1 * Convert.ToDouble(RealInfos[j].DataDecimal[count]);
                                                string name = Informations[countz].Name;
                                                if (appendArray.ContainsKey(name))
                                                {
                                                    Dictionary<int, string> element = new Dictionary<int, string>();
                                                    appendArray.TryGetValue(name, out element);
                                                    int key = (int)dValue;
                                                    string value = null;
                                                    element.TryGetValue(key, out value);
                                                    if (value != null && value != "")
                                                    {
                                                        Informations[countz].Value = value;
                                                    }
                                                    else
                                                    {
                                                        Informations[countz].Value = dValue + "";
                                                    }
                                                }
                                                else
                                                {
                                                    if (Informations[countz].DataMin.IndexOf("x") > -1 && Informations[countz].DataMax.IndexOf("x") > -1)
                                                    {
                                                        Informations[countz].Value = "0x" + (((uint)dValue).ToString("x")) + "";
                                                    }
                                                    else
                                                    {
                                                        Informations[countz].Value = dValue + "";
                                                    }
                                                }
                                                //Informations[countz].Value = dValue + "";
                                            }
                                            else
                                            {
                                                int value11 = (ConvertTos32(bytDate[i * 2 + 2], bytDate[i * 2 + 3], bytDate[i * 2 + 0], bytDate[i * 2 + 1]));
                                                dValue = value11 * Convert.ToDouble(RealInfos[j].DataDecimal[count]);
                                                string name = Informations[countz].Name;
                                                if (appendArray.ContainsKey(name))
                                                {
                                                    Dictionary<int, string> element = new Dictionary<int, string>();
                                                    appendArray.TryGetValue(name, out element);
                                                    int key = (int)dValue;
                                                    string value = null;
                                                    element.TryGetValue(key, out value);
                                                    if (value != null && value != "")
                                                    {
                                                        Informations[countz].Value = value;
                                                    }
                                                    else
                                                    {
                                                        Informations[countz].Value = dValue + "";
                                                    }
                                                }
                                                else
                                                {
                                                    if (Informations[countz].DataMin.IndexOf("x") > -1 && Informations[countz].DataMax.IndexOf("x") > -1)
                                                    {
                                                        Informations[countz].Value = "0x" + (((int)dValue).ToString("x8")) + "";
                                                    }
                                                    else
                                                    {
                                                        Informations[countz].Value = dValue + "";
                                                    }
                                                }
                                                //Informations[countz].Value = dValue + "";
                                            }
                                            i++;
                                        }

                                        count++;
                                        countz++;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                    //throw;
                                }

                            }
                            Thread.Sleep(90);
                        }
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            //刷新数据
                            realGrid.Items.Refresh();
                        }));
                    });
                    realTask.Start();
                    realTask.ContinueWith((a) =>
                    {
                        IsRealContinue = true;
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        uint ConvertTo32(uint la, uint lb, uint lc, uint ld)
        {
            la = 0XFFFFFF | (la << 24);
            lb = 0XFF00FFFF | (lb << 16);
            lc = 0XFFFF00FF | (lc << 8);
            ld = 0XFFFFFF00 | ld;
            return la & lb & lc & ld;
        }
        int ConvertTos32(uint la, uint lb, uint lc, uint ld)
        {
            la = 0XFFFFFF | (la << 24);
            lb = 0XFF00FFFF | (lb << 16);
            lc = 0XFFFF00FF | (lc << 8);
            ld = 0XFFFFFF00 | ld;
            return (int)(la & lb & lc & ld);
        }
        /// <summary>
        /// 参数设置方法
        /// </summary>
        /// <param name="index">单元索引，识别当前单元</param>
        private void ParamSet(string index)
        {
            SettingInfos.Clear();
            SetInfos.Clear();
            Console.WriteLine(DateTime.Now);
            try
            {
                XmlNodeList tableList = XmlReading.getList("table");
                for (int i = 0; i < tableList.Count; i++)
                {
                    XmlElement tableNode = (XmlElement)tableList[i];
                    if (tableNode.GetAttribute("unit").Equals(index))
                    {
                        //register节点
                        XmlNodeList regList = tableList[i].ChildNodes;
                        for (int j = 0; j < regList.Count; j++)
                        {
                            XmlElement regNode = (XmlElement)regList[j];
                            string type = regNode.GetAttribute("type");
                            if (type.Equals("RW"))
                            {
                                string regNum = regNode.GetAttribute("regNum");
                                string startAddress = regNode.GetAttribute("startAddress");
                                XmlNodeList pointNodeList = regNode.ChildNodes;
                                string[] address = new string[pointNodeList.Count];
                                string[] len = new string[pointNodeList.Count];
                                string[] sign = new string[pointNodeList.Count];
                                string[] dec = new string[pointNodeList.Count];
                                for (int k = 0; k < pointNodeList.Count; k++)
                                {
                                    string DataName = ((XmlElement)pointNodeList[k]).GetAttribute("DataName");
                                    string DataAddress = ((XmlElement)pointNodeList[k]).GetAttribute("DataAddress");
                                    string DataLen = ((XmlElement)pointNodeList[k]).GetAttribute("DataLen");
                                    string DataInfo = ((XmlElement)pointNodeList[k]).GetAttribute("DataInfo");
                                    string DataType = ((XmlElement)pointNodeList[k]).GetAttribute("DataType");
                                    string DataSign = ((XmlElement)pointNodeList[k]).GetAttribute("DataSign");
                                    string DataDecimal = ((XmlElement)pointNodeList[k]).GetAttribute("DataDecimal");
                                    string DataUnit = ((XmlElement)pointNodeList[k]).GetAttribute("DataUnit");
                                    string DataMin = ((XmlElement)pointNodeList[k]).GetAttribute("DataMin");
                                    string DataMax = ((XmlElement)pointNodeList[k]).GetAttribute("DataMax");
                                    address[k] = DataAddress;
                                    len[k] = DataLen;
                                    sign[k] = DataSign;
                                    dec[k] = DataDecimal;
                                    if (DataUnit == null || DataUnit == "")
                                        DataUnit = "--";
                                    SettingInfo info = new SettingInfo();
                                    info.Name = DataName;
                                    info.Address = DataAddress;
                                    info.DataDecimal = DataDecimal;
                                    info.Value = "0";
                                    info.Unit = DataUnit;
                                    info.datalen = DataLen;
                                    info.Scope = scaleChange(DataMin, DataDecimal) + "~" + scaleChange(DataMax, DataDecimal);
                                    SettingInfos.Add(info);
                                }
                                SetInfo setInfo = new SetInfo();
                                setInfo.RegNum = regNum;
                                setInfo.StartAddress = startAddress;
                                setInfo.Address = address;
                                setInfo.DataLen = len;
                                setInfo.DataSign = sign;
                                setInfo.DataDecimal = dec;
                                SetInfos.Add(setInfo);
                            }
                        }
                    }else if (index.Equals("-1"))
                    {
                        //register节点
                        XmlNodeList regList = tableList[i].ChildNodes;
                        for (int j = 0; j < regList.Count; j++)
                        {
                            XmlElement regNode = (XmlElement)regList[j];
                            string type = regNode.GetAttribute("type");
                            if (type.Equals("RW"))
                            {
                                string regNum = regNode.GetAttribute("regNum");
                                string startAddress = regNode.GetAttribute("startAddress");
                                XmlNodeList pointNodeList = regNode.ChildNodes;
                                string[] address = new string[pointNodeList.Count];
                                string[] len = new string[pointNodeList.Count];
                                string[] sign = new string[pointNodeList.Count];
                                string[] dec = new string[pointNodeList.Count];
                                for (int k = 0; k < pointNodeList.Count; k++)
                                {
                                    string DataName = ((XmlElement)pointNodeList[k]).GetAttribute("DataName");
                                    string DataAddress = ((XmlElement)pointNodeList[k]).GetAttribute("DataAddress");
                                    string DataLen = ((XmlElement)pointNodeList[k]).GetAttribute("DataLen");
                                    string DataInfo = ((XmlElement)pointNodeList[k]).GetAttribute("DataInfo");
                                    string DataType = ((XmlElement)pointNodeList[k]).GetAttribute("DataType");
                                    string DataSign = ((XmlElement)pointNodeList[k]).GetAttribute("DataSign");
                                    string DataDecimal = ((XmlElement)pointNodeList[k]).GetAttribute("DataDecimal");
                                    string DataUnit = ((XmlElement)pointNodeList[k]).GetAttribute("DataUnit");
                                    string DataMin = ((XmlElement)pointNodeList[k]).GetAttribute("DataMin");
                                    string DataMax = ((XmlElement)pointNodeList[k]).GetAttribute("DataMax");
                                    address[k] = DataAddress;
                                    len[k] = DataLen;
                                    sign[k] = DataSign;
                                    dec[k] = DataDecimal;
                                    if (DataUnit == null || DataUnit == "")
                                        DataUnit = "--";
                                    SettingInfo info = new SettingInfo();
                                    info.Name = DataName;
                                    info.Address = DataAddress;
                                    info.DataDecimal = DataDecimal;
                                    info.datalen = DataLen;
                                    info.Value = "0";
                                    info.Unit = DataUnit;
                                    info.Scope = scaleChange(DataMin, DataDecimal) + "~" + scaleChange(DataMax, DataDecimal);
                                    //info.Scope = DataMin + "~" + DataMax;
                                    SettingInfos.Add(info);
                                }
                                SetInfo setInfo = new SetInfo();
                                setInfo.RegNum = regNum;
                                setInfo.StartAddress = startAddress;
                                setInfo.Address = address;
                                setInfo.DataLen = len;
                                setInfo.DataSign = sign;
                                setInfo.DataDecimal = dec;
                                SetInfos.Add(setInfo);
                            }
                        }
                    }
                    
                }
                setGrid.ItemsSource = null;
                setGrid.ItemsSource = SettingInfos;
                Console.WriteLine(DateTime.Now);
                int i33 = 0;
                //读取参数设置数据
                paramsetValue();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        public string scaleChange(string value, string coefficient)
        {
            string result = value;
            if (!value.Contains("x") && !value.Contains("X"))
            {
                double temp = double.Parse(value) * double.Parse(coefficient);
                result = temp.ToString();
            }
            return result;
        }
        //判断实时数据查询线程任务是否执行完成
        bool IsSetContinue = true;
        //定义新线程用于执行实时数据查询与刷新
        Thread setThread = null;
        private void paramsetValue()
        {
            try
            {
                readDataTimer.Stop();
                Console.WriteLine(this.connect.Content.ToString().Trim());
                if (this.connect.Content.ToString().Trim() == "连接")
                {
                    //MessageBox.Show("通信未连接......");
                    return;
                }

                if (IsSetContinue)                     //如果上个线程任务(查询与界面刷新完成后才会开启新线程)
                {
                    if (setThread != null)
                    {
                        setThread.Abort();
                        setThread = null;
                        if (setThread != null)
                            MessageBox.Show("线程未销毁");
                    }
                    setThread = new Thread(() =>
                    {
                        int countz = 0;
                        IsSetContinue = false;
                        for (int j = 0; j < SetInfos.Count; j++)
                        {
                            byte[] bytDate = new byte[Convert.ToInt32(SetInfos[j].RegNum) * 2];
                            Console.WriteLine(ModbusTcp.Instance.ReadHoldingRegisters2(slaveIdText, Convert.ToInt32(SetInfos[j].StartAddress) - 1, Convert.ToInt32(SetInfos[j].RegNum), bytDate));
                            if (ModbusTcp.Instance.ReadHoldingRegisters2(slaveIdText, Convert.ToInt32(SetInfos[j].StartAddress) - 1, Convert.ToInt32(SetInfos[j].RegNum), bytDate) != 0)
                            {
                                try
                                {
                                    Console.WriteLine("系统EMS控制器");
                                    Thread.Sleep(100);
                                    break;
                                }
                                catch (Exception)
                                {

                                    Console.WriteLine("出现一个tcp连接错误");
                                }
                            }
                            else
                            {
                                short v = 0;
                                uint value1 = 0;
                                double dValue = 0;
                                int length = Convert.ToInt32(SetInfos[j].RegNum);
                                int count = 0;

                                for (int i = 0; i < length; i++)
                                {
                                    int nextAddress = 0;
                                    int interval = 0;
                                    if (count != 0 && Convert.ToInt32(SetInfos[j].Address[count - 1]) + Convert.ToInt32(SetInfos[j].DataLen[count - 1]) / 2 != Convert.ToInt32(SetInfos[j].Address[count]))
                                    {
                                        interval = Convert.ToInt32(SetInfos[j].Address[count]) - (Convert.ToInt32(SetInfos[j].Address[count - 1]) + Convert.ToInt32(SetInfos[j].DataLen[count - 1]) / 2);
                                    }
                                    if (interval != 0)
                                    {
                                        int A = 0;
                                    }
                                    i += interval;
                                    if (count == SetInfos[j].DataLen.Length)
                                    {
                                        break;
                                    }
                                    if (SetInfos[j].DataLen[count] == "2")
                                    {
                                        if (SetInfos[j].DataSign[count] == "U")
                                        {
                                            ushort uv = (ushort)(bytDate[i * 2 + 1] | (bytDate[i * 2] << 8));
                                            dValue = uv * Convert.ToDouble(SetInfos[j].DataDecimal[count]);
                                            if (SettingInfos[countz].Scope.IndexOf("x")>-1)
                                            {
                                                SettingInfos[countz].Value ="0x"+( ((int)dValue).ToString("x")) + "";
                                            }
                                            else
                                            {
                                                SettingInfos[countz].Value = dValue + "";
                                            }
                                            
                                        }
                                        else
                                        {
                                            v = (short)(bytDate[i * 2 + 1] | (bytDate[i * 2] << 8));
                                            dValue = v * Convert.ToDouble(SetInfos[j].DataDecimal[count]);
                                            if (SettingInfos[countz].Scope.IndexOf("x") > -1)
                                            {
                                                SettingInfos[countz].Value = "0x" + (((int)dValue).ToString("x")) + ""; ;
                                            }
                                            else
                                            {
                                                SettingInfos[countz].Value = dValue + "";
                                            }
                                        }
                                    }
                                    else
                                    {

                                        if (SetInfos[j].DataSign[count] == "U")
                                        {
                                            value1 = (uint)(ConvertTo32(bytDate[i * 2 + 2], bytDate[i * 2 + 3], bytDate[i * 2 + 0], bytDate[i * 2 + 1]));
                                            dValue = value1 * Convert.ToDouble(SetInfos[j].DataDecimal[count]);
                                            SettingInfos[countz].Value = dValue + "";
                                            i++;
                                        }
                                        else
                                        {
                                            int value1int = (ConvertTos32(bytDate[i * 2 + 2], bytDate[i * 2 + 3], bytDate[i * 2 + 0], bytDate[i * 2 + 1]));
                                            dValue = value1int * Convert.ToDouble(SetInfos[j].DataDecimal[count]);
                                            SettingInfos[countz].Value = dValue + "";
                                            i++;
                                        }
                                    }
                                    count++;
                                    countz++;
                                }
                            }
                        }
                        IsSetContinue = true;
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            //刷新数据
                            setGrid.Items.Refresh();
                        }));
                    });
                    setThread.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //导入XML文件
        private void importXML_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Designer Files (*.xml)|*.xml|All Files (*.*)|*.*";

            if (openFile.ShowDialog() == true)
            {
                FileStream fs = new FileStream(openFile.FileName, FileMode.Open, FileAccess.Read);
                StreamReader m_streamReader = new StreamReader(fs);
                //使用StreamReader类来读取文件
                m_streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                //从数据流中读取每一行，直到文件的最后一行
                string result = "";
                string strLine = m_streamReader.ReadLine();
                while (strLine != null)
                {
                    result += strLine + "\n";
                    strLine = m_streamReader.ReadLine();
                }
                //关闭此StreamReader对象
                m_streamReader.Close();
                //保存文件到本地
                XmlReading.LoadXml(result);
                string path = AppDomain.CurrentDomain.BaseDirectory + "xmlFile";
                string filePath = Path.Combine(path, "myConfig.xml");
                //保存XML文件
                if (Directory.Exists(path))
                {
                    if (File.Exists(filePath))
                    {
                        File.WriteAllText(filePath, string.Empty);
                        XmlReading.Save(filePath);
                    }
                    else
                    {
                        File.Create(filePath).Close();
                        XmlReading.Save(filePath);
                    }
                }
                else
                {
                    Directory.CreateDirectory(path);
                    File.Create(filePath).Close();
                    XmlReading.Save(filePath);
                }

                try
                {
                    LoadData(openFile.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            //importXML.IsEnabled = false;
        }
        //Tab控件更改触发事件
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.listBox.SelectedIndex != -1)
            {
                string li = this.listBox.SelectedIndex.ToString();
                li = (Convert.ToInt32(li) - 1).ToString();
                if (e.Source is TabControl)
                {
                    switch (this.tabControl.SelectedIndex)
                    {
                        case 0:
                            RealShow(li);
                            if (!readDataTimer.IsEnabled && this.connect.Content.ToString().Trim() == "断开")
                            {
                                //开启定时器
                                readDataTimer.Start();
                            }
                            break;
                        case 1:
                            ParamSet(li);
                            break;
                    }
                }
            }
            
        }
        //序号递增
        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
        //复选框全选
        private void Set_AllSelect_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            ObservableCollection<SettingInfo> tmpParts = (ObservableCollection<SettingInfo>)setGrid.ItemsSource;
            if (tmpParts != null)
            {
                foreach (SettingInfo data in tmpParts)
                {
                    data.Selected = cb.IsChecked.Value;
                }
            }
            setGrid.ItemsSource = tmpParts;
        }

        ArrayList checkList = new ArrayList();
        //单选
        private void Set_Select_Click(object sender, RoutedEventArgs e)
        {
            SettingInfo oldItem = setGrid.SelectedItem as SettingInfo;
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked.Value)
            {
                checkList.Add(oldItem);
            }
            ObservableCollection<SettingInfo> tmpParts = (ObservableCollection<SettingInfo>)setGrid.ItemsSource;
            try
            {
                if (tmpParts != null)
                {
                    foreach (SettingInfo dataItem in tmpParts)
                    {
                        bool isSelected = dataItem.Selected;
                        if (oldItem.Name == dataItem.Name)
                        {
                            dataItem.Selected = cb.IsChecked.Value;
                        }
                    }
                }
                setGrid.ItemsSource = tmpParts;

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                //throw;
            }
        }

        private static string textConnection = "";
        private static byte slaveIdText = 1;
        private void connect_Click(object sender, RoutedEventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "ipConfig";

            if (!importFlag)
            {
                MessageBox.Show("请先导入配置文件！");
                return;
            }
            string ip_pattrn = @"(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])";
            int port_pattrn = Convert.ToInt32(this.portContent.Text);
            //"^[0-9]+$";
            if (
                System.Text.RegularExpressions.Regex.IsMatch(this.ipAddress.Text.Trim(), ip_pattrn)
                && port_pattrn < 65535 && port_pattrn > 0
                )
            {
                if (this.connect.Content.Equals("连接"))
                {
                    this.connect.Content = "断开";
                    textConnection = "断开";
                    this.ipAddress.IsEnabled = false;
                    this.portContent.IsEnabled = false;
                    this.slaveId2.IsEnabled = false;
                    ModbusTcp.serverIp = this.ipAddress.Text.Trim();
                    slaveIdText = Convert.ToByte(this.slaveId2.Text.Trim());
                    ModbusTcp.port = port_pattrn;
                    int a = ModbusTcp.Instance.ReadInputRegisters4(slaveIdText,Convert.ToInt32(addrTableR["心跳计数"])-1,1,new byte[2]);
                    ModbusTcp.Instance.close();
                    if (a == 0)
                    {
                        this.status.Fill = Brushes.SpringGreen;
                    }
                    if (monitor.Visibility == Visibility.Visible)
                    {
                        if (!readDataTimer.IsEnabled)
                        {
                            //开启定时器
                            readDataTimer.Start();
                        }
                    }
                    
                    isConnection = true;
                }
                else if (this.connect.Content.Equals("断开"))
                {
                    this.status.Fill = Brushes.Red;
                    this.connect.Content = "连接";
                    textConnection = "连接";
                    this.ipAddress.IsEnabled = true;
                    this.portContent.IsEnabled = true;
                    this.slaveId2.IsEnabled = true ;
                    if (readDataTimer.IsEnabled)
                    {
                        //关闭定时器
                        readDataTimer.Stop();
                        ModbusTcp.Instance.close();
                    }
                }
                //记录ip和port
                if (Directory.Exists(path))
                {
                    if (File.Exists(ipAndPortPath))
                    {
                        File.WriteAllText(ipAndPortPath, string.Empty);
                        FileStream fs = new FileStream(ipAndPortPath, FileMode.OpenOrCreate, FileAccess.Write);
                        StreamWriter m_streamWriter = new StreamWriter(fs);
                        m_streamWriter.Flush();
                        //设置当前流的位置
                        m_streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
                        //写入内容
                        m_streamWriter.Write(this.ipAddress.Text);
                        m_streamWriter.Write(",");
                        m_streamWriter.Write(this.portContent.Text);
                        m_streamWriter.Write(",");
                        m_streamWriter.Write(this.slaveId2.Text);
                        //关闭此文件
                        m_streamWriter.Flush();
                        m_streamWriter.Close();
                    }
                    else
                    {
                        File.Create(ipAndPortPath).Close();
                        FileStream fs = new FileStream(ipAndPortPath, FileMode.OpenOrCreate, FileAccess.Write);
                        StreamWriter m_streamWriter = new StreamWriter(fs);
                        m_streamWriter.Flush();
                        //设置当前流的位置
                        m_streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
                        //写入内容
                        m_streamWriter.Write(this.ipAddress.Text);
                        m_streamWriter.Write(",");
                        m_streamWriter.Write(this.portContent.Text);
                        m_streamWriter.Write(",");
                        m_streamWriter.Write(this.slaveId2.Text);
                        //关闭此文件
                        m_streamWriter.Flush();
                        m_streamWriter.Close();
                    }

                }
                else
                {
                    Directory.CreateDirectory(path);
                    File.Create(ipAndPortPath).Close();
                    FileStream fs = new FileStream(ipAndPortPath, FileMode.OpenOrCreate, FileAccess.Write);
                    StreamWriter m_streamWriter = new StreamWriter(fs);
                    m_streamWriter.Flush();
                    //设置当前流的位置
                    m_streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
                    //写入内容
                    m_streamWriter.Write(this.ipAddress.Text);
                    m_streamWriter.Write(",");
                    m_streamWriter.Write(this.portContent.Text);
                    m_streamWriter.Write(",");
                    m_streamWriter.Write(this.slaveId2.Text);
                    //关闭此文件
                    m_streamWriter.Flush();
                    m_streamWriter.Close();
                }
                workStatusTimer.Start();
            }
            else
            {
                MessageBox.Show("请输入正确的IP地址和端口号！");
            }
        }
        //通信状态鼠标进入提示
        private void status_MouseEnter(object sender, MouseEventArgs e)
        {
            connectLabel.Visibility = Visibility.Visible;
        }
        //通信状态鼠标离开隐藏
        private void status_MouseLeave(object sender, MouseEventArgs e)
        {
            connectLabel.Visibility = Visibility.Hidden;
        }
        //整机工作状态鼠标进入提示
        private void workStatus_MouseEnter(object sender, MouseEventArgs e)
        {
            workLabel.Visibility = Visibility.Visible;
        }
        //整机工作状态鼠标离开隐藏
        private void workStatus_MouseLeave(object sender, MouseEventArgs e)
        {
            workLabel.Visibility = Visibility.Hidden;
        }
        #endregion

        /*
        #########################################################################################
        ##############################################故障录波模块###############################
        #########################################################################################
        */

        #region 故障录波
        private void download_Click(object sender, RoutedEventArgs e)
        {
            pathTable.TryGetValue("故障录波", out wavePath);
            string sourcePath = DirectoryPath != null ? DirectoryPath : wavePath;
/*            string msg = string.Empty;
            List<string> list = new List<string>();
            if (fileList.SelectedItems != null && fileList.SelectedItems.Count >= 1)
            {
                //多选
                foreach (string lbi in fileList.SelectedItems)
                {
                    list.Add(lbi);
                }
                msg = string.Join(",", list);
            }
            else
            {
                MessageBox.Show("请选择文件！");
                return;
            }
            MessageBox.Show(msg);*/

            string filepath = null;
            if (this.fileList.SelectedItem != null)
            {
                filepath = this.fileList.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("请选择文件！");
                return;
            }

            if(!filepath.EndsWith(".xml"))
            {
                return;
            }

            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
/*            for(int i = 0; i < list.Count; i++)
            {

            }*/
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SFTPHelper sftp = new SFTPHelper(this.ipAddress.Text, "22", "root", "sungrow2016");
                //SFTPHelper sftp = new SFTPHelper("192.168.193.1", "22", "admin", "111111");
                try
                {
                    if (sftp.Connect())
                    {
                        sftp.Get(sourcePath + filepath, dialog.SelectedPath + "\\" + filepath);
                        //sftp.Get(@"/test/" + filepath, dialog.SelectedPath + "\\" + filepath);
                    }
                    MessageBox.Show("下载文件成功！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("下载文件出错：" + ex.Message);
                }
                finally
                {
                    sftp.Disconnect();
                }
            }
        }
        string wavePath;
        string DirectoryPath = null;
        private void getfile_Click(object sender, RoutedEventArgs e)
        {
            pathTable.TryGetValue("故障录波", out wavePath);
            SFTPHelper sftp = new SFTPHelper(this.ipAddress.Text, "22", "root", "sungrow2016");
            //SFTPHelper sftp = new SFTPHelper("192.168.193.1", "22", "admin", "111111");
            try
            {
                if (sftp.Connect())
                {
                    ArrayList list = sftp.GetFileList1(wavePath);
                    //ArrayList list = sftp.GetFileList1(@"/test/");
                    this.fileList.ItemsSource = list;
                    download.IsEnabled = true;
                    delete.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接SFTP服务器出现故障：" + ex.Message);
            }
            finally
            {
                sftp.Disconnect();
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            string filepath = null;
            if (this.fileList.SelectedItem != null)
            {
                filepath = this.fileList.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("请选择文件！");
                return;
            }
            SFTPHelper sftp = new SFTPHelper(this.ipAddress.Text, "22", "root", "sungrow2016");
            //SFTPHelper sftp = new SFTPHelper("192.168.193.1", "22", "admin", "111111");
            try
            {
                if (sftp.Connect())
                {
                    sftp.Delete(wavePath +"/"+ filepath);
                    ArrayList list = sftp.GetFileList1(wavePath);
                    this.fileList.ItemsSource = list;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接SFTP服务器出现故障：" + ex.Message);
            }
            finally
            {
                sftp.Disconnect();
            }
        }

        //文件列表双击事件
        private void fileList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DirectoryPath = null;
            if (fileList.SelectedIndex != -1)
            {
                // 取得容器控件
                var item = fileList.ItemContainerGenerator.ContainerFromIndex(fileList.SelectedIndex) as ListBoxItem;
                // 控件不为空 && 鼠标在控件内 && 左键按下
                if (item != null && item.IsMouseOver && e.ChangedButton == MouseButton.Left)
                {
                    var data = item.DataContext;
                    if (!data.ToString().EndsWith(".xml"))
                    {
                        if (isConnection)
                        {
                            SFTPHelper sftp = new SFTPHelper(this.ipAddress.Text, "22", "root", "sungrow2016");
                            //SFTPHelper sftp = new SFTPHelper("192.168.193.1", "22", "admin", "111111");
                            try
                            {
                                if (sftp.Connect())
                                {
                                    // HG - POWER_5700kW - MDSP(CAN2 - 1)
                                    string path = "/home/SGLogger/FaultWave" + "/" + data.ToString() +"/";
                                    ArrayList list = sftp.GetFileList1(path);
                                    //ArrayList list = sftp.GetFileList1(@"/test/");
                                    this.fileList.ItemsSource = list;
                                    download.IsEnabled = true;
                                    delete.IsEnabled = true;
                                    back.IsEnabled = true;
                                    DirectoryPath = path;
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("请选择正确的文件夹：" + ex.Message);
                            }
                            finally
                            {
                                sftp.Disconnect();
                            }
                        }
                        else
                        {
                            MessageBox.Show("请先进行连接");
                        }
                    }
                    // 处理数据项
                }
            }

        }
        /*        private void upload_Click(object sender, RoutedEventArgs e)
                {
                    SftpUtil sftp = new SftpUtil("192.168.193.1", "22", "admin", "111111");
                    sftp.Connect();
                    OpenFileDialog openFile = new OpenFileDialog();
                    if (openFile.ShowDialog() == true)
                    {
                        sftp.Put(openFile.FileName, @"\test\");
                    }    
                    sftp.Disconnect();
                }*/

        int flag = 0;
        private void query_Click(object sender, RoutedEventArgs e)
        {
            if (isConnection)
            {
                readDataTimer.Stop();
                Console.WriteLine(Convert.ToInt32(addrTableRW["故障录波开关"]));
                Console.WriteLine(Convert.ToInt32(addrTableR["故障录波状态"]));
                //getfile.IsEnabled = false;
                if (realThread != null)
                {
                    realThread.Abort();
                    realThread = null;
                    if (realThread != null)
                        MessageBox.Show("线程未销毁");
                }
                if (this.connect.Content.ToString().Trim() == "连接")
                {
                    MessageBox.Show("请建立通讯连接");
                    return;
                }

                pathTable.TryGetValue("故障录波", out wavePath);
                SFTPHelper sftp = new SFTPHelper(this.ipAddress.Text, "22", "root", "sungrow2016");
                //SFTPHelper sftp = new SFTPHelper("192.168.193.1", "22", "admin", "111111");
                try
                {
                    if (sftp.Connect())
                    {
                        //ArrayList list = sftp.GetFileList1("/home/SGLogger/FaultWave/HG-POWER_5700kW-MDSP(CAN2-1)");
                        //ArrayList list = sftp.GetFileList1(@"/test/");c
                        ArrayList list = sftp.GetFileList1(wavePath);
                        if (list.Count==0)
                        {
                            MessageBox.Show("暂无录波数据");
                        }
                        this.fileList.ItemsSource = list;
                        download.IsEnabled = true;
                        delete.IsEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("连接SFTP服务器出现故障：" + ex.Message);
                }
                finally
                {
                    sftp.Disconnect();
                }
                /*
                getfile.IsEnabled = false;
                if (flag != 0)
                {
                    MessageBox.Show("正在查询稍后再试");
                }
                byte[] bytDate = new byte[2];
                short v = -1;
                if (ModbusTcp.Instance.ReadInputRegisters4(slaveIdText, Convert.ToInt32(addrTableR["故障录波状态"]) - 1, 1, bytDate) != 0)
                {
                    MessageBox.Show("通讯异常");
                    flag = 0;
                }
                else
                {
                    try
                    {
                        flag = 1;
                        v = (short)(bytDate[0 * 2 + 1] | (bytDate[0 * 2] << 8));
                        if (v == 1)
                        {

                            getfile.IsEnabled = true;
                            MessageBox.Show("查询录波完成");
                        }
                        else
                        {
                            //waveON.IsEnabled = false;
                        }
                        Thread.Sleep(1000);
                    }
                    catch (Exception)
                    {

                        flag = 0;
                    }

                }
                */
            }
            else
            {
                MessageBox.Show("请先进行连接");
            }


        }

        private void waveON_Click(object sender, RoutedEventArgs e)
        {
            readDataTimer.Stop();
            if (realThread != null)
            {
                realThread.Abort();
                realThread = null;
                if (realThread != null)
                    MessageBox.Show("线程未销毁");
            }
            if (this.connect.Content.ToString().Trim() == "连接")
            {
                MessageBox.Show("请建立Modbus Tcp连接");
                return;
            }
            waveHolding.Visibility = Visibility.Visible;
            wave.IsEnabled = false;
            byte[] bytDate = new byte[2];
            short v = -1;
            int adress1RW = Convert.ToInt32(addrTableRW["故障录波开关"]) - 1;
            int flagOn = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)adress1RW, 1, new byte[] { (byte)(0xAA >> 8), (byte)(0xAA & 255) });
            if (flagOn != 0)
            {
                flagOn = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)adress1RW, 1, new byte[] { (byte)(0xAA >> 8), (byte)(0xAA & 255) });
            }
            bool isWaveSuccess = false;
            if (flagOn == 0)
            {
                //waveHolding.Visibility = Visibility.Collapsed;
                //wave.IsEnabled = true;
                Thread realThread2 = new Thread(() =>
                {

                    for (int i = 0; i < 30; i++)
                    {
                        if (ModbusTcp.Instance.ReadInputRegisters4(slaveIdText, Convert.ToInt32(addrTableR["故障录波状态"]) - 1, 1, bytDate) != 0)
                        {
                            //MessageBox.Show("通讯异常");
                            flag = 0;
                        }
                        else
                        {
                            try
                            {
                                flag = 1;
                                v = (short)(bytDate[0 * 2 + 1] | (bytDate[0 * 2] << 8));
                                if (v == 2)
                                {
                                    /*
                                    this.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        //刷新数据
                                        //getfile.IsEnabled = true;
                                    }));
                                    */
                                    isWaveSuccess = true;
                                    flag = 0;
                                    break;
                                }
                                else
                                {
                                    //waveON.IsEnabled = false;
                                }
                                Thread.Sleep(1000);
                            }
                            catch (Exception)
                            {

                                flag = 0;
                                break;
                            }
                            finally
                            {
                                //
                            }

                        }
                    }
                    if (isWaveSuccess)
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            waveHolding.Visibility = Visibility.Collapsed;
                            wave.IsEnabled = true;
                            this.fileList.ItemsSource = null;
                            download.IsEnabled = false;
                            delete.IsEnabled = false;
                            MessageBox.Show("录波开启成功");

                        }));

                    }
                    else
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            waveHolding.Visibility = Visibility.Collapsed;
                            wave.IsEnabled = true;
                            this.fileList.ItemsSource = null;
                            download.IsEnabled = false;
                            delete.IsEnabled = false;
                            MessageBox.Show("录波开启失败");
                        }));
                    }
                    flag = 0;
                });
                realThread2.Start();
            }
            else
            {
                //waveHolding.Visibility = Visibility.Collapsed;
                //wave.IsEnabled = true;
                //MessageBox.Show("录波开启失败");
            }


        }

        private void waveShow_Click(object sender, RoutedEventArgs e)
        {
            upgrade1 = 3;
            monitor.Visibility = Visibility.Collapsed;
            upgrade.Visibility = Visibility.Collapsed;
            history.Visibility = Visibility.Collapsed;
            wave.Visibility = Visibility.Visible;
            if (readDataTimer.IsEnabled)
            {
                //关闭定时器
                readDataTimer.Stop();
                //ModbusTcp.Instance.close();
            }
            GraphPane graphPane = Fault_waveform.GraphPane;
            graphPane.Title.Text = "故障波形";
            graphPane.XAxis.Title.Text = "点数";
            graphPane.YAxis.Title.Text = "";
        }

        private void CorrugatedCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CorrugatedInfo corrugatedInfo = (CorrugatedInfo)dataGridCorrugated.SelectedItem;
            try
            {
                if (corrugatedInfo != null)
                {
                    string name = corrugatedInfo.Name;
                    if (waveSources.Count > 0)
                    {
                        foreach (CorrugatedInfo info in waveSources)
                        {
                            if (name == info.Name)
                            {
                                info.Selected = (bool)((CheckBox)sender).IsChecked;
                            }
                        }
                    }
                }
                RefreshPane();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        //定义保存文件展示信息集合
        private Dictionary<string, List<CorrugatedInfo>> lineList = new Dictionary<string, List<CorrugatedInfo>>();
        private List<CorrugatedInfo> waveLines = new List<CorrugatedInfo>();
        //定义存储从xml读取的各条故障波形信息的字典
        private Dictionary<string, Dictionary<string, List<short>>> corrugateList = new Dictionary<string, Dictionary<string, List<short>>>();
        private Dictionary<string, List<short>> waveCorrugates = new Dictionary<string, List<short>>();

        //数据源集合
        private Dictionary<string, ObservableCollection<CorrugatedInfo>> sourceList = new Dictionary<string, ObservableCollection<CorrugatedInfo>>();
        private ObservableCollection<CorrugatedInfo> waveSources = new ObservableCollection<CorrugatedInfo>();
        private void LoadForm_Click(object sender, RoutedEventArgs e)
        {
            if (waveLines != null)
            {
                waveLines.Clear();
            }
            if (waveCorrugates != null)
            {
                waveCorrugates.Clear();
            }
            //刷新数据列表栏
            if (dataGridCorrugated.ItemsSource != null)
            {
                dataGridCorrugated.ItemsSource = null;
            }
            //刷新图形界面
            RefreshPane();
            //创建打开文件对话框对象
            OpenFileDialog openFile = new OpenFileDialog();
            //过滤文件类型
            openFile.Filter = "Designer Files (*.xml)|*.xml|All Files (*.*)|*.*";
            //选择最近一次打开的目录
            openFile.InitialDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.Recent);
            if (openFile.ShowDialog() == true)
            {
                try
                {
                    LoadWave(openFile.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        //定义加载录波数据方法
        private void LoadWave(string xmlName)
        {
            devList.Visibility = Visibility.Visible;
            lineList.Clear();
            sourceList.Clear();
            corrugateList.Clear();
            devList.Items.Clear();
            //devList.ItemsSource = null;
            List<string> comList = new List<string>();
            ComboBoxItem a = new ComboBoxItem();
            a.Content = "请选择设备";
            a.Visibility = Visibility.Collapsed;
            devList.Items.Add(a);
            try
            {
                XmlReading.Load(xmlName);
                //获取根节点
                xe = XmlReading.GetXmlDocumentRoot();
                //将根元素转换为根节点元素
                xn = XmlReading.ExchangeNodeElement(xe);
                //获取根节点元素的子节点
                xnl = xn.ChildNodes;
                if (xnl.Count > 0)
                {
                    for (int i = 0; i < xnl.Count; i++)
                    {
                        XmlElement nodeName = (XmlElement)xnl[i]; //读取第i项属性值
                        if (nodeName.Name.Equals("Version"))
                        {
                            string version = nodeName.InnerText;
                        }
                        else if (nodeName.Name.Equals("RecordCount"))
                        {
                            string recordCount = nodeName.InnerText;
                        }
                        else if (nodeName.Name.Equals("RecordByte"))
                        {
                            string recordByte = nodeName.InnerText;
                        }
                        else if (nodeName.Name.Contains("Record-"))
                        {
                            ComboBoxItem cbi = new ComboBoxItem();
                            cbi.Content = nodeName.Name;
                            devList.Items.Add(cbi);
                            //comList.Add(nodeName.Name);
                            string record = nodeName.InnerText;
                            byte[] byteArray = HexStringToByte(record);
                            parseWaveInfo(byteArray, nodeName.Name);
                        }
                    }
                    //devList.ItemsSource = comList;
                }
                else
                {
                    MessageBox.Show("配置文件不正确，请重新导入");
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public partial class Test
        {
            public string Name { get; set; }
        }
        private void parseWaveInfo(byte[] bytes, string nodeName)
        {
            //定义存储从xml读取的各条故障波形信息的字典
            Dictionary<string, List<short>> corrugatedDictionary = new Dictionary<string, List<short>>();
            List<CorrugatedInfo> lineDictionary = new List<CorrugatedInfo>();
            ObservableCollection<CorrugatedInfo> sourceDictionary = new ObservableCollection<CorrugatedInfo>();
            int i = 0;
            //故障记录条序号
            ushort sequence = (ushort)((bytes[i++] << 8) | bytes[i++]);
            //年
            ushort year = (ushort)((bytes[i++] << 8) | bytes[i++]);
            //月
            ushort month = (ushort)((bytes[i++] << 8) | bytes[i++]);
            //日
            ushort day = (ushort)((bytes[i++] << 8) | bytes[i++]);
            //时
            ushort hours = (ushort)((bytes[i++] << 8) | bytes[i++]);
            //分
            ushort minute = (ushort)((bytes[i++] << 8) | bytes[i++]);
            //秒
            ushort second = (ushort)((bytes[i++] << 8) | bytes[i++]);
            //毫秒
            ushort millisecond = (ushort)((bytes[i++] << 8) | bytes[i++]);
            //故障状态1
            ushort fault1 = (ushort)((bytes[i++] << 8) | bytes[i++]);
            //故障状态2
            ushort fault2 = (ushort)((bytes[i++] << 8) | bytes[i++]);
            //故障状态3
            ushort fault3 = (ushort)((bytes[i++] << 8) | bytes[i++]);
            //故障状态4
            ushort fault4 = (ushort)((bytes[i++] << 8) | bytes[i++]);
            //故障状态5
            ushort fault5 = (ushort)((bytes[i++] << 8) | bytes[i++]);
            //故障状态6
            ushort fault6 = (ushort)((bytes[i++] << 8) | bytes[i++]);
            //故障状态7
            ushort fault7 = (ushort)((bytes[i++] << 8) | bytes[i++]);
            //故障状态8
            ushort fault8 = (ushort)((bytes[i++] << 8) | bytes[i++]);
            //波形记录序号
            ushort recordIndex = (ushort)((bytes[i++] << 8) | bytes[i++]);
            //波形数量
            ushort waveNum = (ushort)((bytes[i++] << 8) | bytes[i++]);
            //保留(U16*2)
            i += 4;
            Random rd = new Random(10);
            for (int wn = 0; wn < waveNum; wn++)
            {
                try
                {
                    //波形代码
                    ushort waveCode = (ushort)((bytes[i++] << 8) | bytes[i++]);
                    //总点数
                    ushort total = (ushort)((bytes[i++] << 8) | bytes[i++]);
                    //故障点数
                    ushort faultNum = (ushort)((bytes[i++] << 8) | bytes[i++]);
                    //前时长
                    ushort preTime = (ushort)((bytes[i++] << 8) | bytes[i++]);
                    //前采样频率
                    i += 2;
                    //前点数
                    i += 2;
                    //后时长
                    ushort afterTime = (ushort)((bytes[i++] << 8) | bytes[i++]);
                    //后采样频率
                    ushort simFrequency = (ushort)((bytes[i++] << 8) | bytes[i++]);
                    //后点数
                    ushort afterNUm = (ushort)((bytes[i++] << 8) | bytes[i++]);
                    //保留
                    i += 2;
                    List<short> list = new List<short>();
                    //波形(S16*总点数)
                    for (int j = 0; j < total; j++)
                    {
                        list.Add((short)(bytes[i++] << 8 | bytes[i++]));
                    }
                    if (list.Count > 0)
                    {
                        corrugatedDictionary.Add(waveCode.ToString(), list);
                        CorrugatedInfo corrugatedInfo = new CorrugatedInfo();
                        corrugatedInfo.Name = waveCode.ToString();
                        corrugatedInfo.AllPoints = total;
                        corrugatedInfo.FaultPoint = faultNum;
                        corrugatedInfo.AfterTime = afterTime;
                        corrugatedInfo.BehindPoints = afterNUm;
                        corrugatedInfo.LineColor = System.Drawing.Color.FromArgb(rd.Next(255), rd.Next(255), rd.Next(255));
                        //数据集合
                        sourceDictionary.Add(corrugatedInfo);
                        lineDictionary.Add(corrugatedInfo);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }
            lineList.Add(nodeName, lineDictionary);
            sourceList.Add(nodeName, sourceDictionary);
            corrugateList.Add(nodeName, corrugatedDictionary);
            selectionChanged();
            //dataGridCorrugated.ItemsSource = waveSources;
        }
        /// <summary>
        /// 刷新波纹查看图形界面
        /// </summary>
        private void RefreshPane()
        {

            List<short> list = new List<short>();
            GraphPane myPane = Fault_waveform.GraphPane;
            //myPane.XAxis.Scale.Min = -startpoint * time;
            //myPane.XAxis.Scale.Max = startpoint * time;
            // myPane.XAxis.Scale.MinorStep = time;
            myPane.CurveList.Clear();
            try
            {
                foreach (CorrugatedInfo v in waveSources)
                {
                    if (v.Selected)
                    {
                        CorrugatedInfo corrugatedInfo = (CorrugatedInfo)dataGridCorrugated.SelectedItem;
                        foreach (CorrugatedInfo a in waveLines)
                        {
                            if (v.Name == a.Name)
                            {
                                PointPairList points = new PointPairList();
                                _ = waveCorrugates.TryGetValue(v.Name, out List<short> value);
                                list = value;
                                for (int i = 0; i < list.Count; i++)
                                {
                                    short y = list[i];
                                    points.Add(i, y);
                                }
                                LineItem lineItem = myPane.AddCurve(v.Name, points, a.LineColor, SymbolType.None);
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Fault_waveform.AxisChange();
            Fault_waveform.Refresh();
        }

        /**
         * describe:十六进制字符串转byte数组
         * author:chengshichao
         * date:2021.12.09
         **/
        public static byte[] HexStringToByte(string hexString)
        {
            //去除掉空格
            hexString = hexString.Replace(" ", "");
            //新建字符数组用于保存转换后的字符
            byte[] returnBytes = new byte[hexString.Length / 2];
            //如果字符串长度不是偶数则在结尾处补位
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            for (int i = 0; i < returnBytes.Length; i++)
                //进行转换
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        public void selectionChanged()
        {
            if (devList.Items.Count > 1)
            {
                devList.SelectedIndex = 0;
            }
        }

        /**
         * 下拉选择框更改触发事件
         */
        private void waveList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (devList.Text != "" && devList.SelectedItem != null)
            {
                //MessageBox.Show(devList.Text);
                devList.Text = ((ComboBoxItem)devList.SelectedItem).Content.ToString();
                //devList.Text = devList.SelectedItem.ToString();
                lineList.TryGetValue(devList.Text, out List<CorrugatedInfo> lineInfo);
                sourceList.TryGetValue(devList.Text, out ObservableCollection<CorrugatedInfo> sourceInfo);
                corrugateList.TryGetValue(devList.Text, out Dictionary<string, List<short>> corrugateInfo);
                waveLines = lineInfo;
                waveCorrugates = corrugateInfo;
                waveSources = sourceInfo;
                dataGridCorrugated.ItemsSource = sourceInfo;
            }

        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            SFTPHelper sftp = new SFTPHelper(this.ipAddress.Text, "22", "root", "sungrow2016");
            //SFTPHelper sftp = new SFTPHelper("192.168.193.1", "22", "admin", "111111");
            try
            {
                if (sftp.Connect())
                {
                    // HG - POWER_5700kW - MDSP(CAN2 - 1)
                    ArrayList list = sftp.GetFileList1("/home/SGLogger/FaultWave");
                    //ArrayList list = sftp.GetFileList1(@"/test/");
                    this.fileList.ItemsSource = list;
                    download.IsEnabled = false;
                    delete.IsEnabled = false;
                    back.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接SFTP服务器出现故障：" + ex.Message);
            }
            finally
            {
                sftp.Disconnect();
            }
        }
        #endregion

        /*
        #########################################################################################
        ##############################################在线升级模块###############################
        #########################################################################################
        */

        #region 在线升级

        int upgrade1 = 0;
        private void upgradeShow_Click(object sender, RoutedEventArgs e)
        {
            upgrade1 = 3;
            monitor.Visibility = Visibility.Collapsed;
            wave.Visibility = Visibility.Collapsed;
            history.Visibility = Visibility.Collapsed;
            upgrade.Visibility = Visibility.Visible;
            if (readDataTimer.IsEnabled)
            {
                //关闭定时器
                readDataTimer.Stop();
                //ModbusTcp.Instance.close();
            }
        }

        //升级文件路径
        string upgradeFilePath = null;
        string fileName = null;
        /// <summary>
        /// 浏览文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void browse_Click(object sender, RoutedEventArgs e)
        {
            if (isConnection)
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Filter = "SGU;SCU|*.sgu;*.scu";

                if (openFile.ShowDialog() == true)
                {
                    try
                    {
                        flagUpload = 0;
                        upgradeFilePath = openFile.FileName;
                        if (upgradeFilePath.EndsWith("scu"))
                        {
                            turnUp.Visibility = System.Windows.Visibility.Hidden;
                        }
                        else if (upgradeFilePath.EndsWith("sgu"))
                        {
                            turnUp.Visibility = System.Windows.Visibility.Visible;
                        }
                        fileName = GetSguName(upgradeFilePath);
                        sourceFile.Text = fileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("请先进行连接");
            }
            
        }
        /// <summary>
        /// 截取文件名
        /// </summary>
        /// <param name="sgufile"></param>
        /// <returns></returns>
        private string GetSguName(string sgufile)
        {
            string sgufiletmp = sgufile.ToLower();
            int beginIndex = sgufiletmp.LastIndexOf("\\");
            //int endIndex = sgufiletmp.LastIndexOf(".sgu");
            //string getstr = sgufile.Substring(beginIndex + 1, endIndex - beginIndex - 1);
            string getstr = sgufile.Substring(beginIndex + 1, sgufiletmp.Length - beginIndex - 1);
            return getstr;

        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private int flagUpload = 0;
        private int flagGujianU = 0;
        private void upload_Click(object sender, RoutedEventArgs e)
        {
            if(upgradeFilePath != null)
            {
                try
                {
                    //SFTPHelper sftp = new SFTPHelper("192.168.193.1", "22", "admin", "111111");

                    SFTPHelper sftp = new SFTPHelper(ModbusTcp.serverIp, "22", "root", "sungrow2016");
                    //sftp.Connect();
                    if (fileName.EndsWith("scu"))
                    {
                        sftp.Put(upgradeFilePath, pathTable["A8自升级"] +@"/" + fileName);
                    }
                    else
                    {
                        // sftp.Put(upgradeFilePath, @"/home/SGLogger/Update/" + fileName);
                        sftp.Put(upgradeFilePath, pathTable["固件在线升级"] + @"/" + fileName);
                    }
                    //sftp.Put(upgradeFilePath, @"/home/SGLogger/Update/" + fileName);
                    flagUpload = 1;
                    flagGujianU = 0;
                    //sftp.Disconnect();
                    Thread.Sleep(1000);
                    MessageBox.Show("上传成功！");
                    upgradeButton.IsEnabled = true;
                }
                catch(Exception ex)
                {
                    flag = 0;
                    MessageBox.Show("升级文件上传失败！");
                }
                
            }
            else
            {
                MessageBox.Show("请选择升级文件！");
            }

        }
        string flagvs1 = "";
        string flagvs11 = "";
        string flagvs2 = "";
        string flagvs21 = "";
        string flagvs3 = "";
        string flagvs31 = "";

        public void uploadG_Click(object sender, RoutedEventArgs e) {
            readDataTimer.Stop();
            if (realThread != null)
            {
                realThread.Abort();
                realThread = null;
                if (realThread != null)
                    MessageBox.Show("线程未销毁");
            }
            if (this.connect.Content.ToString().Trim() == "连接")
            {
                MessageBox.Show("请建立连接");
                return;
            }
            if (flagUpload !=1)
            {
                MessageBox.Show("请上传文件至文件服务器");
                return;
            }
            byte[] bytDate = new byte[2];
            short v = -1;
            
            short vs1 = 0;
            short vs2 = 0;
            short vs3 = 0;
            
            if (fileName.EndsWith("scu"))
            {
                
                int adress1RW = Convert.ToInt32(addrTableRW["A8平台升级设置"]) - 1;
                int flagOn = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)adress1RW, 1, new byte[] { (byte)(0xAA >> 8), (byte)(0xAA & 255) });
                if (flagOn == 0)
                {
                    MessageBox.Show("A8平台升级设置");
                    Thread realThread2 = new Thread(() =>
                    {

                        for (int i = 0; i < 10; i++)
                        {
                            if (ModbusTcp.Instance.ReadInputRegisters4(slaveIdText, Convert.ToInt32(addrTableR["固件升级文件反馈"]) - 1, 1, bytDate) != 0)
                            {
                                MessageBox.Show("通讯异常");
                                flag = 0;
                            }
                            else
                            {
                                try
                                {
                                    flag = 1;
                                    v = (short)(bytDate[0 * 2 + 1] | (bytDate[0 * 2] << 8));
                                    if (v == 1)
                                    {
                                        this.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            //刷新数据
                                            MessageBox.Show("升级scu"+v);
                                        }));

                                        flag = 0;
                                        break;
                                    }
                                    else
                                    {
                                        //waveON.IsEnabled = false;
                                    }
                                    Thread.Sleep(1000);
                                }
                                catch (Exception)
                                {

                                    flag = 0;
                                    break;
                                }
                                finally
                                {
                                    //
                                }

                            }
                        }
                        MessageBox.Show("升级scu" + v);
                        flag = 0;
                    });
                    realThread2.Start();
                }
                else
                {
                    
                    MessageBox.Show("升级失败");
                }
            }
            else if (fileName.EndsWith("sgu"))
            {
                int adress1RW = Convert.ToInt32(addrTableRW["固件上传"]) - 1;
                int adress1fankui = Convert.ToInt32(addrTableRW["固件编码"]) - 1;

                if (fileName.StartsWith("MDSP_HGP"))
                {
                    int flagfankui = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)adress1fankui, 1, new byte[] { (byte)(4 >> 8), (byte)(4 & 255) });
                    if (flagfankui != 0)
                    {
                        flagfankui = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)adress1fankui, 1, new byte[] { (byte)(4 >> 8), (byte)(4 & 255) });
                    }
                    if (flagfankui != 0)
                    {
                        MessageBox.Show("通信异常");
                        return;
                    }
                }
                else if(fileName.StartsWith("SDSP_HGP"))
                {
                    int flagfankui = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)adress1fankui, 1, new byte[] { (byte)(4 >> 8), (byte)(4 & 255) });
                    if (flagfankui != 0)
                    {
                        flagfankui = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)adress1fankui, 1, new byte[] { (byte)(4 >> 8), (byte)(4 & 255) });
                    }
                    if (flagfankui != 0)
                    {
                        MessageBox.Show("通信异常");
                        return;
                    }
                }
                else if (fileName.StartsWith("ARM_HGP"))
                {
                    int flagfankui = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)adress1fankui, 1, new byte[] { (byte)(4 >> 8), (byte)(4 & 255) });
                    if (flagfankui != 0)
                    {
                        flagfankui = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)adress1fankui, 1, new byte[] { (byte)(4 >> 8), (byte)(4 & 255) });
                    }
                    if (flagfankui != 0)
                    {
                        MessageBox.Show("通信异常");
                        return;
                    }
                }
                else if (fileName.StartsWith("OTHER"))
                {
                    int flagfankui = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)adress1fankui, 1, new byte[] { (byte)(4 >> 8), (byte)(4 & 255) });
                    if (flagfankui != 0)
                    {
                        flagfankui = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)adress1fankui, 1, new byte[] { (byte)(4 >> 8), (byte)(4 & 255) });
                    }
                    if (flagfankui != 0)
                    {
                        MessageBox.Show("通信异常");
                        return;
                    }
                }
                else
                {
                    int flagfankui = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)adress1fankui, 1, new byte[] { (byte)(4 >> 8), (byte)(4 & 255) });
                    if (flagfankui != 0)
                    {
                        flagfankui = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)adress1fankui, 1, new byte[] { (byte)(4 >> 8), (byte)(4 & 255) });
                    }
                    if (flagfankui != 0)
                    {
                        MessageBox.Show("通信异常");
                        return;
                    }
                }
                

                int flagOn = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)adress1RW, 1, new byte[] { (byte)(0xAA >> 8), (byte)(0xAA & 255) });
                byte[] bytDateDSP = new byte[6];
                
                if (flagOn == 0)
                {
                    MessageBox.Show("固件上传成功");
                    this._loading.Visibility = Visibility.Visible;
                    this.upgrade.IsEnabled = false;
                    ObservableCollection<upgradeInfo> upgradeInfos = new ObservableCollection<upgradeInfo>();
                    upgradeInfos.Clear();
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        upgradeGrid.ItemsSource = null;
                        //upgradeGrid.Items.Clear();
                    }));
                    Thread realThread2 = new Thread(() =>
                    {//CAN1数量
                        int can1Count = Convert.ToInt32(addrTableR["CAN1数量"]);
                        int can2Count = Convert.ToInt32(addrTableR["CAN2数量"]);
                        for (int i = 0; i < 20; i++)
                        {
                           
                            if (ModbusTcp.Instance.ReadInputRegisters4(slaveIdText, Convert.ToInt32(addrTableR["CAN1端口-升级状态"]) - 1, 3, bytDateDSP) != 0)
                            {
                                // MessageBox.Show("通讯异常");
                                //flag = 0;
                                //break;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                try
                                {
                                    flagGujianU = 1;
                                    vs1 = (short)(bytDateDSP[0 * 2 + 1] | (bytDateDSP[0 * 2] << 8));
                                    vs2 = (short)(bytDateDSP[1 * 2 + 1] | (bytDateDSP[1 * 2] << 8));
                                    vs3 = (short)(bytDateDSP[2 * 2 + 1] | (bytDateDSP[2 * 2] << 8));
                                    Console.WriteLine(vs1+","+vs2+","+vs3);
                                    if (vs1!=0&&vs2!=0&&vs3!=0)
                                    {
                                        break;
                                    }

                                    //v = (short)(bytDateDSP[0 * 2 + 1] | (bytDateDSP[0 * 2] << 8));

                                    Thread.Sleep(3000);
                                }
                                catch (Exception)
                                {

                                    flag = 0;
                                    break;
                                }
                                finally
                                {
                                    //
                                }

                            }
                        }
                        flagGujianU = 2;
                        int can1sort = 0;
                        byte[] bytDateDSP2 = new byte[6];
                        foreach (KeyValuePair<string,string> d in CAN1Sort)
                        {
                            Console.WriteLine(d.Key + " " + d.Value);
                            string keyset = d.Key;
                            can1sort++;
                            if (keyset.IndexOf("固件编码") >= 0)
                            {
                                
                                upgradeInfo upgradeInfo = new upgradeInfo();
                                upgradeInfo.Name = keyset.Substring(0,keyset.IndexOf("设"));
                                if (ModbusTcp.Instance.ReadInputRegisters4(slaveIdText, Convert.ToInt32(CAN1Sort[keyset]) - 1, 3, bytDateDSP2) != 0)
                                {
                                    // MessageBox.Show("通讯异常");
                                    //flag = 0;
                                    //break;
                                }
                                else
                                {
                                    upgradeInfo.Result = resultOK(vs1);
                                    for (int i = 0; i < bytDateDSP2.Length / 2; i++)
                                    {
                                        string firewareCdStr = codeOk((short)(bytDateDSP2[0 * 2 + 1] | (bytDateDSP2[0 * 2] << 8)));
                                        upgradeInfo.FirmwareCode = codeOk((short)(bytDateDSP2[0 * 2 + 1] | (bytDateDSP2[0 * 2] << 8)));
                                        ushort vcan = (ushort)(bytDateDSP2[1 * 2 + 1] | (bytDateDSP2[1 * 2] << 8));
                                        int v1 = (bytDateDSP2[1 * 2] & 0xf0 )>> 4;
                                        int v11 = bytDateDSP2[1 * 2] & 0x0f;
                                        int v12 = bytDateDSP2[1 * 2 + 1];
                                        if (firewareCdStr.Equals("---"))
                                        {
                                            upgradeInfo.FirmwareVersion = firewareCdStr;
                                            upgradeInfo.Result = "---";
                                        }
                                        else
                                        {
                                            if (v11 >= 10)
                                            {
                                                upgradeInfo.FirmwareVersion = "P0" + v1 + "" + "_V" + v11 + "_V" + v12;
                                            }
                                            else
                                            {
                                                upgradeInfo.FirmwareVersion = "P0" + v1 + "" + "_V0" + v11 + "_V" + v12;
                                            }
                                        }
                                        upgradeInfo.ProgressValue = "---";
                                    }
                                    upgradeInfos.Add(upgradeInfo);
                                }
                            }
                            

                            
                        }

                        can1sort = 0;
                        foreach (KeyValuePair<string, string> d in CAN2Sort)
                        {
                            can1sort++;
                            Console.WriteLine(d.Key + " " + d.Value);
                            string keyset = d.Key;
                            if (keyset.IndexOf("固件编码") >= 0)
                            {
                                
                                upgradeInfo upgradeInfo = new upgradeInfo();
                                upgradeInfo.Name = keyset.Substring(0, keyset.IndexOf("设"));
                                if (ModbusTcp.Instance.ReadInputRegisters4(slaveIdText, Convert.ToInt32(CAN2Sort[keyset]) - 1, 3, bytDateDSP2) != 0)
                                {
                                    // MessageBox.Show("通讯异常");
                                    //flag = 0;
                                    //break;
                                }
                                else
                                {
                                    upgradeInfo.Result = resultOK(vs2);
                                    for (int i = 0; i < bytDateDSP2.Length / 2; i++)
                                    {
                                        string firewareCdStr = codeOk((short)(bytDateDSP2[0 * 2 + 1] | (bytDateDSP2[0 * 2] << 8)));
                                        upgradeInfo.FirmwareCode = codeOk((short)(bytDateDSP2[0 * 2 + 1] | (bytDateDSP2[0 * 2] << 8)));
                                        ushort vcan = (ushort)(bytDateDSP2[1 * 2 + 1] | (bytDateDSP2[1 * 2] << 8));
                                        int v1 = (bytDateDSP2[1 * 2] & 0xf0) >> 4;
                                        int v11 = bytDateDSP2[1 * 2] & 0x0f;
                                        int v12 = bytDateDSP2[1 * 2 + 1];
                                        if (firewareCdStr.Equals("---"))
                                        {
                                            upgradeInfo.FirmwareVersion = firewareCdStr;
                                            upgradeInfo.Result = "---";
                                        }
                                        else
                                        {
                                            if (v11 >= 10)
                                            {
                                                upgradeInfo.FirmwareVersion = "P0" + v1 + "" + "_V" + v11 + "_V" + v12;
                                            }
                                            else
                                            {
                                                upgradeInfo.FirmwareVersion = "P0" + v1 + "" + "_V0" + v11 + "_V" + v12;
                                            }
                                        }
                                        upgradeInfo.ProgressValue = "---";
                                    }
                                    upgradeInfos.Add(upgradeInfo);
                                }
                            }



                        }


                        foreach (KeyValuePair<string, string> d in CAN3Sort)
                        {
                            Console.WriteLine(d.Key + " " + d.Value);
                            string keyset = d.Key;
                            if (keyset.IndexOf("升级编码") >= 0)
                            {
                                if (ModbusTcp.Instance.ReadInputRegisters4(slaveIdText, Convert.ToInt32(CAN3Sort[keyset]) - 1, 3, bytDateDSP2) != 0)
                                {
                                    // MessageBox.Show("通讯异常");
                                    //flag = 0;
                                    //break;
                                }
                                else
                                {
                                    upgradeInfo upgradeInfo = new upgradeInfo();
                                    upgradeInfo.Name = keyset.Substring(0, keyset.IndexOf("升"));

                                    upgradeInfo.Result = resultOK(vs3);
                                    for (int i = 0; i < bytDateDSP2.Length / 2; i++)
                                    {

                                        string firewareCdStr = codeOk((short)(bytDateDSP2[0 * 2 + 1] | (bytDateDSP2[0 * 2] << 8)));
                                        upgradeInfo.FirmwareCode = codeOk((short)(bytDateDSP2[0 * 2 + 1] | (bytDateDSP2[0 * 2] << 8)));
                                        ushort vcan = (ushort)(bytDateDSP2[1 * 2 + 1] | (bytDateDSP2[1 * 2] << 8));
                                        int v1 = (bytDateDSP2[1 * 2] & 0xf0) >> 4;
                                        int v11 = bytDateDSP2[1 * 2] & 0x0f;
                                        int v12 = bytDateDSP2[1 * 2 + 1];
                                        if (firewareCdStr.Equals("---"))
                                        {
                                            upgradeInfo.FirmwareVersion = firewareCdStr;
                                            upgradeInfo.Result = "---";
                                        }
                                        else
                                        {
                                            if (v11 >= 10)
                                            {
                                                upgradeInfo.FirmwareVersion = "P0" + v1 + "" + "_V" + v11 + "_V" + v12;
                                            }
                                            else
                                            {
                                                upgradeInfo.FirmwareVersion = "P0" + v1 + "" + "_V0" + v11 + "_V" + v12;
                                            }
                                        }

                                        upgradeInfo.ProgressValue = "---";
                                    }
                                    upgradeInfos.Add(upgradeInfo);
                                }
                                break;
                            }
                            
                           
                        }
                                

                        flagvs1 = "";
                        flagvs2 = "";
                        flagvs3 = "";
                        if (vs1==1)
                        {
                            flagvs1 = "1";
                        }
                        if (vs2==1)
                        {
                            flagvs2 ="2";
                        }
                        if (vs3 == 1) {
                            flagvs3 =  "3";
                        }
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            this._loading.Visibility = Visibility.Collapsed;
                            this.upgrade.IsEnabled = true;
                            upgradeGrid.ItemsSource = null;
                            upgradeGrid.ItemsSource = upgradeInfos;
                        }));
                        flag = 0;
                    });
                    realThread2.Start();
                }
                else
                {
                    MessageBox.Show("升级失败");
                }
            }
            
        }

        public String codeOk(int result)
        {
            String resultStr = "";

            switch (result)
            {
                case 0:
                    resultStr = "---";
                    break;
                case 1:
                    resultStr = "MDSP_HGP";
                    break;
                case 2:
                    resultStr = "SDSP_HGP";
                    break;
                case 3:
                    resultStr = "ARM_HGP";
                    break;
                case 4:
                    resultStr = "OTHER_DEV_VERSION" + result; ;
                    break;
                default:
                    resultStr = "OTHER_DEV_VERSION" + result;
                    break;
            }
            return resultStr;

        }
        public String resultOK(int result) {
            String resultStr = "";

            switch (result)
            {
                case 0:
                    resultStr = "未定义";
                    break;
                case 1:
                    resultStr = "版本读取完成";
                    break;
                case 2:
                    resultStr = "升级完成";
                    break;
                case 3:
                    resultStr = "升级失败";
                    break;
                default:
                    resultStr = "初始状态"+result;
                    break;
            }
            return resultStr;

        }
        short vs1 = 0;
        short vs2 = 0;
        short vs3 = 0;
        int flagA8 = 0;
        public void uploadResult_Click(object sender, RoutedEventArgs e) {
            readDataTimer.Stop();
            
            if (realThread != null)
            {
                realThread.Abort();
                realThread = null;
                if (realThread != null)
                    MessageBox.Show("线程未销毁");
            }
            if (this.connect.Content.ToString().Trim() == "连接")
            {
                MessageBox.Show("请建立Modbus连接");
                flagA8 = 0;
                return;
            }
            if (flagUpload != 1)
            {
                MessageBox.Show("请上传文件至文件服务器");
                return;
            }
            if (flagA8 ==1)
            {
                MessageBox.Show("A8正在升级");
                
                return;
            }
          
            byte[] bytDate = new byte[2];
            short v = -1;

            if (fileName.EndsWith("scu"))
            {
                int adress1RW = Convert.ToInt32(addrTableRW["A8平台自升级设置"]) - 1;
                //ModbusTcp.Instance.WriteMultiRegisters((ushort)adress1RW, 1, new byte[] { (byte)(0xAA >> 8), (byte)(0xAA & 255) });
                //int flagOn = ModbusTcp.Instance.WriteMultiRegisters((ushort)adress1RW, 1, new byte[] { (byte)(0xAA >> 8), (byte)(0xAA & 255) });

                ModbusTcp.Instance.close();
                int flagOn = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)adress1RW, 1, new byte[] { (byte)(0xAA >> 8), (byte)(0xAA & 255) });
                if (flagOn == 0)
                {
                    
                    flagA8 = 1;
                    SFTPHelper sftp = new SFTPHelper(ModbusTcp.serverIp, "22", "root", "sungrow2016");
                    //SFTPHelper sftp = new SFTPHelper("192.168.193.1", "22", "admin", "111111");
                    try
                    {
                        if (sftp.Connect())
                        {
                            ArrayList list = sftp.GetFileList1(@"/home/DeviceInitTest/SCU_Operate/");

                            for (int j = 0; j < list.Count; j++)
                            {
                                string filenamelist = (string)list[j];
                                if (filenamelist.IndexOf("ok") >= 0)
                                {
                                    sftp.Delete(@"/home/DeviceInitTest/SCU_Operate/A8_update-ok.txt");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("连接SFTP服务器出现故障：" + ex.Message);
                        sftp.Disconnect();
                        return;

                    }
                    finally {
                        sftp.Disconnect();
                    }
                    //MessageBox.Show("固件上传成功");
                    this._loading.Visibility = Visibility.Visible;
                    this.upgrade.IsEnabled = false;
                    Thread realThread2 = new Thread(() =>
                    {

                        for (int i = 0; i < 15; i++)
                        {
                            if (ModbusTcp.Instance.ReadInputRegisters4(slaveIdText, Convert.ToInt32(addrTableR["A8平台升级文件反馈态"]) - 1, 1, bytDate) != 0)
                            {
                                //MessageBox.Show("通讯异常");
                                flag = 0;
                            }
                            else
                            {
                                try
                                {
                                    flag = 1;
                                    v = (short)(bytDate[0 * 2 + 1] | (bytDate[0 * 2] << 8));
                                    if (v == 1)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        //waveON.IsEnabled = false;
                                    }
                                    Thread.Sleep(1000);
                                }
                                catch (Exception)
                                {

                                    flag = 0;
                                    break;
                                }
                                finally
                                {
                                    //
                                }

                            }
                        }
                        Console.WriteLine("A8---------------------------------------"+v);
                        if (v == 1)
                        {
                            
                            for (int i = 0; i < 22; i++)
                            {
                                Thread.Sleep(1000);
                            }
                            
                            try
                            {
                                Console.WriteLine("A8s1---------------------------------------" + v);
                                SFTPHelper sftp1 = new SFTPHelper(ModbusTcp.serverIp, "22", "root", "sungrow2016");
                                Console.WriteLine("A8s2---------------------------------------" + v);
                                if (sftp1.Connect())
                                {
                                    ArrayList list = sftp1.GetFileList1(@"/home/DeviceInitTest/SCU_Operate/");


                                    //sftp.Put(upgradeFilePath, pathTable["A8自升级"] + @"/" + fileName);
                                    int i = 0;
                                    int flagsftp = 0;
                                    if (list == null)
                                    {
                                        this.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            //刷新数据
                                            this._loading.Visibility = Visibility.Collapsed;
                                            this.upgrade.IsEnabled = true;
                                            MessageBox.Show("A8平台升级失败");
                                        }));


                                        return;
                                    }

                                    for (int j = 0; j < list.Count; j++)
                                    {
                                        string filenamelist = (string)list[j];
                                        if (filenamelist.IndexOf("ok") >= 0)
                                        {
                                            flagsftp = 1;

                                        }
                                    }
                                    if (flagsftp == 1)
                                    {

                                        this.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            //刷新数据
                                            this._loading.Visibility = Visibility.Collapsed;
                                            this.upgrade.IsEnabled = true;
                                            MessageBox.Show("A8平台升级成功");
                                        }));
                                    }
                                    else
                                    {
                                        this.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            //刷新数据
                                            this._loading.Visibility = Visibility.Collapsed;
                                            this.upgrade.IsEnabled = true;
                                            MessageBox.Show("A8平台升级失败");
                                        }));

                                    }

                                }
                                else
                                {
                                    this.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        
                                        MessageBox.Show("SFTP服务器未开启");
                                    }));
                                }
                                
                                
                            }
                            catch (Exception ex)
                            {
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    //刷新数据
                                    this._loading.Visibility = Visibility.Collapsed;
                                    this.upgrade.IsEnabled = true;
                                    MessageBox.Show("连接SFTP服务器出现故障：" + ex.Message);
                                }));

                                
                            }
                            finally
                            {
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    //刷新数据
                                    this._loading.Visibility = Visibility.Collapsed;
                                    this.upgrade.IsEnabled = true;
                                    upgradeButton.IsEnabled = false;
                                }));
                                sftp.Disconnect();
                            }

                            flagA8 = 0;

                        }
                        else if(v==0)
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                //刷新数据
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    //刷新数据
                                    this._loading.Visibility = Visibility.Collapsed;
                                    this.upgrade.IsEnabled = true;
                                    MessageBox.Show("初始未知状态");
                                }));
                                flagA8 = 0;
                                return;
                            }));
                        }
                        else if (v == 2)
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                //刷新数据
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    //刷新数据
                                    this._loading.Visibility = Visibility.Collapsed;
                                    this.upgrade.IsEnabled = true;
                                    MessageBox.Show("SCU打开失败");
                                }));
                                flagA8 = 0;
                                return;
                            }));
                        }
                        else if (v == 3)
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                //刷新数据
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    //刷新数据
                                    this._loading.Visibility = Visibility.Collapsed;
                                    this.upgrade.IsEnabled = true;
                                    MessageBox.Show("crc校验error");
                                }));
                                flagA8 = 0;
                                return;
                            }));
                        }
                        else
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                //刷新数据
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    //刷新数据
                                    this._loading.Visibility = Visibility.Collapsed;
                                    this.upgrade.IsEnabled = true;
                                    MessageBox.Show("其他中间过程错误");
                                }));
                                flagA8 = 0;
                                return;
                            }));
                        }
                        
                    });
                    realThread2.Start();
                }
                else
                {
                    MessageBox.Show("通信异常，A8平台升级失败");
                }
            }
            else if (fileName.EndsWith("sgu"))
            {
                if (flagGujianU == 0)
                {
                    MessageBox.Show("请先固件升级");
                    return;
                }
                if (flagGujianU == 1)
                {
                    MessageBox.Show("正在读取固件反馈文件");
                    return;
                }
                int adress1RW = Convert.ToInt32(addrTableRW["固件升级设置确认"]) - 1;
                uint value = 0x80000000;
                int flagOn = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)adress1RW, 2, new byte[] { (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF), (byte)((value >> 24) & 0xFF), (byte)((value >> 16) & 0xFF) });
                byte[] bytDateDSP = new byte[6];
                ObservableCollection<upgradeInfo> upgradeInfos = new ObservableCollection<upgradeInfo>();
                if (flagOn != 0)
                {
                    flagOn = ModbusTcp.Instance.WriteMultiRegisters(slaveIdText, (ushort)adress1RW, 2, new byte[] { (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF), (byte)((value >> 24) & 0xFF), (byte)((value >> 16) & 0xFF) });
                }
                if (flagOn == 0)
                {
                    MessageBox.Show("固件上传成功");
                    this._loading.Visibility = Visibility.Visible;
                    this.upgrade.IsEnabled = false;
                    Thread realThread2 = new Thread(() =>
                    {
                        int can1Count = Convert.ToInt32(addrTableR["CAN1数量"]);
                        int can2Count = Convert.ToInt32(addrTableR["CAN2数量"]);
                        int can1sort = 0;
                        
                        bool sucess = false;
                        for (int k = 0; k < 263; k++)
                        {
                            if (ModbusTcp.Instance.ReadInputRegisters4(slaveIdText, Convert.ToInt32(addrTableR["CAN1端口-升级状态"]) - 1, 3, bytDateDSP) != 0)
                            {
                                // MessageBox.Show("通讯异常");
                                //flag = 0;
                                //break;
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                try
                                {
                                    flagGujianU = 1;
                                    vs1 = (short)(bytDateDSP[0 * 2 + 1] | (bytDateDSP[0 * 2] << 8));
                                    vs2 = (short)(bytDateDSP[1 * 2 + 1] | (bytDateDSP[1 * 2] << 8));
                                    vs3 = (short)(bytDateDSP[2 * 2 + 1] | (bytDateDSP[2 * 2] << 8));
                                    /*if ((vs1 == 2 || vs1 == 3) && (vs2 == 2 || vs2 == 3)&& (vs3 == 2  ||  vs3 == 3))
                                    {

                                        break;
                                    }
                                    */
                                    //v = (short)(bytDateDSP[0 * 2 + 1] | (bytDateDSP[0 * 2] << 8));

                                    Thread.Sleep(1000);
                                }
                                catch (Exception)
                                {

                                    flag = 0;
                                    break;
                                }
                                finally
                                {
                                    //
                                }

                            }
                            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                upgradeInfos.Clear();
                            }));
                                
                            
                            byte[] bytDateDSP2 = new byte[6];
                            foreach (KeyValuePair<string, string> d in CAN1Sort)
                            {
                                can1sort++;
                                Console.WriteLine(d.Key + " " + d.Value);
                                string keyset = d.Key;
                                if (keyset.IndexOf("固件编码") >= 0)
                                {
                                    
                                    upgradeInfo upgradeInfo = new upgradeInfo();
                                    upgradeInfo.Name = keyset.Substring(0, keyset.IndexOf("设"));
                                    if (ModbusTcp.Instance.ReadInputRegisters4(slaveIdText, Convert.ToInt32(CAN1Sort[keyset]) - 1, 3, bytDateDSP2) != 0)
                                    {
                                        // MessageBox.Show("通讯异常");
                                        //flag = 0;
                                        //break;
                                    }
                                    else
                                    {
                                        upgradeInfo.Result = resultOK(vs1);
                                        for (int i = 0; i < bytDateDSP2.Length / 2; i++)
                                        {
                                            string firewareCdStr = codeOk((short)(bytDateDSP2[0 * 2 + 1] | (bytDateDSP2[0 * 2] << 8)));
                                            upgradeInfo.FirmwareCode = codeOk((short)(bytDateDSP2[0 * 2 + 1] | (bytDateDSP2[0 * 2] << 8)));
                                            ushort vcan = (ushort)(bytDateDSP2[1 * 2 + 1] | (bytDateDSP2[1 * 2] << 8));
                                            int v1 = (bytDateDSP2[1 * 2] & 0xf0) >> 4;
                                            int v11 = bytDateDSP2[1 * 2] & 0x0f;
                                            int v12 = bytDateDSP2[1 * 2 + 1];
                                            if (firewareCdStr.Equals("---"))
                                            {
                                                upgradeInfo.FirmwareVersion = firewareCdStr;
                                                upgradeInfo.Result = "---";
                                            }
                                            else
                                            {
                                                if(v11 >=10)
                                                {
                                                    upgradeInfo.FirmwareVersion = "P0" + v1 + "" + "_V" + v11 + "_V" + v12;
                                                }
                                                else
                                                {
                                                    upgradeInfo.FirmwareVersion = "P0" + v1 + "" + "_V0" + v11 + "_V" + v12;
                                                }
                                            }
                                            upgradeInfo.ProgressValue = (short)((bytDateDSP2[2 * 2 + 1] )) + "%";
                                            if ((bytDateDSP2[2 * 2 + 1]) == 100)
                                            {
                                                sucess = true;
                                            }
                                        }
                                        System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                                        {
                                            upgradeInfos.Add(upgradeInfo);
                                        }));

                                    }
                                }



                            }

                            can1sort = 0;
                            foreach (KeyValuePair<string, string> d in CAN2Sort)
                            {
                                can1sort++;
                                Console.WriteLine(d.Key + " " + d.Value);
                                string keyset = d.Key;
                                if (keyset.IndexOf("固件编码") >= 0)
                                {
                                    
                                    upgradeInfo upgradeInfo = new upgradeInfo();
                                    upgradeInfo.Name = keyset.Substring(0, keyset.IndexOf("设"));
                                    if (ModbusTcp.Instance.ReadInputRegisters4(slaveIdText, Convert.ToInt32(CAN2Sort[keyset]) - 1, 3, bytDateDSP2) != 0)
                                    {
                                        // MessageBox.Show("通讯异常");
                                        //flag = 0;
                                        //break;
                                    }
                                    else
                                    {
                                        upgradeInfo.Result = resultOK(vs2);
                                        for (int i = 0; i < bytDateDSP2.Length / 2; i++)
                                        {
                                            string firewareCdStr = codeOk((short)(bytDateDSP2[0 * 2 + 1] | (bytDateDSP2[0 * 2] << 8)));
                                            upgradeInfo.FirmwareCode = codeOk((short)(bytDateDSP2[0 * 2 + 1] | (bytDateDSP2[0 * 2] << 8)));
                                            ushort vcan = (ushort)(bytDateDSP2[1 * 2 + 1] | (bytDateDSP2[1 * 2] << 8));
                                            int v1 = (bytDateDSP2[1 * 2] & 0xf0) >> 4;
                                            int v11 = bytDateDSP2[1 * 2] & 0x0f;
                                            int v12 = bytDateDSP2[1 * 2 + 1];
                                            if (firewareCdStr.Equals("---"))
                                            {
                                                upgradeInfo.FirmwareVersion = firewareCdStr;
                                                upgradeInfo.Result = "---";
                                            }
                                            else
                                            {
                                                if (v11 >=10)
                                                {
                                                    upgradeInfo.FirmwareVersion = "P0" + v1 + "" + "_V" + v11 + "_V" + v12;
                                                }
                                                else
                                                {
                                                    upgradeInfo.FirmwareVersion = "P0" + v1 + "" + "_V0" + v11 + "_V" + v12;
                                                }
                                            }
                                            upgradeInfo.ProgressValue = (short)((bytDateDSP2[2 * 2 + 1])) + "%";
                                            if ((bytDateDSP2[2 * 2 + 1]) == 100)
                                            {
                                                sucess = true;
                                            }
                                        }
                                        System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                                        {
                                            upgradeInfos.Add(upgradeInfo);
                                        }));
                                    }
                                }



                            }


                            foreach (KeyValuePair<string, string> d in CAN3Sort)
                            {
                                Console.WriteLine(d.Key + " " + d.Value);
                                string keyset = d.Key;
                                if (keyset.IndexOf("升级编码") >= 0)
                                {
                                    if (ModbusTcp.Instance.ReadInputRegisters4(slaveIdText, Convert.ToInt32(CAN3Sort[keyset]) - 1, 3, bytDateDSP2) != 0)
                                    {
                                        // MessageBox.Show("通讯异常");
                                        //flag = 0;
                                        //break;
                                    }
                                    else
                                    {
                                        upgradeInfo upgradeInfo = new upgradeInfo();
                                        upgradeInfo.Name = keyset.Substring(0, keyset.IndexOf("升"));
                                        upgradeInfo.Result = resultOK(vs3);
                                        for (int i = 0; i < bytDateDSP2.Length / 2; i++)
                                        {
                                            string firewareCdStr = codeOk((short)(bytDateDSP2[0 * 2 + 1] | (bytDateDSP2[0 * 2] << 8)));
                                            upgradeInfo.FirmwareCode = codeOk((short)(bytDateDSP2[0 * 2 + 1] | (bytDateDSP2[0 * 2] << 8)));
                                            ushort vcan = (ushort)(bytDateDSP2[1 * 2 + 1] | (bytDateDSP2[1 * 2] << 8));
                                            int v1 = (bytDateDSP2[1 * 2] & 0xf0) >> 4;
                                            int v11 = bytDateDSP2[1 * 2] & 0x0f;
                                            int v12 = bytDateDSP2[1 * 2 + 1];
                                            if (firewareCdStr.Equals("---"))
                                            {
                                                upgradeInfo.FirmwareVersion = firewareCdStr;
                                            }
                                            else
                                            {
                                                if (v11>=10)
                                                {
                                                    upgradeInfo.FirmwareVersion = "P0" + v1 + "" + "_V" + v11 + "_V" + v12;
                                                }
                                                else
                                                {
                                                    upgradeInfo.FirmwareVersion = "P0" + v1 + "" + "_V0" + v11 + "_V" + v12;
                                                }
                                                
                                            }
                                            upgradeInfo.ProgressValue = (short)((bytDateDSP2[2 * 2+1])) + "%";
                                            if ((bytDateDSP2[2 * 2 + 1]) == 100)
                                            {
                                                sucess = true;
                                            }
                                        }
                                        System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                                        {
                                            upgradeInfos.Add(upgradeInfo);
                                        }));
                                    }
                                    break;
                                }
                                
                            }

                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                //刷新数据
                                upgradeGrid.ItemsSource = null;
                                upgradeGrid.ItemsSource = upgradeInfos;
                            }));
                            if (sucess== true)
                            {
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    this._loading.Visibility = Visibility.Collapsed;
                                    this.upgrade.IsEnabled = true;
                                    //upgradeGrid.ItemsSource = null;
                                }));
                                break;
                            }
                        }
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            this._loading.Visibility = Visibility.Collapsed;
                            this.upgrade.IsEnabled = true;
                            //upgradeGrid.ItemsSource = null;
                        }));
                    });
                    realThread2.Start();
                }
                else
                {
                    MessageBox.Show("升级失败");
                }
            }
        }
        #endregion

        /*
        #########################################################################################
        ##############################################历史记录模块###############################
        #########################################################################################
        */
        #region 历史记录
        private void historyRecord_Click(object sender, RoutedEventArgs e)
        {
            upgrade1 = 3;
            monitor.Visibility = Visibility.Collapsed;
            wave.Visibility = Visibility.Collapsed;
            upgrade.Visibility = Visibility.Collapsed;
            history.Visibility = Visibility.Visible;
        }
        SQLiteHelper SQLite = null;
        //传入数据库文件，实例化数据库连接
        private void LoadSQL(string sqlFile)
        {
            historyRecord.ItemsSource = null;
            //实例化对象
            SQLite = new SQLiteHelper(sqlFile);
            try
            {
                //
                string sqlName = sqlFile.Substring(sqlFile.LastIndexOf('\\') + 1);
                string dataName;
                sqlList.TryGetValue(sqlName, out dataName);
                //SQL查询语句
                string sql = "select * from " + dataName;
                //执行查询，并加载到sql_comboBox列表
                SQLiteDataReader myReader = SQLite.ExecuteReader(sql);
                showDataGrid(myReader);
            }
            catch(Exception ex)
            {
                MessageBox.Show("数据库文件错误！");
            }
            finally
            {
                SQLite.closeConn();
            }
        }
        /// <summary>
        /// 更改下拉框事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sql_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sql_comboBox.SelectedItem != null)
            {
                try
                {
                    string dataName = this.sql_comboBox.SelectedItem as string;
                    //SQL查询语句
                    string sql = "select * from " + dataName;
                    //执行查询，并加载到sql_comboBox列表
                    SQLiteDataReader myReader = SQLite.ExecuteReader(sql);
                    showDataGrid(myReader);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("数据库文件错误！");
                }
            }
        }

        /// <summary>
        /// 展示数据
        /// </summary>
        /// <param name="myReader"></param>
        public void showDataGrid(SQLiteDataReader myReader)
        {
            try
            {
                ArrayList alist = new ArrayList();
                if (myReader.HasRows == false)
                {
                    MessageBox.Show("数据库文件中没有数据");
                }
                if (myReader.FieldCount <= 2)
                {
                    while (myReader.Read())
                    {
                        alist.Add(myReader[1]);
                    }
                    sql_comboBox.Visibility = Visibility.Visible;
                    sql_comboBox.ItemsSource = alist;
                    sql_comboBox.SelectedIndex = 0;
                }
                else
                {
                    if (myReader != null)
                    {
                        System.Data.DataTable Dt = new System.Data.DataTable();
                        Dt.Load(myReader);
                        historyRecord.ItemsSource = Dt.DefaultView;
                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show("数据库文件为空");
            }            
        }

        string hisPath;
        private void sqlGet_Click(object sender, RoutedEventArgs e)
        {
            if (isConnection)
            {
                pathTable.TryGetValue("历史记录", out hisPath);
                SFTPHelper sftp = new SFTPHelper(this.ipAddress.Text, "22", "root", "sungrow2016");
                //SFTPHelper sftp = new SFTPHelper("192.168.28.112", "22", "admin", "111111");
                try
                {
                    if (sftp.Connect())
                    {
                        ArrayList list = sftp.GetFileList1(hisPath);
                        //ArrayList list = sftp.GetFileList1(@"/test/");
                        this.getList.ItemsSource = list;
                        sqlDown.IsEnabled = true;
                        //sqlDele.IsEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("连接SFTP服务器出现故障：" + ex.Message);
                }
                finally
                {
                    sftp.Disconnect();
                }
            }
            else
            {
                MessageBox.Show("通信未连接");
            }
        }

        private void sqlDown_Click(object sender, RoutedEventArgs e)
        {
            List<string> fileList = new List<string>();
            if (this.getList.SelectedItem != null && getList.SelectedItems.Count >= 1)
            {
                foreach (string str in getList.SelectedItems)
                {
                    fileList.Add(str);
                }
            }
            else
            {
                MessageBox.Show("请选择文件！");
                return;
            }
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                readPath = dialog.SelectedPath;
                SFTPHelper sftp = new SFTPHelper(this.ipAddress.Text, "22", "root", "sungrow2016");
                //SFTPHelper sftp = new SFTPHelper("192.168.28.112", "22", "admin", "111111");
                try
                {
                    if (sftp.Connect())
                    {
                        foreach (string filePath in fileList)
                        {
                            sftp.Get(hisPath + "/" + filePath, dialog.SelectedPath + "\\" + filePath);
                            //sftp.Get(@"/test/" + filePath, dialog.SelectedPath + "\\" + filePath);
                        }
                    }
                    MessageBox.Show("下载文件成功！");
                    this.showList.ItemsSource = fileList;
                    this.getList.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("下载文件出错：" + ex.Message);
                }
                finally
                {
                    sftp.Disconnect();
                }
            }
        }

        private void sqlDele_Click(object sender, RoutedEventArgs e)
        {
            string filepath = null;
            if (this.getList.SelectedItem != null)
            {
                filepath = this.getList.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("请选择文件！");
                return;
            }
            SFTPHelper sftp = new SFTPHelper(this.ipAddress.Text, "22", "root", "sungrow2016");
            //SFTPHelper sftp = new SFTPHelper("192.168.193.1", "22", "admin", "111111");
            try
            {
                if (sftp.Connect())
                {
                    sftp.Delete(hisPath + filepath);
                    ArrayList list = sftp.GetFileList1(hisPath);
                    this.getList.ItemsSource = list;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接SFTP服务器出现故障：" + ex.Message);
            }
            finally
            {
                sftp.Disconnect();
            }
        }

        string readPath = string.Empty;
        private void showList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.sql_comboBox.Visibility = Visibility.Collapsed;
            this.sql_comboBox.ItemsSource = null;
            if(this.showList.SelectedItem != null)
                LoadSQL(readPath + "\\" + this.showList.SelectedItem.ToString());
        }


        private void browseFile_Click(object sender, RoutedEventArgs e)
        {
            sql_comboBox.ItemsSource = null;
            //打开文件夹
            System.Windows.Forms.FolderBrowserDialog m_Dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = m_Dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            readPath = m_Dialog.SelectedPath.Trim();
            List<string> fileList = new List<string>();
            string[] files = System.IO.Directory.GetFiles(readPath, "*.*");
            foreach (string s in files)
            {
                System.IO.FileInfo fi = null;
                try
                {
                    fi = new System.IO.FileInfo(s);
                    fileList.Add(fi.Name);
                }
                catch (System.IO.FileNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
                this.showList.ItemsSource = null;
                this.showList.ItemsSource = fileList;
            }
        }

        #endregion

    }
}
