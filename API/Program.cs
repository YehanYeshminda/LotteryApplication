using API.Controllers;
using API.Helpers;
using API.Models;
using API.Repos.Interfaces;
using API.Repos.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl;

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
builder.Services.AddScoped<IScheduler>(_ => StdSchedulerFactory.GetDefaultScheduler().Result);
builder.Services.Configure<TwillioSettings>(builder.Configuration.GetSection("Twilio"));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    DateTime utc9AM = DateTime.UtcNow.Date.AddHours(9);
    DateTime ist9AM = utc9AM.AddHours(5).AddMinutes(30);

    DateTime utc9PM = DateTime.UtcNow.Date.AddHours(21);
    DateTime ist9PM = utc9PM.AddHours(5).AddMinutes(30);

    var jobKey = new JobKey("EasyDrawJob");
    q.AddJob<VerifyWinnerService>(opts => opts.WithIdentity(jobKey)); // Add the job using the class name

    var triggerKey = new TriggerKey("EasyDrawTrigger", "MyTriggerGroup");
    q.AddTrigger(opts => opts
        .ForJob(jobKey) // Associate the trigger with the job's key
        .WithIdentity(triggerKey) // Provide an identity for the trigger
        .WithCronSchedule($"0 0/30 {ist9AM.Hour}-{ist9PM.Hour} * * ?"));


    var jobKeyMega = new JobKey("MegaDrawJob");
    q.AddJob<VerifyMegaDrawService>(opts => opts.WithIdentity(jobKeyMega)); // Add the job using the class name

    var triggerKeyMega = new TriggerKey("MegaDrawTrigger", "MyTriggerGroup");

    q.AddTrigger(opts => opts
        .ForJob(jobKeyMega)
        .WithIdentity(triggerKeyMega)
        .WithCronSchedule($"0 0/59 {ist9AM.Hour}-{ist9PM.Hour} * * ?")); // Run every 30 minutes between 9 AM and 9 PM IST


    var jobKeyLotti = new JobKey("LottiJob");
    q.AddJob<VerifyLottoService>(opts => opts.WithIdentity(jobKeyLotti)); // Add the job using the class name

    var triggerKeyLotti = new TriggerKey("LotiTrigger", "MyTriggerGroup");
    q.AddTrigger(opts => opts
        .ForJob(jobKeyLotti) // Associate the trigger with the job's key
        .WithIdentity(triggerKeyLotti) // Provide an identity for the trigger
        .WithCronSchedule("0 0/15 * * * ?"));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

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
