using Api.Auth;
using Api.Clubs;
using Api.GraphQL;
using Api.Shared;
using Api.Tanks;
using Api.Users;
using Core;
using Core.Clubs;
using Core.Shared;
using EventSourcingDB;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCore();

builder.Services.AddGraphQLServer()
  .AddGlobalObjectIdentification()
  .AddQueryType<Query>()
  .AddTypeExtension<ClubsQueryType>()
  .AddTypeExtension<UsersQueryType>()
  .AddType<ChangeType>()
  .AddType<StateChangeType<ClubType, Club>>()
  .AddType<ClubType>()
  .AddType<UserType>()
  .AddType<TankType>();

builder.Services.AddGraphQL();

builder.Services.AddEventSourcingDb(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserAccessor, UserAccessor>();


var app = builder.Build();

app.UseHttpsRedirection();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
