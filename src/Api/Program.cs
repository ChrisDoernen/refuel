using Api;
using Api.Auth;
using Api.GraphQL;
using Core;
using Core.Shared;
using EventSourcingDB;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCore();

builder.Services.AddGraphQLServer().AddQueryType<Query>();
// builder.Services.AddGraphQL();

builder.Services.AddEventSourcingDb(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserAccessor, UserAccessor>();



var app = builder.Build();

app.UseHttpsRedirection();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
