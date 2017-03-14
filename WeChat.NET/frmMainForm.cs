using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using WeChat.NET.Controls;
using WeChat.NET.Objects;
using WeChat.NET.HTTP;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace WeChat.NET
{
    /// <summary>
    /// 主界面
    /// </summary>
    public partial class frmMainForm : Form
    {
        /// <summary>
        /// 主界面等待提示
        /// </summary>
        private Label _lblWait;
        /// <summary>
        /// 聊天对话框
        /// </summary>
        private WChatBox _chat2friend;
        /// <summary>
        /// 好友信息框
        /// </summary>
        private WPersonalInfo _friendInfo;
        /// <summary>
        /// 当前登录微信用户
        /// </summary>
        private WXUser _me;

        private List<Object> _contact_all = new List<object>();
        private List<object> _contact_latest = new List<object>();

        private WXService wxs = new WXService();
        private int groupwaittime = 5000;

        /// <summary>
        /// 构造方法
        /// </summary>
        public frmMainForm()
        {
            InitializeComponent();

            _chat2friend = new WChatBox();
            _chat2friend.Dock = DockStyle.Fill;
            _chat2friend.Visible = false;
            _chat2friend.FriendInfoView += new FriendInfoViewEventHandler(_chat2friend_FriendInfoView);
            Controls.Add(_chat2friend);

            _friendInfo = new WPersonalInfo();
            _friendInfo.Dock = DockStyle.Fill;
            _friendInfo.Visible = false;
            _friendInfo.StartChat += new StartChatEventHandler(_friendInfo_StartChat);
            Controls.Add(_friendInfo);

            _lblWait = new Label();
            _lblWait.Text = "数据加载...";
            _lblWait.AutoSize = false;
            _lblWait.Size = this.ClientSize;
            _lblWait.TextAlign = ContentAlignment.MiddleCenter;
            _lblWait.Location = new Point(0, 0);
            Controls.Add(_lblWait);
        }


        #region  事件处理程序
        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMainForm_Load(object sender, EventArgs e)
        {
            DoMainLogic();
        }
        /// <summary>
        /// 好友信息框中点击 聊天
        /// </summary>
        /// <param name="user"></param>
        void _friendInfo_StartChat(WXUser user)
        {
            _chat2friend.Visible = true;
            _chat2friend.BringToFront();
            _chat2friend.MeUser = _me;
            _chat2friend.FriendUser = user;
        }
        /// <summary>
        /// 聊天对话框中点击 好友信息
        /// </summary>
        /// <param name="user"></param>
        void _chat2friend_FriendInfoView(WXUser user)
        {
            _friendInfo.FriendUser = user;
            _friendInfo.Visible = true;
            _friendInfo.BringToFront();
        }
        /// <summary>
        /// 聊天列表点击好友   开始聊天
        /// </summary>
        /// <param name="user"></param>
        private void wchatlist_StartChat(WXUser user)
        {
            _chat2friend.Visible = true;
            _chat2friend.BringToFront();
            _chat2friend.MeUser = _me;
            _chat2friend.FriendUser = user;
        }
        /// <summary>
        /// 通讯录中点击好友 查看好友信息
        /// </summary>
        /// <param name="user"></param>
        private void wfriendlist_FriendInfoView(WXUser user)
        {
            _friendInfo.FriendUser = user;
            _friendInfo.Visible = true;
            _friendInfo.BringToFront();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
        #endregion

        #region 主逻辑
        /// <summary>
        /// 
        /// </summary>
        private void DoMainLogic()
        {
            _lblWait.BringToFront();
            ((Action)(delegate()
            {
                JObject init_result = wxs.WxInit();  //初始化
                #region 初始化个人信息和最近聊天列表
                List<object> contact_all = new List<object>();
                if (init_result != null)
                {
                    _me = new WXUser();
                    _me.UserName = init_result["User"]["UserName"].ToString();
                    _me.City = "";
                    _me.HeadImgUrl = init_result["User"]["HeadImgUrl"].ToString();
                    _me.NickName = init_result["User"]["NickName"].ToString();
                    _me.Province = "";
                    _me.PYQuanPin = init_result["User"]["PYQuanPin"].ToString();
                    _me.RemarkName = init_result["User"]["RemarkName"].ToString();
                    _me.RemarkPYQuanPin = init_result["User"]["RemarkPYQuanPin"].ToString();
                    _me.Sex = init_result["User"]["Sex"].ToString();
                    _me.Signature = init_result["User"]["Signature"].ToString();
                    _me.Alias = init_result["Alias"]["Alias"].ToString();

                    foreach (JObject contact in init_result["ContactList"])  //部分好友名单
                    {
                        WXUser user = new WXUser();
                        user.UserName = contact["UserName"].ToString();
                        user.City = contact["City"].ToString();
                        user.HeadImgUrl = contact["HeadImgUrl"].ToString();
                        user.NickName = contact["NickName"].ToString();
                        user.Province = contact["Province"].ToString();
                        user.PYQuanPin = contact["PYQuanPin"].ToString();
                        user.RemarkName = contact["RemarkName"].ToString();
                        user.RemarkPYQuanPin = contact["RemarkPYQuanPin"].ToString();
                        user.Sex = contact["Sex"].ToString();
                        user.Signature = contact["Signature"].ToString();

                        _contact_latest.Add(user);
                    }
                }
                #endregion
                #region 初始化通讯录
                JObject contact_result = wxs.GetContact(); //通讯录
                if (contact_result != null)
                {
                    foreach (JObject contact in contact_result["MemberList"])  //完整好友名单
                    {
                        WXUser user = new WXUser();
                        user.UserName = contact["UserName"].ToString();
                        user.City = contact["City"].ToString();
                        user.HeadImgUrl = contact["HeadImgUrl"].ToString();
                        user.NickName = contact["NickName"].ToString();
                        user.Province = contact["Province"].ToString();
                        user.PYQuanPin = contact["PYQuanPin"].ToString();
                        user.RemarkName = contact["RemarkName"].ToString();
                        user.RemarkPYQuanPin = contact["RemarkPYQuanPin"].ToString();
                        user.Sex = contact["Sex"].ToString();
                        user.Signature = contact["Signature"].ToString();
                        user.VerifyFlag = contact["VerifyFlag"].ToString();
                        user.Alias = contact["Alias"].ToString();

                        contact_all.Add(user);
                    }
                }
                #endregion
                #region 将个人信息、聊天信息、通讯录信息填充到控件中
                IOrderedEnumerable<object> list_all = contact_all.OrderBy(e => (e as WXUser).ShowPinYin);

                WXUser wx; string start_char;
                foreach (object o in list_all)
                {
                    wx = o as WXUser;
                    start_char = wx.ShowPinYin == "" ? "" : wx.ShowPinYin.Substring(0, 1);
                    if (!_contact_all.Contains(start_char.ToUpper()))
                    {
                        _contact_all.Add(start_char.ToUpper());
                    }
                    _contact_all.Add(o);
                }

                this.BeginInvoke((Action)(delegate()  //等待结束
                {
                    _lblWait.Visible = false;

                    wChatList1.Items.AddRange(_contact_latest.ToArray());  //近期联系人
                    wFriendsList1.Items.AddRange(_contact_all.ToArray());  //通讯录
                    wpersonalinfo.FriendUser = _me;
                }));
                #endregion
                #region 检查通讯录好友
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["GroupWaitTime"], out groupwaittime);
                var SpecialUsers = new string[]{"newsapp", "fmessage", "filehelper", "weibo", "qqmail", "tmessage", "qmessage", "qqsync", "floatbottle", "lbsapp", "shakeapp", "medianote", "qqfriend", "readerapp", "blogapp", "facebookapp", "masssendapp",
                    "meishiapp", "feedsapp", "voip", "blogappweixin", "weixin", "brandsessionholder", "weixinreminder", "wxid_novlwrv3lqwv11", "gh_22b87fa7cb3c", "officialaccounts", "notification_messages", "wxitil", "userexperience_alarm"};
                var persons = contact_all.Select(c => c as WXUser).Where(c => c.VerifyFlag == "0"
                    && !SpecialUsers.Contains(c.UserName) && !c.UserName.Contains("@@") && c.UserName != _me.UserName).ToList();
                int groupcount = 2, index = 0;
                string chatroomname = "";
                List<string> deletedlist = new List<string>(), blockedlist = new List<string>();
                Dictionary<string, string> addresult = new Dictionary<string, string>(), deleteresult = new Dictionary<string, string>();
                JObject temp;
                while (index < persons.Count)
                {
                    var groupperson = persons.Where((c, i) => i >= index && i < (index + groupcount)).Select(c => c.UserName);

                    if (chatroomname == "")
                    {
                        temp = wxs.CreateRoom(groupperson.ToList());
                        chatroomname = temp["ChatRoomName"].ToString();
                    }
                    else
                    {
                        temp = wxs.AddMember(chatroomname, groupperson.ToList());
                    }
                    foreach (JObject obj in temp["MemberList"])
                    {
                        if (obj["MemberStatus"].ToString() == "4")
                            deletedlist.Add(obj["UserName"].ToString());
                        else if (obj["MemberStatus"].ToString() == "3")
                            blockedlist.Add(obj["UserName"].ToString());
                    }
                    System.Threading.Thread.Sleep(groupwaittime);
                    var delres = wxs.DeleteMember(chatroomname, groupperson.ToList());
                    foreach (var p in groupperson)
                    {
                        addresult[p] = temp["BaseResponse"]["Ret"].ToString();
                        deleteresult[p] = delres["BaseResponse"]["Ret"].ToString();
                    }
                    index += groupcount;
                    System.Threading.Thread.Sleep(groupwaittime);
                }
                var deletednames = persons.Where(c => deletedlist.Contains(c.UserName))
                    .Select(c => string.IsNullOrEmpty(c.RemarkName) ? c.NickName : c.RemarkName);
                var blockednames = persons.Where(c => blockedlist.Contains(c.UserName))
                    .Select(c => string.IsNullOrEmpty(c.RemarkName) ? c.NickName : c.RemarkName);
                Helper.XmlHelper xh = new Helper.XmlHelper();
                xh.Root.SetAttribute("UserName", _me.UserName);
                xh.Root.SetAttribute("NickName", _me.NickName);
                xh.Root.SetAttribute("Alias", _me.Alias);
                xh.Root.SetAttribute("friendcount", persons.Count.ToString());
                xh.Root.SetAttribute("deletedcount", deletednames.Count().ToString());
                xh.Root.SetAttribute("blockedcount", blockednames.Count().ToString());
                xh.Root.SetAttribute("deletednames", string.Join(",", deletednames));
                xh.Root.SetAttribute("blockednames", string.Join(",", blockednames));
                xh.Root.SetAttribute("checktimesplit", groupwaittime.ToString());
                foreach (var p in persons)
                {
                    XmlElement ele = xh.Doc.CreateElement("Friend");
                    ele.SetAttribute("UserName", p.UserName);
                    ele.SetAttribute("NickName", p.NickName);
                    ele.SetAttribute("Alias", p.Alias);
                    ele.SetAttribute("Province", p.Province);
                    ele.SetAttribute("Sex", p.Sex);
                    ele.SetAttribute("HeadImgUrl", p.HeadImgUrl);
                    ele.SetAttribute("Province", p.Province);
                    ele.SetAttribute("City", p.City);
                    ele.SetAttribute("AddStatus", addresult[p.UserName]);
                    ele.SetAttribute("DeleteStatus", deleteresult[p.UserName]);
                    ele.SetAttribute("FriendStatus", deletedlist.Contains(p.UserName) ? "4" : blockednames.Contains(p.UserName) ? "3" : "0");
                    xh.AddEle(ele);
                }
                xh.Save(string.Format("{0}\\{1}{2}.xml", Environment.GetFolderPath(Environment.SpecialFolder.Desktop).TrimEnd('\\'), _me.UserName, DateTime.Now.ToString("yyyyMMddhhmmss")));
                //wxs.SendMsg(string.Format("删除名单有:{0} 拉黑名单有:{1}", string.Join(",", deletednames), string.Join(",", blockednames)), _me.UserName, chatroomname, 1);
                //MessageBox.Show(string.Format("删除名单有:{0} 拉黑名单有:{1}", string.Join(",", deletednames), string.Join(",", blockednames)));
                #endregion
                #region 保持和微信持续通讯
                System.Threading.Thread t1 = new System.Threading.Thread(new System.Threading.ThreadStart(cycletalk));
                t1.Start();
                #endregion
                #region 循环拉取微信消息
                //string sync_flag = "";
                //JObject sync_result;
                //while (true)
                //{
                //    sync_flag = wxs.WxSyncCheck();  //同步检查
                //    if (sync_flag == null)
                //    {
                //        continue;
                //    }
                //    //这里应该判断 sync_flag中selector的值
                //    else //有消息
                //    {
                //        sync_result = wxs.WxSync();  //进行同步
                //        if (sync_result != null)
                //        {
                //            if (sync_result["AddMsgCount"] != null && sync_result["AddMsgCount"].ToString() != "0")
                //            {
                //                foreach (JObject m in sync_result["AddMsgList"])
                //                {
                //                    string from = m["FromUserName"].ToString();
                //                    string to = m["ToUserName"].ToString();
                //                    string content = m["Content"].ToString();
                //                    string type = m["MsgType"].ToString();

                //                    WXMsg msg = new WXMsg();
                //                    msg.From = from;
                //                    msg.Msg = type == "1" ? content : "请在其他设备上查看消息";  //只接受文本消息
                //                    msg.Readed = false;
                //                    msg.Time = DateTime.Now;
                //                    msg.To = to;
                //                    msg.Type = int.Parse(type);

                //                    if (msg.Type == 51)  //屏蔽一些系统数据
                //                    {
                //                        continue;
                //                    }
                //                    this.BeginInvoke((Action)delegate()
                //                    {
                //                        WXUser user; bool exist_latest_contact = false;
                //                        foreach (Object u in wChatList1.Items)
                //                        {
                //                            user = u as WXUser;
                //                            if (user != null)
                //                            {
                //                                if (user.UserName == msg.From && msg.To == _me.UserName)  //接收别人消息
                //                                {
                //                                    wChatList1.Items.Remove(user);
                //                                    wChatList1.Items.Insert(0, user);
                //                                    exist_latest_contact = true;
                //                                    user.ReceiveMsg(msg);
                //                                    break;
                //                                }
                //                                else if (user.UserName == msg.To && msg.From == _me.UserName)  //同步自己在其他设备上发送的消息
                //                                {
                //                                    wChatList1.Items.Remove(user);
                //                                    wChatList1.Items.Insert(0, user);
                //                                    exist_latest_contact = true;
                //                                    user.SendMsg(msg,true);
                //                                    break;
                //                                }
                //                            }
                //                        }
                //                        if (!exist_latest_contact)
                //                        {
                //                            foreach (object o in wFriendsList1.Items)
                //                            {
                //                                WXUser friend = o as WXUser;
                //                                if (friend != null && friend.UserName == msg.From && msg.To == _me.UserName)
                //                                {
                //                                    wChatList1.Items.Insert(0, friend);
                //                                    friend.ReceiveMsg(msg);
                //                                    break;
                //                                }
                //                                if (friend != null && friend.UserName == msg.To && msg.From == _me.UserName)
                //                                {
                //                                    wChatList1.Items.Insert(0, friend);
                //                                    friend.SendMsg(msg,true);
                //                                    break;
                //                                }
                //                            }
                //                        }
                //                        wChatList1.Invalidate();
                //                    });
                //                }
                //            }
                //        }
                //    }
                //    System.Threading.Thread.Sleep(10);
                //}
                #endregion
            })).BeginInvoke(null, null);
        }

        private void cycletalk()
        {
            string sync_flag = "";
            while (true)
            {
                sync_flag = wxs.WxSyncCheck();
                System.Threading.Thread.Sleep(2000);
            }
        }
        #endregion

    }
}
