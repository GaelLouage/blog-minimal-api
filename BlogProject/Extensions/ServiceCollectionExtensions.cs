using Infrastructuur.Constants;
using Infrastructuur.Entities;
using Infrastructuur.Repositories.Classes;

namespace BlogProject.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureServicesExtensions(this IServiceCollection services, IConfiguration config)
        {
            return services
                  .AddScoped(sp => Scope<User>(Collections.USER, config))
                  .AddScoped(sp => Scope<Blog>(Collections.BLOG, config));
        }
        public static MongoRepository<T> Scope<T>(string collectionName, IConfiguration config) where T : class
        {
            return new MongoRepository<T>(config.GetConnectionString("MongoDatabase"), Database.BLOG, collectionName);
        }
    }
}
