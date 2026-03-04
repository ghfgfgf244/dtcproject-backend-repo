using dtc.Domain.Interfaces;
using dtc.Infrastructure.Persistence.MongoDB;
using MongoDB.Driver;
using System.Linq.Expressions;
using dtc.Domain.Entities; // for BaseEntity if using Guid Id

namespace dtc.Infrastructure.Repositories
{
    public class MongoGenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> _collection;

        public MongoGenericRepository(MongoDBContext context, string collectionName)
        {
            // Reflection mapping based on context, or pass direct collection
            _collection = (IMongoCollection<T>)context.GetType().GetProperty(collectionName)?.GetValue(context)!;
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            if (id is Guid guidId && typeof(BaseEntity).IsAssignableFrom(typeof(T)))
            {
                var filter = Builders<T>.Filter.Eq("Id", guidId);
                return await _collection.Find(filter).FirstOrDefaultAsync();
            }
            // For integer Ids like Address
            var filterInt = Builders<T>.Filter.Eq("Id", id);
            return await _collection.Find(filterInt).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            // Includes are generally ignored in simple MongoDB repository as documents contain embedded data.
            return await _collection.Find(predicate).ToListAsync();
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).AnyAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return (int)await _collection.CountDocumentsAsync(predicate);
        }

        public async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _collection.InsertManyAsync(entities);
        }

        public async Task UpdateAsync(T entity)
        {
            var idProperty = entity.GetType().GetProperty("Id");
            var idValue = idProperty?.GetValue(entity);
            if (idValue != null)
            {
                var filter = Builders<T>.Filter.Eq("Id", idValue);
                await _collection.ReplaceOneAsync(filter, entity);
            }
        }

        public async Task RemoveAsync(T entity)
        {
            var idProperty = entity.GetType().GetProperty("Id");
            var idValue = idProperty?.GetValue(entity);
            if (idValue != null)
            {
                var filter = Builders<T>.Filter.Eq("Id", idValue);
                await _collection.DeleteOneAsync(filter);
            }
        }

        public async Task RemoveRange(IEnumerable<T> entities)
        {
            var ids = new List<object>();
            foreach(var entity in entities)
            {
                var idProperty = entity.GetType().GetProperty("Id");
                var idValue = idProperty?.GetValue(entity);
                if(idValue != null)
                {
                    ids.Add(idValue);
                }
            }

            if(ids.Any())
            {
                var filter = Builders<T>.Filter.In("Id", ids);
                await _collection.DeleteManyAsync(filter);
            }
        }
    }
}
