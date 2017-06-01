using System;
using Nancy.Hosting.Self;

namespace SkladApi.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new NancyHost(new Uri("http://localhost:8000")))
            {
                host.Start();
                Console.WriteLine("Running on http://localhost:8000");
                Console.ReadLine();
            }
        }
    }
}
