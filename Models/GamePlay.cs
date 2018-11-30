using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Threading;
using System.IO;
namespace GamePlay.Models {
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

        public static String GetTimestamp(DateTime value) {
            return value.ToString("yyyyMMddHHmmssffff");
        }
        public Game (string username, string topic, int NumberOfPlayers) {
            String timeStamp = GetTimestamp(DateTime.Now);
            GameId = topic + timeStamp ;
            QuestionTimeout = 10;
            NumberOfPlayersRequired = NumberOfPlayers;
            NumberOfPlayersJoined = 1;
            Topic = topic;
            Users = new List<string>();
            Users.Add(username);
            GameOver=false;
            GameStarted=false;
            PendingGame=true;
        }

        public Game AddUsersToGame(string username, Game game)
        {
            game.Users.Add(username);
            game.NumberOfPlayersJoined++;
            return game;
        }
        public Game() {}

    }

    public class GamePlayManager
    {
        static List<Game> RunningGames = new List<Game>();
        static ICollection<Game> PendingGames = new List<Game>();
        static GamePlayManager gameplaymanager = new GamePlayManager();
        // static ICollection<Game> games = new List<Game>();
        int flag = 0;

        static Game game = new Game();
        public Game CreateGame(string username, string topic, int noOfPlayers)
        {
            if (noOfPlayers == 1)
            {
                game =  new Game(username, topic, noOfPlayers);
                RunningGames.Add(game);
            }
            else
            {
                Console.WriteLine("came here 1");
                foreach(var eachGame in PendingGames)
                {
                    Console.WriteLine("came here 2");
                    if(eachGame.Topic == topic && noOfPlayers == eachGame.NumberOfPlayersRequired)
                    {

                        Console.WriteLine(eachGame.Topic + " " + eachGame.NumberOfPlayersRequired);
                        game = eachGame.AddUsersToGame(username, eachGame);
                        Console.WriteLine("came here 4");
                        flag = 1;
                        break;
                    }
                }
                // if (game.Topic == topic && noOfPlayers == game.NumberOfPlayersRequired)
                // {
                //     Console.WriteLine("came here 1");
                //     game = game.AddUsersToGame(username, game);
                // }
                if(flag==0)
                {
                    Console.WriteLine("came here 5");
                    game = new Game(username, topic, noOfPlayers);
                    PendingGames.Add(game);
                    Stopwatch stopwatch= new Stopwatch();
                    stopwatch.Start();

                    while (game.NumberOfPlayersJoined<game.NumberOfPlayersRequired && stopwatch.ElapsedMilliseconds<=20000)
                    {
                        Thread.Sleep(1000);

                    }
                    stopwatch.Stop();
                    if (game.NumberOfPlayersJoined==game.NumberOfPlayersRequired)
                    {
                        game = gameplaymanager.TransferFromPendingGamesToRunningGames(game);
                    }
                    else
                    {
                        game=null;
                    }
                }
            }
            return game;
        }

        public Game TransferFromPendingGamesToRunningGames(Game game)
        {
            RunningGames.Add(game);
            PendingGames.Remove(game);
            game.PendingGame=false;
            game.GameStarted= false;
            return game;
        }

        public ICollection<Game> GetPendingGames()
        {
            return PendingGames;
        }
    }

}
