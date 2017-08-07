using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;

namespace CatManager
{
    //cat 适配器
    public class CatAdapter
    {
        private static CatAdapter _catAdapter;

        private int PORT = 6000;

        private string HOST = "127.0.0.1";

        private Socket _clientSocket;
        
        public static CatAdapter GetInstance()
        {
            if (_catAdapter == null)
            {
                _catAdapter = new CatAdapter();
            }
            return _catAdapter;
        }

        //尝试连接cat
        public void ConnectToCat()
        {
            if (_clientSocket == null)
            {
                Console.WriteLine("begin connect cat {0}:{1}",HOST,PORT);
                IPAddress ip = IPAddress.Parse(HOST);
                _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    _clientSocket.Connect(new IPEndPoint(ip, PORT)); //配置服务器IP与端口  
                    Console.WriteLine("connect cat success!");
                }
                catch(Exception e)
                {
                    Console.WriteLine("connect cat error : {0}",e.ToString());
                }
            }
            else
            {
                Console.WriteLine("socket connect has exist!!!");
            }
        }
        
        //发送命令
        public void CatSend(string msg)
        {
            
        }
        
        //获取当前COM信息
        public void GetAllComInfo()
        {
            
        }
        
        //发送短信命令
        public void SendMsg(JObject obj)
        {
            string mobile = obj["mobile"].ToString();
            string content = obj["content"].ToString();
            Console.WriteLine("send msg to : {0} content {1}",mobile,content);   
            
        }
    }
}