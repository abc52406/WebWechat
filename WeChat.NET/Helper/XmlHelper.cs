using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace WeChat.NET.Helper
{
    /// <summary>
    /// xml帮助类
    /// </summary>
    public class XmlHelper
    {
        #region 私有变量
        private const string defaultXml="<?xml version=\"1.0\" encoding=\"utf-8\" ?><Items></Items>";
        private XmlElement root = null;
        private XmlDocument xmldoc = new XmlDocument();
        private string path = string.Empty;
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化一个空的实例
        /// </summary>
        public XmlHelper()
            : this(defaultXml, XmlType.String)
        { }

        /// <summary>
        /// 根据传入的xml字符串初始化一个实例
        /// </summary>
        /// <param name="xml">xml字符串</param>
        public XmlHelper(string xml)
            : this(xml, XmlType.String)
        { }

        /// <summary>
        /// 根据传入的xml字符串或xml文件路径初始化一个实例
        /// </summary>
        /// <param name="xml">xml字符串或xml文件路径</param>
        /// <param name="type">初始化类型</param>
        public XmlHelper(string xml,XmlType type)
        {
            try
            {
                if (type == XmlType.Path)
                {
                    xmldoc.Load(xml);
                    path = xml;
                }
                else
                    xmldoc.LoadXml(xml);
                root = xmldoc.DocumentElement;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("初始化xml失败，原因如下：{0}", ex.Message));
            }
        }
        #endregion

        #region 公共函数
        #region 查询
        /// <summary>
        /// 查询符合条件的所有xml节点，采用XPATH语法,例如：/bookstore/book[title='新书3'] 或 "book[title='新书2']
        /// </summary>
        /// <param name="searchStr"></param>
        /// <returns></returns>
        public XmlNodeList QueryNodes(string searchStr)
        {
			return root.SelectNodes(searchStr);
        }

        /// <summary>
        /// 查询符合条件的第一个xml节点，采用XPATH语法,例如：/bookstore/book[title='新书3'] 或 "book[title='新书2']
        /// </summary>
        /// <param name="searchStr"></param>
        /// <returns></returns>
        public XmlNode QueryNode(string searchStr)
        {
            XmlNodeList list = QueryNodes(searchStr);
            return list.Count == 0 ? null : list.Item(0);
        }

        /// <summary>
        /// 查询符合条件的第一个xml节点，采用XPATH语法,例如：/bookstore/book[title='新书3'] 或 "book[title='新书2']
        /// </summary>
        /// <param name="searchStr"></param>
        /// <returns></returns>
        public XmlElement QueryEle(string searchStr)
        {
            XmlNodeList list = QueryNodes(searchStr);
            return list.Count == 0 ? null : (XmlElement)list.Item(0);
        }
        #endregion

        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            if (!string.IsNullOrEmpty(path))
                Save(path);
        }

        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="savepath">保存路径</param>
        public void Save(string savepath)
        {
            xmldoc.Save(savepath);
        }
        #endregion

        #region 添加
        /// <summary>
        /// 根节点下添加xml节点
        /// </summary>
        /// <param name="node">节点</param>
        public void AddNode(XmlNode node)
        {
            AddNode(node, root);
        }

        /// <summary>
        /// 指定路径下的第一个节点中添加xml节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="parentPath">路径</param>
        public void AddNode(XmlNode node, string parentPath)
        {
            XmlNode parentnode = QueryNode(parentPath);
            if (parentnode != null)
                AddNode(node, parentnode);
        }

        /// <summary>
        /// 指定节点下添加xml节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="parentNode">父节点</param>
        public void AddNode(XmlNode node, XmlNode parentNode)
        {
            parentNode.AppendChild(node);
            if (!string.IsNullOrEmpty(path))
                xmldoc.Save(path);
        }

        /// <summary>
        /// 根节点下添加xml节点
        /// </summary>
        /// <param name="nodes">节点</param>
        public void AddNodes(XmlNodeList nodes)
        {
            AddNodes(nodes, root);
        }

        /// <summary>
        /// 指定路径下第一个节点中添加xml节点
        /// </summary>
        /// <param name="nodes">节点</param>
        /// <param name="parentPath">路径</param>
        public void AddNodes(XmlNodeList nodes, string parentPath)
        {
            XmlNode parentnode = QueryNode(parentPath);
            if (parentnode != null)
                AddNodes(nodes, parentnode);
        }

        /// <summary>
        /// 指定节点下添加xml节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="parentNode">父节点</param>
        public void AddNodes(XmlNodeList nodes,XmlNode parentNode)
        {
            foreach (XmlNode node in nodes)
            {
                parentNode.AppendChild(node);
            }
            if (!string.IsNullOrEmpty(path))
                xmldoc.Save(path);
        }

        /// <summary>
        /// 根节点下添加xml节点
        /// </summary>
        /// <param name="ele">节点</param>
        public void AddEle(XmlElement ele)
        {
            AddNode((XmlNode)ele, root);
        }

        /// <summary>
        /// 指定路径下的第一个节点中添加xml节点
        /// </summary>
        /// <param name="ele">节点</param>
        /// <param name="parentPath">路径</param>
        public void AddEle(XmlElement ele, string parentPath)
        {
            XmlNode parentnode = QueryNode(parentPath);
            if (parentnode != null)
                AddNode((XmlNode)ele, parentnode);
        }

        /// <summary>
        /// 指定节点下添加xml节点
        /// </summary>
        /// <param name="ele">节点</param>
        /// <param name="parentEle">父节点</param>
        public void AddEle(XmlElement ele, XmlElement parentEle)
        {
            parentEle.AppendChild(ele);
            if (!string.IsNullOrEmpty(path))
                xmldoc.Save(path);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="node"></param>
        public void DeleteNode(XmlNode node)
        {
            if (node != null && node.ParentNode != null)
            {
                node.ParentNode.RemoveChild(node);
                if (!string.IsNullOrEmpty(path))
                    xmldoc.Save(path);
            }
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="path"></param>
        public void DeleteNode(string path)
        {
            XmlNode node = QueryNode(path);
            if (node != null)
            {
                DeleteNode(node);
            }
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="node"></param>
        public void DeleteNodes(XmlNodeList list)
        {
            bool change = false;
            foreach (XmlNode node in list)
            {
                if (node.ParentNode != null)
                {
                    node.ParentNode.RemoveChild(node);
                    change = true;
                }
            }
            if (change && !string.IsNullOrEmpty(path))
                xmldoc.Save(path);
        }
        #endregion

        #region 判断
        /// <summary>
        /// 是否存在满足条件的节点
        /// </summary>
        /// <param name="searchStr"></param>
        /// <returns></returns>
        public bool Exist(string searchStr)
        {
            return root.SelectNodes(searchStr).Count > 0;
        }
        #endregion
        #endregion

        #region 公共属性
        /// <summary>
        /// 根节点
        /// </summary>
        public XmlElement Root
        {
            get
            {
                return root;
            }
        }

        /// <summary>
        /// xml文件
        /// </summary>
        public XmlDocument Doc
        {
            get
            {
                return xmldoc;
            }
        }

        /// <summary>
        /// xml文件
        /// </summary>
        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
            }
        }
        #endregion
    }

    /// <summary>
    /// Xml类型
    /// </summary>
    public enum XmlType
    {
        /// <summary>
        /// 路径
        /// </summary>
        Path,
        /// <summary>
        /// xml字符串
        /// </summary>
        String
    }
}
