using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;

namespace _07_pub_sub_and_pull_push_server_v2
{
    class Program
    {
        static void Main()
        {
            using (var publisher = new PublisherSocket())
            using (var collector = new PullSocket())
            {
                publisher.Bind("tcp://*:5557");
                collector.Bind("tcp://*:5558");

                while (true)
                {
                    var message = collector.ReceiveFrameString();
                    Console.WriteLine($"server: publishing update => {message}");
                    publisher.SendFrame(message);
                }
            }
        }
    }
}
