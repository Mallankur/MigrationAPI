using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver;
using Revamp_Ank_App.DomainEntites.Repositores.Entites;
using System.Data;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Text;

namespace Revamp_Ank_App.Ankur.Revamp.Infrastructure.Repositories
{
    public class SQLConnetter : ISQLconnecterAnkurMall228
    {
        public readonly string _connectionString;

        private readonly ILogger<SQLConnetter> _logger;

        public SQLConnetter(IOptions<SQLConnectorOptions> options, ILogger<SQLConnetter> logger)
        {
            _connectionString = options.Value.ConnectionString;
            _logger = logger;
        }

        /// <summary> 
        /// Fectch All the Data Async @Ankur 
        /// kz = (1+e)ln(1-xa) -eaxa
        /// T  = T0 + ^Ha{XA}Fa0/(mcp)
        /// </summary>
        /// <param name="sqlConnectionString"></param>
        /// <param name="storeProcedureName"></param>
        /// <param name="CycleId"></param>
        /// <param name="rdids"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> FetchAllSQLBatchDataAsyn(string sqlConnectionString, string storeProcedureName, int CycleId, string rdids)
        {


            var StoredproducerCall = Stopwatch.StartNew();
            var jsonResults = new List<string>();
            const int batchSize = 150000;

            using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.OpenAsync().ConfigureAwait(false);

                SqlCommand sqlCommand = new SqlCommand(storeProcedureName, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@CycleId", CycleId);
                sqlCommand.Parameters.AddWithValue("@RDIds", rdids);

                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataTable dataTable = new DataTable();
                 await Task.Run(() => adapter.Fill(dataTable)).ConfigureAwait(false);
               // adapter.Fill(dataTable);

                var StoredproducerCallEnd = Stopwatch.StartNew();
                _logger.LogInformation($"Stored Procedure Call End for CycleID:{CycleId}" + "Total data fill operation took: {ElapsedMilliseconds} ms",
                    StoredproducerCall.ElapsedMilliseconds);

                int totalRows = dataTable.Rows.Count;
                int batches = (int)Math.Ceiling((double)totalRows / batchSize);

                for (int i = 0; i < batches; i++)
                {
                    int startIndex = i * batchSize;
                    int endIndex = Math.Min((i + 1) * batchSize, totalRows);

                    DataTable batchTable = dataTable.Clone();
                    for (int j = startIndex; j < endIndex; j++)
                    {
                        batchTable.ImportRow(dataTable.Rows[j]);
                    }

                    string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(batchTable);

                    jsonResults.Add(jsonData);

                }
                return jsonResults;
            }
        }

      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlConnectionString"></param>
        /// <param name="storeProcedureName"></param>
        /// <param name="CycleId"></param>
        /// <param name="rdids"></param>
        /// <returns></returns>
        public IEnumerable<string> StreamAllSQLDataInChunks(string sqlConnectionString, string storeProcedureName, int CycleId, string rdids, int chunkSize = 1000000)
        {
            var KafkaStreamingCallstart = Stopwatch.StartNew();

            using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
            {
                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand(storeProcedureName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@CycleId", CycleId);
                    sqlCommand.Parameters.AddWithValue("@RDIds", rdids);

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        List<string> columnNames = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();

                        int count = 0;
                        while (reader.Read())
                        {
                            if (count == 0)
                            {
                                List<Dictionary<string, object>> chunkData = new List<Dictionary<string, object>>();

                                while (reader.Read() && chunkData.Count < chunkSize)
                                {
                                    object[] array = new object[reader.FieldCount];
                                    reader.GetValues(array);

                                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                                    for (int i = 0; i < array.Length; i++)
                                    {
                                        dictionary.Add(columnNames[i], array[i]);
                                    }

                                    chunkData.Add(dictionary);
                                }

                                if (chunkData.Count > 0)
                                {
                                    yield return JsonConvert.SerializeObject(chunkData);
                                }
                            }

                            count++;
                            if (count == chunkSize)
                            {
                                count = 0;
                            }
                        }
                    }
                }
            }

            _logger.LogInformation($"KafkaBridgeElapes for CycleID:{CycleId}. Total data fill operation took: {KafkaStreamingCallstart.ElapsedMilliseconds} ms");
        }
        public IEnumerable<string> StreamAllSQLDataForMulticycle(string sqlConnectionString, string storeProcedureName, int CycleId, string rdids)
        {
            var KafkaStreamingCallstart = Stopwatch.StartNew();


            using SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand(storeProcedureName, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@CycleId", CycleId);
            sqlCommand.Parameters.AddWithValue("@RDIds", rdids);
            using SqlDataReader reader = sqlCommand.ExecuteReader();
            List<string> columnNames = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
            while (reader.Read())
            {
                object[] array = new object[reader.FieldCount];
                reader.GetValues(array);
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                for (int i = 0; i < array.Length; i++)
                {
                    dictionary.Add(columnNames[i], array[i]);
                }

                yield return JsonConvert.SerializeObject(dictionary);
            }

            _logger.LogInformation($"KafkaBridgeElapesfor CycleID:{CycleId}" + "Total data fill operation took: {ElapsedMilliseconds}+StoredproducerCall.ElapsedMilliseconds"
                , KafkaStreamingCallstart.ElapsedMilliseconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlConnectionString"></param>
        /// <param name="CycleId"></param>
        /// <returns></returns>
        public string  ReturnSPRdied(string sqlConnectionString, int CycleId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
            {
                sqlConnection.Open();

                var sqlCommand = new SqlCommand("uspGetCycleRDEIDs", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@CycleId", CycleId);

                var adapter = new SqlDataAdapter(sqlCommand);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                var jsonresult = Newtonsoft.Json.JsonConvert.SerializeObject(dataTable);
                return jsonresult;
           
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlConnectionString"></param>
        /// <param name="storeProcedureName"></param>
        /// <param name="CycleId"></param>
        /// <returns></returns>
        public IEnumerable<string> FetchAllSQLMultiBatchProcewsData(string sqlConnectionString, string storeProcedureName, int CycleId)
        {
            var resultReturnSprdids = ReturnSPRdied(sqlConnectionString, CycleId);
            

          //  var sqlAdapterLogger = Stopwatch.StartNew();

            const int batchSize = 201750;
            var rdidObjects = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(resultReturnSprdids);

            List<string> concatenatedValues = new List<string>();

            foreach (var rdidObject in rdidObjects)
            {
                var rdidValue = rdidObject["Column1"];
                concatenatedValues.Add(rdidValue);
                var sqlAdapterLogger = Stopwatch.StartNew();

                if (concatenatedValues.Count % 50 == 0)
                {
                    var concatenatedString = string.Join(",", concatenatedValues);
                   
                    concatenatedValues.Clear();

                    using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
                    {
                        sqlConnection.Open();

                        SqlCommand sqlCommand = new SqlCommand(storeProcedureName, sqlConnection);
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@CycleId", CycleId);
                        sqlCommand.Parameters.AddWithValue("@RDIds", concatenatedString);

                        SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        _logger.LogInformation($"Total Time Taken Filling The OPERATION{concatenatedString}" + "Total data fill operation took: {ElapsedMilliseconds}+StoredproducerCall.ElapsedMilliseconds"
                              , sqlAdapterLogger.ElapsedMilliseconds);



                        int totalRows = dataTable.Rows.Count;
                        int batches = (int)Math.Ceiling((double)totalRows / batchSize);

                        for (int i = 0; i < batches; i++)
                        {
                            int startIndex = i * batchSize;
                            int endIndex = Math.Min((i + 1) * batchSize, totalRows);

                            DataTable batchTable = dataTable.Clone();
                            for (int j = startIndex; j < endIndex; j++)
                            {
                                batchTable.ImportRow(dataTable.Rows[j]);
                            }

                            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(batchTable);

                            yield return jsonData;
                        }
                    }
                }
            }

            // If there are remaining values not processed in a batch of 10
            if (concatenatedValues.Any())
            {
                var concatenatedString = string.Join(",", concatenatedValues);

                using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
                {
                    sqlConnection.Open();

                    SqlCommand sqlCommand = new SqlCommand(storeProcedureName, sqlConnection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@CycleId", CycleId);
                    sqlCommand.Parameters.AddWithValue("@RDIds", concatenatedString);

                    SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    int totalRows = dataTable.Rows.Count;
                    int batches = (int)Math.Ceiling((double)totalRows / batchSize);
                    /*
                      
                       Enhanced the algorithm to significantly reduce processing time for handling large volumes of data.
                      Ensured consistency and validation of all data points throughout the migration process.
                      */
                    for (int i = 0; i < batches; i++)
                    {
                        int startIndex = i * batchSize;
                        int endIndex = Math.Min((i + 1) * batchSize, totalRows);

                        DataTable batchTable = dataTable.Clone();
                        for (int j = startIndex; j < endIndex; j++)
                        {
                            batchTable.ImportRow(dataTable.Rows[j]);
                        }

                        string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(batchTable);

                        yield return jsonData;
                    }
                }
            }
        }
      
    }
}

