using System.Reflection;
using dtc.Domain.Entities.Blogs;
using dtc.Domain.Entities.Classes;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Entities.Location;
using dtc.Domain.Entities.Notifications;
using dtc.Domain.Entities.Training;
using dtc.Infrastructure.Persistence.MongoDB;
using MongoDB.Driver;

namespace dtc.Infrastructure.Persistence.Seeding
{
    public static class MongoDbSeeder
    {
        public static async Task SeedAsync(MongoDBContext context, CancellationToken cancellationToken = default)
        {
            await UpsertManyAsync(context.Blogs, MongoSeedData.Blogs, cancellationToken);
            await UpsertManyAsync(context.Categories, MongoSeedData.Categories, cancellationToken);
            await UpsertManyAsync(context.Questions, MongoSeedData.Questions, cancellationToken);
            await UpsertManyAsync(context.SampleExams, MongoSeedData.SampleExams, cancellationToken);
            await UpsertManyAsync(context.SampleExamQuestions, MongoSeedData.SampleExamQuestions, cancellationToken);
            await UpsertManyAsync(context.Addresses, MongoSeedData.Addresses, cancellationToken);
            await UpsertManyAsync(context.LearningLocations, MongoSeedData.LearningLocations, cancellationToken);
            await UpsertManyAsync(context.Notifications, MongoSeedData.Notifications, cancellationToken);
            await UpsertManyAsync(context.UserNotifications, MongoSeedData.UserNotifications, cancellationToken);
            await UpsertManyAsync(context.LearningRoadmaps, MongoSeedData.LearningRoadmaps, cancellationToken);
            await UpsertManyAsync(context.ResourceLearnings, MongoSeedData.ResourceLearnings, cancellationToken);
        }

        private static async Task UpsertManyAsync<T>(
            IMongoCollection<T> collection,
            IEnumerable<T> items,
            CancellationToken cancellationToken) where T : class
        {
            foreach (var item in items)
            {
                var id = GetIdValue(item);
                var filter = Builders<T>.Filter.Eq("Id", id);

                await collection.ReplaceOneAsync(
                    filter,
                    item,
                    new ReplaceOptions { IsUpsert = true },
                    cancellationToken);
            }
        }

        private static object GetIdValue<T>(T entity)
        {
            var property = GetProperty(entity!.GetType(), "Id")
                ?? throw new InvalidOperationException($"Entity type '{entity!.GetType().FullName}' does not expose an Id property.");

            return property.GetValue(entity)
                ?? throw new InvalidOperationException($"Entity type '{entity.GetType().FullName}' has a null Id value.");
        }

        private static PropertyInfo? GetProperty(Type type, string propertyName)
        {
            while (type != typeof(object) && type != null)
            {
                var property = type.GetProperty(
                    propertyName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (property != null)
                {
                    return property;
                }

                type = type.BaseType!;
            }

            return null;
        }
    }
}
