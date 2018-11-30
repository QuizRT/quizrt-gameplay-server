using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using GamePlay.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;


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
        public  bool GameOver{get; set;}
        public bool GameStarted{get; set;}
        public bool PendingGame{get; set;}

        public static String GetTimestamp(DateTime value) {
            return value.ToString("yyyyMMddHHmmssffff");
        }
        public Game (string username, string topic, int numberOfPlayers) 
        {
            String timeStamp = GetTimestamp(DateTime.Now);
            GameId = topic + timeStamp ;
            QuestionTimeout = 10;
            NumberOfPlayersRequired = numberOfPlayers;
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
    }
    
    public class GamePlay
    {
        private IHubContext<GamePlayHub> _hub;
        public HttpClient _http;
        public GamePlay(IHubContext<GamePlayHub> gamePlayHub)
        {
            this._hub = gamePlayHub;
            this._http = new HttpClient();
        }
        public async Task StartGame(Game game)
        {
            Console.WriteLine("Starting the Game");
            HttpResponseMessage response = await this._http.GetAsync("http://172.23.238.164:8080/api/quizrt/questions/book");
            HttpContent content = response.Content;
            string data = await content.ReadAsStringAsync();
            JArray json = JArray.Parse(data);
            Random random = new Random();
            // Console.WriteLine("--OO--"+json[0]["questionsList"][random.Next(0,5)]);
            await _hub.Clients.Group(game.GameId).SendAsync("QuestionsReceived", json[random.Next(0, 3)]["questionsList"][random.Next(0, 5)]);
        }

        public async Task NotifyNoOpponentsFound(Game game)
        {
            Console.WriteLine("Notifying No Opponents Found");
            await _hub.Clients.Group(game.GameId).SendAsync("NoOpponentsFound");
        }
    }

    public class GamePlayManager
    {
        static List<Game> RunningGames = new List<Game>();
        static ICollection<Game> PendingGames = new List<Game>();

        private GamePlay _gamePlay;
        
        public GamePlayManager(GamePlay gamePlay)
        {
            _gamePlay = gamePlay;
        }

        public void OnTimerElapsed(object stateInfo)
        {
            var game = (Game)stateInfo;
            if (game.NumberOfPlayersJoined != game.NumberOfPlayersRequired)
            {
                Console.WriteLine("TimerElapsed Hence Removing the Pending Game Object");
                PendingGames.Remove(game);
                _gamePlay.NotifyNoOpponentsFound(game);
            }
            else
            {
                Console.WriteLine("TimerElapsed. Met the conditions of the game. Starting with the Game.");
                RunningGames.Add(game);
                PendingGames.Remove(game);
                _gamePlay.StartGame(game);             
            }
        }

        public string CreateGame(string username, string topic, int noOfPlayers)
        {
            Game game;
            if (noOfPlayers == 1)
            {
                game =  new Game(username, topic, noOfPlayers);
                RunningGames.Add(game);
            }
            else
            {
                game = PendingGames.Where(p => p.Topic == topic && p.NumberOfPlayersRequired == noOfPlayers).FirstOrDefault();

                if (game is null)
                {
                    Console.WriteLine("No existing Game. Hence creating a New Game.");
                    game = new Game(username, topic, noOfPlayers);
                    PendingGames.Add(game);
                    // var gamePlay = new GamePlay(game, startGame, notifyNoOpponentsFound);
                    var timer = new System.Threading.Timer(OnTimerElapsed, game, 10000, -1);
                }
                else
                {
                    Console.WriteLine("Adding Player to the existing Game");
                    game.AddUsersToGame(username);
                }
            }
            return game.GameId;
        }

        public ICollection<Game> GetPendingGames()
        {
            return PendingGames;
        }
    }

}
