using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gameplay_back.Models {
    public class User
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UserId{get; set;}
        public string Score { get; set; }
    }

    public class Questions
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int QuestionsId { get; set; }
        public string Categ { get; set; }
        public string Topic { get; set; }
        public string QuestionGiven { get; set; }
        public List<Options> QuestionOptions { get; set; }
    }
    public class Options
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OptionsId { get; set; }
        public string OptionGiven { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionsId { get; set; }
    }

    public class Game
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string GameId { get; set; }
        public int QuestionTimeout { get; set; }
        public List<Questions> Questions { get; set; }
        public int NumberOfPlayersRequired { get; set; }
        public int NumberOfPlayersJoined { get; set; }
        public string Topic { get; set; }
        public List<User> Users { get; set; }
        public  bool GameOver{get; set;}
        public bool GameStarted{get; set;}

        public bool PendingGame{get; set;} 
    }

}
