using Revamp_Ank_App.Controllers.Ankur.Revamp.Controller;

namespace RevampTestCase
{
    public class UnitTest1
    {
        [Fact]
        public void PostAsyncTest()
        {
            Revamp_Ank_App.DomainEntites.Repositores.Entites.ISQLconnecterAnkur repository = null;
            Microsoft.Extensions.Logging.ILogger<RevampController> logger = null;
            //Arrange 
            var Controller = new RevampController(repository,logger);
            Controller.Request.Method = "Post";
            Controller.RouteData.Routers.Add;


            //  Assert 

            //  Act 
        }
    }
}