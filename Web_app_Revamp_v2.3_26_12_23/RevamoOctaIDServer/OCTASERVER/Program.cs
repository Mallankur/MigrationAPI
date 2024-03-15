using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddIdentityServer()
    .AddInMemoryClients(Config.Clients)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddDeveloperSigningCredential();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
Log.Logger = new LoggerConfiguration()
        .WriteTo.File("C:\\Users\\ankur\\Desktop\\AvinexAnkurAPP\\Octa_ID_Server4_Logs\\log.txt", rollingInterval: RollingInterval.Day,
          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
           fileSizeLimitBytes: null,
                                            retainedFileCountLimit: null,
                                            shared: true,
                                            flushToDiskInterval: TimeSpan.FromSeconds(5))

        .CreateLogger();
builder.Logging.AddSerilog();



var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseRouting();
app.UseIdentityServer();
app.Run();
