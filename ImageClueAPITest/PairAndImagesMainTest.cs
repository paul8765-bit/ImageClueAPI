using Microsoft.VisualStudio.TestTools.UnitTesting;
using PairAndImagesLibrary;
using System;
using System.Collections.Generic;
using Twilio.Exceptions;

namespace PairAndImagesGameTest
{
    [TestClass]
    public class PairAndImagesMainTest
    {
        [TestMethod]
        public void TestGetAdjectives()
        {
            string adjectivesFilePath = "Adjectives.txt";
            List<string> adjectives = PairAndImagesLibraryMain.ReadAllLinesFromFile(adjectivesFilePath);
            Assert.IsTrue(adjectives.Count > 10);
            Assert.AreEqual("itchy", adjectives[0]);
            Assert.AreEqual("horny", adjectives[1]);
        }

        [TestMethod]
        public void TestGetNouns()
        {
            string nounsFilePath = "Nouns.txt";
            List<string> nouns = PairAndImagesLibraryMain.ReadAllLinesFromFile(nounsFilePath);
            Assert.IsTrue(nouns.Count > 10);
            Assert.AreEqual("cat", nouns[0]);
            Assert.AreEqual("lemon", nouns[1]);
            Assert.AreEqual("gourd", nouns[2]);
        }

        [TestMethod]
        public void TestGetTeamsSmall()
        {
            List<List<string>> teams = GetSmallTeam();
            Assert.AreEqual(2, teams.Count);
            Assert.AreEqual(2, teams[0].Count);
            Assert.AreEqual(2, teams[1].Count);

            // Also check the names are different!
            Assert.IsFalse(teams[0].Contains(teams[1][0]) || teams[0].Contains(teams[1][1]));
        }

        [TestMethod]
        public void TestGetTeamsMedium()
        {
            List<List<string>> teams = GetMediumTeam();
            Assert.AreEqual(3, teams.Count);
            Assert.AreEqual(3, teams[0].Count);
            Assert.AreEqual(2, teams[1].Count);
            Assert.AreEqual(2, teams[2].Count);

            // Also check the names are different!
            Assert.IsFalse(teams[0].Contains(teams[1][0]) || teams[0].Contains(teams[1][1]));
        }

        [TestMethod]
        public void TestGetTeamsLarge()
        {
            List<List<string>> teams = GetLargeTeam();
            Assert.AreEqual(5, teams.Count);
            Assert.AreEqual(2, teams[0].Count);
            Assert.AreEqual(2, teams[1].Count);
            Assert.AreEqual(2, teams[2].Count);
            Assert.AreEqual(2, teams[3].Count);
            Assert.AreEqual(2, teams[4].Count);

            // Also check the names are different!
            Assert.IsFalse(teams[0].Contains(teams[1][0]) || teams[0].Contains(teams[1][1]));
        }

        [TestMethod]
        public void TestGetCluesSmallTeam()
        {
            List<List<string>> teams = GetSmallTeam();
            List<Clue> clues = PairAndImagesLibraryMain.GetClues(teams);
            Assert.AreEqual(2, teams.Count);
            Assert.AreEqual(2, clues.Count);

            // Assert that the sentence ends with the same noun
            Assert.AreEqual(clues[0].Noun, clues[1].Noun);

            // Assert that the adjectives are different
            Assert.AreNotEqual(clues[0].Adjective, clues[1].Adjective);
        }

        [TestMethod]
        public void TestGetCluesMediumTeam()
        {
            List<List<string>> teams = GetMediumTeam();
            List<Clue> clues = PairAndImagesLibraryMain.GetClues(teams);
            Assert.AreEqual(3, teams.Count);
            Assert.AreEqual(3, clues.Count);

            // Assert that the sentence ends with the same noun
            Assert.AreEqual(clues[0].Noun, clues[1].Noun);
            Assert.AreEqual(clues[0].Noun, clues[2].Noun);

            // Assert that the adjectives are different
            Assert.AreNotEqual(clues[0].Adjective, clues[1].Adjective);
            Assert.AreNotEqual(clues[0].Adjective, clues[2].Adjective);
        }

        [TestMethod]
        public void TestCheckTeamsAndCluesLengthSuccess()
        {
            PairAndImagesLibraryMain.CheckTeamsAndCluesLength(3, 3);
            // Not expecting an exception here so this is success
        }

        [TestMethod]
        public void TestCheckTeamsAndCluesLengthFailure()
        {
            try
            {
                PairAndImagesLibraryMain.CheckTeamsAndCluesLength(3, 2);
            }
            catch (Exception e)
            {
                // Are expecting an exception here though!
                Assert.IsTrue(e.Message.Equals(
                    "Expected teamsCount and cluesCount to be equal. Instead teamsCount=3 and cluesCount=2"));
                return;
            }
            // If no exception was thrown this test is failed
            Assert.Fail("Expected exception to be thrown");
        }

        [TestMethod]
        public void SendTwilioSMSTestSuccess()
        {
            bool result = PairAndImagesLibraryMain.SendTwilioSMS(
                new Tuple<string, string>("Paul", "+447986869466"), new Clue("randy", "nun"));
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SendTwilioSMSTestFailure()
        {
            try
            {
                bool result = PairAndImagesLibraryMain.SendTwilioSMS(
                    new Tuple<string, string>("Paul", "+4472618"), new Clue("randy", "nun"));
            }
            catch (ApiException e)
            {
                // We expected this exception
                Assert.IsTrue(e.Message.Contains("not a valid phone number"));
                return;
            }

            // If we reach this stage, fail the test
            Assert.Fail("Expected an exception as the phone number was invalid");
        }

        private List<List<string>> GetSmallTeam()
        {
            string players1FilePath = "Players1.txt";
            List<string> players = PairAndImagesLibraryMain.ReadAllLinesFromFile(players1FilePath);
            Assert.AreEqual(4, players.Count);

            // If 4 players, 2 teams of 2
            // If 5 players, 1 team of 2, 1 team of 3
            // If 6 players, 3 teams of 2
            // If 7 players, 2 teams of 2, 1 team of 3
            // If 8 players, 4 teams of 2
            List<List<string>> teams = PairAndImagesLibraryMain.GetTeams(players);
            return teams;
        }

        private List<List<string>> GetMediumTeam()
        {
            string players2FilePath = "Players2.txt";
            List<string> players = PairAndImagesLibraryMain.ReadAllLinesFromFile(players2FilePath);
            Assert.AreEqual(7, players.Count);

            // If 4 players, 2 teams of 2
            // If 5 players, 1 team of 2, 1 team of 3
            // If 6 players, 3 teams of 2
            // If 7 players, 2 teams of 2, 1 team of 3
            // If 8 players, 4 teams of 2
            List<List<string>> teams = PairAndImagesLibraryMain.GetTeams(players);
            return teams;
        }

        private List<List<string>> GetLargeTeam()
        {
            string players3FilePath = "Players3.txt";
            List<string> players = PairAndImagesLibraryMain.ReadAllLinesFromFile(players3FilePath);
            Assert.AreEqual(10, players.Count);

            // If 4 players, 2 teams of 2
            // If 5 players, 1 team of 2, 1 team of 3
            // If 6 players, 3 teams of 2
            // If 7 players, 2 teams of 2, 1 team of 3
            // If 8 players, 4 teams of 2
            List<List<string>> teams = PairAndImagesLibraryMain.GetTeams(players);
            return teams;
        }
    }
}
