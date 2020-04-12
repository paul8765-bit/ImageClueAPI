using System;
using System.Collections.Generic;
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
            List<Tuple<string, string>> playersAndPhoneNumbers = 
                JsonConvert.DeserializeObject<List<Tuple<string, string>>>(playerlist);
            return JsonConvert.SerializeObject(PairAndImagesLibraryMain.GetTeams(playersAndPhoneNumbers));
        }

        [HttpGet("getclues/{teamslist}")]
        public ActionResult<string> GetClues(string teamslist)
        {
            List<List<Tuple<string, string>>> teams = JsonConvert.DeserializeObject<List<List<Tuple<string, string>>>>(teamslist);
            return JsonConvert.SerializeObject(PairAndImagesLibraryMain.GetClues(teams));
        }

        [HttpGet("getteams/verbose/{playerlist}")]
        public ActionResult<string> GetTeamsVerbose(string playerlist)
        {
            StringBuilder output = new StringBuilder();
            List<Tuple<string, string>> playersAndPhoneNumbers =
                JsonConvert.DeserializeObject<List<Tuple<string, string>>>(playerlist);
            List<List<Tuple<string, string>>> teams = PairAndImagesLibraryMain.GetTeams(playersAndPhoneNumbers);
            output.Append(string.Format("Successfully allocated players across {0} teams", teams.Count));
            for (int teamIndex = 0; teamIndex < teams.Count; teamIndex++)
            {
                List<Tuple<string, string>> currentTeam = teams[teamIndex];
                output.Append(string.Format("{0} Team {1} has {2} people",
                    Environment.NewLine, teamIndex + 1, currentTeam.Count));
                foreach (Tuple<string, string> currentTeamMember in currentTeam)
                {
                    output.Append(string.Format("{0}    {1}", Environment.NewLine, currentTeamMember.Item1));
                }
            }
            return output.ToString();
        }

        [HttpGet("sendsms/{teamslistandclues}")]
        public ActionResult<string> SendSMS(string teamslistandclues)
        {
            string[] teamsAndCluesSplit = teamslistandclues.Split("|");
            string teamslist = teamsAndCluesSplit[0];
            string clues = teamsAndCluesSplit[1];
            List<List<Tuple<string, string>>> teams = 
                JsonConvert.DeserializeObject<List<List<Tuple<string, string>>>>(teamslist);
            List<Clue> cluesList = JsonConvert.DeserializeObject<List<Clue>>(clues);
            return JsonConvert.SerializeObject(PairAndImagesLibraryMain.SendSMS(teams, cluesList));
        }
    }
}
