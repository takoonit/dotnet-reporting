using MongoDB.Bson;
using MongoDB.Driver;
using Report_MS.Repository;

namespace Report_MS.Models
{
    namespace Repository
    {
        public class MongoRepository<T> : IMongoRepository<T>
        {
            private readonly IMongoCollection<T> _collection;

            public MongoRepository(string connectionString, string databaseName, string collectionName)
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(databaseName);
                _collection = database.GetCollection<T>(collectionName);
            }

            public async Task Create(T entity)
            {
                await _collection.InsertOneAsync(entity);
            }

            public async Task<T> GetById(string id)
            {
                var filter = Builders<T>.Filter.Eq("_id", id);
                return await _collection.Find(filter).FirstOrDefaultAsync();
            }

            public async Task<IEnumerable<T>> GetAll()
            {
                return await _collection.Find(_ => true).ToListAsync();
            }

            public async Task Update(T entity)
            {
                var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(GetIdFromEntity(entity)));
                await _collection.ReplaceOneAsync(filter, entity);
            }

            public async Task Delete(string id)
            {
                var filter = Builders<T>.Filter.Eq("_id", id);
                await _collection.DeleteOneAsync(filter);
            }

            private string GetIdFromEntity(T entity)
            {
                var propertyInfo = entity.GetType().GetProperty("Id");
                if (propertyInfo != null)
                {
                    var idValue = propertyInfo.GetValue(entity);
                    if (idValue != null) return idValue.ToString();
                }

                throw new ArgumentException("Entity does not have a valid ID property.");
            }
        }
    }
}