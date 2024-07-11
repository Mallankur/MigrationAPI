namespace Revamp_Ank_App.DomainEntites.Repositores.Entites
{
    public interface ISQLconnecterAnkurMall228
    {
        Task<IEnumerable<string>> FetchAllSQLBatchDataAsyn(string sqlConnectionString, string storeProcedureName, int CycleId, string rdids);

       

    }
}
