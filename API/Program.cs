using API.Controllers;
using API.Helpers;
using API.Models;
using API.Repos.Interfaces;
using API.Repos.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<LotteryContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));

// services
builder.Services.AddHttpClient();
builder.Services.AddScoped<IRegisterRepository, RegisterService>();
builder.Services.AddScoped<IAccountRepository, AccountService>();
builder.Services.AddScoped<IHistoryService, HistoryService>();
builder.Services.AddScoped<Generators>();
builder.Services.AddSingleton<GlobalDataService>();
builder.Services.AddOptions();
builder.Services.Configure<TwillioSettings>(builder.Configuration.GetSection("Twilio"));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors(builder =>
{
    builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
});

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

//using (var scope = app.Services.CreateScope())
//{
//    var serviceProvider = scope.ServiceProvider;
//    var lotteryContext = serviceProvider.GetRequiredService<LotteryContext>();
//    var generators = serviceProvider.GetRequiredService<Generators>();
//    var yourApiController = new GeneratorController(lotteryContext, generators);
//    var cronJob = new CronJob(serviceProvider);
//}

app.Run();
