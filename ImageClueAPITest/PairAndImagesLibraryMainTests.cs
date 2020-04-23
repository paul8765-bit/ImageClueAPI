using Microsoft.VisualStudio.TestTools.UnitTesting;
using PairAndImagesLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PairAndImagesLibrary.Tests
{
    [TestClass()]
    public class PairAndImagesLibraryMainTests
    {
        [TestMethod()]
        public void GetTeamsDetailsTest()
        {
            string response = PairAndImagesLibraryMain.GetTeamDetailsSQLQuery();
            Assert.AreEqual("use ImageClueDB; select TeamsFormatted from Teams where TeamID=@teamID", response);
        }

        [TestMethod()]
        public void GetTeamDetailsSQLParamsTest()
        {
            Dictionary<string, string> sqlParams = PairAndImagesLibraryMain.GetTeamDetailsSQLParams(1);
            Assert.AreEqual(1, sqlParams.Count);
            KeyValuePair<string, string> entry = sqlParams.First();
            Assert.AreEqual("@teamID", entry.Key);
            Assert.AreEqual("1", entry.Value);
        }

        [TestMethod()]
        public void GetTeamsFromPlayersTest()
        {
            List<Tuple<string, string>> playersAndPhoneNumbers = new List<Tuple<string, string>>();
            playersAndPhoneNumbers.Add(new Tuple<string, string>("Paul", "1111"));
            playersAndPhoneNumbers.Add(new Tuple<string, string>("Emily", "2222"));
            playersAndPhoneNumbers.Add(new Tuple<string, string>("Chris", "3333"));
            playersAndPhoneNumbers.Add(new Tuple<string, string>("Joe", "4444"));
            List<List<Tuple<string, string>>> results =
                PairAndImagesLibraryMain.GetTeamsFromPlayers(playersAndPhoneNumbers);
            Assert.AreEqual(2, results.Count);
            List<Tuple<string, string>> team1 = results[0];
            List<Tuple<string, string>> team2 = results[1];
            Assert.AreEqual(2, team1.Count);
            Assert.AreEqual("Paul", team1[0].Item1);
            Assert.AreEqual("Chris", team1[1].Item1);
            Assert.AreEqual(2, team2.Count);
            Assert.AreEqual("Emily", team2[0].Item1);
            Assert.AreEqual("Joe", team2[1].Item1);
        }

        [TestMethod()]
        public void GenerateCluesTest()
        {
            List<string> adjectives = new string[] { "randy", "hairy" }.ToList();
            List<Clue> clues = PairAndImagesLibraryMain.GenerateClues("nun", adjectives, 2);
            Assert.AreEqual(2, clues.Count);
            Assert.AreEqual(clues[0].Noun, clues[1].Noun);
            Assert.AreNotEqual(clues[0].Adjective, clues[1].Adjective);
        }

        [TestMethod()]
        public void GetAdjectivePerTeamTest()
        {
            List<string> adjectives = PairAndImagesLibraryMain.GetAdjectivePerTeam(3);
            Assert.AreEqual(3, adjectives.Count);
            Assert.IsFalse(adjectives[0].Equals(adjectives[1]));
            Assert.IsFalse(adjectives[0].Equals(adjectives[2]));
            Assert.IsFalse(adjectives[1].Equals(adjectives[2]));
        }

        [TestMethod()]
        public void GetTeamsCountSQLQueryTest()
        {
            string query = PairAndImagesLibraryMain.GetTeamsCountSQLQuery();
            Assert.AreEqual("use ImageClueDB; select TeamsCount from Teams where TeamID = @teamsID", query);
        }

        [TestMethod()]
        public void GetTeamsCountSQLParamsTest()
        {
            Dictionary<string, string> sqlParams = PairAndImagesLibraryMain.GetTeamsCountSQLParams(-1);
            KeyValuePair<string, string> teamIDParam = sqlParams.First();
            Assert.AreEqual("@teamsID", teamIDParam.Key);
            Assert.AreEqual("-1", teamIDParam.Value);
        }

        [TestMethod()]
        public void GetCluesFormattedTest()
        {
            string expected = "Team 1: please draw a Hot Dog\nTeam 2: please draw a God Dog";
            List<Clue> clues = new Clue[] { new Clue("Hot", "Dog"), new Clue("God", "Dog") }.ToList();
            string actual = PairAndImagesLibraryMain.GetCluesFormatted(clues);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetCluesIdSQLQueryTest()
        {
            string expected = "use ImageClueDB; select top 1 CluesID from Clues order by CluesID desc;";
            string actual = PairAndImagesLibraryMain.GetCluesIdSQLQuery();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetPersistCluesSQLQueryTest()
        {
            string expected = "use ImageClueDB; insert into Clues (CluesJson, CluesFormatted, TeamID) VALUES (@CluesJson, @CluesFormatted, @TeamID)";
            string actual = PairAndImagesLibraryMain.GetPersistCluesSQLQuery();
            Assert.AreEqual(expected, actual);

        }

        [TestMethod()]
        public void GetPersistCluesQueryParamsTest()
        {
            string cluesJson = "[{\"Adjective\": \"Heavy\", \"Noun\":\"Queen\"}]";
            string formattedClues = "Please draw a Heavy Queen";
            int teamsID = 1;
            Dictionary<string, string> queryParams =
                PairAndImagesLibraryMain.GetPersistCluesQueryParams(cluesJson, formattedClues, teamsID);
            Assert.AreEqual(queryParams.Count, 3);
            Assert.IsTrue(queryParams.ContainsKey("@CluesJson"));
            Assert.IsTrue(queryParams.ContainsKey("@CluesFormatted"));
            Assert.IsTrue(queryParams.ContainsKey("@TeamID"));
        }

        [TestMethod()]
        public void GetCluesDetailsSQLQueryTest()
        {
            string expected = "use ImageClueDB; select CluesFormatted from Clues where CluesID = @cluesID";
            string actual = PairAndImagesLibraryMain.GetCluesDetailsSQLQuery();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetCluesDetailsSQLParamsTest()
        {
            Dictionary<string, string> sqlParams = PairAndImagesLibraryMain.GetCluesDetailsSQLParams(12);
            Assert.AreEqual(1, sqlParams.Count);
            KeyValuePair<string, string> firstValue = sqlParams.First();
            Assert.AreEqual("@cluesID", firstValue.Key);
            Assert.AreEqual("12", firstValue.Value);
        }

        [TestMethod()]
        public void GetCluesJsonSqlParamsTest()
        {
            Dictionary<string, string> sqlParams = PairAndImagesLibraryMain.GetCluesJsonSqlParams(17);
            KeyValuePair<string, string> firstVal = sqlParams.First();
            Assert.AreEqual(1, sqlParams.Count);
            Assert.AreEqual("@clueID", firstVal.Key);
            Assert.AreEqual("17", firstVal.Value);
        }

        [TestMethod()]
        public void GetCluesJsonSqlQueryTest()
        {
            string expected = "use ImageClueDB; select CluesJson from Clues where CluesID = @clueID";
            string actual = PairAndImagesLibraryMain.GetCluesJsonSqlQuery();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetTeamsJsonSqlQueryTest()
        {
            string expected = "use ImageClueDB; select TeamsJson from Teams where TeamID = @teamID";
            string actual = PairAndImagesLibraryMain.GetTeamsJsonSqlQuery();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetTeamsJsonSqlParamsTest()
        {
            Dictionary<string, string> sqlParams = PairAndImagesLibraryMain.GetTeamsJsonSqlParams(7);
            KeyValuePair<string, string> firstVal = sqlParams.First();
            Assert.AreEqual(1, sqlParams.Count);
            Assert.AreEqual("@teamID", firstVal.Key);
            Assert.AreEqual("7", firstVal.Value);
        }
    }
}