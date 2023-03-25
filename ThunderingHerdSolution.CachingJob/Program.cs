using ThunderingHerdSolution.CachingJob;
using ThunderingHerdSolution.Core;
using ThunderingHerdSolution.Core.Abstractions;
using ThunderingHerdSolution.Core.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCoreModule(builder.Configuration);
builder.Services.AddSingleton<IJobRescheduler, JobRescheduler>();
builder.Services.AddHostedService<RedisChannelQueueListener>();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();



app.Run();


