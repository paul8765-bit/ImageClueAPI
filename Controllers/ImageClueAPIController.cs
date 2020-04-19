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
            try
            {
                List<Tuple<string, string>> playersAndPhoneNumbers =
                                JsonConvert.DeserializeObject<List<Tuple<string, string>>>(playerlist);
                return JsonConvert.SerializeObject(PairAndImagesLibraryMain.GetTeams(playersAndPhoneNumbers));
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [HttpGet("getclues/{teamslist}")]
        public ActionResult<string> GetClues(string teamslist)
        {
            try
            {
                List<List<Tuple<string, string>>> teams = JsonConvert.DeserializeObject<List<List<Tuple<string, string>>>>(teamslist);
                return JsonConvert.SerializeObject(PairAndImagesLibraryMain.GetClues(teams));
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [HttpGet("sendsms/{teamslistandclues}")]
        public ActionResult<string> SendSMS(string teamslistandclues)
        {
            try
            {
                string[] teamsAndCluesSplit = teamslistandclues.Split("|");
                string teamslist = teamsAndCluesSplit[0];
                string clues = teamsAndCluesSplit[1];
                List<List<Tuple<string, string>>> teams =
                    JsonConvert.DeserializeObject<List<List<Tuple<string, string>>>>(teamslist);
                List<Clue> cluesList = JsonConvert.DeserializeObject<List<Clue>>(clues);
                return JsonConvert.SerializeObject(PairAndImagesLibraryMain.SendSMS(teams, cluesList));
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
