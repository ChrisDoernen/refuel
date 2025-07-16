using Api;
using Api.Auth;
using Api.ClubMembership;
using Api.Clubs;
using Api.GraphQL;
using Api.Shared;
using Api.Tanks;
using Api.Users;
using Core;
using Core.Infrastructure;
using Core.Infrastructure.Cqrs;
using Core.Tanks;
using dotenv.net;
using EventSourcing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MongoDB;
using Shared.Testing;
using EventType = Api.Shared.EventType;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load(new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 6));

Environment.SetEnvironmentVariable("RANDOMIZE_HOST_PORT", "false");


builder.Services
  .AddGraphQLServer()
  .AddAuthorization()
  .AddGlobalObjectIdentification()
  .AddQueryType<Query>()
  .AddMutationType<Mutation>()
  .AddTypeExtension<UsersMutationType>()
  .AddTypeExtension<ClubsQueryType>()
  .AddTypeExtension<UsersQueryType>()
  .AddTypeExtension<SharedQueryType>()
  .AddType<EventType>()
  .AddType<StateChangeType<TankType, Tank>>()
  .AddType<RoleType>()
  .AddType<ClubMemberType>()
  .AddType<TankRoleAssignmentsType>()
  .AddType<ClubType>()
  .AddType<UserType>()
  .AddType<TankType>()
  .AddType<SignUpCommandInputType>()
  .AddTypeConverter<string, ClubCompoundId>(ClubCompoundId.Parse)
  .AddTypeConverter<ClubCompoundId, string>(x => x.ToString())
  .ModifyRequestOptions(o => o.IncludeExceptionDetails = builder.Environment.IsDevelopment());

builder.Services.AddGraphQL();
builder.Services.AddErrorFilter<ErrorFilter>();


builder.Services.AddCoreInfrastructure(typeof(Core.ServiceCollectionExtensions).Assembly);
builder.Services.AddEventSourcingDb(builder.Configuration);
builder.Services.AddMongoDb(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthenticationMiddleware>();
builder.Services.AddScoped<IUserAccessor, UserAccessor>();

if (builder.Environment.IsDevelopment())
{
  builder.Services.AddTesting();
}

builder.Services.AddCore();


#if DEBUG

builder.Services
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(
    options =>
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = false,
        SignatureValidator = (token, _) => new JsonWebToken(token),
        RequireExpirationTime = false,
        ValidateLifetime = false,
        ClockSkew = TimeSpan.Zero,
        RequireSignedTokens = false,
      }
  );

#endif


var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.MapGraphQL();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuthenticationMiddleware>();

app.RunWithGraphQLCommands(args);
