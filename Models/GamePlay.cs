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
using RabbitMQ.Client;
using System.Text;


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
            //Questions =  new JArray();
            Task<JArray> Quest = System.Threading.Tasks.Task<string>.Run(() => GetQuestions().Result);
            Questions = Quest.Result;
            // Questions = new JArray();
            // Console.WriteLine(Questions.Count+"---------------");
            GameOver = false;
            GameStarted = false;
            PendingGame = true;
        }
        public Game() { }

        public async Task<JArray> GetQuestions()
        {
            HttpResponseMessage response = await this._http.GetAsync("http://172.23.238.164:7000/questiongenerator/questions/skyscraper");
            HttpContent content = response.Content;
            string data = await content.ReadAsStringAsync();
            Questions = JArray.Parse(data);
            // Console.WriteLine(Questions.Count+"+++++++++++++++++++");
            return Questions;
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
            if (game.QuestionCount >= 7)
            {
                QueueScore(game.Users);
                await _hub.Clients.Group(game.GameId).SendAsync("GameOver");
            }
            else
            {
                await _hub.Clients.Group(game.GameId).SendAsync("ProvideGroupId", game.GameId);
                await _hub.Clients.Group(game.GameId).SendAsync("QuestionsReceived", game.Questions[0]["questionsList"][random.Next(0, 4)]);
                game.QuestionCount++;
                await _hub.Clients.Group(game.GameId).SendAsync("StartClock");
                var timer = new System.Threading.Timer(NextQuestion, game, 10000, -1);
            }
        }

        public void QueueScore(List<User> user)
		{
            Console.WriteLine("--Sending Score to Social--");
			var factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "rabbitmq", Password = "rabbitmq" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
					queue: "Score",
					durable: false,
					exclusive: false,
					autoDelete: false,
					arguments: null
                );

                String jsonifiedUser = JsonConvert.SerializeObject(user);
                var body = Encoding.UTF8.GetBytes(jsonifiedUser);

                channel.BasicPublish(
					exchange: "",
					routingKey: "Score",
					basicProperties: null,
					body: body
                );
                Console.WriteLine("--{0} Score Queued--", user);
            }
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

        public async Task SendPendingGamesToUser(ICollection<Game> pendinggames)
        {
            await _hub.Clients.All.SendAsync("GetPendingGames", pendinggames);
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
                _gamePlay.SendQuestions(game);
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
                    // timer.Dispose();
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

        public int ScoreCalculator(string groupname, string username, string option, JObject question, int counter)
        {
            // Game game = new Game();

            Console.WriteLine("came to score calculator");
            Console.WriteLine(RunningGames[0].GameId);
            Game game = RunningGames.Where(g => g.GameId == groupname).FirstOrDefault();
            Console.WriteLine(game.GameId);
            User user = game.Users.FirstOrDefault(t => t.username == username);
            // Console.WriteLine(game.Questions[0]["questionsList"][0]["correctOption"].GetType());
            if (question["correctOption"].ToString() == option)
            {
                user.score += counter * 2;
            }
            else
            {
                user.score += 0;
            }
            Console.WriteLine(user.score);
            return user.score;
        }

        public void GetPendingGames()
        {
            _gamePlay.SendPendingGamesToUser(PendingGames);
        }
    }

}
