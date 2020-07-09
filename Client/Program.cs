using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using System;
using System.Threading.Tasks;

namespace Fork
{
    public class Program
    {
        static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                using (var client = await ConnectClient())
                {
                    await DoClientWork(client);
                    Console.ReadKey();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nException while trying to run client: {e.Message}");
                Console.WriteLine("Make sure the silo the client is trying to connect to is running.");
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
                return 1;
            }
        }

        private static async Task<IClusterClient> ConnectClient()
        {
            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansBasics";
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IFork).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }

        private static async Task<IFork> GetEmptyGame(IClusterClient client) 
        {
            bool gameFound = false;
            int i = 0;
            IFork friend = null;
            while (!gameFound) 
            {
                friend = client.GetGrain<IFork>(i);
                bool hasPlayerInGame = await friend.HasPlayer();
                if (hasPlayerInGame) 
                {
                    i++;
                }
                else 
                {
                    gameFound = true;
                }
            }
            return friend;
        }

        private static async Task DoClientWork(IClusterClient client)
        {
            // example of calling grains from the initialized client
            var friend = await GetEmptyGame(client);
            var response = await friend.SayHello("Good morning, HelloGrain!");
            Console.WriteLine($"\n\n{response}\n\n");

            Console.WriteLine("Do you want to play? What's your name?");
            string name = Console.ReadLine();

            var player = client.GetGrain<IPlayer>(Guid.NewGuid());
            player.SetName(name).Wait();

            name = player.Name().Result;
            Console.WriteLine($"Greetings {name}, nice to meet you");

            await player.SetForkGame(friend);

            response = await player.GetCurrentWord();
            Console.WriteLine($"\n\n{response}\n\n");

            bool isGameOver = false;
            while (!isGameOver) 
            {
                Console.WriteLine("Type the letter to find the word: ");
                string letterString = Console.ReadLine();
                if (letterString.Length > 1) 
                {
                    Console.WriteLine("Try again, just ONE letter");
                    continue;
                }
                char letter = char.Parse(letterString);    
                isGameOver = await player.CheckLetter(letter);
                response = await player.GetCurrentWord();
                Console.WriteLine($"The Word is: {response}|");
            }

            Console.WriteLine("\n Great Job! Congratulations!! You Win the Game!");
            player.ExitGame();
        }
    }
}