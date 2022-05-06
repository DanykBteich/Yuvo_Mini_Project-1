using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Mini_Project.Data.Services
{
    public class LoaderService
    {
        public IConfiguration Configuration { get; }
        private Utilities _utilities;

        public LoaderService(IConfiguration configuration)
        {
            Configuration = configuration;
            _utilities = new Utilities(configuration);
        }

        public void LoadToVertica()
        {
            // Get values from appsettings
            string filesPath = _utilities.GetLoaderFilesPath();
            string processedPath = _utilities.GetLoaderProcessedPath();
            string tableDbName = _utilities.GetLoaderDatabaseTableName();
            string delimiter = _utilities.GetLoaderDelimiter();
            string extension = _utilities.GetLoaderFilesExtensions();
            string logTableName = _utilities.GetLogTableName();
            string dateTimeNow = DateTime.Now.ToString("MM-dd-yyyy hhmmss");

            // Set the date, rejected, and exceptions files path
            string localDateStr = DateTime.Now.ToString("MM.dd.yyyy_HH.mm.ss");
            string logsPath = _utilities.GetLoaderLogsPath();

            // Create the path in case it's not created else do nothing the directories already available
            string folderRejectPath = $"{logsPath}Rejected";
            string folderExceptionsPath = $"{logsPath}Exceptions";

            _utilities.PathExist(folderRejectPath);
            _utilities.PathExist(folderExceptionsPath);

            // The Full Info about the files inside the configured folder path
            DirectoryInfo directoryInfo = new DirectoryInfo(filesPath);
            FileInfo[] files = directoryInfo.GetFiles(extension);

            foreach (FileInfo file in files)
            {

                string fileName = Path.GetFileNameWithoutExtension(file.Name);

                // Get if a File has been already Loaded.
                string queryString = $"SELECT Date_Loaded FROM {logTableName} WHERE FileName = '{fileName}'";
                string queryValue = _utilities.GetValueFromQuery(queryString);

                if (queryValue != "")
                {
                    continue;
                }

                queryString = @$"COPY {tableDbName}
                                 FROM LOCAL '{file.FullName}'
                                 DELIMITER '{delimiter}'
                                 DIRECT
                                 REJECTED DATA '{logsPath}Rejected\{fileName}_{localDateStr}.txt'
                                 EXCEPTIONS '{logsPath}Exceptions\{fileName}_{localDateStr}.txt'";

                _utilities.ExecuteQuery(queryString);

                // Add the file name into the Loaded Files Table if the file exists
                _utilities.CheckFileLog("Date_Loaded", fileName, dateTimeNow);

                // Move the processed files to a Processed Folder
                File.Move(file.FullName, $"{processedPath}{fileName}_{dateTimeNow}{file.Extension}");
            }
        }
    }
}