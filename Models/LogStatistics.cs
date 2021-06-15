using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveFiles.Models
{
    public class LogStatistics
    {

        private LogFile Log { get; set; }

        public LogStatistics(LogFile log)
        {
            Log = log;
        }

        public int TotalFilesTransfer => Log.ListPackets.Sum(p => p.CountFilesMoved);
       
        public decimal TotalFileSizeTransfer => Log.ListPackets.Sum(f => f.TotalFilesSizeMoved) / 1024 / 1024;


        public Dictionary<string, decimal> TotalWeightByExtensions()
        {

            List<Dictionary<string, decimal>> listDictionarys = new List<Dictionary<string, decimal>>();

            Log.ListPackets.ForEach(x => listDictionarys.Add(x.GetFilesSizeByExtension()));



            var result = listDictionarys
                .SelectMany(d => d)
                .GroupBy(
                    kvp => kvp.Key,
                    (key, kvps) => new { Key = key, Value = kvps.Sum(kvp => kvp.Value) }
                 )
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (var item in result)
            {
                Console.WriteLine(item.Key + "     ->     " + item.Value + " KB");
            }

            return result;
        }

        public Dictionary<string, decimal> TotalWeightByHour()
        {

            List<Dictionary<string, decimal>> listDictionarys = new List<Dictionary<string, decimal>>();

            Log.ListPackets.ForEach(x => listDictionarys.Add(x.GetFilesSizeByHour()));

            var result = listDictionarys
                .SelectMany(d => d)
                .GroupBy(
                    kvp => kvp.Key,
                    (key, kvps) => new { Key = key, Value = kvps.Sum(kvp => kvp.Value) }
                 )
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (var item in result)
            {
                Console.WriteLine(item.Key + "H     ->     " + item.Value + "KB");
            }

            return result;
        }

    }




}

