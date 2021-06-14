using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveFiles.Models
{
    public class LogFile
    {
        [JsonProperty]
        private List<PacketFilesMoved> ListPackets { get; set;}
        [JsonIgnore]
        public int CountPackets => ListPackets.Count;


        public LogFile()
        {
            ListPackets = new List<PacketFilesMoved>();
        }


        public void InsertPacketFilesMoved(PacketFilesMoved packet)
        {
            if(packet != null)
                ListPackets.Add(packet);
        }


        








    }
}
