using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;

namespace _11_dealer_router_async_client_thread
{
    class ClientTask
    {
        private readonly string id;
        private DealerSocket socket;
        private string identity;
        private NetMQPoller poller;

        public ClientTask(string id)
        {
            this.id = id;
        }

        private void RecvHandler()
        {
            while (true)
            {
                if (socket.Poll(TimeSpan.FromMilliseconds(1000)))
                {
                    var msg = socket.ReceiveFrameString();
                    Console.WriteLine($"{identity} received: {msg}");
                }
            }
            
        }

        public void Run()
        {
            using (socket = new DealerSocket())
            using (poller = new NetMQPoller { socket })
            {
                identity = this.id;
                socket.Options.Identity = Encoding.ASCII.GetBytes($"{identity}");
                socket.Connect("tcp://localhost:5570");
                Console.WriteLine($"Client {identity} started");

                socket.ReceiveReady += (s, a) => { };

                int reqs = 0;
                var clientThread = new Thread(() =>
                {
                    RecvHandler();
                });
                clientThread.IsBackground = false;
                clientThread.Start();

                while (true)
                {
                    reqs++;
                    Console.WriteLine($"Req #{reqs} sent..");
                    socket.SendFrame($"request #{reqs}");
                    Thread.Sleep(1000);
                }

            }
        }

        public void Stop()
        {
            poller?.Stop();
            socket?.Close();
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

