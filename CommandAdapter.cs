using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;  
namespace CatManager
{
    //socket.io 接收服务器命令
    public class CommandAdapter
    {
        private static CommandAdapter _commandAdapter;

        private int PORT = 4000;

        private string HOST = "139.196.210.143";

        private Client _client;
        
        public static CommandAdapter GetInstance()
        {
            if (_commandAdapter == null)
            {
                _commandAdapter = new CommandAdapter();
            }
            return _commandAdapter;
        }

        //尝试连接commandServer
        public void ConnectToCommandServer()
        {
            if (_client == null)
            {
                _client = new Client("Command",HOST, PORT, true,(pkgBytes) =>
                {
                    DealCommandWithBytes(pkgBytes);
                });
                _client.Connect();   
            }
        }

        //处理命令
        public void DealCommandWithBytes(byte[] bytes)
        {
            Console.WriteLine("deal command: {0}", Encoding.UTF8.GetString(bytes));
            JObject commandobj = (JObject)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes));
            string cType = commandobj["type"].ToString();
            JObject content = (JObject)commandobj["content"];
            Debug.WriteLine(cType);
            if (cType.Equals("sendMsg"))
            {
                CatAdapter.GetInstance().SendMsg(content);
            }
            else if (cType.Equals("call"))
            {
                CatAdapter.GetInstance().Call(content);
            }
        }
    }
}