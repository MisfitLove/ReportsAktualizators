using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Report_Update_Automation
{
    public static class MrtFiles
    {
        //todo use full outer join
        //consider case when file is present in configuration and not present in report binaries
        public static IEnumerable<Report> GetMrtFiles(IReadOnlyCollection<SystemEntityVersion> csvSystemReports, string reportsConfPath, string reportsBinPath)
        {
            var configurationReports = Directory
                .GetFiles(reportsConfPath, "*.mrt", SearchOption.AllDirectories)
                .Select(x => new FileInfo(x));
            
            var reportFiles = Directory
                .GetFiles(reportsBinPath, "*.mrt", SearchOption.AllDirectories)
                .Select(x => new FileInfo(x));

            var allReports = reportFiles
                .Select(binReport => new Report()
                {
                    ReportBinaryFile = binReport,
                    ReportBinFileName = binReport.Name,
                    ConfigurationFile = configurationReports.SingleOrDefault(confReport => confReport.Name == binReport.Name),
                    ConfigurationFileName = configurationReports.SingleOrDefault(confReport => confReport.Name == binReport.Name)?.Name,
                    ReportCode = binReport.Name.Substring(0, 7),
                    IsSystemReport = csvSystemReports.Select(x => x.Name).Contains(binReport.Name.Substring(0, 7))
                });
            
            PrintJsonDebugInfo.PrintMrtInfo(allReports);
            return allReports;
        }
    }
}