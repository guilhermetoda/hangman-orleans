using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
//using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Fork
{
    public class CosmosDB
    {
        //Read configuration
        private static readonly string cosmosDatabaseId = "outDatabase";
        private static readonly string containerId = "MyCollection";

        private static readonly string endpoint = "https://raining-takos-sql.documents.azure.com:443/";
        private static readonly string authKey = "D0DBk0uyXgRFkionUyJZzMXZN7l6fmyKyPmiUkuovEl1phdkPUztsE2AHtX0y33qLEA59WvXOdvP20IdTf2jbQ==";

        private static Database cosmosDatabase = null;
        private static Container container;
        private static CosmosClient cosmosClient;

        public static async Task ConnectToCosmos()
        {
            Console.WriteLine("Connecting to Cosmos DB");
            try 
            {
                cosmosClient = new CosmosClient(endpoint, authKey);
                cosmosDatabase = cosmosClient.GetDatabase(cosmosDatabaseId);
                container = cosmosDatabase.GetContainer(containerId);
                container = await container.ReadContainerAsync();
                
            }
            catch (CosmosException ex) 
            {
                Console.WriteLine("Couldn't connect to the COsmos DB");
                Console.WriteLine(ex);
            }
            
            
            Console.WriteLine("Cosmos DB connected");
        }

        public static async Task<Player> GetPlayerFromDatabase(string playerName)
        {
            // Query using two properties within each item. WHERE Id == "" AND Address.City == ""
            // notice here how we are doing an equality comparison on the string value of City
            Console.WriteLine("Executing Query");
            QueryDefinition query = new QueryDefinition("SELECT * FROM c WHERE c.Name = @name")
                .WithParameter("@name", playerName);

            List<Player> results = new List<Player>();
            using (FeedIterator<Player> resultSetIterator = container.GetItemQueryIterator<Player>(
                query))
            {
                
                //Console.WriteLine(resultSetIterator.HasMoreResults);
                while (resultSetIterator.HasMoreResults)
                {
                    
                    FeedResponse<Player> response = await resultSetIterator.ReadNextAsync();
                    results.AddRange(response);
                    Console.WriteLine(response.StatusCode);
                    if (response.Diagnostics != null)
                    {
                        Console.WriteLine($"\nQueryWithSqlParameters Diagnostics: {response.Diagnostics.ToString()}");
                    }
                    Console.WriteLine(results);
                }

                //Assert("Expected only 1 family", results.Count == 1);
            }
            if (results.Count > 0) 
            {
                return results[0];
            }
            
            return null;
            
        }

    }
    
}