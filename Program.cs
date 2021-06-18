using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Report_Update_Automation
{
    class Program
    {
        // const string pathToConfReports = @"AboveCloud/src/AboveCloud.Configuration/ConfigurationEntities/TenantEntities/Reports/SystemReportVersions.csv";
        const string pathToConfReports = @"AboveCloud/src/AboveCloud.Configuration/ConfigurationEntities/TenantEntities/Reports/Correspondence/SystemCorrespondenceVersions.csv";
        const string pathToSystemReportCsv = @"SystemReportsVersions.CSV";
        const string pathToSystemCorrespondenceCsv = @"SystemCorrespondenceVersions.CSV";
        const string pathToConfigurationReportTemplates =
            @"AboveCloud/src/AboveCloud.Configuration.WebApi/Data/PredefinedFiles/ReportTemplate";

        private const string reportsConfPath =
            @"/home/pjakubanes/Documents/repos/backend-configuration/AboveCloud/src/AboveCloud.Configuration.WebApi/Data/PredefinedFiles/ReportTemplate";
        private const string reportsBinPath = @"/home/pjakubanes/Documents/repos/report-binaries/Reports";
        
        static void Main(
            string reportBinariesRepoPath = @"/home/pjakubanes/Documents/repos/report-binaries", //make app env + 1 up
            string configurationRepoPath = @"/home/pjakubanes/Documents/repos/backend-configuration")
        {
            var systemReports = CsvDeserializer.Deserialize(reportBinariesRepoPath + '/' + pathToSystemReportCsv);
            var configurationReports = CsvDeserializer.Deserialize(configurationRepoPath + '/' + pathToConfReports);   
            
            var allReports = MrtFiles.GetMrtFiles(systemReports, reportsConfPath: reportsConfPath, reportsBinPath: reportsBinPath);

            PrepareFilesForConfigurationRepo(allReports, configurationReports);
        }
        
        private static void PrepareFilesForConfigurationRepo(IEnumerable<Report> allReports, IReadOnlyCollection<SystemEntityVersion> configurationReports)
        {
            var outputDir = $"{Environment.CurrentDirectory}/out";
            var reportOutput = $"{outputDir}/ReportTemplate";
            Directory.CreateDirectory(reportOutput);

            foreach (var report in allReports.Where(x => x.IsSystemReport))
            {
                File.Copy(report.ReportBinaryFile.FullName, $"{reportOutput}/{report.ReportBinaryFile.Name}", true);
            }

            var updatedCsv = allReports
                .Where(x => x.AreReportsEqual is false && x.IsSystemReport is true)
                .Select(report => new
                {
                    report, systemEntityVersion = configurationReports.SingleOrDefault(s => s.Name == $"SYS_{report.ReportCode}")
                })
                .Select(x => new SystemEntityVersion(name: "SYS_" + (x.systemEntityVersion?.Name ?? x.report.ReportCode),
                    version: (x.systemEntityVersion?.Version + 1) ?? 1));
            CsvDeserializer.Serialize(updatedCsv.ToArray(), outputDir);
        }
        // var to = ParamsGenerator.GenerateReportParameteres(ParamsGenerator.reportParameters);

        // var rr =String.Join(Environment.NewLine, to);
        // var systemReports = CsvDeserializer.Deserialize(reportBinariesRepoPath + '/' + pathToSystemReportCsv);
        // var configurationReports = CsvDeserializer.Deserialize(configurationRepoPath + '/' + pathToConfReports);
        //
        // var differences = GetDifferences(systemReports.Select(x => new SystemEntityVersion("SYS_" + x.Name, x.Version)), configurationReports);

        // AddReportTemplates(differences, reportBinariesRepoPath, configurationRepoPath + '/' + pathToConfigurationReportTemplates);
        
        //make this strongly typed and cleanup
        private static IEnumerable<SystemEntityVersion> GetDifferences(IEnumerable<SystemEntityVersion> systemReports, IEnumerable<SystemEntityVersion> confReports)
        {
            var common = systemReports.Join(confReports,
                systemReport => systemReport.Name,
                confReport => confReport.Name,
                (systemReport, confReport) => (systemReport, confReport));

            var differingOrMissing = systemReports
                .Where(x => !common.Select(x => x.confReport).Contains(x))
                .Select(diff => (diff, confReports.SingleOrDefault(x => x.Name == diff.Name)))
                .ToArray();
            
            Console.WriteLine($@"Differing or missing reports: {String.Join(Environment.NewLine, differingOrMissing.Select(x => x.diff.Name + "system report vs configuration version: " + x.diff.Version + " vs " + x.Item2?.Version))}");

            return differingOrMissing.Select(x => x.diff);
        }

        private static void AddReportTemplates(IEnumerable<SystemEntityVersion> entities, string searchPath, string saveDirPath)
        {
            var reportFiles = Directory
                .GetFiles(searchPath + @"/Reports", "*.mrt", SearchOption.AllDirectories)
                .Select(x => new FileInfo(x));

            //some codes are XXX_000,0 or XXX_ use regex instead?
            //get files by codes XXX_000,0
            var exampleCode = "XXX_000";
            var reportsToUpdate = entities.Join(
                reportFiles,
                systemEntityVersion => systemEntityVersion.Name.Substring(4, exampleCode.Length),
                reportFile => reportFile.Name.Substring(0, exampleCode.Length),
                (systemEntityVersion, fileInfo) => fileInfo);

            foreach (var report in reportsToUpdate)
            {
                File.Copy(report.FullName,  saveDirPath + '/' + report.Name, true);
            }
        }
    }
}
