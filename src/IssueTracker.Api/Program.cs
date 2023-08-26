using IssueTracker.Application.Projections;
using IssueTracker.Application.Views;
using IssueTracker.Infrastructure;
using IssueTracker.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
    config.Lifetime = ServiceLifetime.Scoped;
});
builder.Services.AddElasticSearch(builder.Configuration)
                .AddEventStore(builder.Configuration);
                
builder.Services.AddProjection<UserView, UserViewProjection>();
builder.Services.AddProjection<IssueView, IssueViewProjection>();

builder.Services.AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
