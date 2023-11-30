using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;

namespace _06_pub_sub_and_pull_push_client
{
    class Program
    {
        static void Main(string[] args)
        {

            string clientID;
            if (args.Length != 2)
            {
                Console.WriteLine("실행시 clientID를 입력하세요. 입력하지 않은 경우 1번 입니다.");
                clientID = "client#1";
            }
            else
                clientID = args[1];


            using (var subscriber = new SubscriberSocket())
            using (var publisher = new PushSocket())
            using (var poller = new NetMQPoller { subscriber })
            {
                subscriber.Subscribe("");
                subscriber.Connect("tcp://localhost:5557");

                publisher.Connect("tcp://localhost:5558");

                var random = new Random((int)DateTime.Now.Ticks);

                subscriber.ReceiveReady += (s, a) => { };

                while (true)
                {
                    if (subscriber.Poll(TimeSpan.FromMilliseconds(100)))
                    {
                        var message = subscriber.ReceiveFrameString();
                        Console.WriteLine("{0}: receive status => {1}", clientID, message);
                    }
                    else
                    {
                        int rand = random.Next(1, 101);
                        if (rand < 10)
                        {
                            Thread.Sleep(1000);
                            string msg = $"({clientID}:ON)";
                            publisher.SendFrame(msg);
                            Console.WriteLine("{0}: send status - activated", clientID);
                        }
                        else if (rand > 90)
                        {
                            Thread.Sleep(1000);
                            string msg = $"({clientID}:OFF)";
                            publisher.SendFrame(msg);
                            Console.WriteLine("{0}: send status - deactivated", clientID);
                        }
                    }
                }
            }
        }

    }
}
