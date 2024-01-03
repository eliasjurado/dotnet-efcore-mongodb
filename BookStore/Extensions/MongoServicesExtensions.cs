using BookStore.Models;
using MongoDB.Driver;

namespace BookStore.Extensions
{
    public static class MongoServicesExtensions
    {
        public static void AddMongo(this WebApplicationBuilder builder)
        {
            var options = builder.Configuration.GetSection(nameof(ApiSettings)).Get<ApiSettings>();

            builder.Services.AddScoped(_ => new MongoClient(options.ConnectionString));

            builder.Services.AddScoped(svc =>
            {
                var client = svc.GetRequiredService<MongoClient>();
                var database = client.GetDatabase(options.DatabaseName);
                return database;
            });

            // register collections
            builder.Services.AddScoped(svc =>
            {
                var db = svc.GetRequiredService<IMongoDatabase>();
                return db.GetCollection<Book>(options.BooksCollectionName ?? Book.Collection);
            });

        }
    }
}
