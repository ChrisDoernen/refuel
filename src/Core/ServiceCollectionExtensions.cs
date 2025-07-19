using Core.ClubMembership.Projections;
using Core.Clubs.Models;
using Core.Infrastructure;
using Core.Infrastructure.Caching;
using Core.Tanks.Projections;
using Core.Users.Models;
using EventSourcing;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ServiceCollectionExtensions
{
  private record DocumentDefinition(
    Type Type,
    string CollectionName
  );

  private record IdentifiedProjectionDefinition(
    Type Type,
    CacheKey CacheKey,
    Func<Subject, Guid> IdSelector
  );

  public static void AddCore(
    this IServiceCollection services
  )
  {
    services.AddHostedService<EventStoreProviderInitService>();

    AddDocuments(services);
    AddReadModels(services);
  }

  private static void AddReadModels(IServiceCollection services)
  {
    IEnumerable<IdentifiedProjectionDefinition> readModels =
      new List<IdentifiedProjectionDefinition>
      {
        new(
          typeof(ClubMember),
          new CacheKey("/clubMembers"),
          Subject.FromLevel(1)
        ),
        new(
          typeof(Tank),
          new CacheKey("/tanks"),
          Subject.FromLevel(1)
        )
      };
    foreach (var readModel in readModels)
    {
      services.AddIdentifiedReadModel(readModel.Type, readModel.CacheKey, readModel.IdSelector);
    }
  }

  private static void AddDocuments(IServiceCollection services)
  {
    IEnumerable<DocumentDefinition> documents =
      new List<DocumentDefinition>
      {
        new(typeof(User), "users"),
        new(typeof(Club), "clubs")
      };
    foreach (var document in documents)
    {
      services.AddDocumentStore(document.Type, document.CollectionName);
    }
  }
}
