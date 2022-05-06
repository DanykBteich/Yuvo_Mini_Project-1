# Yuvo_Mini_Project-1
Mini Project Requested as part of Yuvo Bootcamp
First you have to create Four Folders:
 - Load_From
 - Load_Processed
 - Parse_From
 - Parse_Processed


Inside the Visual Studio Solution 
Navigate to the "appsettings.json" file, and change the following accordingly:
 - VerticaConnectionString	=> (Insert the corresponding connection string to link the project to your database)
 - LoaderCopyFrom 			    => (Change it to be the path of the folder "Load_From" created in the above step)
 - LoaderProcessed			    => (Change it to be the path of the folder "Load_Processed" created in the above step)
 - LoaderLogsPath			      => (Change it to be the path of your choice)
 - ParserFrom				        => (Change it to be the path of the folder "Parse_From" created in the above step)
 - ParserProcessed			    => (Change it to be the path of the folder "Parse_Processed" created in the above step)
 
 - LoaderDelimiter and ParserDelimiter, are the delimiter values of the files respectfully
 - LoaderFilesExtensions and ParserFilesExtensions, are the extensions for your files respectfully


Second you must Create the Database Tables required for the project:

Table 1:
--------
CREATE TABLE TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER 
( 
    NETWORK_SID           INT, 
    DATETIME_KEY          DATETIME, 
    NEID                  FLOAT, 
    OBJECT_VAL            VARCHAR(40), 
    TIME_VAL              DATETIME, 
    INTERVAL_VAL          INT, 
    DIRECTION             VARCHAR(20), 
    NEALIAS               VARCHAR(40), 
    NETYPE                VARCHAR(40), 
    RXLEVELBELOWTS1       INT, 
    RXLEVELBELOWTS2       INT, 
    MINRXLEVEL            FLOAT, 
    MAXRXLEVEL            FLOAT, 
    TXLEVELABOVETS1       INT, 
    MINTXLEVEL            FLOAT, 
    MAXTXLEVEL            FLOAT, 
    FAILUREDESCRIPTION    VARCHAR(50), 
    LINK                  VARCHAR(20), 
    TID                   VARCHAR(10), 
    FARENDTID             VARCHAR(10), 
    SLOT                  INT, 
    PORT                  INT,
    FileLogID             INT
);

Table 2:
--------
CREATE TABLE TRANS_MW_AGG_SLOT_HOURLY  
( 
	NETWORK_SID  	INT, 
	DATE_HOURLY   	DATETIME, 
	LINK         	VARCHAR(20),  
	SLOT         	INT, 
	NEALIAS      	VARCHAR(40), 
	NETYPE       	VARCHAR(40), 
	Max_RX_Level 	FLOAT, 
	Max_TX_Level 	FLOAT, 
	RSL_Deviation	FLOAT,
        FileLogID       INT
);

Table 3:
--------
CREATE TABLE TRANS_MW_AGG_SLOT_DAILY  
( 
	NETWORK_SID  	INT, 
	DATE_DAILY   	DATETIME, 
	LINK         	VARCHAR(20),  
	SLOT         	INT, 
	NEALIAS      	VARCHAR(40), 
	NETYPE       	VARCHAR(40), 
	Max_RX_Level 	FLOAT, 
	Max_TX_Level 	FLOAT, 
	RSL_Deviation	FLOAT,
        FileLogID       INT
);

Table 4:
--------
CREATE TABLE FILESLOG ( 
    FileID             AUTO_INCREMENT,
    FileName           VARCHAR(100), 
    Date_Parsed        DATETIME, 
    Date_Loaded        DATETIME, 
    Date_Aggregated    DATETIME
)
