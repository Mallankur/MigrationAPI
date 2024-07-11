namespace Revamp_Ank_App.DomainEntites.Repositores.Entites
{
    public class SqlConnectorData
    {
        public string  SqlConnectionString { get; set; }

        public int CycleID { get; set; }

        public string[]  RDIDS { get; set; }
        public string  StoredProcedureName { get; set; }


    }
}
