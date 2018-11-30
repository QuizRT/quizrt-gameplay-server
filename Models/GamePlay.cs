using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Threading;
using System.IO;
using GamePlay.Hubs;
using System.Net.Http;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

// using System.Timers;
namespace GamePlay.Models
{
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
        public bool GameOver { get; set; }
        public bool GameStarted { get; set; }
        public bool PendingGame { get; set; }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
        public Game(string username, string topic, int NumberOfPlayers)
        {
            String timeStamp = GetTimestamp(DateTime.Now);
            GameId = topic + timeStamp;
            QuestionTimeout = 10;
            NumberOfPlayersRequired = NumberOfPlayers;
            NumberOfPlayersJoined = 1;
            Topic = topic;
            Users = new List<string>();
            Users.Add(username);
            GameOver = false;
            GameStarted = false;
            PendingGame = true;
        }

        public void AddUsersToGame(string username)
        {
            Users.Add(username);
            NumberOfPlayersJoined++;
        }
        public Game() { }

    }

    public class  GamePlay
    {
        HttpClient http;
        IHubContext<GamePlayHub> _hub;
        public GamePlay(IHubContext<GamePlayHub> _gamePlayHub)
        {
            _hub = _gamePlayHub;
            http = new HttpClient();
        }

        public async Task StartGame(Game game)
        {
             HttpResponseMessage response = await this.http.GetAsync("http://172.23.238.164:7000/questiongenerator/questions/book");
            HttpContent content = response.Content;
            string data = await content.ReadAsStringAsync();
            JArray json = JArray.Parse(data);
            Random random = new Random();
            // Console.WriteLine("--OO--"+json[0]["questionsList"][random.Next(0,5)]);
            await _hub.Clients.Group(game.GameId).SendAsync("QuestionsReceived", json[random.Next(0, 3)]["questionsList"][random.Next(0, 5)]);
        }

        public async Task NotifyOnNoOpponentFound()
        {
            await _hub.Clients.All.SendAsync("NotifyOnNotFound", "Clients not found..");
        }
    }

    public class GamePlayManager
    {
        static List<Game> RunningGames = new List<Game>();
        static ICollection<Game> PendingGames = new List<Game>();
        static GamePlay gamePlay;
        public GamePlayManager(GamePlay _gamePlay)
        {
            gamePlay = _gamePlay;
        }
        public string CreateGame(string username, string topic, int noOfPlayers)
        {
            Game game;
            if (noOfPlayers == 1)
            {
                game = new Game(username, topic, noOfPlayers);
                RunningGames.Add(game);
                return game.GameId;
            }
            else
            {
                // Console.WriteLine("came here 1");
                game = PendingGames.FirstOrDefault(t => t.Topic == topic && t.NumberOfPlayersRequired == noOfPlayers);
                if (game != null)
                {
                    Console.WriteLine("Existing Game found..add user to it " + game.GameId);
                    game.AddUsersToGame(username);

                }
                else
                {
                    Console.WriteLine("No existing game found.. Creating a new game...");
                    game = new Game(username, topic, noOfPlayers);
                    PendingGames.Add(game);
                    var autoEvent = new AutoResetEvent(false);
                    var timer = new Timer(OnTimerElapsed, game, 10000, -1);
                }
                return game.GameId;
            }
        }

        public static void OnTimerElapsed(Object stateInfo)
        {
            Game game = (Game)stateInfo;

            if (game.NumberOfPlayersJoined != game.NumberOfPlayersRequired)
            {
                Console.WriteLine("Conditions not met... destroying game object...");
                PendingGames.Remove(game);
                gamePlay.NotifyOnNoOpponentFound();
            }
            else
            {
                Console.WriteLine("Conditions met... game will start");
                RunningGames.Add(game);
                PendingGames.Remove(game);
                gamePlay.StartGame(game);
            }
        }

        public ICollection<Game> GetPendingGames()
        {
            return PendingGames;
        }
    }

}
