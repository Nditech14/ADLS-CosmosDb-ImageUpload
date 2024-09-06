using Cosmos.Application.Entities;
using Cosmos.Application.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Cosmos.Application.Services;
namespace WebApi.Extension
{
    public static class ExtensionService
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register CosmosClient as Singleton
            services.AddSingleton<CosmosClient>(provider =>
            {
                var cosmosDbConfig = configuration.GetSection("CosmosDb");
                var account = cosmosDbConfig["Account"];
                var key = cosmosDbConfig["Key"];
                return new CosmosClient(account, key);
            });

            // Register CosmosDbService<Product> as Singleton
            services.AddSingleton<ICosmosDbService<Product>>(provider =>
            {
                var cosmosClient = provider.GetRequiredService<CosmosClient>();
                return new CosmosDbService<Product>(cosmosClient, configuration);
            });

            // Register CosmosDbService<Category> as Singleton
            services.AddSingleton<ICosmosDbService<Category>>(provider =>
            {
                var cosmosClient = provider.GetRequiredService<CosmosClient>();
                return new CosmosDbService<Category>(cosmosClient, configuration);
            });

            // Register CosmosDbService<SubCategory> as Singleton if needed
            services.AddSingleton<ICosmosDbService<SubCategory>>(provider =>
            {
                var cosmosClient = provider.GetRequiredService<CosmosClient>();
                return new CosmosDbService<SubCategory>(cosmosClient, configuration);
            });



            // Register ProductService as Scoped
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISubCategoryService,SubCategoryService>();

            return services;
        }
    }
}
