using System.Collections.Generic;
using gameplay_back.Models;

namespace gameplay_back.Models
{
    public class IGameRepository
    {
        private static GamePlayManager gameplaymanager;
        private static List<Game> game;
        private static IGameRepository instance=null;

        public static IGameRepository GetInstance()
        {
            if(instance==null)
            {
                instance=new IGameRepository();
            }
            return instance;
        }

        public void Add_Users_To_Game  (string user, int noOfPlayersJoined)
        {
            //Where users will get joined
        }

        public void Send_To_Pending_Games (string topic, Game game)
        {
            if (game.NumberOfPlayersJoined < game.NumberOfPlayersRequired)
            {
                gameplaymanager.PendingGames.Add(topic, game);
            }
            else
            {
                From_Pending_To_Running(game);
            }
        }

        public void  From_Pending_To_Running (Game game)
        {
            gameplaymanager.RunningGames.Add(game);
        }

        
    }
}

