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
        int i;
        // static IGameRepository gameRepository= new IGameRepository();
        private static Game game= new Game();
        static GamePlayManager gamePlayManager= new GamePlayManager();
        static Stopwatch stopwatch= new Stopwatch();
        HttpClient http= new HttpClient();
        int clock=10;

        public async Task StartClock () {
            while(clock>=0)
            {
                await Clients.All.SendAsync ("ClockStarted", clock--);
                Thread.Sleep(1000);
            }
            clock=10;
        }
        public  async Task OnConnectedAsync (string username, string topic,  int noOfPlayers)
        {
            game = gamePlayManager.Create_Game(username, topic, noOfPlayers);
                        // HttpResponseMessage response = await this.http.GetAsync ("http://172.23.238.164:8080/api/quizrt/question");
                        // HttpContent content = response.Content;
                        // string data = await content.ReadAsStringAsync();
                        // JArray json = JArray.Parse(data);
                        // Random random = new Random();
                        // for (i=0;i<7;i++)
                        // {
                        //     Console.WriteLine("came here 3");
                        //     await Clients.All.SendAsync("SendQuestions", "Hi");
                        //     Thread.Sleep(10000);
                        // }
            if(game!= null)
            {
                await Clients.All.SendAsync("SendQuestions", "Hi");
            }
            else
            {
                await Clients.Caller.SendAsync("SendQuestions", "Can't find " + noOfPlayers + " players... Go Back");
            }
            await base.OnConnectedAsync ();


        }

        public async Task OnDisconnectedAsync (string username) {
            await Groups.RemoveFromGroupAsync (Context.ConnectionId, "SignalR Users");
            await Clients.All.SendAsync("usersDisconnect",username); 
            // await base.OnDisconnectedAsync ();
        }

        public async Task AddToGroup(string userName, string groupName, int noOfPlayers) {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            // Random group= new Random();
            // var groupName=group.Next(1,50).ToString();
            await Clients.Group(groupName).SendAsync("Send", userName, groupName);
        }
    }
}

