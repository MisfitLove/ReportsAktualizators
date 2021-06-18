using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace Report_Update_Automation
{
    public static class CsvDeserializer
    {
        public static IReadOnlyCollection<SystemEntityVersion> Deserialize(string path)
        {
            using var csvParser = new TextFieldParser(path);
            csvParser.SetDelimiters(",");

            var list = new List<SystemEntityVersion>();
            while (!csvParser.EndOfData)
            {
                string[] fields = csvParser.ReadFields();
                list.Add(new SystemEntityVersion(name: fields[0], version: Int32.Parse(fields[1])));
            }

            return list;
        }
        
        public static void Serialize(IReadOnlyCollection<SystemEntityVersion> toSerialize, string path)
        {
            var toWrite = string.Join(Environment.NewLine, toSerialize.Select(x => $"{x.Name},{x.Version}"));
            File.WriteAllText($"{path}/SystemReportsVersions.csv", toWrite);
        }
    }
}