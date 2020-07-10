using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Fork
{
    public class ForkGrain : Orleans.Grain, IFork
    {
        private readonly ILogger logger; 
        private string[] words = {"GEARS", "HALO", "ORI", "TOMB RAIDER", "FIFA", "SUPER MARIO WORLD", "ZELDA", "MEGA MAN", "LIFE OF PIGEON"};

        private string theWord;
        private int wordIndex;
        private bool foundWord = false;

        private Player playerConnected;
        //private EventHub eventuHubConnection;

        Task IFork.Start(Player player) 
        {
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

        Task<string> IFork.SelectWord() 
        {
            int randomNumber = random.Next(0, words.Length);
            logger.LogInformation($"\n Trying to select the word");
            wordIndex = randomNumber;
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
            return Task.FromResult(theWord); 

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

    }
}