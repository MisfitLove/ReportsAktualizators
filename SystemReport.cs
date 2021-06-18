using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Report_Update_Automation
{
    //todo make immutable
    public class Report
    {
        [JsonIgnore]
        public FileInfo ReportBinaryFile { get; set; }

        public string ReportBinFileName { get; set; }
        [JsonIgnore]
        public FileInfo ConfigurationFile { get; set; }

        public string ConfigurationFileName { get; set; }
        public string ReportCode { get; set; }
        public bool IsSystemReport { get; set; }
        public bool AreReportsEqual
        {
            get => AreFileContentsEqual(ReportBinaryFile, ConfigurationFile);
        }

        public Report()
        {
            
        }
        
        public static bool AreFileContentsEqual(FileInfo reportBinaryFile, FileInfo configurationFile)
        {
            if (configurationFile is null)
            {
                return false;
            }
            
            return reportBinaryFile.Length == configurationFile.Length &&
                   (reportBinaryFile.Length == 0 || File.ReadAllBytes(reportBinaryFile.FullName).SequenceEqual(
                       File.ReadAllBytes(configurationFile.FullName)));
        }
    }
}