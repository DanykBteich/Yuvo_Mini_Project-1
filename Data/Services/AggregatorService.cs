using Microsoft.Extensions.Configuration;
using System;

namespace Mini_Project.Data.Services
{
    public class AggregatorService
    {
        public IConfiguration Configuration { get; }
        private Utilities _utilities;

        public AggregatorService(IConfiguration configuration)
        {
            Configuration = configuration;
            _utilities = new Utilities(configuration);
        }

        public void AggregateData(string fileName)
        {
            string dateTimeNow = DateTime.Now.ToString();

            string dailyAggregationQuery = @$"INSERT INTO TRANS_MW_AGG_SLOT_DAILY 
                                                (   DATE_DAILY,
                                                    LINK,
                                                    SLOT,
                                                    NEALIAS,
                                                    NETYPE,
                                                    MAX_RX_LEVEL,
                                                    MAX_TX_LEVEL,
                                                    RSL_DEVIATION,
                                                    FileLogID)
                                            SELECT  DATE_TRUNC('DAY', TIME_VAL), LINK, SLOT, NEALIAS, NETYPE,
                                                    MAX(MAXRXLEVEL) AS ""Max_RX_Level"", 
                                                    MAX(MAXTXLEVEL) AS ""Max_TX_Level"", 
                                                    ABS(""Max_RX_Level"") - ABS(""Max_TX_Level"") AS ""RSL_Deviation"",
                                                    FileLogID
                                            FROM TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER
                                            WHERE FileLogID NOT IN(
                                                    SELECT FileLogID FROM TRANS_MW_AGG_SLOT_DAILY)
                                            GROUP BY 1, 2, 3, 4, 5, FileLogID; ";

            string hourlyAggregationQuery = @$"INSERT INTO TRANS_MW_AGG_SLOT_HOURLY 
                                                (   DATE_HOURLY,
                                                    LINK,
                                                    SLOT,
                                                    NEALIAS,
                                                    NETYPE,
                                                    MAX_RX_LEVEL,
                                                    MAX_TX_LEVEL,
                                                    RSL_DEVIATION,
                                                    FileLogID)
                                            SELECT  DATE_TRUNC('HOUR', TIME_VAL), LINK, SLOT, NEALIAS, NETYPE,
                                                    MAX(MAXRXLEVEL) AS ""Max_RX_Level"", 
                                                    MAX(MAXTXLEVEL) AS ""Max_TX_Level"", 
                                                    ABS(""Max_RX_Level"") - ABS(""Max_TX_Level"") AS ""RSL_Deviation"",
                                                    FileLogID
                                            FROM TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER
                                            WHERE FileLogID NOT IN(
                                                    SELECT FileLogID FROM TRANS_MW_AGG_SLOT_HOURLY)
                                            GROUP BY 1, 2, 3, 4, 5, FileLogID; ";

            _utilities.ExecuteQuery(dailyAggregationQuery);
            _utilities.ExecuteQuery(hourlyAggregationQuery);

            fileName = fileName.Substring(0, fileName.LastIndexOf("_"));
            _utilities.CheckFileLog("Date_Aggregated", fileName, dateTimeNow);
        }
    }
}
