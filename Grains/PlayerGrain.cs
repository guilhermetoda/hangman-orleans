using Orleans;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Fork
{
    public class PlayerGrain : Orleans.Grain, IPlayer
    {
        Player player;

        public IFork currentGame; // Current word

        private int lives;
        private const int NUMBER_OF_LIVES = 5;
        private bool dead = false;

        public override Task OnActivateAsync()
        {
            this.player = new Player { Key = this.GetPrimaryKey(), Name = "nobody" };
            return base.OnActivateAsync();
        }

        async Task<bool> IPlayer.GetUserFromDB(string playerName) 
        {
            Player responseFromDB = await CosmosDB.GetPlayerFromDatabase(playerName);
            if (responseFromDB!= null) 
            {
                player = responseFromDB;
                return true;
            }
            return false;
        }

        Task IPlayer.SetForkGame(IFork game)
        {
            this.currentGame = game;
            lives = NUMBER_OF_LIVES;
            game.Start(player);
            game.SelectWord();
            return Task.CompletedTask; 

        }

        Task<int> IPlayer.GetLives() 
        {
            return Task.FromResult(lives);
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
            if (this.player.Points < 0) 
            {
                this.player.Points = 0;
            }
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

        Task<bool> CheckAlive() 
        {
            if (lives == 0) 
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        Task<bool> IPlayer.IsPlayerDead() 
        {
            return Task.FromResult(dead);
        }

        async Task<bool> IPlayer.CheckLetter(char letter) 
        {
            bool hasLetter = await currentGame.HasLetter(letter);
            if (!hasLetter)
            {
                lives -= 1;
                dead = await this.CheckAlive();
            }
            bool isWordFound = await currentGame.IsWordFound();
            return isWordFound;
        }

        Task IPlayer.ExitGame() 
        {
            EventHub.SendMessage(player).Wait(3);
            currentGame.RemovePlayerFromGame();
            return Task.CompletedTask;
        }

        Task<Player> IPlayer.GetPlayerInfo() 
        {
            return Task.FromResult(player);
        }
    }
}