using Core.ClubMembership;
using Core.Clubs;
using Core.Infrastructure;
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

  private record ReadModelDefinition(
    Type Type,
    string CollectionName,
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
    IEnumerable<ReadModelDefinition> readModels =
      new List<ReadModelDefinition>
      {
        new(
          typeof(ClubMember),
          "clubMembers",
          Subject.FromLevel(1)
        ),
        new(
          typeof(Tank),
          "tanks",
          Subject.FromLevel(1)
        )
      };
    foreach (var readModel in readModels)
    {
      services.AddReadModel(readModel.Type, readModel.CollectionName, readModel.IdSelector);
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
