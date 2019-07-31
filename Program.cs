using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThingMagic;

namespace Jack_Testing_Code
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Clear();
            //Console.ForegroundColor = ConsoleColor.Blue;
            String start = Init;
            if (start == "1")
            {
                jackAngleTesting instance = new jackAngleTesting();
                int dur = Int32.Parse(grabInput("Please enter duration in ms:  "));
                String comPort = GrabCom("Please enter com port (e.g. com9) :  ");
                String checkBitEnable = grabCheckEnable("Do you want to enable the check bit analysis ? (Y/N)  "); // edit this out, as enbaled should be default

                for (; ; )
                {
                    int readerAngle = Int32.Parse(grabInput("What is the reader angle (e.g. 50) "));
                    int readerRange = Int32.Parse(grabInput("What is the reader range (e.g. 50) "));
                    instance.Go(comPort, dur, checkBitEnable, readerRange, readerAngle);

                }
            }
        }


        public static string Init
        {
            get
            {
                Console.Title = "Testing Software";
              
                Console.WriteLine("-------- SYSTEM BENCHMARK TOOL --------\n");
                Console.WriteLine("Author : R HORNE");
                Console.WriteLine("Version : 1.0");
                Console.WriteLine("");
                Console.WriteLine("Please select mode");
                Console.WriteLine(" ");
                Console.WriteLine("1 for default");
                //Console.WriteLine("2) Settings ");
                //Console.WriteLine("3) Generic Read Scan");
                //Console.WriteLine("4) Generic Write to Tag");
                //Console.WriteLine("5) Free Run Analysis\n");
                Console.Write("Your Choice : ");
                String inputInit = Console.ReadLine();
                return inputInit;
            }
        }
        public static String grabInput(String userRequest)
        {
            /*
            * Function to grab user input based on a text request.
            */
            Console.Write(userRequest);
            String inputInit = Console.ReadLine();
            return inputInit;
        }

        public static String grabCheckEnable(String userEnbale)
        {
            Console.Write(userEnbale);
            String userEnableString = Console.ReadLine();
            userEnableString.ToUpper();
            return userEnableString;
        }

        public static string GrabCom(string userRequest)
        {

            /*
            * Function to grab the serial port that the user desires and package it in the way the thingmagic library wants. 
            */
            Console.Write(userRequest);
            String inputInit = Console.ReadLine();
            inputInit = "tmr:///" + inputInit;
            return inputInit;
        }


            private static void r_ReadException(object sender, ReaderExceptionEventArgs e)
        {
            Console.WriteLine("Error: " + e.ReaderException.Message);
        }

        #region ParseAntennaList

        private static int[] ParseAntennaList(IList<string> args, int argPosition)
        {
            int[] antennaList = null;
            try
            {
                string str = args[argPosition + 1];
                antennaList = Array.ConvertAll<string, int>(str.Split(','), int.Parse);
                if (antennaList.Length == 0)
                {
                    antennaList = null;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Missing argument after args[{0:d}] \"{1}\"", argPosition, args[argPosition]);

            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}\"{1}\"", ex.Message, args[argPosition + 1]);

            }
            return antennaList;
        }

        #endregion

    }



}
