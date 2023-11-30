using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ZeroMQ;

namespace _03_pub_sub_basic_server
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rand = new Random();
            Console.WriteLine("Publishing updates at weather server...");

            using (var context = new ZContext())
            using (var socket = new ZSocket(context, ZSocketType.PUB))
            {
                socket.Bind(string.Format("tcp://{0}", "127.0.0.1:5556"));

                while (true)
                {
                    String message;

                    int zipcode = rand.Next(1, 100000);
                    int temperature = rand.Next(-80, 135);
                    int relhumidity = rand.Next(10, 60);

                    message = string.Format("{0} {1} {2}", zipcode, temperature, relhumidity);


                    using (var updateFrame = new ZFrame(message))
                    {
                        socket.Send(updateFrame);
                    }

                }
            }
        }
    }


}
