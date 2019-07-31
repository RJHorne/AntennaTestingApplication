using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;
using ThingMagic;

// Tool for jack to test the performance of antennas at specific angles from an M6E reader antenna. 

namespace Jack_Testing_Code
{
    class jackAngleTesting
    {
        public void Go(string comPort, int duration, string checkBitEnable, int rangeFromReader, int angleFromReader)
        {

            Thread.Sleep(1000);
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("*************** Jack Mode ENGAGED ****************");
            Thread.Sleep(1000);


            Console.Write("Establishing connection");
            for (int i = 0; i < 30; i++)
            {
                Console.Write(".");
                i++;
                Thread.Sleep(100);

            }
            Console.WriteLine(".");




            EM4325Tag tag = new EM4325Tag
            {
                Range = rangeFromReader,
                Angle = angleFromReader,
                Check = checkBitEnable,
                FullRead = false
            };
            int[] antennaList = null;
            double maxTick = 0;
            double minTick = 100;
            double tBuff = 0;
            float goodFrame = 0;
            float badFrame = 0;
            float frameRecieved = 0;
            TagFilter filter0;
            var csv = new StringBuilder();
            csv.Capacity = 1000000; // Need a nice large sized CSV file. 
            int testDuration = duration;
            DateTime now = DateTime.Now;

            filter0 = new TagData(tag.defaultEPCHeader);
            Reader r = Reader.Create(comPort);



            try
            {
                r.Connect();
                Console.WriteLine("Connection Successful");
                //csv.AppendLine("Time" + "," + "X" + "," + "Y" + "," + "Z");
                TagOp op = new Gen2.ReadData(Gen2.Bank.USER, tag.UserMemoryLocation, 3);

                if (checkBitEnable == "Y")
                {
                    op = new Gen2.ReadData(Gen2.Bank.USER, tag.UserMemoryLocation, 4);
                }

                if (tag.FullRead)
                {
                    op = new Gen2.ReadData(Gen2.Bank.USER, tag.UserMemoryLocation, 6);
                }

                SimpleReadPlan plan = new SimpleReadPlan(antennaList, TagProtocol.GEN2, filter0, op, true, 150);
                Gen2.BAPParameters bap = (Gen2.BAPParameters)r.ParamGet("/reader/gen2/bap");
                bap.FREQUENCYHOPOFFTIME = 20;
                bap.POWERUPDELAY = 3;
                // r.ParamSet("/reader/gen2/Q", new Gen2.StaticQ(2));
                r.ParamSet("/reader/gen2/blf", Gen2.LinkFrequency.LINK640KHZ); // this is required to get the desired speed. 
                r.ParamSet("/reader/read/asyncOnTime", testDuration);
                r.ParamSet("/reader/read/asyncOffTime", 5);
                r.ParamSet("/reader/read/plan", plan);
                int checkInt1 = 0;
                int buffcheck = 0;
                double tBuffstart = 0;
                // Create and add tag listeners
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                r.TagRead += delegate (Object sender, TagReadDataEventArgs e)
                {
                    TimeSpan tsB = stopWatch.Elapsed;
                    tBuffstart = tsB.TotalMilliseconds;
                    double tempV = tBuffstart - tBuff;
                    tag.RSSI = e.TagReadData.Rssi.ToString();
                    tag.UserMemory = ByteFormat.ToHex(e.TagReadData.Data).ToString();  // convert the input into a hex string
                    tag.EPC = e.TagReadData.EpcString;
                    tag.UserMemory.Trim();
                    int length = 4;
                    int substringposition = 2; // need to offset two, due to return hex eg 0x2000          


                    if (tag.EPC == tag.defaultEPCHeader)
                    {
                        //SimpleReadPlan planA = new SimpleReadPlan(antennaList, TagProtocol.GEN2, filter1, op, true, 50);
                        while (substringposition < 3)  // split the string into small chunks per value and convert to INT
                        {
                            tag.mem1 = tag.UserMemory.Substring(substringposition, length);
                            tag.mem1Int = Convert.ToInt32(tag.mem1, 16);
                            substringposition = substringposition + length;
                            tag.XString = tag.XInteger.ToString();

                            tag.X = tag.UserMemory.Substring(substringposition, length);
                            tag.YInteger = Convert.ToInt32(tag.Y, 16);
                            substringposition = substringposition + length;
                            tag.YString = tag.YInteger.ToString();

                            tag.Y = tag.UserMemory.Substring(substringposition, length);
                            tag.ZInteger = Convert.ToInt32(tag.Z, 16);
                            substringposition = substringposition + length;
                            tag.ZString = tag.ZInteger.ToString();
                            if (checkBitEnable == "Y")
                            {
                                tag.Check = tag.UserMemory.Substring(substringposition, length);
                                checkInt1 = Convert.ToInt32(tag.Check, 16);
                                substringposition = substringposition + length;
                                buffcheck = tag.XInteger + tag.YInteger + tag.ZInteger;
                            }
                        }
                    }
                    if (checkInt1 != buffcheck && checkBitEnable == "Y") // Essentially looking to see if the total sum is correct, if not report a bad frame. 
                    {
                        badFrame++;
                        frameRecieved++;
                    }
                    if (checkInt1 == buffcheck || checkBitEnable == "N")
                    {
                        goodFrame++;
                        tsB = stopWatch.Elapsed;
                        tBuff = tsB.TotalMilliseconds;
                        if (tempV > 0.0002)
                        {
                            if (tempV > maxTick)
                            {
                                maxTick = tempV;
                            }
                            if (tempV < minTick)
                            {
                                minTick = tempV;
                            }
                        }
                        frameRecieved++;
                    }
                };
                Console.WriteLine("");
                r.StartReading();
                Thread.Sleep(testDuration);

                // File.WriteAllText("C:\\dmp\\testingCrap.csv", csv.ToString());

                float ratio = (badFrame / frameRecieved) * 100;
                float ptime = testDuration / frameRecieved;
                float accelHZ = (goodFrame / testDuration) * 1000;
                Console.WriteLine("------------------------------------------");
                Console.WriteLine("Summary of stats");
                Console.WriteLine("Test conducted " + now.TimeOfDay);
                Console.WriteLine("EPC : " + tag.EPC);
                Console.WriteLine("------------------------------------------");
                Console.WriteLine("Test Ran for : " + testDuration + " ms ");
                Console.WriteLine("Number of Packets recieved : " + frameRecieved);
                Console.WriteLine("CORRUPT Packets : " + badFrame);
                Console.WriteLine("GOOD Packets : " + goodFrame);
                Console.WriteLine("Effective Packet loss = " + ratio + " %");
                Console.WriteLine("Estimated Packet time : " + ptime + " ms");
                Console.WriteLine("Min Packet time : " + minTick + " ms");
                Console.WriteLine("Max Packet time : " + maxTick + " ms");
                Console.WriteLine("Effective Accel Update rate = " + accelHZ + " Hz");
                Console.WriteLine("Reader Range " + tag.Range);
                Console.WriteLine("Reader Angle " + tag.Angle);

                csv.AppendLine("------------------------------------------");
                csv.AppendLine("Summary of stats");
                csv.AppendLine("Test conducted " + now.TimeOfDay);
                csv.AppendLine("EPC : " + tag.EPC);
                csv.AppendLine("------------------------------------------");
                csv.AppendLine("Test Ran for : " + testDuration + " ms ");
                csv.AppendLine("Number of Packets recieved : " + frameRecieved);
                csv.AppendLine("CORRUPT Packets : " + badFrame);
                csv.AppendLine("GOOD Packets : " + goodFrame);
                csv.AppendLine("Effective Packet loss = " + ratio + " %");
                csv.AppendLine("Estimated Packet time : " + ptime + " ms");
                csv.AppendLine("Min Packet time : " + minTick + " ms");
                csv.AppendLine("Max Packet time : " + maxTick + " ms");
                csv.AppendLine("Effective Accel Update rate = " + accelHZ + " Hz");
                csv.AppendLine("Reader Range " + tag.Range);
                csv.AppendLine("Reader Angle " + tag.Angle);


                r.Dispose();
                Console.WriteLine("");
                Console.WriteLine("");
                string filename = DateTime.Now.ToString("yyyy-MM-d--HH-mm-ss--Angle--" + angleFromReader);
                string dateNow = @"C:\temp\" + filename + ".csv"; // The program will log data to a file with a correct Timestamp.
                try
                {

                    File.WriteAllText(dateNow, csv.ToString());
                }
                catch (ArgumentException e)

                {

                    //  Console.WriteLine("Can't write file.");
                    Console.WriteLine("{0}: {1}", e.GetType().Name, e.Message);
                }


                Console.WriteLine("Preparing for next run");
                for (int i = 0; i < 30; i++)
                {
                    Console.Write(".");
                    i++;
                    Thread.Sleep(100);

                }
                Console.WriteLine(".");
                stopWatch.Stop();
                Thread.Sleep(2500);
                Console.Clear();
                Console.WriteLine("");
            }
            catch
            {
                Console.WriteLine("Connection Failed.");
                Console.WriteLine("Please retry (press any key or crtl-c to exit)");
                if (Console.ReadKey() != null)
                {

                }


            }

        }
    }
}
