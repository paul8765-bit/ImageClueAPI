using Microsoft.VisualStudio.TestTools.UnitTesting;
using PairAndImagesLibrary;
using System.Collections.Generic;
using System.Linq;

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
            Assert.AreEqual(2, adjectives.Count);
            Assert.AreEqual("Itchy", adjectives[0]);
            Assert.AreEqual("Horny", adjectives[1]);
        }

        [TestMethod]
        public void TestGetNouns()
        {
            string nounsFilePath = "Nouns.txt";
            List<string> nouns = PairAndImagesLibraryMain.ReadAllLinesFromFile(nounsFilePath);
            Assert.AreEqual(3, nouns.Count);
            Assert.AreEqual("Cat", nouns[0]);
            Assert.AreEqual("Lemon", nouns[1]);
            Assert.AreEqual("Gourd", nouns[2]);
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
            List<string> clues = PairAndImagesLibraryMain.GetClues(teams);
            Assert.AreEqual(2, teams.Count);
            Assert.AreEqual(2, clues.Count);

            // Assert that the sentence ends with the same noun
            string clueForFirstTeam = clues[0];
            string nounForFirstTeam = clueForFirstTeam.Split(" ").Last();
            string clueForSecondTeam = clues[1];
            string nounForSecondTeam = clueForSecondTeam.Split(" ").Last();

            Assert.AreEqual(nounForFirstTeam, nounForSecondTeam);

            string adjectiveForFirstTeam = clueForFirstTeam.Split(" ")[clueForFirstTeam.Split(" ").Count() - 2];
            string adjectiveForSecondTeam = clueForSecondTeam.Split(" ")[clueForSecondTeam.Split(" ").Count() - 2];
            Assert.AreNotEqual(adjectiveForFirstTeam, adjectiveForSecondTeam);
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
