using Microsoft.Extensions.Configuration;
using Mini_Project.Data.Models;
using System;
using System.Collections.Generic;

namespace Mini_Project.Data.Services
{
    public class UIValuesService
    {
        public IConfiguration Configuration { get; }
        private Utilities _utilities;

        public UIValuesService(IConfiguration configuration)
        {
            Configuration = configuration;
            _utilities = new Utilities(configuration);
        }

        public List<UIValues> GetData(string aggType, string dateTimeFrom, string datetimeTo)
        {
            List<UIValues> dailyValues = new List<UIValues>();
            string queryTable = "";
            string aggDate = "";
            string query = "";

            // Make sure the value entered in Either Daily or Hourly Only
            if (aggType == "daily")
            {
                queryTable = _utilities.GetDailyTableName();
                aggDate = "DATE_DAILY";
            }
            else if (aggType == "hourly")
            {
                queryTable = _utilities.GetHourlyTableName();
                aggDate = "DATE_HOURLY";
            }

            if (dateTimeFrom != null && datetimeTo != null)
            {
                query = $"SELECT * FROM {queryTable} WHERE {aggDate} BETWEEN '{dateTimeFrom}' AND '{datetimeTo}'";
            }

            if (query != "" && queryTable != "")
            {
                dailyValues = _utilities.GetValuesFromQuery(query, aggDate);
            }

            return dailyValues;
        }
    }
}
