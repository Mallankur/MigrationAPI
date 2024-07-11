using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Revamp_Ank_App.Ankur.Revamp.Infrastructure.Repositories;
using Revamp_Ank_App.DomainEntites.Repositores.Entites;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using Revamp_Ank_App.Client.ExcelMigration;
using System.ComponentModel;
using OfficeOpenXml;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RevampApp", Version = "v1@ankMall" });
    // Configure XML comments for your controllers and models here
    // c.IncludeXmlComments("your-xml-comments-file.xml");
});
builder.Services.Configure<MongoScoket>(builder.Configuration.GetSection("MongoObject"));
builder.Services.Configure<SQLConnectorOptions>(builder.Configuration.GetSection("SQLConnector"));



builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:32773/";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

Log.Logger = new LoggerConfiguration()
        .WriteTo.File("C:\\Users\\ankur\\Desktop\\MigrationAPIlog\\WebApiLog\\log.txt", rollingInterval: RollingInterval.Day,
          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
           fileSizeLimitBytes: null,
                                            retainedFileCountLimit: null,
                                            shared: true,
                                            flushToDiskInterval: TimeSpan.FromSeconds(5))
       
        .CreateLogger();
builder.Logging.AddSerilog();




builder.Services.AddMvc().AddXmlSerializerFormatters();

builder.Services.AddScoped<ISQLconnecterAnkur, RevampRepositoryAnkurServises>();
builder.Services.AddScoped<ISQLconnecterAnkurMall228, SQLConnetter>();
builder.Services.AddScoped<SQLConnetter>();


builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
