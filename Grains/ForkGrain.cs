using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fork
{
    public class ForkGrain : Orleans.Grain, IFork
    {
        private readonly ILogger logger; 
        private string[] words = {"GEARS", "HALO", "ORI AND THE BLIND FOREST", "TOMB RAIDER", "FIFA", "SUPER MARIO WORLD", "ZELDA", "MEGA MAN", "LIFE OF PIGEON", "MINECRAFT", "SUNSET OVERDRIVE", "METAL GEAR SOLID", "GTA", "NBA 2K", "SEA OF THIEVES"};

        private string theWord;
        public int wordIndex;
        private bool foundWord = false;

        private Player playerConnected;
        //private EventHub eventuHubConnection;

        Task IFork.Start(Player player) 
        {
            Console.WriteLine("A player has connected");
            playerConnected = player;
            random = new System.Random();
            theWord = "";
            foundWord = false;
            
            return Task.CompletedTask;
        }
        
        // Temp
        System.Random random;

        Task<bool> IFork.HasPlayer() 
        {
            logger.LogInformation("Checking if has player in the game");
            return Task.FromResult(playerConnected != null);
        }

        public ForkGrain(ILogger<ForkGrain> logger)
        {
            this.logger = logger;
        }

        Task<string> IFork.SayHello(string greeting)
        {
            logger.LogInformation($"\n SayHello message received: greeting = '{greeting}'");
            return Task.FromResult($"\n Client said: '{greeting}', so HelloGrain says: Hello!");
        }

        Task<bool> IFork.SelectWord() 
        {
            bool selected = false;
            logger.LogInformation("Selecting Word...");
            if (words.Length == playerConnected.GamesGuessed.Count) 
            {
                logger.LogInformation($"\nWord not selected");
                return Task.FromResult(false);
            }
            logger.LogInformation("Before loop Loop");
            while (!selected) 
            {
                logger.LogInformation("Selecting Loop");
                int randomNumber = random.Next(0, words.Length);
                wordIndex = randomNumber;
                logger.LogInformation($"\n Trying to select the word {words[wordIndex]}");
                selected = !IsWordPlayedBefore(wordIndex);
            }
            
            //Fill the word with -
            for (int i = 0; i < words[wordIndex].Length; ++i) 
            {
                if (words[wordIndex][i] == ' ') 
                {
                    theWord += ' ';
                    continue;
                }    
                theWord += '_';
            }
            logger.LogInformation($"\n The word is: {words[wordIndex]}");
            return Task.FromResult(true); 

        }

        private bool IsWordPlayedBefore(int index) 
        {
            for (int i = 0; i< playerConnected.GamesGuessed.Count; i++) 
            {
                if (index == playerConnected.GamesGuessed[i]) 
                {
                    return true;
                }
            }
            return false;
            
        }

        Task<bool> IFork.HasLetter(char letter) 
        {
            int i;
            letter = char.ToUpper(letter);
            char[] theWordChar = theWord.ToCharArray();
            bool needsLetter = false;
            bool hasTheLetter = false;
            for (i = 0; i < words[wordIndex].Length; ++i) 
            {
                if (words[wordIndex][i] == letter) 
                {
                    theWordChar[i] = letter;
                    hasTheLetter = true;
                }

                if (theWordChar[i] == '_') 
                {
                    needsLetter = true;
                }
            }
            foundWord = !needsLetter;
            theWord = new string(theWordChar);
            logger.LogInformation($"\n Checking word: {words[wordIndex]} - {theWord}");
            return Task.FromResult(hasTheLetter);
        }

        Task<int> IFork.WordIndex() 
        {
            return Task.FromResult(wordIndex);
        }

        Task<string> IFork.TheWord() 
        {
            return Task.FromResult(theWord);
        }

        Task<bool> IFork.IsWordFound() 
        {
            logger.LogInformation("Checking if the word was found");
            return Task.FromResult(foundWord);
        }

        Task IFork.RemovePlayerFromGame() 
        {
            playerConnected = null;
            return Task.CompletedTask;
        }

        Task<string> IFork.WordsFound(List<int> indexList) 
        {
            string output = "";
            for (int i = 0; i < indexList.Count; i++)  
            {
                output += words[indexList[i]]+ "  ";
            }
            return Task.FromResult(output);
        }

    }
}