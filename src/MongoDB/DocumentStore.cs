using System.Linq.Expressions;
using MongoDB.Driver;

namespace MongoDB;

public class DocumentStore<TDocument>(
  IMongoDatabase database,
  string collectionName
) : IDocumentStore<TDocument> where TDocument : IDocument
{
  private readonly IMongoCollection<TDocument> _collection = database.GetCollection<TDocument>(collectionName);

  public IQueryable<TDocument> AsQueryable()
  {
    return _collection.AsQueryable();
  }

  public async Task<IEnumerable<TDocument>> FilterBy(
    Expression<Func<TDocument, bool>> filter,
    CancellationToken cancellationToken
  )
  {
    var documents = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

    return documents.ToEnumerable(cancellationToken: cancellationToken);
  }

  public async Task<TDocument> GetSingle(
    Expression<Func<TDocument, bool>> filter,
    CancellationToken cancellationToken
  )
  {
    var documents = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

    return await documents.FirstAsync(cancellationToken: cancellationToken);
  }

  public async Task<bool> ExistsById(Guid id, CancellationToken cancellationToken)
  {
    var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);
    var documents = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

    return await documents.AnyAsync(cancellationToken: cancellationToken);
  }

  public IEnumerable<TProjected> FilterBy<TProjected>(
    Expression<Func<TDocument, bool>> filter,
    Expression<Func<TDocument, TProjected>> projectionExpression,
    CancellationToken cancellationToken
  )
  {
    return _collection
      .Find(filter)
      .Project(projectionExpression)
      .ToEnumerable(cancellationToken: cancellationToken);
  }

  public async Task<IEnumerable<TDocument>> GetAll(CancellationToken cancellationToken)
  {
    var documents = await _collection.FindAsync(
      Builders<TDocument>.Filter.Empty,
      cancellationToken: cancellationToken
    );

    return documents.ToEnumerable(cancellationToken: cancellationToken);
  }

  public async Task<TDocument> GetById(Guid id, CancellationToken cancellationToken)
  {
    try
    {
      var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);
      var documents = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

      return await documents.SingleAsync(cancellationToken: cancellationToken);
    }
    catch (InvalidOperationException)
    {
      throw new KeyNotFoundException($"Document {typeof(TDocument).Name} with id {id} was not found in the database");
    }
  }

  public async Task<IEnumerable<TDocument>> GetManyById(
    IEnumerable<Guid> ids,
    CancellationToken cancellationToken
  )
  {
    var documentIds = ids.ToList();
    var filter = Builders<TDocument>.Filter.In(d => d.Id, documentIds.Select(id => id));
    var documentsCursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
    var documents = await documentsCursor.ToListAsync(cancellationToken: cancellationToken);

    return documentIds.Count != documents.Count
      ? throw new KeyNotFoundException("One or more documents have not been found in the database.")
      : documents;
  }

  public async Task<TDocument?> GetByIdIfExisting(Guid id, CancellationToken cancellationToken)
  {
    try
    {
      return await GetById(id, cancellationToken);
    }
    catch (KeyNotFoundException)
    {
      return default;
    }
  }

  public async Task CreateOne(TDocument document, CancellationToken cancellationToken)
  {
    await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
  }

  public async Task CreateMany(IEnumerable<TDocument> documents, CancellationToken cancellationToken)
  {
    await _collection.InsertManyAsync(documents, cancellationToken: cancellationToken);
  }

  public async Task UpdateOne(TDocument document, CancellationToken cancellationToken)
  {
    var filter = Builders<TDocument>.Filter.Eq(d => d.Id, document.Id);
    await _collection.FindOneAndReplaceAsync(
      filter,
      document,
      cancellationToken: cancellationToken
    );
  }

  public async Task UpdateMany(
    IEnumerable<TDocument> documents,
    bool upsert = true,
    CancellationToken cancellationToken = default
  )
  {
    var updates = new List<WriteModel<TDocument>>();
    var filterBuilder = Builders<TDocument>.Filter;

    foreach (var document in documents)
    {
      var filter = filterBuilder.Where(d => d.Id == document.Id);
      updates.Add(new ReplaceOneModel<TDocument>(filter, document) { IsUpsert = upsert });
    }

    await _collection.BulkWriteAsync(updates, cancellationToken: cancellationToken);
  }

  public async Task DeleteOne(TDocument document, CancellationToken cancellationToken)
  {
    var filter = Builders<TDocument>.Filter.Eq(d => d.Id, document.Id);
    await _collection.FindOneAndDeleteAsync(
      filter,
      cancellationToken: cancellationToken
    );
  }

  public async Task DeleteMany(
    Expression<Func<TDocument, bool>> filterExpression,
    CancellationToken cancellationToken
  )
  {
    await _collection.DeleteManyAsync(filterExpression, cancellationToken: cancellationToken);
  }

  public async Task DeleteMany(IEnumerable<TDocument> documents, CancellationToken cancellationToken)
  {
    var filter = Builders<TDocument>.Filter.In(d => d.Id, documents.Select(d => d.Id));

    await _collection.DeleteManyAsync(filter, cancellationToken: cancellationToken);
  }

  public async Task<bool> Exists(
    Expression<Func<TDocument, bool>> expression,
    CancellationToken cancellationToken
  )
  {
    try
    {
      var documents = await _collection.FindAsync(expression, cancellationToken: cancellationToken);
      return await documents.AnyAsync(cancellationToken: cancellationToken);
    }
    catch (InvalidOperationException)
    {
      return false;
    }
  }

  public void Dispose() { }
}
