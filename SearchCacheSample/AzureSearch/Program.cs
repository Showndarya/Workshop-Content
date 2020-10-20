using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest.Azure;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AzureSearch
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(@"C:\Users\shmadhav\source\repos\SearchCacheSample\AzureSearch\appsettings.json");
            IConfigurationRoot configuration = builder.Build();

            string serviceName = configuration["SearchServiceName"];
            string searchServiceUri = configuration["SearchServiceUri"];
            string indexName = "books";
            string apiKey = configuration["SearchServiceAdminApiKey"];
            string queryKey = configuration["SearchServiceQueryKey"];
            string sqlConnectionString = configuration["AzureSQLConnectionString"];

            SearchServiceClient searchService = new SearchServiceClient(serviceName, new SearchCredentials(apiKey));

            Microsoft.Azure.Search.Models.Index index = new Microsoft.Azure.Search.Models.Index(indexName, FieldBuilder.BuildForType<Book>());

            bool exists = await searchService.Indexes.ExistsAsync(index.Name);
            if (exists)
            {
                await searchService.Indexes.DeleteAsync(index.Name);
            }

            await searchService.Indexes.CreateAsync(index);

            DataSource dataSource = DataSource.AzureSql("books-index", sqlConnectionString, "Book",
                new SoftDeleteColumnDeletionDetectionPolicy("IsDeleted", "true"));
            dataSource.DataChangeDetectionPolicy = new SqlIntegratedChangeTrackingPolicy();

            await searchService.DataSources.CreateOrUpdateAsync(dataSource);

            Indexer indexer = new Indexer(name: "books-indexer", dataSource.Name, index.Name, null, null, new IndexingSchedule(TimeSpan.FromDays(1)));

            exists = await searchService.Indexers.ExistsAsync(indexer.Name);
            if (exists)
            {
                await searchService.Indexers.ResetAsync(indexer.Name);
            }

            await searchService.Indexers.CreateOrUpdateAsync(indexer);

            try
            {
                // await searchService.Indexers.RunAsync(indexer.Name);
            }
            catch (CloudException e) when (e.Response.StatusCode == (HttpStatusCode)429)
            {
                Console.WriteLine("Failed to run indexer: {0}", e.Response.Content);
            }

            SearchClient searchclient = new SearchClient(new Uri(searchServiceUri), indexName, new Azure.AzureKeyCredential(queryKey));
            RunQueries(searchclient);
        }

        private static void RunQueries(SearchClient srchclient)
        {
            SearchOptions options;
            SearchResults<Book> response;

            Console.WriteLine("Query #1: Search on the term 'Book2'\n");

            options = new SearchOptions() { Filter = "", OrderBy = { "" } };

            response = srchclient.Search<Book>("Book2", options);
            DisplayResults(response);

            Console.WriteLine("Query #2: Find hotels where 'Author' equals Author1...\n");

            options = new SearchOptions() { Filter = "Author eq 'Author1'", };

            response = srchclient.Search<Book>("*", options);
            DisplayResults(response);

            Console.WriteLine("Query #3: Filter on copies less than 5 and sort by book name descending...\n");

            options = new SearchOptions() { Filter = "Copies lt 5",OrderBy = { "Name desc" } };

            response = srchclient.Search<Book>("*", options);
            DisplayResults(response);
        }

        private static void DisplayResults(SearchResults<Book> searchResults)
        {
            foreach (var response in searchResults.GetResults())
            {
                Book doc = response.Document;
                var score = response.Score;
                Console.WriteLine($"Book: {doc.Name}, Author: {doc.Author}, Genre: {doc.Genre}, Copies: {doc.Copies}, IsDeleted: {doc.IsDeleted}");
            }

            Console.WriteLine();
        }
    }


}
