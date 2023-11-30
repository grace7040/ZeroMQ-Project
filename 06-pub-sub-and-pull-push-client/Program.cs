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
        static void Main()
        {
            using (var subscriber = new SubscriberSocket())
            using (var publisher = new PushSocket())
            using(var poller = new NetMQPoller { subscriber })
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
                       var message = subscriber.ReceiveFrameBytes();
                        Console.WriteLine($"I: received message b'{Encoding.UTF8.GetString(message)}'");
                    }
                    else
                    {
                        var rand = random.Next(1, 101);
                        if (rand < 10)
                        {
                            publisher.SendFrame(Encoding.UTF8.GetBytes($"{rand}"));
                            Console.WriteLine($"I: sending message {rand}");
                        }
                    }
                }
            }
        }
    
    }
}
