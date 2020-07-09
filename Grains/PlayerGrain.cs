using Orleans;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Fork
{
    public class PlayerGrain : Orleans.Grain, IPlayer
    {
        Player player;

        public IFork currentGame; // Current word

        public override Task OnActivateAsync()
        {
            this.player = new Player { Key = this.GetPrimaryKey(), Name = "nobody" };
            return base.OnActivateAsync();
        }

        Task IPlayer.SetForkGame(IFork game)
        {
            this.currentGame = game;
            game.Start(player);
            game.SelectWord();
            return Task.CompletedTask; 

        }

        Task<string> IPlayer.Name() 
        {
            return Task.FromResult(player.Name);
        }

        Task IPlayer.SetName(string name)
        {
            this.player.Name = name;
            return Task.CompletedTask;
        }

        Task<int> IPlayer.Points() 
        {
            return Task.FromResult(player.Points);
        }

        Task IPlayer.IncrementPoints(int newPoints) 
        {
            this.player.Points += newPoints;
            return Task.CompletedTask;
        }

        Task<IFork> IPlayer.GetCurrentGame() 
        {
            return Task.FromResult(currentGame);
        }

        Task<string> IPlayer.GetCurrentWord() 
        {
            return currentGame.TheWord();
        }

        Task<bool> IPlayer.CheckLetter(char letter) 
        {
            currentGame.HasLetter(letter);
            return currentGame.IsWordFound();
        }

        Task IPlayer.ExitGame() 
        {
            currentGame.RemovePlayerFromGame();
            return Task.CompletedTask;
        }
    }
}