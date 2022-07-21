using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;


namespace Modbus
{
    public class ModbusTcp
    {
        private static ModbusTcp instance = null;
        public static ModbusTcp Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ModbusTcp();
                }
                return instance;
            }
        }
        private static ModbusTcp instance2 = null;
        public static ModbusTcp Instance2
        {
            get
            {
                if (instance2 == null)
                {
                    instance2 = new ModbusTcp();
                }
                return instance2;
            }
        }

        #region 字段、属性
        public const byte FUN_READ_HOLDING = 0x03;
        public const byte FUN_READ_INPUT = 0X04;
        public const byte FUN_WRITE_MULTI = 0X10;
        public const byte FUN_READ_INPUT2 = 0X34;
        public const int buflen = 1024;
        //录波缓冲区长度
        private const int buflen34H = 1400;
        /// <summary>
        /// 包括MBAP长度和功能码字节
        /// </summary>
        private const int headerLen = 8;
        private byte[] sendBuf = new byte[buflen];
        private volatile byte[] recvBuf = new byte[buflen];
        private byte[] recvBuf6 = new byte[buflen];
        private byte[] recvBuf5 = new byte[buflen];
        //录波缓冲区
        private byte[] sendBuf34H = new byte[buflen34H];
        private byte[] recvBuf34H = new byte[buflen34H];
        private ushort usTID = 0;
        //public string serverIp = "192.168.0.122";
        public static int port = 502;
        public static string serverIp = "192.168.11.206";
        //private int port = 8088;
        private int isPort;
        private string isIp = null;
        private TcpClient tcpclient = null;
        private NetworkStream stream = null;
        private Mutex mutex = new Mutex();

        //定义构造器初始化ip和端口号
        public ModbusTcp(string isIp, string isPort)
        {
            this.isIp = serverIp;
            this.isPort = Int32.Parse("502");
        }
        //无参构造器
        public ModbusTcp() { }
        public bool WaitMutex(int milliseconds)
        {
            return mutex.WaitOne(milliseconds);
        }
        public void ReleaseMutex()
        {
            mutex.ReleaseMutex();
        }

        public bool IsConnected
        {
            get { return (tcpclient != null) && (tcpclient.Connected); }

        }
        public bool isTcpClientState
        {
            get { return (tcpclient != null); }
        }
        /// <summary>
        /// 设备地址
        /// </summary>
        private byte deviceAddr = 0x01;
        #endregion
        #region 外部接口
        public bool MakeConnect()
        {
            try
            {
                if (IsConnected == false && tcpclient != null)
                {
                    ModbusTcp.instance.close();
                    tcpclient = null;
                }
                if (tcpclient == null)
                {
                    tcpclient = new TcpClient();
                }
                Task task = tcpclient.ConnectAsync(serverIp, port);

                task.Wait(2000);
                if (tcpclient.Connected)
                {
                    stream = tcpclient.GetStream();
                }
                return tcpclient.Connected;
            }
            catch (Exception ex)
            {
                if (ModbusTcp.instance != null)
                {
                    ModbusTcp.instance.close();
                }

                tcpclient = null;
                return false;
            }
        }
        public void close()
        {
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }
            if (tcpclient != null)
            {
                tcpclient.Close();
                tcpclient = null;
            }
        }
        public int ReadInputRegisters(ushort startAddr, ushort num, byte[] buf)
        {
            int i = headerLen;
            int j = 0;
            sendBuf[i++] = (byte)((startAddr >> 8) & 0xff);
            sendBuf[i++] = (byte)(startAddr & 0xff);
            sendBuf[i++] = (byte)((num >> 8) & 0xff);
            sendBuf[i++] = (byte)(num & 0xff);
            if (!SendData(FUN_READ_INPUT, 4))
            {
                return -1;
            }
            byte recvFunCode = 0;
            ushort recvDataLength = 0;
            int ret = RecvData(ref recvFunCode, ref recvDataLength);
            if (ret <= 0)
            {
                return -2;
            }
            if (recvFunCode != FUN_READ_INPUT)
            {
                return -3;
            }
            i = headerLen;
            byte len = recvBuf[i++];
            Array.Copy(recvBuf, i, buf, 0, len);
            usTID++;
            return 0;
        }
        public int ReadInputRegisters3(ushort startAddr, ushort num, byte[] buf)
        {
            int i = headerLen;
            int j = 0;
            sendBuf[i++] = (byte)((startAddr >> 8) & 0xff);
            sendBuf[i++] = (byte)(startAddr & 0xff);
            sendBuf[i++] = (byte)((num >> 8) & 0xff);
            sendBuf[i++] = (byte)(num & 0xff);
            if (!SendData2(FUN_READ_INPUT, 4))
            {
                return -1;
            }
            byte recvFunCode = 0;
            ushort recvDataLength = 0;
            int ret = RecvData2(ref recvFunCode, ref recvDataLength);
            if (ret <= 0)
            {
                return -2;
            }
            if (recvFunCode != FUN_READ_INPUT)
            {
                return -3;
            }
            i = headerLen;
            byte len = recvBuf[i++];
            Array.Copy(recvBuf, i, buf, 0, len);
            usTID++;
            return 0;
        }

        public int WriteMultiRegisters2(byte deviceAddr1, ushort startAddr, ushort num, byte[] buf)
        {
            int i = headerLen;
            int j = 0;
            sendBuf[i++] = (byte)((startAddr >> 8) & 0xff);
            sendBuf[i++] = (byte)(startAddr & 0xff);
            sendBuf[i++] = (byte)((num >> 8) & 0xff);
            sendBuf[i++] = (byte)(num & 0xff);
            sendBuf[i++] = (byte)(num * 2); //比特数
            Array.Copy(buf, 0, sendBuf, i, num * 2);
            if (!SendData4(deviceAddr1, FUN_WRITE_MULTI, (ushort)(5 + num * 2)))
            {
                return -1;
            }
            byte recvFunCode = 0;
            ushort recvDataLength = 0;
            int ret = RecvData2(ref recvFunCode, ref recvDataLength);
            if (ret <= 0)
            {
                return -2;
            }
            if (recvFunCode != FUN_WRITE_MULTI)
            {
                return -3;
            }
            i = headerLen;
            ushort recvStartAddr = 0;
            ushort recvNum;
            recvStartAddr = (ushort)(recvBuf[i++] << 8);
            recvStartAddr |= (ushort)(recvBuf[i++]);
            recvNum = (ushort)(recvBuf[i++] << 8);
            recvNum |= (ushort)(recvBuf[i++]);
            if (recvStartAddr != startAddr || recvNum != num)
            {
                return -4;
            }

            usTID++;
            return 0;
        }

        public int ReadHoldingRegisters5(byte deviceAddr1, ushort startAddr, ushort num, byte[] buf)
        {
            int i = headerLen;
            int j = 0;
            sendBuf[i++] = (byte)((startAddr >> 8) & 0xff);
            sendBuf[i++] = (byte)(startAddr & 0xff);
            sendBuf[i++] = (byte)((num >> 8) & 0xff);
            sendBuf[i++] = (byte)(num & 0xff);
            if (!SendData4(deviceAddr1, FUN_READ_HOLDING, 4))
            {
                return -1;
            }
            byte recvFunCode = 0;
            ushort recvDataLength = 0;
            int ret = RecvData5(ref recvFunCode, ref recvDataLength);
            if (ret <= 0)
            {
                return -2;
            }
            if (recvFunCode != FUN_READ_HOLDING)
            {
                return -3;
            }
            i = headerLen;
            byte len = recvBuf[i++];
            Array.Copy(recvBuf, i, buf, 0, len);
            usTID++;
            return 0;
        }
        public int ReadHoldingRegisters6(byte deviceAddr1, ushort startAddr, ushort num, byte[] buf)
        {
            int i = headerLen;
            int j = 0;
            sendBuf[i++] = (byte)((startAddr >> 8) & 0xff);
            sendBuf[i++] = (byte)(startAddr & 0xff);
            sendBuf[i++] = (byte)((num >> 8) & 0xff);
            sendBuf[i++] = (byte)(num & 0xff);
            if (!SendData4(deviceAddr1, FUN_READ_HOLDING, 4))
            {
                return -1;
            }
            byte recvFunCode = 0;
            ushort recvDataLength = 0;
            int ret = RecvData2(ref recvFunCode, ref recvDataLength);
            if (ret <= 0)
            {
                return -2;
            }
            if (recvFunCode != FUN_READ_HOLDING)
            {
                return -3;
            }
            i = headerLen;
            byte len = recvBuf[i++];
            Array.Copy(recvBuf, i, buf, 0, len);
            usTID++;
            return 0;
        }
        public int ReadInputRegisters5(byte deviceAddr1, int startAddr, ushort num, byte[] buf)
        {
            int i = headerLen;
            int j = 0;
            sendBuf[i++] = (byte)((startAddr >> 8) & 0xff);
            sendBuf[i++] = (byte)(startAddr & 0xff);
            sendBuf[i++] = (byte)((num >> 8) & 0xff);
            sendBuf[i++] = (byte)(num & 0xff);
            if (!SendData4(deviceAddr1, FUN_READ_INPUT, 4))
            {
                return -1;
            }
            byte recvFunCode = 0;
            ushort recvDataLength = 0;
            int ret = RecvData5(ref recvFunCode, ref recvDataLength);
            if (ret <= 0)
            {
                return -2;
            }
            if (recvFunCode != FUN_READ_INPUT)
            {
                return -3;
            }
            i = headerLen;
            byte len = recvBuf5[i++];
            Array.Copy(recvBuf5, i, buf, 0, len);
            //usTID++;
            return 0;
        }
        
        public  int ReadInputRegisters4(byte deviceAddr1, int startAddr, int num, byte[] buf)
        {
            
                int i = headerLen;
                int j = 0;
                sendBuf[i++] = (byte)((startAddr >> 8) & 0xff);
                sendBuf[i++] = (byte)(startAddr & 0xff);
                sendBuf[i++] = (byte)((num >> 8) & 0xff);
                sendBuf[i++] = (byte)(num & 0xff);
                if (!SendData4(deviceAddr1, FUN_READ_INPUT, 4))
                {
                    return -1;
                }
                byte recvFunCode = 0;
                ushort recvDataLength = 0;
                int ret = RecvData2(ref recvFunCode, ref recvDataLength);
                if (ret <= 0)
                {
                    return -2;
                }
                if (recvFunCode != FUN_READ_INPUT)
                {
                    return -3;
                }
                i = headerLen;
                byte len = recvBuf[i++];
                Console.WriteLine(buf.Length+";"+len);
                Array.Copy(recvBuf, i, buf, 0, len);

                //usTID++;
                return 0;
                
                
        }
        public int ReadInputRegisters6(byte deviceAddr1, int startAddr, int num, byte[] buf)
        {
            int i = headerLen;
            int j = 0;
            sendBuf[i++] = (byte)((startAddr >> 8) & 0xff);
            sendBuf[i++] = (byte)(startAddr & 0xff);
            sendBuf[i++] = (byte)((num >> 8) & 0xff);
            sendBuf[i++] = (byte)(num & 0xff);
            if (!SendData6(deviceAddr1, FUN_READ_INPUT, 4))
            {
                return -1;
            }
            byte recvFunCode = 0;
            ushort recvDataLength = 0;
            int ret = RecvData6(ref recvFunCode, ref recvDataLength);
            if (ret <= 0)
            {
                return -2;
            }
            if (recvFunCode != FUN_READ_INPUT)
            {
                return -3;
            }
            i = headerLen;
            byte len = recvBuf6[i++];
            Array.Copy(recvBuf6, i, buf, 0, len);
            //usTID++;
            return 0;
        }
        public int ReadInputRegisters34H(byte deviceAddr1, ushort startAddr, ushort num, byte[] buf)
        {
            int i = headerLen;
            int j = 0;
            sendBuf34H[i++] = (byte)((startAddr >> 8) & 0xff);
            sendBuf34H[i++] = (byte)(startAddr & 0xff);
            sendBuf34H[i++] = (byte)((num >> 8) & 0xff);
            sendBuf34H[i++] = (byte)(num & 0xff);
            if (!SendData34(deviceAddr1, FUN_READ_INPUT2, 4))
            {
                return -1;
            }
            byte recvFunCode = 0;
            ushort recvDataLength = 0;
            int ret = RecvData34(ref recvFunCode, ref recvDataLength);
            if (ret <= 0)
            {
                return -2;
            }
            if (recvFunCode != FUN_READ_INPUT2)
            {
                return -3;
            }
            i = headerLen;
            //byte len = 700;
            Array.Copy(recvBuf34H, 10, buf, 0, 1226);
            //usTID++;
            return 0;
        }
        public int ReadInputRegisters2(ushort startAddr, ushort num, byte[] buf)
        {
            int i = headerLen;
            int j = 0;
            sendBuf[i++] = (byte)((startAddr >> 8) & 0xff);
            sendBuf[i++] = (byte)(startAddr & 0xff);
            sendBuf[i++] = (byte)((num >> 8) & 0xff);
            sendBuf[i++] = (byte)(num & 0xff);
            if (!SendData(FUN_READ_INPUT2, 4))
            {
                return -1;
            }
            byte recvFunCode = 0;
            ushort recvDataLength = 0;
            int ret = RecvData(ref recvFunCode, ref recvDataLength);
            if (ret <= 0)
            {
                return -2;
            }
            if (recvFunCode != FUN_READ_INPUT2)
            {
                return -3;
            }
            i = headerLen;
            byte len = recvBuf[i++];
            Array.Copy(recvBuf, i, buf, 0, len);
            usTID++;
            return 0;
        }
        public int ReadHoldingRegisters(ushort startAddr, ushort num, byte[] buf)
        {
            int i = headerLen;
            int j = 0;
            sendBuf[i++] = (byte)((startAddr >> 8) & 0xff);
            sendBuf[i++] = (byte)(startAddr & 0xff);
            sendBuf[i++] = (byte)((num >> 8) & 0xff);
            sendBuf[i++] = (byte)(num & 0xff);
            if (!SendData(FUN_READ_HOLDING, 4))
            {
                return -1;
            }
            byte recvFunCode = 0;
            ushort recvDataLength = 0;
            int ret = RecvData(ref recvFunCode, ref recvDataLength);
            if (ret <= 0)
            {
                return -2;
            }
            if (recvFunCode != FUN_READ_HOLDING)
            {
                return -3;
            }
            i = headerLen;
            byte len = recvBuf[i++];
            Array.Copy(recvBuf, i, buf, 0, len);
            usTID++;
            return 0;
        }

        public int ReadHoldingRegisters2(byte deviceAddr1, int startAddr, int num, byte[] buf)
        {
            int i = headerLen;
            int j = 0;
            sendBuf[i++] = (byte)((startAddr >> 8) & 0xff);
            sendBuf[i++] = (byte)(startAddr & 0xff);
            sendBuf[i++] = (byte)((num >> 8) & 0xff);
            sendBuf[i++] = (byte)(num & 0xff);
            if (!SendData4(deviceAddr1, FUN_READ_HOLDING, 4))
            {
                return -1;
            }
            byte recvFunCode = 0;
            ushort recvDataLength = 0;
            int ret = RecvData2(ref recvFunCode, ref recvDataLength);
            if (ret <= 0)
            {
                return -2;
            }
            if (recvFunCode != FUN_READ_HOLDING)
            {
                return -3;
            }
            i = headerLen;
            byte len = recvBuf[i++];
            Array.Copy(recvBuf, i, buf, 0, len);
            //usTID++;
            return 0;
        }
        public int WriteMultiRegisters(byte deviceAddr1,ushort startAddr, ushort num, byte[] buf)
        {
            int i = headerLen;
            int j = 0;
            sendBuf[i++] = (byte)((startAddr >> 8) & 0xff);
            sendBuf[i++] = (byte)(startAddr & 0xff);
            sendBuf[i++] = (byte)((num >> 8) & 0xff);
            sendBuf[i++] = (byte)(num & 0xff);
            sendBuf[i++] = (byte)(num * 2); //比特数
            Array.Copy(buf, 0, sendBuf, i, num * 2);
            ////!SendData4(deviceAddr1, FUN_READ_INPUT, 4)
            if (!SendData4(deviceAddr1,FUN_WRITE_MULTI, (ushort)(5 + num * 2)))
            {
                return -1;
            }
            byte recvFunCode = 0;
            ushort recvDataLength = 0;
            int ret = RecvData(ref recvFunCode, ref recvDataLength);
            if (ret <= 0)
            {
                return -2;
            }
            if (recvFunCode != FUN_WRITE_MULTI)
            {
                return -3;
            }
            i = headerLen;
            ushort recvStartAddr = 0;
            ushort recvNum;
            recvStartAddr = (ushort)(recvBuf[i++] << 8);
            recvStartAddr |= (ushort)(recvBuf[i++]);
            recvNum = (ushort)(recvBuf[i++] << 8);
            recvNum |= (ushort)(recvBuf[i++]);
            if (recvStartAddr != startAddr || recvNum != num)
            {
                return -4;
            }

            usTID++;
            return 0;
        }
        #endregion
        #region 内部函数
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tid">事物元标识符</param>
        /// <param name="funCode"></param>
        /// <param name="startAddr"></param>
        /// <param name="datalength">实际数据的长度，不包括设备地址和功能码</param>
        /// <param name="buf"></param>
        /// <returns></returns>
        private bool SendData(byte funCode, ushort datalength)
        {
            try
            {
                if (!IsConnected)
                {
                    if (!MakeConnect())
                    {
                        return false;
                    }
                }
                int i = 0;
                usTID = 0;
                sendBuf[i++] = (byte)((usTID >> 8) & 0xff);
                sendBuf[i++] = (byte)(usTID & 0xff);
                sendBuf[i++] = 0x00;
                sendBuf[i++] = 0x00;
                sendBuf[i++] = (byte)(((datalength + 2) >> 8) & 0xff);
                sendBuf[i++] = (byte)((datalength + 2) & 0xff);
                sendBuf[i++] = deviceAddr;
                sendBuf[i++] = funCode;
                stream.Write(sendBuf, 0, headerLen + datalength);
                //stream.Flush();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool SendData4(byte deviceAddr1, byte funCode, ushort datalength)
        {
            try
            {
                //Console.WriteLine(IsConnected);
                if (!IsConnected)
                {
                    if (!MakeConnect())
                    {
                        return false;
                    }
                }
                int i = 0;
                usTID = 0;
                sendBuf[i++] = (byte)((usTID >> 8) & 0xff);
                sendBuf[i++] = (byte)(usTID & 0xff);
                sendBuf[i++] = 0x00;
                sendBuf[i++] = 0x00;
                sendBuf[i++] = (byte)(((datalength + 2) >> 8) & 0xff);
                sendBuf[i++] = (byte)((datalength + 2) & 0xff);
                sendBuf[i++] = deviceAddr1;
                sendBuf[i++] = funCode;
                stream.Write(sendBuf, 0, headerLen + datalength);
                //stream.Flush();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool SendData6(byte deviceAddr1, byte funCode, ushort datalength)
        {
            try
            {
                //Console.WriteLine(IsConnected);
                if (!IsConnected)
                {
                    if (!MakeConnect())
                    {
                        return false;
                    }
                }
                int i = 0;
                usTID = 0;
                sendBuf[i++] = (byte)((usTID >> 8) & 0xff);
                sendBuf[i++] = (byte)(usTID & 0xff);
                sendBuf[i++] = 0x00;
                sendBuf[i++] = 0x00;
                sendBuf[i++] = (byte)(((datalength + 2) >> 8) & 0xff);
                sendBuf[i++] = (byte)((datalength + 2) & 0xff);
                sendBuf[i++] = deviceAddr1;
                sendBuf[i++] = funCode;
                stream.Write(sendBuf, 0, headerLen + datalength);
                //stream.Flush();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool SendData34(byte deviceAddr1, byte funCode, ushort datalength)
        {
            try
            {
                if (!IsConnected)
                {
                    if (!MakeConnect())
                    {
                        return false;
                    }
                }
                int i = 0;
                usTID = 0;
                sendBuf34H[i++] = (byte)((usTID >> 8) & 0xff);
                sendBuf34H[i++] = (byte)(usTID & 0xff);
                sendBuf34H[i++] = 0x00;
                sendBuf34H[i++] = 0x00;
                sendBuf34H[i++] = (byte)(((datalength + 2) >> 8) & 0xff);
                sendBuf34H[i++] = (byte)((datalength + 2) & 0xff);
                sendBuf34H[i++] = deviceAddr1;
                sendBuf34H[i++] = funCode;
                stream.Write(sendBuf34H, 0, headerLen + datalength);
                //stream.Flush();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool SendData2(byte funCode, ushort datalength)
        {
            try
            {
                if (!IsConnected)
                {
                    if (!MakeConnect())
                    {
                        return false;
                    }
                }
                int i = 0;
                sendBuf[i++] = (byte)(byte)((usTID >> 8) & 0xff);
                sendBuf[i++] = (byte)(usTID & 0xff);
                sendBuf[i++] = 0x00;
                sendBuf[i++] = 0x00;
                sendBuf[i++] = (byte)(((datalength + 2) >> 8) & 0xff);
                sendBuf[i++] = (byte)((datalength + 2) & 0xff);
                sendBuf[i++] = 17;
                sendBuf[i++] = funCode;
                stream.Write(sendBuf, 0, headerLen + datalength);
                //stream.Flush();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="datalength">长度 不包括单元标识符和功能码</param>
        /// <param name="funCode"></param>
        /// <returns>返回的字节数</returns>
        private int RecvData(ref byte funCode, ref ushort datalength)
        {
            //Console.WriteLine(tcpclient+","+IsConnected+"1");
            if (!IsConnected)
            {
                if (!MakeConnect())
                {
                    return -1;
                }
            }
            int i = 0;
            int j = 0;
            NetworkStream stream = tcpclient.GetStream();
            for (j = 0; j < 200; j++)
            {
                if (tcpclient == null)
                {
                    return -10;
                }
                if (tcpclient.Available > 6)
                {
                    break;
                }
                Thread.Sleep(10);
            }

            if (tcpclient.Available < headerLen)
            {
                ModbusTcp.instance.close();
                return -2;
            }
            int ret = stream.Read(recvBuf, 0, recvBuf.Length);
            //Console.WriteLine(string.Join(",", recvBuf));
            //Array.Clear(recvBuf, 0, recvBuf.Length);
            if (ret <= 0)
            {
                return ret;
            }
            ushort tid = 0;
            i = 0;
            tid = recvBuf[i++];
            tid |= (ushort)(recvBuf[i++] << 8);
            if (tid != usTID)
            {
                return -3;
            }
            i += 2;
            datalength = (ushort)(recvBuf[i++] << 8);
            datalength |= recvBuf[i++];
            datalength -= 2;
            byte recvAddr = recvBuf[i++];
            /*
            if (recvAddr != this.deviceAddr)
            {
                return -4;
            }*/
            funCode = recvBuf[i++];

            return ret;
        }

        private int RecvData2(ref byte funCode, ref ushort datalength)
        {
            try
            {
                if (!IsConnected)
                {
                    if (!MakeConnect())
                    {
                        return -1;
                    }
                }
                int i = 0;
                int j = 0;
                NetworkStream stream = tcpclient.GetStream();
                for (j = 0; j < 45; j++)
                {
                    if (tcpclient == null)
                    {
                        //Console.WriteLine("tcpclient==null");
                        return -10;
                    }
                    if (tcpclient.Available > 6)
                    {
                        break;
                    }
                    Thread.Sleep(8);
                }

                if (tcpclient.Available < headerLen)
                {
                    //ModbusTcp.instance.close();
                    Console.WriteLine(-2);
                    return -2;
                }
                int ret = stream.Read(recvBuf, 0, recvBuf.Length);

                if (ret <= 0)
                {
                    return ret;
                }
                ushort tid = 0;
                i = 0;
                tid = recvBuf[i++];
                tid |= (ushort)(recvBuf[i++] << 8);
                if (tid != usTID)
                {
                    return -3;
                }
                i += 2;
                datalength = (ushort)(recvBuf[i++] << 8);
                datalength |= recvBuf[i++];
                datalength -= 2;
                byte recvAddr = recvBuf[i++];
                if (recvAddr != 27)
                {
                    //return -4;
                }
                funCode = recvBuf[i++];

                return ret;
            }
            catch (Exception)
            {
                Console.WriteLine("33333333333333333333");
                return -9;
            }
            
        }
        private int RecvData6(ref byte funCode, ref ushort datalength)
        {
            if (!IsConnected)
            {
                if (!MakeConnect())
                {
                    return -1;
                }
            }
            int i = 0;
            int j = 0;
            NetworkStream stream = tcpclient.GetStream();
            for (j = 0; j < 10; j++)
            {
                if (tcpclient == null)
                {
                    //Console.WriteLine("tcpclient==null");
                    return -10;
                }
                if (tcpclient.Available > 6)
                {
                    break;
                }
                Thread.Sleep(20);
            }

            if (tcpclient.Available < headerLen)
            {
                //ModbusTcp.instance.close();
                return -2;
            }
            int ret = stream.Read(recvBuf6, 0, recvBuf6.Length);

            if (ret <= 0)
            {
                return ret;
            }
            ushort tid = 0;
            i = 0;
            tid = recvBuf6[i++];
            tid |= (ushort)(recvBuf6[i++] << 8);
            if (tid != usTID)
            {
                return -3;
            }
            i += 2;
            datalength = (ushort)(recvBuf6[i++] << 8);
            datalength |= recvBuf6[i++];
            datalength -= 2;
            byte recvAddr = recvBuf6[i++];
            if (recvAddr != 27)
            {
                //return -4;
            }
            funCode = recvBuf6[i++];

            return ret;
        }
        private int RecvData5(ref byte funCode, ref ushort datalength)
        {
            if (!IsConnected)
            {
                if (!MakeConnect())
                {
                    return -1;
                }
            }
            int i = 0;
            int j = 0;
            NetworkStream stream = tcpclient.GetStream();
            for (j = 0; j < 100; j++)
            {
                if (tcpclient == null)
                {
                    //Console.WriteLine("tcpclient==null");
                    return -10;
                }
                if (tcpclient.Available > 6)
                {
                    break;
                }
                Thread.Sleep(10);
            }

            if (tcpclient.Available < headerLen)
            {
                return -2;
            }
            int ret = stream.Read(recvBuf5, 0, recvBuf5.Length);
            //Console.WriteLine(string.Join(",", recvBuf5));
            if (ret <= 0)
            {
                return ret;
            }
            ushort tid = 0;
            i = 0;
            tid = recvBuf5[i++];
            tid |= (ushort)(recvBuf5[i++] << 8);
            if (tid != usTID)
            {
                return -3;
            }
            i += 2;
            datalength = (ushort)(recvBuf5[i++] << 8);
            datalength |= recvBuf5[i++];
            datalength -= 2;
            byte recvAddr = recvBuf5[i++];
            if (recvAddr != 27)
            {
                //return -4;
            }
            funCode = recvBuf5[i++];

            return ret;
        }

        private int RecvData34(ref byte funCode, ref ushort datalength)
        {
            if (!IsConnected)
            {
                if (!MakeConnect())
                {
                    return -1;
                }
            }
            int i = 0;
            int j = 0;
            NetworkStream stream = tcpclient.GetStream();
            for (j = 0; j < 100; j++)
            {
                if (tcpclient.Available > 6)
                {
                    break;
                }
                Thread.Sleep(10);
            }

            if (tcpclient.Available < headerLen)
            {
                return -2;
            }
            int ret = stream.Read(recvBuf34H, 0, recvBuf34H.Length);
            if (ret <= 0)
            {
                return ret;
            }
            ushort tid = 0;
            i = 0;
            tid = recvBuf34H[i++];
            tid |= (ushort)(recvBuf34H[i++] << 8);
            if (tid != usTID)
            {
                return -3;
            }
            i += 2;
            datalength = (ushort)(recvBuf34H[i++] << 8);
            datalength |= recvBuf34H[i++];
            datalength -= 2;
            byte recvAddr = recvBuf34H[i++];
            if (recvAddr != 27)
            {
                //return -4;
            }
            funCode = recvBuf34H[i++];

            return ret;
        }

        #endregion

    }
}
