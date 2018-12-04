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
        static int countPlayersJoined = 0;

        private GamePlayManager _gamePlayManager;

        public GamePlayHub(GamePlayManager gamePlayManager)
        {
            _gamePlayManager = gamePlayManager;
        }
        public async Task StartClock(string groupName)
        {
            await Clients.Caller.SendAsync("ClockStarted", true);
        }
        
        public async Task Init(string username, string topic, int noOfPlayers)
        {
            var gameId = _gamePlayManager.CreateGame(username, topic, noOfPlayers);
            // Console.WriteLine(gameId);
            // Console.WriteLine(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            GamePlayManager.weakReferences.Select(r => r.IsAlive).ToList().ForEach(Console.WriteLine);
        }

        // public async Task GameOver(string groupName)
        // {
        //     await Clients.OthersInGroup(groupName).SendAsync("GameOver");
        // }

        public async Task SendTicks(string groupName, int counter)
        {
            await Clients.OthersInGroup(groupName).SendAsync("GetTicks", counter);
        }

        public async Task CalculateScore(string groupName, string username,string option,JObject question, int counter)
        {
            var score = _gamePlayManager.ScoreCalculator(groupName, username, option,question, counter);
            await Clients.Group(groupName).SendAsync("GetScore", username, score);
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

