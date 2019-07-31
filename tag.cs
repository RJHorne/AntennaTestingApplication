using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack_Testing_Code
{
    public class EM4325Tag
    {
        public string Name { get; set; }

        public string X { get; set; }   // incoming string of byte data for X Axis
        public string Y { get; set; }   // incoming string of byte data for Y Axis
        public string Z { get; set; }   // incoming string of byte data for Z Axis

        public string mem1 { get; set; }   // incoming string of byte data for X Axis
        public string mem2 { get; set; }   // incoming string of byte data for Y Axis
        public string mem3 { get; set; }   // incoming string of byte data for Z Axis

        public string mem4 { get; set; }   // incoming string of byte data for X Axis
        public string mem5 { get; set; }   // incoming string of byte data for Y Axis
        public string mem6 { get; set; }   // incoming string of byte data for Z Axis

        public int mem1Int { get; set; }   // incoming string of byte data for X Axis
        public int mem2Int { get; set; }   // incoming string of byte data for Y Axis
        public int mem3Int { get; set; }   // incoming string of byte data for Z Axis

        public int mem4Int { get; set; }   // incoming string of byte data for X Axis
        public int mem5Int { get; set; }   // incoming string of byte data for Y Axis
        public int mem6Int { get; set; }   // incoming string of byte data for Z Axis

        public string Check { get; set; }
        public string CheckSum { get; set; }
        public string CheckBit { get; set; }

        public uint UserMemoryLocation = 0x104;
        public string defaultEPCHeader = "008";

        public int XInteger { get; set; }
        public int YInteger { get; set; }
        public int ZInteger { get; set; }

        public string XString { get; set; }
        public string YString { get; set; }
        public string ZString { get; set; }

        public string UserMemory { get; set; }
        public string RSSI { get; set; }
        public string Phase { get; set; }
        public string EPC { get; set; }

        public int Range { get; set; }
        public int Angle { get; set; }

        public int ReadAmount { get; set; }
        public Boolean FullRead { get; set; }
    }

    public class RFM2100
    {
        public uint sensorMemLocation = 0xB0;
        public string RSSI { get; set; }
        public string Phase { get; set; }
        public string EPC { get; set; }
        public string SensorMemory { get; set; }
        public string defaultEPC = "";
    }
}
