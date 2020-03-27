﻿using QuickFix;
using System;
using Acceptor;

namespace FixApp
{
    class Program
    {
        private const string HttpServerPrefix = "http://127.0.0.1:5080/";

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("=============");
            Console.WriteLine("This is only an example program, meant to be used with the TradeClient example.");
            Console.WriteLine("=============");

            if (args.Length != 1)
            {
                Console.WriteLine("usage: Executor CONFIG_FILENAME");
                Environment.Exit(2);
            }

            try
            {
                SessionSettings settings = new SessionSettings(args[0]);
                IApplication executorApp = new Executor();
                IMessageStoreFactory storeFactory = new FileStoreFactory(settings);
                ILogFactory logFactory = new FileLogFactory(settings);
                ThreadedSocketAcceptor acceptor = new ThreadedSocketAcceptor(executorApp, storeFactory, settings, logFactory);
                HttpServer srv = new HttpServer(HttpServerPrefix, settings);

                acceptor.Start();
                srv.Start();

                Console.WriteLine("View Executor status: " + HttpServerPrefix);
                Console.WriteLine("press <enter> to quit");
                Console.Read();

                srv.Stop();
                acceptor.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine("==FATAL ERROR==");
                Console.WriteLine(e.ToString());
            }
        }
    }
}
