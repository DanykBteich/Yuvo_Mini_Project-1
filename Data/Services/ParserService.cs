using Microsoft.Extensions.Configuration;
using Mini_Project.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mini_Project.Data.Services
{
    public class ParserService
    {
        public IConfiguration Configuration { get; }
        private Utilities _utilities;

        public ParserService(IConfiguration configuration)
        {
            Configuration = configuration;
            _utilities = new Utilities(configuration);
        }

        public void ParseToCSVFile()
        {
            // Get values from appsettings
            string loaderPath = _utilities.GetLoaderFilesPath();
            string parsePath = _utilities.GetParserFilesPath();
            string delimiter = _utilities.GetParserDelimiter();
            string filesExtensions = _utilities.GetParserFilesExtensions();
            string parseProcessedPath = _utilities.GetParserProcessedPath();
            string dateTimeNow = DateTime.Now.ToString("MM-dd-yyyy hhmmss");

            // The Full Info about the files inside the configured folder path
            DirectoryInfo directoryInfo = new DirectoryInfo(parsePath);
            FileInfo[] files = directoryInfo.GetFiles(filesExtensions);

            foreach (FileInfo file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file.Name);

                DateTime DateTime_Key = _utilities.GetDateTimeKey(fileName);

                // skipping the header from the original file
                string[] lines = File.ReadAllLines(file.FullName);
                lines = lines.Skip(1).ToArray();
                var valuesList = new List<ParsedFile>();

                // Append each line to the list
                foreach (var line in lines)
                {
                    var values = line.Split(delimiter);
                    if (values[2] != "Unreachable Bulk FC" &&
                        values[17] == "-")
                    {
                        var ObjectParts = new ObjectParts();
                        ObjectParts = _utilities.GetObjectParts(values[2]);

                        if (ObjectParts.SLOT.Contains("+"))
                        {
                            string[] slots = ObjectParts.SLOT.Split("+");

                            foreach (string slot in slots)
                            {
                                var originalFile = new ParsedFile()
                                {
                                    Network_SID = _utilities.GetHashedValue(values[2], values[6]),
                                    DateTime_Key = DateTime_Key,
                                    NEID = float.Parse(values[1]),
                                    Object = values[2],
                                    Time = DateTime.Parse(values[3]),
                                    Interval = int.Parse(values[4]),
                                    Direction = values[5],
                                    NeAlias = values[6],
                                    NeType = values[7],
                                    RxLevelBelowTS1 = values[9],
                                    RxLevelBelowTS2 = values[10],
                                    MinRxLevel = float.Parse(values[11]),
                                    MaxRxLevel = float.Parse(values[12]),
                                    TxLevelAboveTS1 = values[13],
                                    MinTxLevel = float.Parse(values[14]),
                                    MaxTxLevel = float.Parse(values[15]),
                                    FailureDescription = values[17],
                                    LINK = ObjectParts.LINK,
                                    TID = ObjectParts.TID,
                                    FARENDTID = ObjectParts.FARENDTID,
                                    SLOT = slot,
                                    PORT = ObjectParts.PORT
                                };
                                valuesList.Add(originalFile);
                            }
                        }
                        else
                        {
                            var originalFile = new ParsedFile()
                            {
                                Network_SID = _utilities.GetHashedValue(values[2], values[6]),
                                DateTime_Key = DateTime_Key,
                                NEID = float.Parse(values[1]),
                                Object = values[2],
                                Time = DateTime.Parse(values[3]),
                                Interval = int.Parse(values[4]),
                                Direction = values[5],
                                NeAlias = values[6],
                                NeType = values[7],
                                RxLevelBelowTS1 = values[9],
                                RxLevelBelowTS2 = values[10],
                                MinRxLevel = float.Parse(values[11]),
                                MaxRxLevel = float.Parse(values[12]),
                                TxLevelAboveTS1 = values[13],
                                MinTxLevel = float.Parse(values[14]),
                                MaxTxLevel = float.Parse(values[15]),
                                FailureDescription = values[17],
                                LINK = ObjectParts.LINK,
                                TID = ObjectParts.TID,
                                FARENDTID = ObjectParts.FARENDTID,
                                SLOT = ObjectParts.SLOT,
                                PORT = ObjectParts.PORT
                            };
                            valuesList.Add(originalFile);
                        }
                    }
                }
                
                // Get if a File has been already Parsed.
                _utilities.CheckFileLog("Date_Parsed", fileName, dateTimeNow);
                // Get the Id of the Added/Existing File
                int FileLogID =_utilities.GetIDValue(fileName);

                using (StreamWriter writer = new StreamWriter($"{loaderPath}{fileName}.csv", true))
                {
                    valuesList.ForEach(
                        x =>
                        {
                            string row = $"{x.Network_SID},{x.DateTime_Key},{x.NEID},{x.Object},{x.Time},{x.Interval},{x.Direction},{x.NeAlias},{x.NeType},{x.RxLevelBelowTS1},{x.RxLevelBelowTS2},{x.MinRxLevel},{x.MaxRxLevel},{x.TxLevelAboveTS1},{x.MinTxLevel},{x.MaxTxLevel},{x.FailureDescription},{x.LINK},{x.TID},{x.FARENDTID},{x.SLOT},{x.PORT},{FileLogID}";
                            writer.WriteLine(row);
                        }
                    );
                }

                // Move the parsed files to a Processed Folder
                File.Move(file.FullName, $"{parseProcessedPath}{fileName}_{dateTimeNow}{file.Extension}");
            }
        }
    }
}