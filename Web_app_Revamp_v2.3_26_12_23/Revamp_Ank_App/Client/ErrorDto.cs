namespace Revamp_Ank_App.Client
{
    public class ErrorDto
    {
        
            public string Reason { get; set; }
            public string Message { get; set; }
            public Dictionary<string, List<ErrorDto>> Params { get; set; }
        
    }
}
