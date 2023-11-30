using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;

namespace _10_dealer_router_async_client
{

    class ClientTask
    {
        private readonly string id;

        public ClientTask(string id)
        {
            this.id = id;
        }

        public void Run()
        {
            using (var socket = new DealerSocket())
            using (var poller = new NetMQPoller { socket })
            {
                string identity = this.id;
                socket.Options.Identity = Encoding.ASCII.GetBytes($"{identity}");
                socket.Connect("tcp://localhost:5570");
                Console.WriteLine($"Client {identity} started");

                socket.ReceiveReady += (s, a) => { };

                int reqs = 0;
                while (true)
                {
                    reqs++;
                    Console.WriteLine($"Req #{reqs} sent..");
                    socket.SendFrame($"request #{reqs}");

                    Thread.Sleep(1000);

                    if (socket.Poll(TimeSpan.FromMilliseconds(1000)))
                    {
                        var msg = socket.ReceiveFrameString();
                        Console.WriteLine($"{identity} received: {msg}");
                    }
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: Program.exe <client_id>");
                return;
            }

            var client = new ClientTask(args[0]);
            client.Run();
        }
    }
}
