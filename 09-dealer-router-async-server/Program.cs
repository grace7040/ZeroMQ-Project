using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;

namespace _09_dealer_router_async_server
{
    class ServerTask
    {
        private readonly int numServer;

        public ServerTask(int numServer)
        {
            this.numServer = numServer;
        }

        public void Run()
        {
            using (var frontend = new RouterSocket())
            using (var backend = new DealerSocket())
            {
                frontend.Bind("tcp://*:5570");
                backend.Bind("inproc://backend");

                Thread[] workers = new Thread[numServer];
                for (int i = 0; i < numServer; i++)
                {
                    workers[i] = new Thread(() => RunWorker(i));
                    workers[i].Start();
                }

                Proxy proxy = new Proxy(frontend, backend);
                proxy.Start();

                frontend.Close();
                backend.Close();
            }
        }
        private void RunWorker(int id)
        {
            using (var worker = new DealerSocket())
            {
                worker.Connect("inproc://backend");
                Console.WriteLine($"Worker#{id} started");

                while (true)
                {
                    var ident = worker.ReceiveFrameString();
                    var msg = worker.ReceiveFrameString();

                    Console.WriteLine($"Worker#{id} received {msg} from {ident}");

                    var message = new NetMQMessage();
                    message.Append(ident.ToString());
                    message.Append(msg.ToString());
                    worker.SendMultipartMessage(message);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1 || !int.TryParse(args[0], out int numServer))
            {
                Console.WriteLine("Usage: Program.exe <numServer>");
                return;
            }

            var serverTask = new ServerTask(numServer);
            serverTask.Run();
        }
    }
}
