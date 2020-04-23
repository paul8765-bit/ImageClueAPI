using System;
using System.Collections.Generic;
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

        [HttpGet("getteamsdetails/{teamID}")]
        public ActionResult<string> GetTeamsDetails(int teamID)
        {
            try
            {
                return JsonConvert.SerializeObject(PairAndImagesLibraryMain.GetTeamsDetails(teamID));
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [HttpGet("getclues/{teamID}")]
        public ActionResult<string> GetClues(int teamID)
        {
            try
            {
                return JsonConvert.SerializeObject(PairAndImagesLibraryMain.GetClues(teamID));
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [HttpGet("getcluesdetails/{cluesID}")]
        public ActionResult<string> GetCluesDetails(int cluesID)
        {
            try
            {
                return JsonConvert.SerializeObject(PairAndImagesLibraryMain.GetCluesDetails(cluesID));
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [HttpGet("sendsms/{teamsid}/{cluesid}")]
        public ActionResult<string> SendSMS(int teamsid, int cluesid)
        {
            try
            {
                return JsonConvert.SerializeObject(PairAndImagesLibraryMain.SendSMS(teamsid, cluesid));
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
