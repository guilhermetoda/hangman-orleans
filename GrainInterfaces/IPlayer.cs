using Orleans;
using System.Threading.Tasks;

namespace Fork
{
    public interface IPlayer : IGrainWithGuidKey
    {
        Task SetForkGame(IFork game);
        Task<bool> GetUserFromDB(string playerName);

        Task<string> Name();
        Task SetName(string name);

        Task<int> Points();
        Task IncrementPoints(int newPoints);

        Task<IFork> GetCurrentGame();
        Task<string> GetCurrentWord();
        Task<bool> CheckLetter(char letter);

        Task ExitGame();

        Task<int> GetLives();
        Task<bool> IsPlayerDead();

        Task<Player> GetPlayerInfo();

        
    }
}