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
        public string OptionGiven { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class User
    {
        public string username { get; set; }
        public int score { get; set; }

        public User(string _username, int _score)
        {
            username = _username;
            score = _score;
        }
    }

    public class Game
    {
        public string GameId { get; set; }
        public int QuestionCount { get; set; }
        // public List<Questions> Questions { get; set; }
        public int NumberOfPlayersRequired { get; set; }
        public int NumberOfPlayersJoined { get; set; }
        public string Topic { get; set; }
        public List<User> Users { get; set; }
        public JArray Questions { get; set; }
        public List<Options> options { get; set; }
        public bool GameOver { get; set; }
        public bool GameStarted { get; set; }
        public bool PendingGame { get; set; }
        HttpClient _http;

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
        public Game(string username, string topic, int numberOfPlayers)
        {
            String timeStamp = GetTimestamp(DateTime.Now);
            GameId = topic + timeStamp;
            QuestionCount = 0;
            NumberOfPlayersRequired = numberOfPlayers;
            NumberOfPlayersJoined = 1;
            Topic = topic;
            Users = new List<User>();
            Users.Add(new User(username, 0));
            _http = new HttpClient();
            Task<bool> vvv = System.Threading.Tasks.Task<string>.Run(() => GetQuestions().Result);
            Console.WriteLine(Questions+"---------------");
            GameOver = false;
            GameStarted = false;
            PendingGame = true;
        }

        public async Task<bool> GetQuestions()
        {
            HttpResponseMessage response = await this._http.GetAsync("http://172.23.238.164:7000/questiongenerator/questions/book");
            HttpContent content = response.Content;
            string data = await content.ReadAsStringAsync();
            Questions = JArray.Parse(data);
            return true;
            // Console.WriteLine(Questions);
        }

        public void AddUsersToGame(string username)
        {
            Users.Add(new User(username, 0));
            NumberOfPlayersJoined++;
        }
    }

    public class GamePlay
    {
        private IHubContext<GamePlayHub> _hub;

        public GamePlay(IHubContext<GamePlayHub> gamePlayHub)
        {
            _hub = gamePlayHub;

        }
        public async Task SendQuestions(Game game)
        {
            Console.WriteLine("Starting the Game");
            Random random = new Random();
            game.QuestionCount++;
            if (game.QuestionCount >= 7)
            {
                await _hub.Clients.Group(game.GameId).SendAsync("GameOver");
            }
            // options.Add(optionname and isCorrect)
            await _hub.Clients.Group(game.GameId).SendAsync("ProvideGroupId", game.GameId);
            Console.WriteLine(game.Questions[random.Next(0, 3)]["questionsList"][random.Next(0, 5)]);
            await _hub.Clients.Group(game.GameId).SendAsync("QuestionsReceived", game.Questions[random.Next(0, 3)]["questionsList"][random.Next(0, 5)]);
            // await _hub.Clients.Group(game.GameId).SendAsync("QuestionsReceived", "Hii", game.GameId);
            // await _hub.Clients.Group(game.GameId).SendAsync("SendOptions", game.options);
            await _hub.Clients.Group(game.GameId).SendAsync("StartClock");
            var timer = new System.Threading.Timer(NextQuestion, game, 10000, -1);
        }

        public void NextQuestion(Object stateInfo)
        {
            var game = (Game)stateInfo;
            SendQuestions(game);
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
        public static List<System.Threading.Timer> timerCollection = new List<System.Threading.Timer>();
        public static List<WeakReference> weakReferences = new List<WeakReference>();

        private GamePlay _gamePlay;

        public GamePlayManager(GamePlay gamePlay)
        {
            _gamePlay = gamePlay;
        }

        public void OnTimerElapsed(object stateInfo)
        {
            Console.WriteLine("Calling on Timer Elapsed");
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
                _gamePlay.SendQuestions(game);
            }
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
                game = PendingGames.Where(p => p.Topic == topic && p.NumberOfPlayersRequired == noOfPlayers).FirstOrDefault();

                if (game is null)
                {
                    Console.WriteLine("No existing Game. Hence creating a New Game.");
                    game = new Game(username, topic, noOfPlayers);
                    PendingGames.Add(game);
                    // var gamePlay = new GamePlay(game, startGame, notifyNoOpponentsFound);
                    var timer = new System.Threading.Timer(OnTimerElapsed, game, 10000, -1);
                    timerCollection.Add(timer);
                }
                else
                {
                    Console.WriteLine("Adding Player to the existing Game");
                    game.AddUsersToGame(username);
                }
                return game.GameId;
            }
            // return game.GameId;
        }

        public int ScoreCalculator(string groupname, string username, Object options, int counter)
        {
            var option = (Options)options;
            var game = RunningGames.FirstOrDefault(g => g.GameId == groupname);
            var user = game.Users.FirstOrDefault(t => t.username == username);
            if (option.IsCorrect == true)
            {
                user.score += counter * 2;
            }
            else
            {
                user.score = 0;
            }
            return user.score;
        }

        public ICollection<Game> GetPendingGames()
        {
            return PendingGames;
        }
    }

}
