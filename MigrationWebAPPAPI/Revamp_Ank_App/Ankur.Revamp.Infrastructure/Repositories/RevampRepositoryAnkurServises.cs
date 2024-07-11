
using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Revamp_Ank_App.Client.ExcelMigration;
using Revamp_Ank_App.Domain.Repositores.Entites;
using Revamp_Ank_App.DomainEntites.Repositores.Entites;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using ThirdParty.Json.LitJson;

namespace Revamp_Ank_App.Ankur.Revamp.Infrastructure.Repositories
{
    public class RevampRepositoryAnkurServises : ISQLconnecterAnkur, IExeclSheetReader
    {
        public IMongoCollection<RevampMongoDataModel> RevampCollection { get; set; }
        private readonly SQLConnetter _sqlConnetter;
        private readonly ILogger<RevampRepositoryAnkurServises> _logger;
        public RevampRepositoryAnkurServises(IOptions<MongoScoket> connect, SQLConnetter sql, ILogger<RevampRepositoryAnkurServises> logger)
        {
            _sqlConnetter = sql;
            MongoClient client = new MongoClient(connect.Value.ConnectionString);
            IMongoDatabase database = client.GetDatabase(connect.Value.DatabaseName);
            RevampCollection = database.GetCollection<RevampMongoDataModel>(connect.Value.CollectionName);
            _logger = logger;
        }

        /// <summary>
        /// ALp
        /// </summary>
        /// <param name="storeProcedureName"></param>
        /// <param name="CycleId"></param>
        /// <param name="rdids"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> FetchAllSQLBatchProceesData(string storeProcedureName, int CycleId, string rdids)
        {


            var res = await _sqlConnetter.FetchAllSQLBatchDataAsyn(_sqlConnetter._connectionString, storeProcedureName, CycleId, rdids);
            return res;

        }



        /// <summary>
        /// Asynchronously fetches all SQL data for multi-cycle processing using the specified store procedure name, cycle ID, and RD IDs.
        /// </summary>
        /// <param name="storeProcedureName">The name of the store procedure to execute.</param>
        /// <param name="CycleId">The cycle ID to pass as a parameter to the store procedure.</param>
        /// <param name="rdids">The RD IDs to pass as a parameter to the store procedure.</param>
        /// <returns>An asynchronous enumerable of strings representing the fetched SQL data.</returns>

        public async Task<IEnumerable<string>> FetchAllSQLkafkaProcessing(string storeProcedureName, int CycleId, string rdids)
        {
            var multiCycleStreaming = _sqlConnetter.FetchAllSQLMultiBatchProcewsData(_sqlConnetter._connectionString, storeProcedureName, CycleId);
            return multiCycleStreaming;
        }






        /// <summary>
        ///  @ankur                                  
        /// </summary>
        /// <param name="storeProcedureName"></param>
        /// <param name="CycleId"></param>
        /// <param name="rdids"></param>
        /// <returns></returns>

        public async Task<bool> CreateData_Using_SQL_SP_ConnectorAsync(string storeProcedureName, int CycleId, string rdids)
        {
            int batchCount = 0;
            bool IsInseretd = false;
            var pushTOMongoStopwatch = Stopwatch.StartNew();


            // if (CycleId == 3081&& rdids == "")  
            // {
            var collection = RevampCollection;
            var indexKeysDefintionBuilder = Builders<RevampMongoDataModel>.IndexKeys;
            var indexModelList = new List<CreateIndexModel<RevampMongoDataModel>>()
            {
                new CreateIndexModel<RevampMongoDataModel>(indexKeysDefintionBuilder.Ascending(x=>x.ReportDataEntityId)),
                new CreateIndexModel<RevampMongoDataModel>(indexKeysDefintionBuilder.Ascending(x=>x.ReportDataHeaderId)),

            };
            await collection.Indexes.CreateManyAsync(indexModelList);

            var sqlDataTabeleStreaming = await FetchAllSQLkafkaProcessing(storeProcedureName, CycleId, rdids);
            var kafkaConnector = Stopwatch.StartNew();
            foreach (var jsonData in sqlDataTabeleStreaming)
            {

                var stream_Documents = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<RevampMongoDataModel>>(jsonData, new DateTimeToMillisecondConverter());
                //Newtonsoft.Json.JsonConvert.DeserializeObject<RevampMongoDataModel>(jsonData, new DateTimeToMillisecondConverter());

                if (stream_Documents.Any())
                {

                    RevampCollection.InsertManyAsync(stream_Documents);
                    IsInseretd = true;
                }

            }
            _logger.LogInformation
                 ("Total data push to MongoDB operation took: {ElapsedMilliseconds} ms", kafkaConnector.Elapsed);


  
            
            
            return IsInseretd;
            
        }










        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeProcedureName"></param>
        /// <param name="CycleId"></param>
        /// <param name="rdids"></param>
        /// <returns></returns>

        public async Task<List<IEnumerable<RevampMongoDataModel>>> StreamBatchData(string storeProcedureName, int CycleId, string rdids)
        {

            var lstOf_RevampData = new List<IEnumerable<RevampMongoDataModel>>();

            var sqlDataTabeleResult = await FetchAllSQLBatchProceesData(storeProcedureName, CycleId, rdids);

            if (sqlDataTabeleResult.Any())
            {
                foreach (var item in sqlDataTabeleResult)
                {

                    IEnumerable<RevampMongoDataModel> revamapData = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<RevampMongoDataModel>>(item, new DateTimeToMillisecondConverter());
                    if (revamapData.Any())
                    {
                        lstOf_RevampData.Add(revamapData);

                    }
                }
            }

            return lstOf_RevampData;
        }









        /// <summary>
        /// Retrieves a RevampMongoDataModel object asynchronously by its rdids.
        /// </summary>
        /// <param name="rdids">An array of rdids to search for.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result contains the RevampMongoDataModel object found, or null if not found.</returns>
        /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>

        public Task<RevampMongoDataModel> GetRevamDataByRDIDSAsync(string[] rdids, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Retrieves the raw RevampMongoDataModel data asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the raw RevampMongoDataModel data.</returns>
        /// <exception cref="NotImplementedException">This method is not implemented.</exception>

        public Task<RevampMongoDataModel> GetRevampRawDataAsync()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///   /// Asynchronously creates a collection in MongoDB using data from an Excel file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public async Task<bool> CreateCollectionUsingExcl(string filePath, string sheetName)
        {
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var conf = new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        };

                        var dataSet = reader.AsDataSet(conf);

                        DataTable dataTable = null;


                        foreach (DataTable table in dataSet.Tables)
                        {
                            if (table.TableName == sheetName)
                            {
                                dataTable = table;
                                break;
                            }
                        }

                        if (dataTable == null)
                        {
                            _logger.LogError($"Sheet {sheetName} not found in the workbook.");
                            return false;
                        }

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var document = new RevampMongoDataModel
                            {
                                _id = ObjectId.GenerateNewId().ToString(),
                                Sapunitno = row["Sapunitno"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["Sapunitno"]),
                                Cycleno = row["Cycleno"] == DBNull.Value ? null : (long?)Convert.ToInt64(row["Cycleno"]),
                                CycleId = row["CycleID"] == DBNull.Value ? null : (long?)Convert.ToInt64(row["CycleID"]),
                                DOS = row["DOS"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["DOS"]),
                                DM = row["DM"] == DBNull.Value ? null : (long?)Convert.ToInt64(row["DM"]),
                                DOSDATE = row["DOSDate"] == DBNull.Value ? null : (long?)Convert.ToInt64(row["DOSDate"]),
                                CC_Fields_Defs_Id = row["CC_Fields_Defs_Id"] == DBNull.Value ? null : (int?)Convert.ToInt64(row["CC_Fields_Defs_Id"]),
                                CSISValue = row["CSISValue"] == DBNull.Value ? null : (double?)Convert.ToDouble(row["CSISValue"]),
                                ImputedValue = row["ImputedValue"] == DBNull.Value ? null : (double?)Convert.ToDouble(row["ImputedValue"]),
                                ImputedValueMetric = row["ImputedValueMetric"] == DBNull.Value ? null : (double?)Convert.ToDouble(row["ImputedValueMetric"]),
                                ImputedValueImperial = row["ImputedValueImperial"] == DBNull.Value ? null : (double?)Convert.ToDouble(row["ImputedValueImperial"]),
                                CleansedValue = row["CleansedValue"] == DBNull.Value ? null : (double?)Convert.ToDouble(row["CleansedValue"]),
                                ValueMetric = row["ValueMetric"] == DBNull.Value ? null : (double?)Convert.ToDouble(row["ValueMetric"]),
                                ImportedValue = row["ImportedValue"] == DBNull.Value ? null : (double?)Convert.ToDouble(row["ImportedValue"]),
                                CSISDataTypeId = row["CSISDataTypeId"] == DBNull.Value ? null : (long?)Convert.ToInt64(row["CSISDataTypeId"]),
                                CSISTestRunId = row["CSISTestRunId"] == DBNull.Value ? null : (long?)Convert.ToInt64(row["CSISTestRunId"]),
                                CSISPredictionId = row["CSISPredictionId"] == DBNull.Value ? null : (long?)Convert.ToInt64(row["CSISPredictionId"]),
                                EOMobileLabId = row["EOMobileLabId"] == DBNull.Value ? null : (long?)Convert.ToInt64(row["EOMobileLabId"]),
                                ReportDataEntityId = row["ReportDataEntityId"] == DBNull.Value ? null : (long?)Convert.ToInt64(row["ReportDataEntityId"]),
                                IgnoreError = row["IgnoreError"] == DBNull.Value ? null : (bool?)Convert.ToBoolean(row["IgnoreError"]),
                                ApplicationId = row["ApplicationId"] ==

                                  DBNull.Value ? null : (int?)Convert.ToInt32(row["ApplicationId"]),
                                Mode = row["Mode"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["Mode"]),
                                Value = row["Value"] == DBNull.Value ? null : (double?)Convert.ToDouble(row["Value"]),
                                ReportDataHeaderId = row["ReportDataHeaderId"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["ReportDataHeaderId"]),
                                CatCheckConnectData = row["CatCheckConnectData"] == DBNull.Value ? null : (double?)Convert.ToDouble(row["CatCheckConnectData"])
                            };

                            await RevampCollection.InsertOneAsync(document);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while creating the collection from Excel: {ex.Message}");
                return false;
            }
        }
    }
}
