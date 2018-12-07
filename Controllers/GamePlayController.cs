using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GamePlay.Models;

namespace Gameplay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamePlayController : ControllerBase
    {
        GamePlayManager gameplaymanager = null;
        public GamePlayController(GamePlayManager _Gameplaymanager)
        {
            this.gameplaymanager = _Gameplaymanager;
        }
        [HttpGet]
        // public IActionResult Get_Pending_Games()
        // {
        //     var get_pending_games = gameplaymanager.GetPendingGames();
        //     if (get_pending_games != null)
        //     {
        //         return Ok(get_pending_games);
        //     }
        //     else
        //     {
        //         return NotFound("No Pending games found");
        //     }
        // }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult Get([FromQuery]int id)
        {
            // var get=  GameRepository.GetGameById(id);

            // if(get!=null)
            // {
            //     return Ok(get);
            // }
            // else
            // {
            return NotFound("The game you are looking for cannot be found");
            // }
        }
        [HttpGet("{topic}")]
        public ActionResult GetPendingGames([FromQuery]string topic)
        {
            // var getpending= GameRepository.GetPendingGamesByTopic(topic);
            //  if(getpending!=null)
            //  {
            //  return Ok(getpending);
            //  }
            //  else
            //  {
            return NotFound("pending games not anymore..");
            //  }
        }

        // POST api/values
        [HttpPost]
        // public IActionResult Post([FromBody]User user, [FromBody]string topic,[FromBody] int no_of_players)
        // {
        //     var post= GameRepository.PostGame(user, topic, no_of_players);
        //     if(post)
        //     {
        //         return Ok("Game Created..");
        //     }
        //     else
        //     {
        //         return BadRequest("Can't post. Try Again..");
        //     }

        // }

        // PUT api/values/5
        [HttpPatch]
        // public IActionResult PatchUsers([FromBody] User  user,[FromBody] string topic, [FromBody] int no_of_players)
        // {
        //     // var patch= GameRepository.PatchUsersInGame(user, topic, no_of_players);
        //     // if(patch)
        //     // {
        //     //     return Ok("User added to game_id ");
        //     // }
        //     // else
        //     // {
        //         return BadRequest("Can't patch.Try Again..");
        //     // }

        // }

        [HttpPatch("{id}")]
        public IActionResult PatchQuestions([FromQuery]int id)
        {

            // var patch= GameRepository.PatchQuestionsInGame(id);
            // if(patch)
            // {
            //     return Ok("User added to game_id ");
            // }
            // else
            // {
            return BadRequest("Can't patch.Try Again..");
            // }

        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
