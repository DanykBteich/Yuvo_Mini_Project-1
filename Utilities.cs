using Microsoft.Extensions.Configuration;
using Mini_Project.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Mini_Project
{
    public class Utilities
    {
        public IConfiguration Configuration { get; }

        public Utilities(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public Utilities()
        {
        }

        public void ExecuteQuery(string queryString)
        {
            string connetionString = Configuration.GetConnectionString("VerticaConnectionString");

            OdbcCommand command = new OdbcCommand(queryString);

            using (OdbcConnection connection = new OdbcConnection(connetionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public string GetValueFromQuery(string queryString)
        {
            string returnedValue = "";

            string connetionString = Configuration.GetConnectionString("VerticaConnectionString");

            OdbcCommand command = new OdbcCommand(queryString);

            using (OdbcConnection connection = new OdbcConnection(connetionString))
            {
                command.Connection = connection;
                connection.Open();

                OdbcCommand databaseCommand = connection.CreateCommand();
                databaseCommand.CommandText = queryString;
                OdbcDataReader dataReader = databaseCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    try
                    {
                        returnedValue = dataReader.GetString(0);
                    }
                    catch (Exception)
                    {
                        returnedValue = "";
                    }
                }
                dataReader.Close();
                connection.Close();
            }

            return returnedValue;
        }

        public List<UIValues> GetValuesFromQuery(string queryString, string aggDate)
        {
            List<UIValues> valuesFromQuery = new List<UIValues>();

            string connetionString = Configuration.GetConnectionString("VerticaConnectionString");

            OdbcCommand command = new OdbcCommand(queryString);

            using (OdbcConnection connection = new OdbcConnection(connetionString))
            {
                command.Connection = connection;
                connection.Open();

                OdbcCommand databaseCommand = connection.CreateCommand();
                databaseCommand.CommandText = queryString;
                OdbcDataReader dataReader = databaseCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    UIValues returnedValues = new UIValues()
                    {
                        Date = DateTime.Parse(dataReader[$"{aggDate}"].ToString()),
                        Link = dataReader["LINK"].ToString(),
                        Slot = dataReader["SLOT"].ToString(),
                        NeAlias = dataReader["NEALIAS"].ToString(),
                        NeType = dataReader["NETYPE"].ToString(),
                        MaxRXLevel = float.Parse(dataReader["Max_RX_Level"].ToString()),
                        MaxTXLevel = float.Parse(dataReader["Max_TX_Level"].ToString()),
                        RSLDeviation = float.Parse(dataReader["RSL_Deviation"].ToString())
                    };

                    valuesFromQuery.Add(returnedValues);
                }
            }

            return valuesFromQuery;
        }

        public void PathExist(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        public string GetLoaderFilesPath() => Configuration.GetValue<string>("LoaderCopyFrom");
        public string GetLoaderProcessedPath() => Configuration.GetValue<string>("LoaderProcessed");
        public string GetLoaderDatabaseTableName() => Configuration.GetValue<string>("TableDbName");
        public string GetLoaderDelimiter() => Configuration.GetValue<string>("LoaderDelimiter");
        public string GetLoaderFilesExtensions() => Configuration.GetValue<string>("LoaderFilesExtensions");
        public string GetLoaderLogsPath() => Configuration.GetValue<string>("LoaderLogsPath");


        public string GetParserFilesPath() => Configuration.GetValue<string>("ParserFrom");
        public string GetParserDelimiter() => Configuration.GetValue<string>("ParserDelimiter");
        public string GetParserFilesExtensions() => Configuration.GetValue<string>("ParserFilesExtensions");
        public string GetParserProcessedPath() => Configuration.GetValue<string>("ParserProcessed");

        public string GetLogTableName() => Configuration.GetValue<string>("LogTable");
        public string GetDailyTableName() => Configuration.GetValue<string>("DailyTable");
        public string GetHourlyTableName() => Configuration.GetValue<string>("HourlyTable");

        public DateTime GetDateTimeKey(string fileName)
        {
            // Get the last "_" before date and time from the file name
            string removeTime = fileName.Substring(0, fileName.LastIndexOf("_"));
            string removeDate = removeTime.Substring(0, removeTime.LastIndexOf("_"));
            int getPosition = removeDate.Length;

            // Get the substring date time based on the "_" before the last one
            string getDateTime = fileName.Substring(getPosition + 1);
            getDateTime = getDateTime.Replace("_", " ");

            // Correct the datetime format to become as the desiredFormatStr
            string fileNameFormatStr = "yyyyMMdd HHmmss";
            DateTime dateTimeKey = DateTime.ParseExact(getDateTime, fileNameFormatStr, null);
            return dateTimeKey;
        }

        public ObjectParts GetObjectParts(string Object)
        {
            var ObjectParts = new ObjectParts();

            // Get FARENDTID
            string FARENDTID = Object.Substring(Object.LastIndexOf("_") + 1);
            ObjectParts.FARENDTID = FARENDTID;

            string newObject = Object.Substring(0, Object.LastIndexOf("__"));

            // Get TID
            string TID = newObject.Substring(newObject.LastIndexOf("_") + 1);
            ObjectParts.TID = TID;

            newObject = newObject.Substring(0, newObject.LastIndexOf("__"));

            if (newObject.Contains("."))
            {
                newObject = newObject.Substring(newObject.IndexOf("/") + 1);
                newObject = newObject.Substring(0, newObject.IndexOf("/"));
                string[] SlotPort = newObject.Split(".");
                ObjectParts.SLOT = SlotPort[0];
                ObjectParts.PORT = SlotPort[1];
                ObjectParts.LINK = $"{ObjectParts.SLOT}/{ObjectParts.PORT}";
            }
            else
            {
                // Get LINK
                string LINK = newObject.Substring(newObject.IndexOf("/") + 1);
                ObjectParts.LINK = LINK;

                // Get PORT
                string PORT = newObject.Substring(newObject.LastIndexOf("/") + 1);
                ObjectParts.PORT = PORT;

                newObject = newObject.Substring(0, newObject.LastIndexOf("/"));

                // Get SLOT
                string SLOT = newObject.Substring(newObject.LastIndexOf("/") + 1);
                ObjectParts.SLOT = SLOT;
            }
            return ObjectParts;
        }

        public int GetHashedValue(string Object, string NeAlias)
        {
            string valueToBeHashed = $"{Object}{NeAlias}";
            int hashedValue = 0;
            //int hashedValue = valueToBeHashed.GetHashCode();
            using (var MD5Hash = MD5.Create())
            {
                var sourceBytes = Encoding.UTF8.GetBytes(valueToBeHashed);

                var hashBytes = MD5Hash.ComputeHash(sourceBytes);

                int hash = BitConverter.ToInt32(hashBytes);
                hashedValue = hash;
            }

            return hashedValue;
        }

        public void CheckFileLog(string colName, string fileName, string dateTimeNow)
        {
            string logTableName = GetLogTableName();

            string queryStringCheck = $"SELECT FileName FROM {logTableName} WHERE FileName = '{fileName}'";
            string dateValue = GetValueFromQuery(queryStringCheck);

            if (dateValue == "")
            {
                // Add the file name and the Date Time
                string insertFile = $"INSERT INTO {logTableName} (FileName, {colName}) VALUES ('{fileName}', '{dateTimeNow}')";
                ExecuteQuery(insertFile);
            }
            else
            {
                string updateDate = $"UPDATE {logTableName} SET {colName} = '{dateTimeNow}' WHERE FileName = '{fileName}'";
                ExecuteQuery(updateDate);
            }
        }

        public int GetIDValue(string fileName)
        {
            string logTableName = GetLogTableName();

            string queryIDString = $"SELECT FileID FROM {logTableName} WHERE FileName = '{fileName}'";

            string idValue = GetValueFromQuery(queryIDString);

            int ID = int.Parse(idValue);
            
            return ID;
        }
    }
}