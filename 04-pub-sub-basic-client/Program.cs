using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ZeroMQ;

namespace _04_pub_sub_basic_client
{
    class Program
    {
        static void Main(string[] args)
        {
            string zip_filter;
            if (args.Length != 2)
                zip_filter = "10001";
            else
                zip_filter = args[1];

            int total_temp = 0;
            int i = 0;
            using (var context = new ZContext())
            using (var socket = new ZSocket(context, ZSocketType.SUB))
            {
                socket.Connect(string.Format("tcp://{0}", "127.0.0.1:5556"));
                Console.WriteLine("Collecting updates from weather server...");

                socket.SetOption(ZSocketOption.SUBSCRIBE, zip_filter);
                
                for(i = 0; i < 20; i++)
                {
                    var replyFrame = socket.ReceiveFrame();
                    string message = replyFrame.ReadString();
                    string[] words = message.Split(' ');
                    total_temp += Int32.Parse(words[1]);
                    Console.WriteLine($"Receive temperature for zipcode '{zip_filter}' was {words[1]} F");
                }

            }

            Console.WriteLine($"Average temperature for zipcode '{zip_filter}' was {total_temp / i} F");
            Console.ReadLine();
        }
    }
}
