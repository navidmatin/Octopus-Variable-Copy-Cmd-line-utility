using OctopusDeployVariableCopy.BL_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OctopusDeployVariableCopy
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var octopus = ConnectToTheOctopusServer(args);
                while (true)
                {
                    var allLibVarSets = octopus.GetAllLibraryVariableSetsAvaialable();
                    string userInput1, newVarName, numberOfCopies;
                    int option = 0, numOfCopies = 1;
                    bool keepScope = true;

                    for (int i = 1; i <= allLibVarSets.Count; i++)
                    {
                        Console.WriteLine($"{i}-{allLibVarSets.ElementAt(i - 1).Key}");
                    }
                    //Get the libvarset name
                    Console.WriteLine("Please enter the line number of variable set that you want to copy:");
                    userInput1 = Console.ReadLine();
                    if (!Int32.TryParse(userInput1, out option) || option > allLibVarSets.Count)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Bad Input, press enter to try again!");
                        Console.ResetColor();
                        if (Console.ReadKey().Key == ConsoleKey.Enter)
                            continue;
                        else
                            break;
                    }
                    //Get the new name
                    Console.Write("New Variable Set Name:");
                    Console.ForegroundColor = ConsoleColor.Green;
                    SendKeys.SendWait(allLibVarSets.ElementAt(option - 1).Key);
                    newVarName = Console.ReadLine();
                    Console.ResetColor();

                    //Get number of copies
                    Console.WriteLine("Number of copies:");
                    Console.ForegroundColor = ConsoleColor.Green;
                    SendKeys.SendWait(1.ToString());
                    numberOfCopies = Console.ReadLine();
                    if (!Int32.TryParse(numberOfCopies, out numOfCopies) || numOfCopies < 1 || numOfCopies > 10)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Bad Input, please enter a number between 1-10, press enter to try again!");
                        Console.ResetColor();
                        if (Console.ReadKey().Key == ConsoleKey.Enter)
                            continue;
                        else
                            break;
                    }
                    Console.ResetColor();

                    //Keep the scopes?
                    Console.WriteLine("Would you like to keep the scope of each variable?(y/n)");
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.Y:
                            keepScope = true;
                            break;
                        case ConsoleKey.N:
                            keepScope = false;
                            break;
                    }
                    Console.WriteLine("Copying....");
                    octopus.CopyVariableSet(allLibVarSets.ElementAt(option - 1).Key, newVarName, numOfCopies, keepScope);
                    Console.Clear();
                    Console.WriteLine("Copying was successful!");
                    Console.WriteLine("Press enter to copy anotehr variable, or any other key to exit!");
                    if (Console.ReadKey().Key != ConsoleKey.Enter)
                    {
                        break;
                    }

                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"There was a problem processing your request:\n {e.Message}");
                Console.ResetColor();
            }

        }

        static OctopusHandler ConnectToTheOctopusServer(string[] args)
        {
            try
            {
                string server;
                string apiKey;
                if (args.Length == 2)
                {
                    server = MakeURL(args[0]);
                    apiKey = args[1];
                }
                else
                {
                    Console.WriteLine("Please enter server address:");
                    server = MakeURL(Console.ReadLine());
                    Console.WriteLine("Please enter the api key: (http://docs.octopusdeploy.com/display/OD/How+to+create+an+API+key)");
                    apiKey = Console.ReadLine();
                }
                return new OctopusHandler(server, apiKey);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to connect to server!! Exception:{e.Message}", e);
            }

        }
        private static string MakeURL(string url)
        {
            Uri server;
            if (!Uri.TryCreate(url, UriKind.Absolute, out server))
            {
                //If failed try to add http in front of it
                if (!Uri.TryCreate($"http://{url}", UriKind.Absolute, out server))
                {
                    Console.WriteLine("Bad address!");
                    Console.ReadKey();
                    throw new Exception("Bad Server Address");
                }
                else
                    return server.AbsoluteUri;
            } else
            {
                return server.AbsoluteUri;
            }
        }
    }
}
