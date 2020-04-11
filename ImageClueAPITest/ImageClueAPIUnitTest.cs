using ImageClueAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PairAndImagesLibrary;
using System;
using System.Collections.Generic;

namespace ImageClueAPITest
{
    [TestClass]
    public class ImageClueAPIUnitTest
    {
        [TestMethod]
        public void TestGetTeams()
        {
            string team1 = "[{\"Item1\":\"paul\",\"Item2\":\"447986869466\"},{\"Item1\":\"chris\",\"Item2\":\"447986869466\"},{\"Item1\":\"emily\",\"Item2\":\"447986869466\"},{\"Item1\":\"ben\",\"Item2\":\"447986869466\"}]";
            ActionResult<string> result = new ImageClueAPIController(null).GetTeams(team1);
            string teamsString = result.Value;
            List<List<Tuple<string, string>>> teams = JsonConvert.DeserializeObject<List<List<Tuple<string, string>>>>(teamsString);
            Assert.AreEqual(2, teams.Count);
            Assert.AreEqual(2, teams[0].Count);
            Assert.AreEqual(2, teams[1].Count);
        }

        [TestMethod]
        public void TestGetClues()
        {
            string team1Json = "[[{\"Item1\":\"paul\",\"Item2\":\"447986869466\"},{\"Item1\":\"emily\",\"Item2\":\"447986869466\"}],[{\"Item1\":\"chris\",\"Item2\":\"447986869466\"},{\"Item1\":\"ben\",\"Item2\":\"447986869466\"}]]";
            ActionResult<string> result = new ImageClueAPIController(null).GetClues(team1Json);
            string cluesString = result.Value;
            List<Clue> clues = JsonConvert.DeserializeObject<List<Clue>>(cluesString);
            Assert.AreEqual(2, clues.Count);

            // Asert that the nouns are the same, but the adjectives are different
            Assert.IsTrue(clues[0].Noun == clues[1].Noun);
            Assert.IsTrue(clues[0].Adjective != clues[1].Adjective);
        }

        [TestMethod]
        public void TestSendSMS()
        {
            string teamJson = "[[{\"Item1\":\"paul\",\"Item2\":\"447986869466\"},{\"Item1\":\"emily\",\"Item2\":\"447986869466\"}],[{\"Item1\":\"chris\",\"Item2\":\"447986869466\"},{\"Item1\":\"ben\",\"Item2\":\"447986869466\"}]]";
            string cluesJson = "[{\"Adjective\":\"cocky\",\"Noun\":\"French chef\"},{\"Adjective\":\"confused\",\"Noun\":\"French chef\"}]";
            ActionResult<string> result = new ImageClueAPIController(null).SendSMS(teamJson, cluesJson);
            bool outcome = JsonConvert.DeserializeObject<bool>(result.Value);
            Assert.IsTrue(outcome);
        }
    }
}
