using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveFiles.Models
{
    public class PacketFilesMoved
    {
        [JsonProperty]
        private List<FileMoved> ListFilesMoved { get; set; }
        [JsonProperty]
        private string Origin { get; set;}
        [JsonProperty]
        private string Destination { get; set;}

        public PacketFilesMoved(string origin, string destination)
        {
            Origin = origin;
            Destination = destination;
            ListFilesMoved = new List<FileMoved>();
        }


        public void InsertFileMove(FileMoved fileMoved)
        {
            if(fileMoved != null)
                ListFilesMoved.Add(fileMoved);
        }

        [JsonIgnore]
        public int CountFilesMoved => ListFilesMoved.Count;

    }
}
