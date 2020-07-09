using System.Threading.Tasks;

namespace Fork
{
    public interface IFork : Orleans.IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);

        Task<string> SelectWord();
        Task<string> HasLetter(char letter);
        Task<string> TheWord();
        Task<bool> IsWordFound();

        Task Start(Player player);
        
        Task<bool> HasPlayer();
        Task RemovePlayerFromGame();
    }
}