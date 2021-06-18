using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Report_Update_Automation
{
    public static class MrtFiles
    {
        private static readonly List<string> ExcludedFolders = new() { "Archive", "Widgets", "Customer Reports", "Dashboard" };
        private static readonly List<string> ExcludedKnownReports = new() {"SYSTEM_CHECK_REPORT.mrt"};
        
        //todo use full outer join
        //consider case when file is present in configuration and not present in report binaries
        public static IEnumerable<Report> GetMrtFiles(IReadOnlyCollection<SystemEntityVersion> csvSystemReports, string reportsConfPath, string reportsBinPath)
        {
            var configurationReports = Directory
                .GetFiles(reportsConfPath, "*.mrt", SearchOption.AllDirectories)
                .Select(x => new FileInfo(x));

            var reportFiles = Directory
                .GetFiles(reportsBinPath, "*.mrt", SearchOption.AllDirectories)
                .Where(it => !ExcludedFolders.Any(it.Contains))
                .Select(x => new FileInfo(x))
                .Where(x => !ExcludedKnownReports.Any(x.Name.Contains));

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