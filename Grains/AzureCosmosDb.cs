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

        private static readonly string endpoint = "";
        private static readonly string authKey = "";

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
                    if (response.Diagnostics != null)
                    {
                        Console.WriteLine($"\nQueryWithSqlParameters Diagnostics: {response.Diagnostics.ToString()}");
                    }
                }

                //Assert("Expected only 1 family", results.Count == 1);
            }
            if (results.Count > 0) 
            {
                Player playerResponse = new Player();
                playerResponse.Key = results[0].Key;
                playerResponse.Name = results[0].Name;
                playerResponse.FoundTheWord = false;
                for (int i =0 ; i < results.Count; i++) 
                {    
                    playerResponse.GamesGuessed.Add(playerResponse.WordIndex);
                }
                return playerResponse;
            }
            
            return null;
            
        }

    }
    
}