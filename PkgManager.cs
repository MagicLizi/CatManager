using System;
using System.Text;

namespace CatManager
{
    public class PkgManager
    {
        private static PkgManager _pkgManager;

        public static PkgManager GetInstance()
        {
            if (_pkgManager == null)
            {
                _pkgManager = new PkgManager();
            }
            return _pkgManager;
        }
        
        public void parseBytes(byte[] bytes)
        {
            parsePackage(bytes);
        }

        public Action<byte[]> pkgCallback;
        
        public byte[] curInParseBytes;
        public int sourceLaskPkgLength = 0;
        private void parsePackage(byte[] receiveByte)
        {
            byte[] nzReceiveBytes = DecodeZero(receiveByte);
            if (curInParseBytes==null)//当前没有正在解析中的包
            {
                if (nzReceiveBytes.Length >= 2)//找到包头
                {
                    byte[] lengthBytes = new byte[2];
                    lengthBytes[0] = nzReceiveBytes[0];
                    lengthBytes[1] = nzReceiveBytes[1];
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(lengthBytes);
                    }
                    int pkgLength = BitConverter.ToInt16(lengthBytes, 0);
                    sourceLaskPkgLength = nzReceiveBytes.Length - 2;
                    curInParseBytes = new byte[pkgLength];
                    Array.Copy(nzReceiveBytes, 2, curInParseBytes, 0, sourceLaskPkgLength);
                    //开始寻找包体
                    if (sourceLaskPkgLength >= pkgLength) //包体内包含完整内容
                    {
                        Console.WriteLine("receive full package: {0}", Encoding.UTF8.GetString(curInParseBytes));
                        
                        if (pkgCallback != null)
                        {
                            pkgCallback(curInParseBytes);
                        }
                        
                        curInParseBytes = null;
                        sourceLaskPkgLength = 0;
                        

                        if (nzReceiveBytes.Length - pkgLength - 2 > 0)
                        {
                            byte[] pkgLastBytes = new byte[nzReceiveBytes.Length - pkgLength - 2];
                            Buffer.BlockCopy(nzReceiveBytes,2 + pkgLength - 1,pkgLastBytes,0,pkgLastBytes.Length);
                            parsePackage(pkgLastBytes); 
                        }
                    }
                }
            }
            else
            {
                int nextNeedLength = curInParseBytes.Length - sourceLaskPkgLength;
                if (nzReceiveBytes.Length < nextNeedLength)
                {
                    Array.Copy(nzReceiveBytes, 0, curInParseBytes, sourceLaskPkgLength, nzReceiveBytes.Length);
                    sourceLaskPkgLength = sourceLaskPkgLength + nzReceiveBytes.Length;
                }
                else
                {
                    Array.Copy(nzReceiveBytes, 0, curInParseBytes, sourceLaskPkgLength, nextNeedLength);
                    Console.WriteLine("receive full package: {0}", Encoding.UTF8.GetString(curInParseBytes));
                    
                    if (pkgCallback!=null)
                    {
                        pkgCallback(curInParseBytes);
                    }
                    
                    curInParseBytes = null;
                    sourceLaskPkgLength = 0;
                   
                    
                    if (nzReceiveBytes.Length - nextNeedLength > 0)
                    {
                        byte[] pkgLastBytes = new byte[nzReceiveBytes.Length - nextNeedLength];
                        Buffer.BlockCopy(nzReceiveBytes,nzReceiveBytes.Length - nextNeedLength - 1,pkgLastBytes,0,nzReceiveBytes.Length - nextNeedLength);
                        parsePackage(pkgLastBytes); 
                    }
                    
                  
                }
            }
        }
        
        private byte[] DecodeZero(byte[] packet)
        {
            var i = packet.Length - 1;
            while(packet[i] == 0)
            {
                --i;
            }
            var temp = new byte[i + 1];
            Array.Copy(packet, temp, i + 1);
            return temp;
        }
        
        
    }
}