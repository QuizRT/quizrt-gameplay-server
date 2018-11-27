using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using gameplay_back.Models;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Diagnostics;

namespace gameplay_back.hubs
{
    public class GamePlayHub: Hub
    {
        static int i=0;
        private static Game game= new Game();
        static GamePlayManager gamePlayManager= new GamePlayManager();
        HttpClient http= new HttpClient();

        public async Task SendQuestions(string groupName)
        {
            // HttpResponseMessage response = await this.http.GetAsync ("http://172.23.238.164:8080/api/quizrt/question");
                // HttpContent content = response.Content;
                // string data = await content.ReadAsStringAsync();
                // JArray json = JArray.Parse(data);
                // Random random = new Random();
                for(i=0;i<7;i++)
                {
                    await Clients.Groups(groupName).SendAsync("QuestionsReceived", "Hi "+ i);
                    Thread.Sleep(10000);
                }
        }
        public async Task StartClock (string groupName) {
                Console.WriteLine("came to clock");
                await Clients.Groups(groupName).SendAsync ("ClockStarted", true);
        }
        public  async Task OnConnectedAsync (string username, string topic,  int noOfPlayers)
        {
            game = gamePlayManager.Create_Game(username, topic, noOfPlayers);
            if(game!= null)
            {
                await Clients.Caller.SendAsync("usersConnected", game.GameId);
            }
            else
            {
                await Clients.Caller.SendAsync("usersConnected", null);
            }
            await base.OnConnectedAsync ();


        }

        public async Task OnDisconnectedAsync (string username) {
            await Groups.RemoveFromGroupAsync (Context.ConnectionId, "SignalR Users");
            await Clients.All.SendAsync("usersDisconnect",username);
        }

        public async Task AddToGroup(string userName, string groupName) {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("SendToGroup", true);
        }
    }
}

