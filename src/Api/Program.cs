using Api.Auth;
using Api.ClubMembership;
using Api.Clubs;
using Api.GraphQL;
using Api.Shared;
using Api.Tanks;
using Api.Users;
using Core;
using Core.Clubs;
using Core.Shared;
using Core.Tanks;
using Dev;
using dotenv.net;
using EventSourcingDB;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load(new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 6));

Environment.SetEnvironmentVariable("RANDOMIZE_HOST_PORT", "false");

builder.Services.AddCore();

builder.Services.AddGraphQLServer()
  .AddGlobalObjectIdentification()
  .AddQueryType<Query>()
  .AddTypeExtension<ClubsQueryType>()
  .AddTypeExtension<UsersQueryType>()
  .AddTypeExtension<SharedQueryType>()
  .AddType<ChangeType>()
  .AddType<StateChangeType<ClubType, Club>>()
  .AddType<StateChangeType<TankType, Tank>>()
  .AddType<RoleType>()
  .AddType<ClubMemberType>()
  .AddType<ClubType>()
  .AddType<UserType>()
  .AddType<TankType>();

builder.Services.AddGraphQL();

builder.Services.AddEventSourcingDb(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserAccessor, UserAccessor>();

if (builder.Environment.IsDevelopment())
{
  builder.Services.AddHostedService<TestContainerService>();
  builder.Services.AddHostedService<DevDataRestoreService>();
}

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
