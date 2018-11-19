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
// using gameplay_back.hubs;
namespace gameplay_back.hubs {
    /// <summary>
    /// This hub receives request from the front-end and responds to the specified client 
    /// </summary>
    // public class GamePlayHub123:Hub {
    //     static int i = 10;
    //     WaitingGamePlay waiting_game;
    //     GamePlay[] play_game;
    //     int players_joined=0;
    //     static Queue waiting = new Queue();
    //     HttpClient http= new HttpClient();
    //     static Dictionary<string,int> gameGroup= new Dictionary<string, int>();

    //     public async Task NewMessage(string username, string message)
    //     {
    //         await Clients.All.SendAsync("messageReceived", username, message);
    //         // ("Reached here");
    //     }
        
    //     public async Task SendMessage (string user, string message) {
    //         await Clients.All.SendAsync ("ReceiveMessage", user, message);
    //     }
    //     public async Task StartClock(int questionCounter) {
    //         while(i>=0)
    //         {
    //             Console.WriteLine(i + " is the static variable");
    //             await Clients.All.SendAsync("counter",i);
    //             Thread.Sleep(1000);
    //             i--;

    //             if(i<0)
    //             {
    //                 i=10;
    //             }
    //             if(questionCounter>7)
    //             {
    //                 break;
    //             }
    //         }
    
    //     }
    //     public async Task SendScore (string user, int score){
    //         // Console.WriteLine(user + "this is User" + score + "this is score");
    //         await Clients.Others.SendAsync("receive",user, score);
    //     }
    //     public Task SendMessageToCaller (string message) {
    //         return Clients.Caller.SendAsync ("ReceiveMessage1", (i++));
    //     }
    //     public Task SendMessageToGroups (string message) {
    //         List<string> groups = new List<string> () { "SignalR Users" };
    //         return Clients.Groups (groups).SendAsync ("ReceiveMessage", message);
    //     }
    //     public async Task SendQuestions() {
    //         HttpResponseMessage response = await this.http.GetAsync("http://172.23.238.164:8080/api/quizrt/question");
    //         HttpContent content = response.Content;
    //         string data = await content.ReadAsStringAsync();
    //         JArray json = JArray.Parse(data);
    //         Random random = new Random();
    //         await Clients.Caller.SendAsync("questions", json[random.Next(1,json.Count)]);
    //     }

    //     public async Task SendQuestionsToMulti(string groupname, int no_of_players) {
    //         if(waiting_game.players_count== no_of_players )
    //        { 
    //         HttpResponseMessage response = await this.http.GetAsync("http://172.23.238.164:8080/api/quizrt/question");
    //         Console.WriteLine("came here");
    //          HttpContent content = response.Content;
    //         string data = await content.ReadAsStringAsync();
    //         JArray json = JArray.Parse(data);
    //         Random random = new Random();
    //         await Clients.Groups(groupname).SendAsync("questionsToMulti", json[random.Next(1,json.Count)]);
    //     }
    //     }


    //     public async Task GameOver(bool game) {
    //         // Console.WriteLine("Reached game over");
    //         await Clients.All.SendAsync("game", game);
    //     }
    //     public  async Task OnConnectedAsync (string username, string topic,  int noOfPlayers)
    //      {  
    //         if(gameGroup.ContainsKey(topic)){
    //             gameGroup[topic]++;

    //              waiting_game.username=username;
    //              waiting_game.topic=topic;
    //             if(waiting_game.players_count<noOfPlayers)
    //              {
    //                  waiting_game.players_count++;
    //                  waiting.Enqueue(waiting_game);
    //                 }
    //              else if (waiting_game.players_count==noOfPlayers){
    //                     waiting.CopyTo(play_game,0);
    //                     waiting.Clear();
                        
                        
    //                     for(i=0;i<noOfPlayers;i++){
    //                         play_game[i].group_id=guid.ToString();
    //                         play_game[i].game_started=true;
    //                         await Groups.AddToGroupAsync(Context.ConnectionId, play_game[i].group_id);
    //                     }
    //                     }
    //              }

    //         else{
    //             gameGroup.Add(topic,1);
    //             waiting_game.username=username;
    //             waiting_game.topic=topic;
    //             waiting_game.players_count=1;
    //             waiting.Enqueue(waiting_game);
    //         }

    //         await Clients.All.SendAsync("users", username);
    //         await base.OnConnectedAsync ();
    //     }
    //     public async Task OnDisconnectedAsync (string username) {
    //         // await Groups.RemoveFromGroupAsync (Context.ConnectionId, "SignalR Users");
    //         await Clients.All.SendAsync("usersDisconnect",username); 
    //         // await base.OnDisconnectedAsync (exception);
    //     }

    //     public async Task AddToGroup(string userName, string groupName, int noOfPlayers) {
    //         await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    //         // Random group= new Random();
    //         // var groupName=group.Next(1,50).ToString();
    //         await Clients.Group(groupName).SendAsync("Send", userName, groupName);
    //     }
    // }
}