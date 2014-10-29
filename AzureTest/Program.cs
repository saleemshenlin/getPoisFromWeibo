using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureTest
{
    class Program
    {
        // Value specifying how often to provide an update to the console.
        private static long loopCheck = 100000000;
        private static long nTimes = 0, nLoops = 0;

        private static double[,] distances;
        private static String[] cityNames;
        private static int[] bestOrder;
        private static double minDistance;

        private static NamespaceManager namespaceManager;
        private static QueueClient queueClient;
        private static String queueName = "TSPQueue";
        private static void BuildDistances(String fileLocation, int numCities)
        {

            try
            {
                StreamReader sr = new StreamReader(fileLocation);
                String[] sep1 = { ", " };

                double[,] cityLocs = new double[numCities, 2];

                for (int i = 0; i < numCities; i++)
                {
                    String[] line = sr.ReadLine().Split(sep1, StringSplitOptions.None);
                    cityNames[i] = line[0];
                    cityLocs[i, 0] = Convert.ToDouble(line[1]);
                    cityLocs[i, 1] = Convert.ToDouble(line[2]);
                }
                sr.Close();

                for (int i = 0; i < numCities; i++)
                {
                    for (int j = i; j < numCities; j++)
                    {
                        distances[i, j] = hypot(Math.Abs(cityLocs[i, 0] - cityLocs[j, 0]), Math.Abs(cityLocs[i, 1] - cityLocs[j, 1]));
                        distances[j, i] = distances[i, j];
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static double hypot(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        private static void permutation(List<int> startCities, double distSoFar, List<int> restCities)
        {
            try
            {

                nTimes++;
                if (nTimes == loopCheck)
                {
                    nLoops++;
                    nTimes = 0;
                    DateTime dateTime = DateTime.Now;
                    Console.Write("Current time is {0}.", dateTime);
                    Console.WriteLine(" Completed {0} iterations of size of {1}.", nLoops, loopCheck);
                }

                if ((restCities.Count == 1) && ((minDistance == -1) || (distSoFar + distances[restCities[0], startCities[0]] + distances[restCities[0], startCities[startCities.Count - 1]] < minDistance)))
                {
                    startCities.Add(restCities[0]);
                    newBestDistance(startCities, distSoFar + distances[restCities[0], startCities[0]] + distances[restCities[0], startCities[startCities.Count - 2]]);
                    startCities.Remove(startCities[startCities.Count - 1]);
                }
                else
                {
                    for (int i = 0; i < restCities.Count; i++)
                    {
                        startCities.Add(restCities[0]);
                        restCities.Remove(restCities[0]);
                        permutation(startCities, distSoFar + distances[startCities[startCities.Count - 1], startCities[startCities.Count - 2]], restCities);
                        restCities.Add(startCities[startCities.Count - 1]);
                        startCities.Remove(startCities[startCities.Count - 1]);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static void newBestDistance(List<int> cities, double distance)
        {
            try
            {
                minDistance = distance;
                String cityList = "Shortest distance is " + minDistance + ", with route: ";

                for (int i = 0; i < bestOrder.Length; i++)
                {
                    bestOrder[i] = cities[i];
                    cityList += cityNames[bestOrder[i]];
                    if (i != bestOrder.Length - 1)
                        cityList += ", ";
                }
                Console.WriteLine(cityList);
                queueClient.Send(new BrokeredMessage(cityList));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        static void Main(string[] args)
        {
            try
            {

                //String serviceBusNamespace = "azuretest-ns";
                //String keyname = "RootManageSharedAccessKey";
                //String key = "3HzV1a5bcs/OOj0YS+Iec7XPA9ys2s9oqrUrg8pcILA=";

                //String connectionString = @"Endpoint=sb://" +
                //       serviceBusNamespace +
                //       @".servicebus.chinacloudapi.cn/;SharedAccessKeyName=" +
                //       keyname + @";SharedSecretValue=" + key;

                String connectionString = @"Endpoint=sb://azuretest-ns.servicebus.chinacloudapi.cn/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=3HzV1a5bcs/OOj0YS+Iec7XPA9ys2s9oqrUrg8pcILA=";

                int numCities = 10; // Use as the default, if no value is specified
                // at the command line.
                if (args.Count() != 0)
                {

                    if (args[0].ToLower().CompareTo("createqueue") == 0)
                    {
                        // No processing to occur other than creating the queue.
                        namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
                        namespaceManager.CreateQueue(queueName);
                        Console.WriteLine("Queue named {0} was created.", queueName);
                        Environment.Exit(0);
                    }

                    if (args[0].ToLower().CompareTo("deletequeue") == 0)
                    {
                        // No processing to occur other than deleting the queue.
                        namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
                        namespaceManager.DeleteQueue("TSPQueue");
                        Console.WriteLine("Queue named {0} was deleted.", queueName);
                        Environment.Exit(0);
                    }

                    // Neither creating or deleting a queue.
                    // Assume the value passed in is the number of cities to solve.
                    numCities = Convert.ToInt32(args[0]);
                }

                Console.WriteLine("Running for {0} cities.", numCities);

                queueClient = QueueClient.CreateFromConnectionString(connectionString, "TSPQueue");

                List<int> startCities = new List<int>();
                List<int> restCities = new List<int>();

                startCities.Add(0);
                for (int i = 1; i < numCities; i++)
                {
                    restCities.Add(i);
                }
                distances = new double[numCities, numCities];
                cityNames = new String[numCities];
                BuildDistances(@"c:\tsp\cities.txt", numCities);
                minDistance = -1;
                bestOrder = new int[numCities];
                permutation(startCities, 0, restCities);
                Console.WriteLine("Final solution found!");
                queueClient.Send(new BrokeredMessage("Complete"));

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
