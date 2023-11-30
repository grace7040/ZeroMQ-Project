using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ZeroMQ;

namespace _02_req_rep_basic_client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connecting to hello world server...");
            using (var context = ZContext.Create())
            using (var socket = new ZSocket(context, ZSocketType.REQ))
            {
                socket.Connect(string.Format("tcp://{0}", "127.0.0.1:5555"));
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine($"Sending request {i} ...");

                    byte[] sendBytes = Encoding.UTF8.GetBytes("Hello");
                    socket.SendFrames(new List<ZFrame>() { new ZFrame(sendBytes.Length), new ZFrame(sendBytes, 0, sendBytes.Length) });

                    List<ZFrame> frames = socket.ReceiveFrames(2).ToList();
                    byte[] rcvBytes = new byte[frames[0].ReadInt32()];
                    frames[1].Read(rcvBytes, 0, rcvBytes.Length);

                    Console.WriteLine($"Received reply {i} [ b'{Encoding.UTF8.GetString(rcvBytes)}' ]");

                }
            }

            Console.ReadLine();
        }
    }
}
