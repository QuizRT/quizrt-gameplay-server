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
        static int i=10;
        static IGameRepository gameRepository= new IGameRepository();
            
        private static Game game;
        static GamePlayManager gamePlayManager= new GamePlayManager(); 
        static Stopwatch stopwatch= new Stopwatch();
        HttpClient http= new HttpClient();
        public  async Task OnConnectedAsync (string username, string topic,  int noOfPlayers)
        { 
                
            if (noOfPlayers==1)
            {
                gamePlayManager.Create_Game(username, topic, 1);
                HttpResponseMessage response = await this.http.GetAsync("http://172.23.238.164:8080/api/quizrt/question");
                HttpContent content = response.Content;
                string data = await content.ReadAsStringAsync();
                JArray json = JArray.Parse(data);
                Random random = new Random();
                for (i=0;i<7;i++)
                    {
                        Console.WriteLine("came here");
                        await Clients.Caller.SendAsync("SendQuestions", json[random.Next(1,json.Count)]);
                        Thread.Sleep(10000);
                    }

            }
            else if (noOfPlayers>1)
            {
                if (game.Topic==topic)
                { 
                   game.AddUsersToGame(username, game);
                    stopwatch.Start();
                    while (game.NumberOfPlayersJoined<game.NumberOfPlayersRequired && stopwatch.ElapsedMilliseconds<=90000)
                    {
                        Thread.Sleep(1000);
                    }
                    stopwatch.Stop();
                    if (game.NumberOfPlayersJoined==noOfPlayers)
                    {
                        game.PendingGame=false;
                        game.GameStarted=true;
                        HttpResponseMessage response = await this.http.GetAsync ("http://172.23.238.164:8080/api/quizrt/question");
                        HttpContent content = response.Content;
                        string data = await content.ReadAsStringAsync();
                        JArray json = JArray.Parse(data);
                        Random random = new Random();
                        for (i=0;i<7;i++)
                        {
                            Console.WriteLine("came here");
                            await Clients.All.SendAsync("SendQuestions", json[random.Next(1,json.Count)]);
                            Thread.Sleep(10000);
                        }

                    }
                    else if(game.NumberOfPlayersJoined<noOfPlayers)
                    {
                        await Clients.All.SendAsync("SendQuestions", "game plan changed");
                    }
                }
                else
                {
                   gamePlayManager.Create_Game (username, topic, noOfPlayers);
                }
            }


        }
    }
}
