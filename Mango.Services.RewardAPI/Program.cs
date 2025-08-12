using log4net;
using log4net.Config;
using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Extension;
using Mango.Services.RewardAPI.Messaging;
using Mango.Services.RewardAPI.Service;
using Mango.Services.RewardAPI.Service.IService;
using Mango.Services.RewardAPI.Utility;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure log4net
var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IRewardService, RewardService>();
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
builder.Services.AddSingleton<IOrderService, OrderService>();
builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();

builder.Services.AddHttpClient("Order", u => u.BaseAddress =
new Uri(builder.Configuration["ServiceUrls:OrderAPI"])).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
await DbInitializer.InitDb(app);

app.UseAzureServiceBusConsumer();

app.Run();
