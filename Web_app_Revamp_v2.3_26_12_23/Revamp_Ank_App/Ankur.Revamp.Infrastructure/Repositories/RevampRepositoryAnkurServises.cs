
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
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
    public class RevampRepositoryAnkurServises : ISQLconnecterAnkur
    {
        public IMongoCollection<RevampMongoDataModel> RevampCollection { get; set; }
        private readonly SQLConnetter _sqlConnetter;
        private readonly ILogger<RevampRepositoryAnkurServises> _logger;    
        public RevampRepositoryAnkurServises(IOptions<MongoScoket> connect, SQLConnetter sql,ILogger<RevampRepositoryAnkurServises> logger)
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
       
        /// </summary>
        /// <param name="storeProcedureName"></param>
        /// <param name="CycleId"></param>
        /// <param name="rdids"></param>
        /// <returns></returns>

        public async Task<IEnumerable<string>> FetchAllSQLkafkaProcessing(string storeProcedureName, int CycleId, string rdids)
        {
            var multiCycleStreaming =  _sqlConnetter.FetchAllSQLMultiBatchProcewsData(_sqlConnetter._connectionString, storeProcedureName, CycleId);
             return multiCycleStreaming; 
        }


        



        /// <summary>
        /// 
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
                                                    //}


            //else
            //{
            //    var sqlDataTabeleResult = await FetchAllSQLBatchProceesData(storeProcedureName, CycleId, rdids);

            //    foreach (var Batchrow in sqlDataTabeleResult)
            //    {
            //        IEnumerable<RevampMongoDataModel> revamapData = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<RevampMongoDataModel>>(Batchrow, new DateTimeToMillisecondConverter());
            //        var batchInsertStopwatch = Stopwatch.StartNew();
            //        if (revamapData.Any())
            //        {
            //            RevampCollection.InsertManyAsync(revamapData);
            //            batchInsertStopwatch.Stop();
            //            batchCount++;
            //            _logger.LogInformation("Batch {BatchNumber} insert took: {ElapsedMilliseconds} ms", batchCount, batchInsertStopwatch.ElapsedMilliseconds);
            //            IsInseretd = true;
            //        }
            //    }
            //    pushTOMongoStopwatch.Stop();
            //    _logger.LogInformation
            //        ("Total data push to MongoDB operation took: {ElapsedMilliseconds} ms", pushTOMongoStopwatch.ElapsedMilliseconds);
            //    return IsInseretd;
            //}

            //          return IsInseretd;
        }

        







            
            /// <summary>
            /// 
            /// </summary>
            /// <param name="storeProcedureName"></param>
            /// <param name="CycleId"></param>
            /// <param name="rdids"></param>
            /// <returns></returns>

       public async  Task<List<IEnumerable<RevampMongoDataModel>>> StreamBatchData(string storeProcedureName, int CycleId, string rdids)
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


        








        public Task<RevampMongoDataModel> GetRevamDataByRDIDSAsync(string[] rdids, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<RevampMongoDataModel> GetRevampRawDataAsync()
        {
            throw new NotImplementedException();
        }

    }
}
