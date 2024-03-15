using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver.Core.Configuration;
using Revamp_Ank_App.DomainEntites.Repositores.Entites;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using Revamp_Ank_App.Domain.Repositores.Entites;
using System.Diagnostics.Contracts;
using Revamp_Ank_App.Contract.Entites.RevampMongoCollection;
using Revamp_Ank_App.Client;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Revamp_Ank_App.Controllers.Ankur.Revamp.Controller
{
    [ApiController]
    [Route("api/[controller]/")]
   // [Authorize]

    public class RevampController : ControllerBase
    {
        private readonly ILogger<RevampController> _logger;
        private readonly ISQLconnecterAnkur _repository;
        //private readonly IConfiguration _config;

        public RevampController(ISQLconnecterAnkur repository ,ILogger<RevampController> logger)
        {
            _repository = repository;
            _logger = logger;
           
        }

       
        [HttpPost]
        [Route("PostBatchProcessingAsync")]
        // [ProducesResponseType(typeof(RevampMongoEntity , StatusCodes.Status201Created)]
        public async Task<IActionResult> PostBatchProcessingAsync(
             string storeProcedureName, int CycleId,
             [FromBody] string rdids)
        {
            _logger.LogInformation($"Data Processing in API {DateTime.Now}");
            _logger.LogInformation($"Data import Process started for Unit: <Unit Name> Cycle: {CycleId} {DateTime.Now}");
            var result = await _repository.CreateData_Using_SQL_SP_ConnectorAsync(storeProcedureName,CycleId,rdids);
            _logger.LogInformation($"Data import process ended for Unit: <Unit Name> Cycle: {CycleId} {DateTime.Now}");

            if (result)
            {

                 return Ok("Data received and processed");
                
            }
            _logger.LogInformation($"Data Processing in API End {DateTime.Now}");

            return BadRequest(" This exception was originally thrown at this call stack:" +
                "System.Text.Json.ThrowHelper.ThrowJsonReaderException(ref System.Text.Json.Utf8JsonReader, " +
                "System.Text.Json.ExceptionResource, byte, System.ReadOnlySpan<byte>)" +
                " System.Text.Json.Utf8JsonReader.ConsumeValue(byte)\r\n    " +
                "System.Text.Json.Utf8JsonReader.ConsumeNextTokenUntilAfterAllCommentsAreSkipped(byte)" +
                "  System.Text.Json.Utf8JsonReader.ConsumeNextToken(byte)  "); 
        }

        [HttpPost]
        [Route("GetDataByrdids")]

        public async Task<IActionResult> GetDataByrdidsAsync( 
            [FromBody] string  ridis   )

        {
            return Ok(ridis.ToString());
        }

        [HttpPost]
        [Route("CycleData")]
        public async Task<List<RevampDocument>> CycleData(
    string storeProcedureName, int CycleId,
    [FromBody] string rdids)
        {
            var res = await _repository.StreamBatchData(storeProcedureName, CycleId, rdids);

            if (res.Count >= 0)
            {




                var transformedData = res.SelectMany(documents =>
                    documents.Select(document => new RevampDocument
                    {
                       
                        ApplicationId = document.ApplicationId,
                       Sapunitno = document.Sapunitno,
                       Cycleno = document.Cycleno,
                       CycleID = document.CycleID,
                       DOS = document.DOS,
                       DOSDATE = document.DOSDATE,
                       CC_Fields_Defs_Id = document.CC_Fields_Defs_Id,  

                        
                    })).ToList();
                return transformedData;
            }

            else return  new List<RevampDocument>();
        }

        [HttpGet]
        [Route("GetCycleRawData")]

        public async Task<List<RevampDocument>> GetCycleRawData(
     int CycleId
      )
        {
            string spName = "CC_Revamp_uspGetUMData";
            var res = await _repository.StreamBatchData(spName, CycleId, "");

            if (res.Count >= 0)
            {




                var transformedData = res.SelectMany(documents =>
                    documents.Select(document => new RevampDocument
                    {
                        
                        ApplicationId = document.ApplicationId,
                        Sapunitno = document.Sapunitno,
                        Cycleno = document.Cycleno,
                        CycleID = document.CycleID,
                        DOS = document.DOS,
                        DOSDATE = document.DOSDATE,
                        CC_Fields_Defs_Id = document.CC_Fields_Defs_Id,
                        CSISValue = document.CSISValue,
                        ImputedValue = document.ImputedValue,
                        ImputedValueMetric = document.ImputedValueMetric,
                        ImputedValueImperial = document.ImputedValueImperial,
                        CleansedValue = document.CleansedValue,
                        ValueMetric = document.ValueMetric,
                        ImportedValue = document.ImportedValue,
                        CSISDataTypeId = document.CSISDataTypeId,
                        CSISPredictionId = document.CSISPredictionId,
                        EOMobileLabId = document.EOMobileLabId,
                        ReportDataEntityId = document.ReportDataEntityId,
                        IgnoreError = document.IgnoreError, 
                        CSISTestRunId  = document.CSISTestRunId,
                        Mode = document.Mode,




                    })).ToList();
                return transformedData;
            }

            else return new List<RevampDocument>();
        }




    }
}
