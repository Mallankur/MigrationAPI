using Revamp_Ank_App.Domain.Repositores.Entites;

namespace Revamp_Ank_App.DomainEntites.Repositores.Entites
{
    public interface ISQLconnecterAnkur
    {
        Task<RevampMongoDataModel> GetRevampRawDataAsync();

        Task<RevampMongoDataModel> GetRevamDataByRDIDSAsync(String[] rdids, CancellationToken cancellationToken);

        Task<bool> CreateData_Using_SQL_SP_ConnectorAsync(string storeProcedureName, int CycleId, string rdids);


        Task<IEnumerable<string>> FetchAllSQLBatchProceesData(string storeProcedureName, int CycleId, string rdids);
        Task<IEnumerable<string>> FetchAllSQLkafkaProcessing(string storeProcedureName, int CycleId, string rdids);

        //  Get Cycle_Data_on_UI 



        Task<List<IEnumerable<RevampMongoDataModel>>> StreamBatchData(string storeProcedureName, int CycleId, string rdids);


    }
}
