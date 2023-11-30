using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;

namespace _05_pub_sub_and_pull_push_server {
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
                    Console.WriteLine($"I: publishing update b'{message}'");
                    publisher.SendFrame(message);
                }
            }
        }
    }
}
