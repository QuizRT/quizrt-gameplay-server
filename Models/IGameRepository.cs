using System.Collections.Generic;

namespace gameplay_back.Models
{
    public interface IGameRepository
    {
        // List<Game> GetPendingGamesByTopic(string topic);
        bool PostGame(User user, string topic, int no_of_players_required );
        // bool PatchUsersInGame(User user, string topic, int no_of_players_required);
        // bool PatchQuestionsInGame(int game_id);
        // List<Game> GetRunningGames();
        // Game GetGameById(int game_id);
    }
}

