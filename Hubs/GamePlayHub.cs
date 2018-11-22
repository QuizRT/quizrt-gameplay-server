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

       public async Task NewMessage(string username, string message)
        {
            await Clients.All.SendAsync("messageReceived", username, message);
            // ("Reached here");
        }
        public async Task SendMessage (string user, string message) {
            await Clients.All.SendAsync ("ReceiveMessage", user, message);
        }

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
            
            if (noOfPlayers==1)
            {
                gamePlayManager.Create_Game(username, topic, 1);
                // HttpResponseMessage response = await this.http.GetAsync("http://172.23.238.164:8080/api/quizrt/question");
                // HttpContent content = response.Content;
                // string data = await content.ReadAsStringAsync();
                // JArray json = JArray.Parse(data);
                // Random random = new Random();
                for (i=0;i<7;i++)
                    {
                        Console.WriteLine("came here1");
                        await Clients.Caller.SendAsync("SendQuestions", "Hi");
                        Thread.Sleep(10000);
                    }

            }
            else if (noOfPlayers>1)
            {
                try{
                    Console.WriteLine("came here 1");
                if (game.Topic==topic)
                {
                    Console.WriteLine("came here 2");
                   game = game.AddUsersToGame(username, game);
                    if(game!=null)
                    {
                        Console.WriteLine("came here 6");
                        // HttpResponseMessage response = await this.http.GetAsync ("http://172.23.238.164:8080/api/quizrt/question");
                        // HttpContent content = response.Content;
                        // string data = await content.ReadAsStringAsync();
                        // JArray json = JArray.Parse(data);
                        // Random random = new Random();
                        for (i=0;i<7;i++)
                        {
                            Console.WriteLine("came here 3");
                            await Clients.All.SendAsync("SendQuestions", "Hi");
                            Thread.Sleep(10000);
                        }

                    }
                    else
                    {
                        Console.WriteLine("came here 7");
                        await Clients.All.SendAsync("SendQuestions", "No Players Joined... Can't Play");
                    }
                }
                
                // catch(Exception e)
                // {
                //     // throw new Exception("Not Working ",e);
                
                else
                {

                    Console.WriteLine("came here 4");
                    game = gamePlayManager.Create_Game (username, topic, noOfPlayers);
                //    throw new Exception("Not Working ",e);
                }
                }
                catch (Exception e)
                {
                    throw new Exception("Not Working ", e);
                }
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

