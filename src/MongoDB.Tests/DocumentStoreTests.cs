using FluentAssertions;
using Xunit;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace MongoDB.Tests;

public class DocumentStoreTests(
  ITestOutputHelper testOutputHelper,
  MongoDbFixture fixture
) : TestBed<MongoDbFixture>(testOutputHelper, fixture)
{
  private readonly IDocumentStore<TestDocument> _store = fixture.Get<IDocumentStore<TestDocument>>(testOutputHelper);

  [Fact]
  public async Task TestMongoDbConnection()
  {
    var testDocument = new TestDocument
    {
      Property = "Test Document",
      Timestamp = DateTime.UtcNow
    };

    await _store.CreateOne(testDocument);

    var retrievedDocument = await _store.GetById(testDocument.Id);

    retrievedDocument.Should().BeOfType<TestDocument>();
    retrievedDocument.Should().BeEquivalentTo(testDocument);
  }

  [Fact]
  public async Task GetByIdIfExistingReturnsNull()
  {
    var document = await _store.GetByIdIfExisting(Guid.CreateVersion7());

    document.Should().BeNull();
  }
}
