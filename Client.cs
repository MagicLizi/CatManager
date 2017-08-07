using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CatManager
{
    public class Client
    {
        private Socket _socket;

        private string _host;

        private int _port;
        
        public Client(string host, int port)
        {
            _host = host;
            _port = port;
            PkgManager.GetInstance().pkgCallback = (pkgBytes) =>
            {
                CommandAdapter.GetInstance().DealCommandWithBytes(pkgBytes);
            };
        }

        public void Connect()
        {
            if (_socket == null)
            {
                IPAddress ip = IPAddress.Parse(_host);
                IPEndPoint iep = new IPEndPoint(ip,_port);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.BeginConnect(iep, new AsyncCallback(ConnectCallback),_socket);
            }
        }

        private void ConnectCallback(IAsyncResult iar)
        {
            Socket client = (Socket)iar.AsyncState;
            try
            {
                client.EndConnect(iar);
                Console.WriteLine("connect server success!");
                Receive(client);  
            }
            catch (Exception e)
            {
                Console.WriteLine("connect error:{0}",e.ToString());
                ClearSocket();
                Connect();
            }
        }
        
        private void ClearSocket()
        {
            if (_socket!=null)
            {
                try
                {
                    _socket.Close();
                }
                catch (Exception e)
                {
    
                }
                finally
                {
                    _socket = null;
                }
            }
        }

        
        private void Receive(Socket client)
        {
            try
            {
                StateObject state = new StateObject();
                state.workSocket = client;
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine("begin receive error error:{0}",e.ToString());
            }
        }
        
        
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {   
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                int bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    PkgManager.GetInstance().parseBytes(state.buffer);
                    
                    StateObject newstate = new StateObject();
                    newstate.workSocket = client;
                    client.BeginReceive(newstate.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), newstate);
                }
                else
                {
                    if (client.Available == 0)
                    {
                        Console.WriteLine("从服务器断开，请重新连接！");
                        ClearSocket();
                        Connect();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("receive data error :{0}",e.ToString());
            }
        }
    }
}