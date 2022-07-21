using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WpfApp1
{
    /// <summary>
    /// 文件读取类
    /// </summary>
    class XmlReading
    {
        //定义读取XML文件类
        private static XmlDocument doc = new XmlDocument();
        //定义文件路径
        private static string fileUrl;
        //文件路径
        public static String FileUrl
        {
            set { fileUrl = value; }
            get { return fileUrl; }
        }

        /// <summary>
        /// 载入Xml文件
        /// </summary>
        /// <param name="url">文件路径</param>
        public static void LoadXml(String url)
        {
            doc.LoadXml(url);
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void Save(string filePath)
        {
            doc.Save(filePath);
        }
        /// <summary>
        /// 加载文件
        /// </summary>
        /// <param name="url"></param>
        public static void Load(String url)
        {
            doc.Load(url);
        }
        /// <summary>
        /// 获取XmlDocument的根节点
        /// </summary>
        /// <returns>返回的XmlElement元素根节点</returns>
        public static XmlElement GetXmlDocumentRoot()
        {
            return doc.DocumentElement;
        }
        /// <summary>
        /// 获取指定元素的指定Attribute值
        /// </summary>
        /// <param name="xe">表示一个XmlElement</param>
        /// <param name="attr">表示Attribute的名字</param>
        /// <returns>返回获取的Attribute的值</returns>
        public static String GetAttribute(XmlElement xe, String attr)
        {
            return xe.GetAttribute(attr);
        }
        /// <summary>
        /// 获取指定节点的指定Attribute值
        /// </summary>
        /// <param name="xn">表示一个XmlNode</param>
        /// <param name="attr"></param>
        /// <returns>返回获取的Attribute的值</returns>
        public static String GetNodeAttribute(XmlNode xn, String attr)
        {
            XmlElement xe = ExchangeNodeElement(xn);
            return xe.GetAttribute(attr);
        }
        /// <summary>
        /// XmlElement对象转换成XmlNode对象
        /// </summary>
        /// <param name="xe">XmlElement对象</param>
        /// <returns>返回XmlNode对象</returns>
        public static XmlNode ExchangeNodeElement(XmlElement xe)
        {
            return (XmlNode)xe; ;
        }
        /// <summary>
        /// XmlNode对象转换成XmlElement对象
        /// </summary>
        /// <param name="xe">XmlNode对象</param>
        /// <returns>返回XmlElement对象</returns>
        public static XmlElement ExchangeNodeElement(XmlNode xn)
        {
            return (XmlElement)xn; ;
        }
        /// <summary>
        /// 获取节点的文本
        /// </summary>
        /// <param name="xn"></param>
        /// <param name="nodename">节点的名称</param>
        /// <returns></returns>
        public static String GetXmlNodeInnerText(XmlNode xn, String nodeName)
        {
            XmlNode childxn = xn.SelectSingleNode(nodeName);
            return childxn.InnerText;
        }
        /// <summary>
        /// 获取指定节点的子节点
        /// </summary>
        /// <param name="xn">节点对象</param>
        /// <returns>返回子节点数</returns>
        public static int GetXmlNodeCount(XmlNode xn)
        {
            return xn.ChildNodes.Count;
        }
        /// <summary>
        /// 获取元素的文本
        /// </summary>
        /// <param name="xn">XmlElement元素</param>
        /// <param name="nodename">元素的名称</param>
        /// <returns></returns>
        public static String GetXmlElementInnerText(XmlElement xe, String nodeName)
        {
            XmlNode childxn = xe.SelectSingleNode(nodeName);
            return childxn.InnerText;
        }
        /// <summary>
        /// 获取XmlNode是否具有指定Attribute值
        /// </summary>
        /// <param name="xn">XmlNode对象</param>
        /// <param name="attr">Attribute的名称</param>
        /// <param name="compare">Attribute的值</param>
        /// <returns>返回bool值</returns>
        public static bool GetXmlNodeByAttribute(XmlNode xn, String attr, String compare)
        {
            if(GetNodeAttribute(xn, attr) == compare)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取XmlElement是否具有指定Attribute值
        /// </summary>
        /// <param name="xn">XmlElement对象</param>
        /// <param name="attr">Attribute的名称</param>
        /// <param name="compare">Attribute的值</param>
        /// <returns>返回bool值</returns>
        public static bool GetXmlNodeByAttribute(XmlElement xe, String attr, String compare)
        {
            if(GetAttribute(xe, compare) == attr)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取一个具有指定Attrtibute的XmlNode子节点
        /// </summary>
        /// <param name="xn">XmlNode对象</param>
        /// <param name="attr">Attrtibute的名称</param>
        /// <param name="compare">Attrtibute的值</param>
        /// <returns>返回相应的子节点</returns>
        public static XmlNode GetXmlChildNodeByAttribute(XmlNode xn, String attr, String compare)
        {
            foreach(XmlNode cxn in xn.ChildNodes)
            {
                if(GetXmlNodeByAttribute(cxn, attr, compare))
                {
                    return cxn;
                }
            }
            return null;
        }

        public static XmlNodeList getList(string name)
        {
            return doc.GetElementsByTagName(name);
        }

    }
}
