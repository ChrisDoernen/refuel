using Core.ClubMembership;
using Core.Clubs;
using Core.Infrastructure;
using Core.Infrastructure.Caching;
using Core.Infrastructure.ReadModels;
using Core.Tanks;
using Core.Users;
using EventSourcing;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ServiceCollectionExtensions
{
  private record DocumentDefinition(
    Type Type,
    string CollectionName
  );

  private record IdentifiedReadModelDefinition(
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
    IEnumerable<IdentifiedReadModelDefinition> readModels =
      new List<IdentifiedReadModelDefinition>
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
