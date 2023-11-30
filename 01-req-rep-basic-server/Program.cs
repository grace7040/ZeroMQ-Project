using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ZeroMQ;

namespace _01_req_rep_basic_server
{
    
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = ZContext.Create())
            using (var socket = new ZSocket(context, ZSocketType.REP))
            {
                socket.Bind(string.Format("tcp://{0}", "127.0.0.1:5555"));
                while (true)
                {
                    List<ZFrame> frames = socket.ReceiveFrames(2).ToList();
                    byte[] rcvBytes = new byte[frames[0].ReadInt32()];
                    frames[1].Read(rcvBytes, 0, rcvBytes.Length);
                    Console.WriteLine($"Received request: b'{Encoding.UTF8.GetString(rcvBytes)}'");

                    Thread.Sleep(1000);

                    byte[] sendBytes = Encoding.UTF8.GetBytes("World");
                    socket.SendFrames(new List<ZFrame>() { new ZFrame(sendBytes.Length), new ZFrame(sendBytes, 0, sendBytes.Length) });
                }
            }
        }

    }
}
