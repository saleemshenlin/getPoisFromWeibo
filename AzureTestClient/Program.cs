using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureTestClient
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {

                Console.WriteLine("Starting at {0}", DateTime.Now);

                //String serviceBusNamespace = "your_service_bus_namespace";
                //String issuer = "your_service_bus_owner";
                //String key = "your_service_bus_key";

                String connectionString = @"Endpoint=sb://azuretest-ns.servicebus.chinacloudapi.cn/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=3HzV1a5bcs/OOj0YS+Iec7XPA9ys2s9oqrUrg8pcILA=";

                QueueClient queueClient = QueueClient.CreateFromConnectionString(connectionString, "TSPQueue");

                BrokeredMessage message;

                int waitMinutes = 3;  // Use as the default, if no value
                // is specified at command line.

                if (0 != args.Length)
                {
                    waitMinutes = Convert.ToInt16(args[0]);
                }

                String waitString;
                waitString = (waitMinutes == 1) ? "minute" : waitMinutes.ToString() + " minutes";

                while (true)
                {
                    message = queueClient.Receive();

                    if (message != null)
                    {
                        try
                        {
                            string str = message.GetBody<string>();
                            Console.WriteLine(str);

                            // Remove message from queue
                            message.Complete();

                            if ("Complete" == str)
                            {
                                Console.WriteLine("Finished at {0}.", DateTime.Now);
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            // Indicates a problem. Unlock the message in the queue.
                            message.Abandon();
                            throw e;
                        }
                    }
                    else
                    {
                        // The queue is empty.
                        Console.WriteLine("Queue is empty. Sleeping for another {0}.", waitString);
                        System.Threading.Thread.Sleep(60000 * waitMinutes);
                    }
                }
                queueClient.Close();
                Environment.Exit(0);
            }
            catch (ServerBusyException serverBusyException)
            {
                Console.WriteLine("ServerBusyException encountered");
                Console.WriteLine(serverBusyException.Message);
                Console.WriteLine(serverBusyException.StackTrace);
                Environment.Exit(-1);
            }
            catch (ServerErrorException serverErrorException)
            {
                Console.WriteLine("ServerErrorException encountered");
                Console.WriteLine(serverErrorException.Message);
                Console.WriteLine(serverErrorException.StackTrace);
                Environment.Exit(-1);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception encountered");
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
                Environment.Exit(-1);
            }
        }
    }
}
