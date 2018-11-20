using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Threading;
using System.IO;
namespace gameplay_back.Models {

//    public class User
//    {
//        string username;
//        string topic;
//        int no_of_players;


//    }
    public class Questions
    {
        public int QuestionsId { get; set; }
        public string Categ { get; set; }
        public string Topic { get; set; }
        public string QuestionGiven { get; set; }
        public List<Options> QuestionOptions { get; set; }
    }
    public class Options
    {
        public int OptionsId { get; set; }
        public string OptionGiven { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionsId { get; set; }
    }

    public class Game
    {
        public string GameId { get; set; }
        public int QuestionTimeout { get; set; }
        // public List<Questions> Questions { get; set; }
        public int NumberOfPlayersRequired { get; set; }
        public int NumberOfPlayersJoined { get; set; }
        public string Topic { get; set; }
        public List<string> Users { get; set; }
        public  bool GameOver{get; set;}
        public bool GameStarted{get; set;}
        public bool PendingGame{get; set;}

        public Game (string username, string topic, int NumberOfPlayers) {
            Guid guid= new Guid();
            GameId = guid.ToString();
            QuestionTimeout = 10;
            NumberOfPlayersRequired = NumberOfPlayers;
            NumberOfPlayersJoined = 1;
            Topic = topic;
            Users.Add(username);
            GameOver=false;
            GameStarted=false;
            PendingGame=true;
        }
        public Game AddUsersToGame(string username, Game game)
        {
            game.Users.Add(username);
            game.NumberOfPlayersJoined++;
            Stopwatch stopwatch= new Stopwatch();
            while (game.NumberOfPlayersJoined<game.NumberOfPlayersRequired && stopwatch.ElapsedMilliseconds<=60000)
            {
                Thread.Sleep(1000);
            }
            stopwatch.Stop();
            if (game.NumberOfPlayersJoined==game.NumberOfPlayersRequired)
            {
                GamePlayManager gamePlayManager = new GamePlayManager();
                game = gamePlayManager.TransferFromPendingGamesToRunningGames(game);
                return game;
            }
            else
            {
                game=null;
            }
            return game;
        }
        public Game() {}

    }

    public class GamePlayManager
    {
        public Dictionary<string, Game> PendingGames{get; set;}
        public List<Game> RunningGames{get; set;}

        public Game Create_Game(string username, string topic, int no_Of_Players)
        {
            //Where to create the gameplay object. Here or globally
             Game game = new Game(username, topic, no_Of_Players);
            if (no_Of_Players==1)
            {
                RunningGames.Add(game);
            }
            else
            {
               PendingGames.Add(game.GameId,game);
            }
            return game;
        }

        public Game TransferFromPendingGamesToRunningGames(Game game)
        {
            RunningGames.Add(game);
            var remove = PendingGames.Where(p => p.Value ==  game); 
            
            return game;

        }
    }

}
