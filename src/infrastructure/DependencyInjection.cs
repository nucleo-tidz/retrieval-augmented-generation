using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel.Memory;

namespace infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSemanticKernel(this IServiceCollection services, IConfiguration configuration)
        {
            //https://johnnyreilly.com/using-kernel-memory-to-chunk-documents-into-azure-ai-search
            return services.AddTransient(serviceProvider =>
            {
                RedisConfig redisConfig = new RedisConfig
                {
                    ConnectionString = configuration.GetConnectionString("Redis"),
                };
                redisConfig.Tags.Add("collection", ',');
                redisConfig.Tags.Add("__part_n", ',');
                IKernelMemoryBuilder memoryBuilder = new KernelMemoryBuilder().
                WithAzureBlobsDocumentStorage(new AzureBlobsConfig
                {
                    ConnectionString = configuration.GetConnectionString("AzureBlobStorage"),
                    Container = "docs",
                    Auth = AzureBlobsConfig.AuthTypes.ConnectionString

                })
                .WithAzureOpenAITextEmbeddingGeneration(new AzureOpenAIConfig
                {
                    APIType = AzureOpenAIConfig.APITypes.EmbeddingGeneration,
                    Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                    APIKey = configuration["APIKey"],
                    Endpoint = "https://nucleo-tidz.openai.azure.com/",
                    Deployment = "vectoriser"

                })
                .WithAzureOpenAITextGeneration(new AzureOpenAIConfig
                {
                    APIType = AzureOpenAIConfig.APITypes.ChatCompletion,
                    Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                    APIKey = configuration["APIKey"],
                    Endpoint = "https://nucleo-tidz.openai.azure.com/",
                    Deployment = "gpt-4o"
                }).WithRedisMemoryDb(redisConfig);
#if false
                }).WithSqlServerMemoryDb(
                    new Microsoft.KernelMemory.MemoryDb.SQLServer.SqlServerConfig
                    {
                        ConnectionString = 
                    });
#endif 
                return memoryBuilder.Build();
            });
        }

    }
}
