using System.Threading.Tasks;
using System.Collections.Generic;

namespace Fork
{
    public interface IFork : Orleans.IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);

        Task<bool> SelectWord();
        Task<bool> HasLetter(char letter);
        Task<string> TheWord();
        Task<int> WordIndex();
        Task<bool> IsWordFound();
        Task<string> WordsFound(List<int> indexList);

        Task Start(Player player);
        
        Task<bool> HasPlayer();
        Task RemovePlayerFromGame();
    }
}