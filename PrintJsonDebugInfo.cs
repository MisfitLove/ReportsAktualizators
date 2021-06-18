using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Report_Update_Automation
{
    public static class PrintJsonDebugInfo
    {
        public static void PrintMrtInfo(IEnumerable<Report> allReports)
        {
            var outputDirectory = Directory.CreateDirectory(Environment.CurrentDirectory + "/out");
            File.WriteAllText(outputDirectory.FullName + $"/{nameof(allReports)}.json",   JsonConvert.SerializeObject(allReports.ToArray(), Formatting.Indented));

            var allSystemReports = allReports.Where(x => x.IsSystemReport);
            File.WriteAllText(outputDirectory.FullName + $"/{nameof(allSystemReports)}.json",  JsonConvert.SerializeObject(allSystemReports, Formatting.Indented));
            
            var reportsToCreate = allReports
                .Where(x => x.ConfigurationFile is null)
                .Select(x => x.ReportBinFileName);
            File.WriteAllText(outputDirectory.FullName + $"/{nameof(reportsToCreate)}.json",  JsonConvert.SerializeObject(reportsToCreate, Formatting.Indented));

            var reportsToUpdate = allReports
                .Where(x => x.AreReportsEqual is not true && x.ConfigurationFile is not null)
                .Select(x => x.ReportBinFileName);
            File.WriteAllText(outputDirectory.FullName + $"/{nameof(reportsToUpdate)}.json",  JsonConvert.SerializeObject(reportsToUpdate, Formatting.Indented));
            
            var missing = allReports.Where(x => x.IsSystemReport == false && x.ConfigurationFile == null)
                .Select(x => x.ReportBinaryFile.Name);
            File.WriteAllText(outputDirectory.FullName + $"/{nameof(missing)}.json",  JsonConvert.SerializeObject(missing, Formatting.Indented));
        }
    }
}