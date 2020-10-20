using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace AzureCache
{
    class Program
    {
        static void Main(string[] args)
        {
            IDatabase cache = Connection.GetDatabase();

            cache.StringSet("key1", "value");
            Connection.GetDatabase(1).StringSet("key2", 25, TimeSpan.FromMinutes(1));
            
            string key1 = cache.StringGet("key1");
            int key2 = (int)cache.StringGet("key2");
            Console.Write(key1);
            Console.Write(key2);

            cache.StringSet("book1", JsonConvert.SerializeObject(new Book { Name = "Book1", Genre = "Genre1", Author = "Author1" }));
            Console.WriteLine(JsonConvert.DeserializeObject<Book>(cache.StringGet("book1")).Genre);
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        { 
            return ConnectionMultiplexer.Connect(""); 
        });
        public static ConnectionMultiplexer Connection 
        { 
            get 
            { 
                return lazyConnection.Value; 
            } 
        }
    }
}
