using Api.GraphQL;
using Core;
using EventSourcingDbClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCore();

builder.Services.AddGraphQLServer().AddQueryType<Query>();
// builder.Services.AddGraphQL();

builder.Services.AddEventSourcingDb();

builder.Services.AddHttpContextAccessor();


var app = builder.Build();

app.UseHttpsRedirection();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
