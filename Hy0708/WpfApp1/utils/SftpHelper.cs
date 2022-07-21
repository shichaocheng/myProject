using Renci.SshNet;
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace WpfApp1.utils
{
    /// <summary>
    /// SFTP操作类
    /// </summary>
    public class SFTPHelper
    {
        #region 字段或属性
        private SftpClient sftp;
        /// <summary>
        /// SFTP连接状态
        /// </summary>
        public bool Connected { get { return sftp.IsConnected; } }
        #endregion

        #region 构造
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="ip">IP</param>
        /// <param name="port">端口</param>
        /// <param name="user">用户名</param>
        /// <param name="pwd">密码</param>
        public SFTPHelper(string ip, string port, string user, string pwd)
        {
            sftp = new SftpClient(ip, Int32.Parse(port), user, pwd);
        }
        #endregion

        #region 连接SFTP
        /// <summary>
        /// 连接SFTP
        /// </summary>
        /// <returns>true成功</returns>
        public bool Connect()
        {
            try
            {
                if (!Connected)
                {
                    sftp.Connect();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                // TxtLog.WriteTxt(CommonMethod.GetProgramName(), string.Format("连接SFTP失败，原因：{0}", ex.Message));
                //throw new Exception(string.Format("连接SFTP失败，原因：{0}", ex.Message));
            }
        }
        #endregion

        #region 断开SFTP
        /// <summary>
        /// 断开SFTP
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (sftp != null && Connected)
                {
                    sftp.Disconnect();
                }
            }
            catch (Exception ex)
            {
                // TxtLog.WriteTxt(CommonMethod.GetProgramName(), string.Format("断开SFTP失败，原因：{0}", ex.Message));
                throw new Exception(string.Format("断开SFTP失败，原因：{0}", ex.Message));
            }
        }
        #endregion

        #region SFTP上传文件
        /// <summary>
        /// SFTP上传文件
        /// </summary>
        /// <param name="localPath">本地路径</param>
        /// <param name="remotePath">远程路径</param>
        public void Put(string localPath, string remotePath)
        {
            try
            {
                using (var file = File.OpenRead(localPath))
                {
                    Connect();
                    sftp.UploadFile(file, remotePath, true);
                    Disconnect();
                }
            }
            catch (Exception ex)
            {
                // TxtLog.WriteTxt(CommonMethod.GetProgramName(), string.Format("SFTP文件上传失败，原因：{0}", ex.Message));
                throw new Exception(string.Format("SFTP文件上传失败，原因：{0}", ex.Message));
            }
        }
        #endregion

        #region SFTP获取文件
        /// <summary>
        /// SFTP获取文件
        /// </summary>
        /// <param name="remotePath">远程路径</param>
        /// <param name="localPath">本地路径</param>
        public void Get(string remotePath, string localPath)
        {
            try
            {
                 var byt = sftp.ReadAllBytes(remotePath);
                 File.WriteAllBytes(localPath, byt);   
            }
            catch (Exception ex)
            {
                // TxtLog.WriteTxt(CommonMethod.GetProgramName(), string.Format("SFTP文件获取失败，原因：{0}", ex.Message));
                throw new Exception(string.Format("SFTP文件获取失败，原因：{0}", ex.Message));
            }

        }
        #endregion

        #region 删除SFTP文件
        /// <summary>
        /// 删除SFTP文件
        /// </summary>
        /// <param name="remoteFile">远程路径</param>
        public void Delete(string remoteFile)
        {
            try
            {
                sftp.Delete(remoteFile);
            }
            catch (Exception ex)
            {
                // TxtLog.WriteTxt(CommonMethod.GetProgramName(), string.Format("SFTP文件删除失败，原因：{0}", ex.Message));
                throw new Exception(string.Format("SFTP文件删除失败，原因：{0}", ex.Message));
            }
        }
        #endregion

        #region 获取SFTP文件列表
        /// <summary>
        /// 获取SFTP文件列表
        /// </summary>
        /// <param name="remotePath">远程目录</param>
        /// <param name="fileSuffix">文件后缀</param>
        /// <returns></returns>
        public ArrayList GetFileList(string remotePath, string fileSuffix)
        {
            try
            {
                Connect();
                var files = sftp.ListDirectory(remotePath);
                Disconnect();
                var objList = new ArrayList();
                foreach (var file in files)
                {
                    string name = file.Name;
                    if (name.Length > (fileSuffix.Length + 1) && fileSuffix == name.Substring(name.Length - fileSuffix.Length))
                    {
                        objList.Add(name);
                    }
                }
                return objList;
            }
            catch (Exception ex)
            {
                // TxtLog.WriteTxt(CommonMethod.GetProgramName(), string.Format("SFTP文件列表获取失败，原因：{0}", ex.Message));
                throw new Exception(string.Format("SFTP文件列表获取失败，原因：{0}", ex.Message));
            }
        }
        #endregion
        //获取SFTP文件列表        
        public ArrayList GetFileList1(string remotePath)
        {
            string keyword;
            try
            {
                //Tamir.SharpSsh.java.util.Vector vvv =  sftp.ListDirectory(remotePath);
                var files = sftp.ListDirectory(remotePath);

                ArrayList objList = new ArrayList();
                foreach (var file in files)
                {
                    string sss = file.Name.ToString();
                    byte[] buffer = Encoding.GetEncoding("GB2312").GetBytes(sss);
                    keyword = Encoding.UTF8.GetString(buffer);
                    byte[] buffer1 = Encoding.UTF8.GetBytes(sss);
                    string keyword1 = Encoding.GetEncoding("GB2312").GetString(buffer);
                    Encoding d = Encoding.GetEncoding("GBK");
                    Encoding.UTF8.GetString(d.GetBytes(sss));
                    ////声明字符集   
                    //System.Text.Encoding utf8, gb2312;
                    ////gb2312   
                    //gb2312 = System.Text.Encoding.GetEncoding("gb2312");
                    ////utf8   
                    //utf8 = System.Text.Encoding.GetEncoding("utf-8");
                    //byte[] gb;
                    //gb = gb2312.GetBytes(sss);
                    //gb = System.Text.Encoding.Convert(gb2312, utf8, gb);
                    //得到4种编码的string 
                    //sss.IndexOf('.') != -1 &&
                    if (!sss.StartsWith("."))
                    {
                        objList.Add(keyword);
                    }
                    else
                    {
                        continue;
                    }
                }
                return objList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #region 移动SFTP文件
        /// <summary>
        /// 移动SFTP文件
        /// </summary>
        /// <param name="oldRemotePath">旧远程路径</param>
        /// <param name="newRemotePath">新远程路径</param>
        public void Move(string oldRemotePath, string newRemotePath)
        {
            try
            {
                Connect();
                sftp.RenameFile(oldRemotePath, newRemotePath);
                Disconnect();
            }
            catch (Exception ex)
            {
                // TxtLog.WriteTxt(CommonMethod.GetProgramName(), string.Format("SFTP文件移动失败，原因：{0}", ex.Message));
                throw new Exception(string.Format("SFTP文件移动失败，原因：{0}", ex.Message));
            }
        }
        #endregion

    }
}
