using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PairAndImagesLibrary;

namespace ImageClueAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("AllowAllHeaders")]
    public class ImageClueAPIController : ControllerBase
    {
        private readonly ILogger<ImageClueAPIController> _logger;

        public ImageClueAPIController(ILogger<ImageClueAPIController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            List<string> output = new List<string>();
            output.Add("Successfully connected to Get API");
            return output;
        }

        [HttpGet("getteams/{playerlist}")]
        public ActionResult<string> GetTeams(string playerlist)
        {
            string[] playerlistArray = playerlist.Split("|");
            return JsonConvert.SerializeObject(PairAndImagesLibraryMain.GetTeams(playerlistArray.ToList()));
        }

        [HttpGet("getclues/{teamslist}")]
        public ActionResult<string> GetClues(string teamslist)
        {
            List<List<string>> teams = JsonConvert.DeserializeObject<List<List<string>>>(teamslist);
            return JsonConvert.SerializeObject(PairAndImagesLibraryMain.GetClues(teams));
        }

        [HttpGet("getteams/verbose/{playerlist}")]
        public ActionResult<string> GetTeamsVerbose(string playerlist)
        {
            StringBuilder output = new StringBuilder();
            string[] playerlistArray = playerlist.Split("|");
            List<List<string>> teams = PairAndImagesLibraryMain.GetTeams(playerlistArray.ToList());
            output.Append(string.Format("Successfully allocated players across {0} teams", teams.Count));
            for (int teamIndex = 0; teamIndex < teams.Count; teamIndex++)
            {
                List<string> currentTeam = teams[teamIndex];
                output.Append(string.Format("{0} Team {1} has {2} people",
                    Environment.NewLine, teamIndex + 1, currentTeam.Count));
                foreach (string currentTeamMember in currentTeam)
                {
                    output.Append(string.Format("{0}    {1}", Environment.NewLine, currentTeamMember));
                }
            }
            return output.ToString();
        }
    }
}
