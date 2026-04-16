using DisasterAlert.context;
using DisasterAlert.service.Services;
using DisasterAlert.service.Services.CacheService;
using DisasterAlert.service.Services.MessagingService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ExternalApiSetting>(builder.Configuration.GetSection("ExternalApi"));
builder.Services.Configure<TwilioServiceSetting>(builder.Configuration.GetSection("TwilioApiKey"));

//Add Db concetion.
var PstgresconnectionString = builder.Configuration.GetConnectionString("PstgresConnection");
builder.Services.AddDbContext<DisasterAlertContext>(option => option.UseNpgsql(
    PstgresconnectionString
));

//Add Cacher.
var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");
builder.Services.AddSingleton<ICacheService, RedisService>(c => new RedisService(redisConnectionString));

//Add Depencies.
builder.Services.AddScoped<AppService, AppserviceIpm>();
builder.Services.AddScoped<IMessagingService, TwilioService>();
builder.Services.AddHttpClient<ExternalData, ExternalApi>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

//Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
