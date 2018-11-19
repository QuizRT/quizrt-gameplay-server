using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace  gameplay_back.Models
{
    public class GameFunctions : IGameRepository {

        efmodel database = null;

        public GameFunctions(efmodel  obj)
        {
            this.database=obj;
        }

        public bool PostGame(User user, string topic, int no_of_players){
            using (database)
            {
                    Guid guid= Guid.NewGuid();
                    Game newGame = new Game();
                    newGame.GameId = guid.ToString();
                    newGame.NumberOfPlayersRequired = no_of_players;
                    newGame.Topic=topic;
                    newGame.QuestionTimeout=10;
                    newGame.PendingGame=true;
                    newGame.Questions=null;
                    newGame.NumberOfPlayersJoined=1;
                    newGame.GameOver=false;
                    newGame.GameStarted=false;
                    newGame.Users.Add(user);
                    database.game.Add(newGame);

                    database.Users.Add(user);
                    database.SaveChanges();
                    return true;
                }
                // else{
                //     return false;
                // }
            }
        }


}
