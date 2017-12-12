using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Add these references:
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.Generic;
using System.Net;

namespace PSConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            RunMain();
        }

        private static void RunMain()
        {
            //Screen formating
            Console.Title = "Custom PowerShell Demo";
            Console.WriteLine("Custom PowerShell Demo");
            Console.WriteLine("Written by Justin Williams\n@fpieces\n\n\n");
            Console.WriteLine("1. Run Cmdlet\t\t2. Run Script");
            Console.WriteLine("3. Run From Web\n\n\n");
            Console.Write("Select an option to continue (x to quit):");

            //Get user selection input
            var Entry = Console.ReadLine();

            //Runs the selection logic
            if (Entry == "x")
            {
                //Exits gracefully
                Environment.Exit(0);
            }
            else
            {
                //Tests input for integer
                bool IsInt = int.TryParse(Entry, out int Selection);
                if (IsInt)
                {
                    //Option 1 runs a generic cmdlet...
                    if (Selection == 1)
                    {
                        RunCmdlet();
                    }
                    //Option 2 runs a custom script...
                    else if (Selection == 2)
                    {

                        RunScript();
                    }
                    //Option 3 runs raw script from web...
                    else if (Selection == 3)
                    {
                        RunWebRequest();
                    }
                    //Return to main funcion screen
                    Console.Clear();
                    RunMain();
                }
                else
                {
                    //Is not a valid selection, reset gracefully
                    Console.Clear();
                    Console.WriteLine("Selection is invalid\nPress any key to continue");
                    Console.ReadKey();
                    Console.Clear();
                    RunMain();
                }
            }
        }

        private static void RunCmdlet()
        {
            //Runs the cmdlet specified.
            Console.Write("CmdLet Name: ");
            var CmdletName = Console.ReadLine();
            PowerShell ps = PowerShell.Create();
            ps.AddCommand(CmdletName);

            //This section adds the parameters
            bool AddParameters = true;
            while (AddParameters)
            {
                //add parameter name
                Console.Write("Parameter Name (Enter for None): ");
                var ParameterName = Console.ReadLine();
                if (ParameterName != "")
                {
                    //add parameter value
                    Console.Write("Parameter Value: ");
                    var ParameterValue = Console.ReadLine();
                    ps.AddParameter(ParameterName, ParameterValue);
                }
                else
                {
                    //Parameter name is blank, break while...
                    AddParameters = false;
                }
            }

            Console.Clear();
            ExecutePowerShell(ps);
        }


        //takes a raw string and executes as is...
        private static void RunScript()
        {
            PowerShell ps = PowerShell.Create();
            Console.WriteLine("Enter raw PowerShell: ");
            var PowerShellText = Console.ReadLine();
            //adds the script and executes.
            ps.AddScript(PowerShellText);

            Console.Clear();
            ExecutePowerShell(ps);
        }

        //runs raw script text from the internet using Web Request class.
        private static void RunWebRequest()
        {
            //gets the web address where the raw string is hosted.
            Console.WriteLine("Enter web address for script: ");
            var Uri = Console.ReadLine();
            try
            {
                //creates the web request
                var Request = WebRequest.Create(Uri);
                var WebResponse = Request.GetResponse();
                var ResponseStream = WebResponse.GetResponseStream();
                var Reader = new System.IO.StreamReader(ResponseStream);
                var Results = Reader.ReadToEnd();

                var ps = PowerShell.Create();
                //adds the script and executes
                ps.AddScript(Results);

                Console.Clear();
                ExecutePowerShell(ps);

                Console.WriteLine(Results);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        //This is the cheese in the deep dish of this project...
        private static void ExecutePowerShell (PowerShell ps)
        {
            try
            {
                //Executes the cmdlets and returns a generic list 
                //containing the objects.
                foreach (var PowerShellObject in ps.Invoke())
                {
                    List<Result> Results = new List<Result>();
                    foreach (var Property in PowerShellObject.Properties)
                    {
                        //Script properties are throwing errors, and needs fixing.
                        //Ignoring those for now, will fix in future revision.
                        if (Property.GetType().ToString() == "System.Management.Automation.PSProperty" || Property.GetType().ToString() == "System.Management.Automation.PSNoteProperty")
                        {
                            if (Property.Value != null)
                            {
                                var r = new Result(Property.Name, Property.Value.ToString());
                                Results.Add(r);
                            }
                            else
                            {
                                var r = new Result(Property.Name, "");
                            }
                        }

                    }
                    //Created a helper function that formats the result output
                    //similar to how the format list output in PowerShell works.
                    FormatList(Results);
                    Console.WriteLine();
                }
            }
            //Exception handling
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadLine();
            }
        }

        //Writes the results to the console for list formatting.
        static void FormatList(List<Result> ResultList)
        {
            var maxLength = (ResultList.OrderByDescending(x => x.NameLength).FirstOrDefault()).NameLength;
            foreach (var item in ResultList)
            {
                var diffLength = maxLength - item.NameLength;
                Console.WriteLine(item.Name + AddSpaces(diffLength) +  " : " + item.Value);
            }
        }

        //only exists to get the spaces to format the : in the formatted list string.
        static string AddSpaces(int Spaces2Add)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Spaces2Add; i++)
            {
                sb.Append(" ");
            }
            return sb.ToString();
        }
    }
}
