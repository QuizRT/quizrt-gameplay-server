using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using GamePlay.Models;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Diagnostics;


namespace GamePlay.Hubs
{
    public class GamePlayHub : Hub
    {
        // static int i = 0;
        // private static Game game = new Game();
        static GamePlayManager gamePlayManager;

        public GamePlayHub(GamePlayManager _manager)
        {
            gamePlayManager = _manager;
        }
        HttpClient http = new HttpClient();
        static int countPlayersJoined = 0;

        public async Task SendQuestions(string groupName)
        {
            HttpResponseMessage response = await this.http.GetAsync("http://172.23.238.164:7000/questiongenerator/questions/book");
            HttpContent content = response.Content;
            string data = await content.ReadAsStringAsync();
            JArray json = JArray.Parse(data);
            Random random = new Random();
            // Console.WriteLine("--OO--"+json[0]["questionsList"][random.Next(0,5)]);
            await Clients.Group(groupName).SendAsync("QuestionsReceived", json[random.Next(0, 3)]["questionsList"][random.Next(0, 5)]);
        }
        public async Task StartClock(string groupName)
        {
            await Clients.Caller.SendAsync("ClockStarted", true);
        }
        public async Task Init(string username, string topic, int noOfPlayers)
        {
            var gameId = gamePlayManager.CreateGame(username, topic, noOfPlayers);
            Console.WriteLine(gameId);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            // if (gameId != null)
            // {
            //     // Console.WriteLine(game.Users[0]+" "+game.Users[1]);
            //     await Clients.Caller.SendAsync("usersConnected", gameId);
            //     // game = null;
            // }
            // else
            // {
            //     await Clients.Caller.SendAsync("usersConnected", null);
            // }
            await base.OnConnectedAsync();
        }
        public async Task GameOver(string groupName)
        {
            await Clients.OthersInGroup(groupName).SendAsync("GameOver");
        }

        public async Task SendTicks(string groupName, int counter)
        {
            await Clients.OthersInGroup(groupName).SendAsync("GetTicks", counter);
        }

        public async Task SendScore(string groupName, string username, int score)
        {
            await Clients.OthersInGroup(groupName).SendAsync("GetScore", username, score);
        }

        public async Task OnDisconnectedAsync(string username)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await Clients.All.SendAsync("usersDisconnect", username);
        }

        public async Task AddToGroup(string userName, string groupName, int noOfPlayers)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            countPlayersJoined++;
            await Clients.Caller.SendAsync("SendToGroup", countPlayersJoined);
            if (countPlayersJoined == noOfPlayers)
            {
                countPlayersJoined = 0;
            }
        }
    }
}

