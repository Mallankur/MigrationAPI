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
using Revamp_Ank_App.Client.ExcelMigration;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Revamp_Ank_App.Controllers.Ankur.Revamp.Controller
{
    [ApiController]
    [Route("api/[controller]/")]


    public class RevampController : ControllerBase
    {
        private readonly ILogger<RevampController> _logger;
        private readonly ISQLconnecterAnkur _repository;
        private readonly IExeclSheetReader _excelhandler;


        public RevampController(ISQLconnecterAnkur repository, ILogger<RevampController> logger)
        {
            _repository = repository;
            _logger = logger;

        }

        /// <summary>
        /// processes the batch data using a SQL stored procedure.

        /// </summary>
        /// <param name="storeProcedureName"></param>
        /// <param name="CycleId"></param>
        /// <param name="rdids"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostBatchProcessingAsync")]

        public async Task<IActionResult> PostBatchProcessingAsync(
             string storeProcedureName, int CycleId,
             [FromBody] string rdids)
        {
            _logger.LogInformation($"Data Processing in API {DateTime.Now}");
            _logger.LogInformation($"Data import Process started for Unit: <Unit Name> Cycle: {CycleId} {DateTime.Now}");
            var result = await _repository.CreateData_Using_SQL_SP_ConnectorAsync(storeProcedureName, CycleId, rdids);
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
        /// <summary>
        /// /// Migrates data from an Excel file to MongoDB.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> MigrateDataFromExcel([FromBody] ExcelMigrationRequest request)
        {
            try
            {
                var result = await _excelhandler.CreateCollectionUsingExcl(request.FilePath, request.sheetname);
                if (result)
                    return Ok("Data migrated successfully.");
                else
                    return BadRequest("Failed to migrate data.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while migrating data from Excel to MongoDB.");
                return StatusCode(500, "Internal server error.");
            }
        }






    }
}
