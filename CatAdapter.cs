using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;
using System.Text;
using Newtonsoft.Json;

namespace CatManager
{
    //cat 适配器
    public class CatAdapter
    {
        private static CatAdapter _catAdapter;

        private int PORT = 6000;

        private string HOST = "127.0.0.1";

        private Client _client;

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
            if (_client == null)
            {
                _client = new Client("Cat",HOST, PORT, false, (pkgBytes) =>
                {
                    Console.WriteLine("receive cat callback: {0}", Encoding.UTF8.GetString(pkgBytes));
                });
                _client.Connect();
            }
        }
        
        //发送命令
        public void CatSend(string msg)
        {
            byte[] msgBytes = Encoding.GetEncoding("gbk").GetBytes(msg);
            _client.Send(msgBytes);
        }
        
        //获取当前COM信息
        public void GetAllComInfo()
        {
            string command = "AP$PORTS?";
            CatSend(command);
        }
        
        //发送短信命令
        public void SendMsg(JObject obj)
        {
            GetAllComInfo();
            string mobile = obj["mobile"].ToString();
            string content = obj["content"].ToString();
            string channelId = "0";
            if (obj["channelId"]!=null)
            {
                channelId = obj["channelId"].ToString();
            }
            //Console.WriteLine("send msg to : {0} content {1}",mobile,content);   
            JObject commandObj = new JObject();
            commandObj.Add(new JProperty("taskname", "短信任务"));
            commandObj.Add(new JProperty("tasktype", "短信"));
            commandObj.Add(new JProperty("number", mobile));
            commandObj.Add(new JProperty("content", content));
            commandObj.Add(new JProperty("count", 1));
            commandObj.Add(new JProperty("waittime", 0));
            string cStr = JsonConvert.SerializeObject(commandObj);
            int leng = Encoding.GetEncoding("gbk").GetBytes(cStr.ToCharArray()).Length;
            string command = "AP$TASK="+leng+ ","+channelId+"," + cStr;
            Console.WriteLine(command);
            CatSend(command);
        }

        public void Call(JObject obj)
        {
            string mobile = obj["mobile"].ToString();
            string channelId = "0";
            if (obj["channelId"] != null)
            {
                channelId = obj["channelId"].ToString();
            }
            JObject commandObj = new JObject();
            commandObj.Add(new JProperty("taskname", "语音任务"));
            commandObj.Add(new JProperty("tasktype", "语音"));
            commandObj.Add(new JProperty("number", mobile));
            commandObj.Add(new JProperty("content", "0,无;0,无;0,无;0,无;0,无;0,无;0,无;0,无;0,无;0,无;0,无;"));
            commandObj.Add(new JProperty("count",1));
            commandObj.Add(new JProperty("waittime", 0));
            string cStr = JsonConvert.SerializeObject(commandObj);
            int leng = Encoding.GetEncoding("gbk").GetBytes(cStr.ToCharArray()).Length;
            string command = "AP$TASK=" + leng + ","+ channelId + "," + cStr;
            Console.WriteLine(command);
            CatSend(command);

        }
    }
}