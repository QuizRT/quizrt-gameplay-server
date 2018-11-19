using System.Collections.Generic;

namespace gameplay_back.Models
{
    public class IGameRepository
    {
        private static ICollection<Game> pending_games;
        private static ICollection<User> connected_users;
        private static IGameRepository instance=null;

        public static IGameRepository GetInstance()
        {
            if(instance==null)
            {
                instance=new IGameRepository();
            }
            return instance;
        }

        private IGameRepository()
        {
            connected_users = new List<User>();
            pending_games = new List<Game>();
        }

        public void AddUser(User user)
        {
            connected_users.Add(user);
        }

        public void RemoveUser(User user)
        {
            connected_users.Remove(user);
        }
    }
}

