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
        public List<FileMoved> ListFilesMoved { get; set; }
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
        [JsonIgnore]
        public decimal TotalFilesSizeMoved => ListFilesMoved.Select(f => f.FileSize).Sum();
        

        public Dictionary<string, decimal> GetFilesSizeByExtension()
        {
            var listaAgrupada = from file in ListFilesMoved
                                group file by file.FileExtension into ExtensioGrupo
                                orderby ExtensioGrupo.Key ascending
                                select ExtensioGrupo;


            Dictionary<string, decimal> FileSizeByExtension = new Dictionary<string, decimal>();

            foreach (var grupo in listaAgrupada)
            {
                var TotalBytesByExtension = grupo.Sum(f => f.FileSize);
                FileSizeByExtension.Add(grupo.Key, TotalBytesByExtension);
             
            }
           
            return FileSizeByExtension;
        }


        public Dictionary<string, decimal> GetFilesSizeByHour()
        {
            var listaAgrupada = from file in ListFilesMoved
                                group file by file.Hour into HourGrupo
                                orderby HourGrupo.Key ascending
                                select HourGrupo;


            Dictionary<string, decimal> FileSizeByHour = new Dictionary<string, decimal>();

            foreach (var grupo in listaAgrupada)
            {
                var TotalBytesByHour = grupo.Sum(f => f.FileSize);
                FileSizeByHour.Add(grupo.Key + "", TotalBytesByHour);

            }
            
            return FileSizeByHour;
        }




    }

    
}



