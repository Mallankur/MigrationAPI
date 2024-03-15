using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Revamp_Ank_App.DomainEntites.Repositores.Entites;
using Newtonsoft.Json;


namespace Revamp_Ank_App.Client
{
    public class RevampDocument
    {
       
       
        public int? Sapunitno { get; set; }
      
        public long? Cycleno { get; set; }
        
        public long? CycleID { get; set; }
        public int? DOS { get; set; }

        public long? DM { get; set; }
        public long?  DOSDATE { get; set; }

       
        public int? CC_Fields_Defs_Id { get; set; }

        public double? CSISValue { get; set; }
       
        public double? ImputedValue { get; set; }
      
        public double? ImputedValueMetric { get; set; }
        
        public double? ImputedValueImperial { get; set; }
        
        public double? CleansedValue { get; set; }
       
        public double? ValueMetric { get; set; }
      
        public double? ImportedValue { get; set; }

       
        public long? CSISDataTypeId { get; set; }
        
        public long? CSISTestRunId { get; set; }
       
        public long? CSISPredictionId { get; set; }
        
        public long? EOMobileLabId { get; set; }
       
        public long? ReportDataEntityId { get; set; }


        public bool? IgnoreError { get; set; }
       
        public int? ApplicationId { get; set; }
      
        public int? Mode { get; set; }

    }
}

