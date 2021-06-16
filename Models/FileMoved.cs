using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveFiles.Models
{
    public class FileMoved
    {
        
        public string FileName { get; set; }
        public decimal FileSize { get; set; }
        [JsonIgnore]
        public string FileExtension => string.IsNullOrEmpty(FileName) ? "Desconhecido" : FileName.Split('.')[1];
        public DateTime MovedTime { get; set; }
        [JsonIgnore]
        public int Hour => MovedTime.Hour;

    }


}
