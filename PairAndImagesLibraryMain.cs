using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;

namespace PairAndImagesLibrary
{
    public static class PairAndImagesLibraryMain
    {
        public static List<string> ReadAllLinesFromFile(string filepath)
        {
            Task<string[]> allLines = File.ReadAllLinesAsync(filepath);
            List<string> output = new List<string>(allLines.Result);
            return output;
        }

        public static List<List<Tuple<string, string>>> GetTeams(List<Tuple<string, string>> playersAndPhoneNumbers)
        {
            // If 4 players, 2 teams of 2
            // If 5 players, 1 team of 2, 1 team of 3
            // If 6 players, 3 teams of 2
            // If 7 players, 2 teams of 2, 1 team of 3
            // If 8 players, 4 teams of 2
            int numberOfTeams = playersAndPhoneNumbers.Count / 2;
            List<List<Tuple<string, string>>> teams = InitialiseTeamsArray(numberOfTeams);

            // Now cycle through the names and assign each to a team
            for (int playerIndex = 0; playerIndex < playersAndPhoneNumbers.Count; playerIndex++)
            {
                int teamToAllocateTo = playerIndex % numberOfTeams;
                teams[teamToAllocateTo].Add(playersAndPhoneNumbers[playerIndex]);
            }

            return teams;
        }

        private static List<List<Tuple<string, string>>> InitialiseTeamsArray(int numberOfTeams)
        {
            List<List<Tuple<string, string>>> teams = new List<List<Tuple<string, string>>>(numberOfTeams);
            for (int teamIndex = 0; teamIndex < numberOfTeams; teamIndex++)
            {
                // init capacity is 3 as each team will be either 2 or 3 players
                teams.Add(new List<Tuple<string, string>>(3));
            }
            return teams;
        }

        public static List<Clue> GetClues(List<List<Tuple<string, string>>> teams)
        {
            string sharedNoun = GetRandomNoun();
            List<string> adjectivePerTeam = GetAdjectivePerTeam(teams.Count);

            List<Clue> clueForEachTeam = new List<Clue>();
            for (int teamIndex = 0; teamIndex < teams.Count; teamIndex++)
            {
                clueForEachTeam.Add(new Clue(adjectivePerTeam[teamIndex], sharedNoun));
            }
            return clueForEachTeam;
        }

        private static List<string> GetAdjectivePerTeam(int teamCount)
        {
            List<string> allAdjectives = ReadAllLinesFromFile("Adjectives.txt");

            // Want to ensure each team has a unique adjective
            if (allAdjectives.Count < teamCount)
            {
                throw new InvalidDataException(string.Format(
                    "You have fewer adjectives ({0}) than teams ({1})",
                    allAdjectives.Count, teamCount));
            }

            HashSet<string> uniqueAdjectives = new HashSet<string>();
            while (uniqueAdjectives.Count < teamCount)
            {
                uniqueAdjectives.Add(allAdjectives[new Random().Next(allAdjectives.Count)]);
            }
            return uniqueAdjectives.ToList();
        }

        public static bool SendSMS(List<List<Tuple<string, string>>> teams, List<Clue> cluesList)
        {
            // check list of teams and clues is the same length
            CheckTeamsAndCluesLength(teams.Count, cluesList.Count);

            // for each team/clue
            for (int teamIndex = 0; teamIndex < teams.Count; teamIndex++)
            {
                // for each team member in that team
                List<Tuple<string, string>> currentTeamPlayers = teams[teamIndex];
                Clue currentClue = cluesList[teamIndex];

                foreach (Tuple<string, string> currentPlayer in currentTeamPlayers)
                {
                    // send an SMS containing the clue, using Twillo API
                    SendTwilioSMS(currentPlayer, currentClue);
                }
            }

            return true;
        }

        public static void CheckTeamsAndCluesLength(int teamsCount, int cluesCount)
        {
            if (teamsCount != cluesCount)
            {
                throw new Exception(string.Format(
                    "Expected teamsCount and cluesCount to be equal. Instead teamsCount={0} and cluesCount={1}", 
                    teamsCount, cluesCount));
            }
        }

        public static bool SendTwilioSMS(Tuple<string, string> teamMember, Clue clue)
        {
            const string accountSid = "AC944080b9808a962a5b6e0e4f3bd2b198";
            const string authToken = "33101919863d8ec5552f18ebfc276b1a";

            TwilioClient.Init(accountSid, authToken);
            MessageResource message = MessageResource.Create(
                body: string.Format("Hello {0}, from the Image Clue game. {1}.", teamMember.Item1, clue.ToString()),
                from: new Twilio.Types.PhoneNumber("+12566078498"),
                to: new Twilio.Types.PhoneNumber(FormatPhoneNumber(teamMember.Item2))
            );

            if (message.ErrorCode != null)
            {
                throw new ApiException(message.ErrorCode + message.ErrorMessage);
            }
            return true;
        }

        public static string FormatPhoneNumber(string number)
        {
            if (!number.StartsWith("+"))
            {
                return "+" + number;
            }
            else
            {
                return number;
            }
        }

        private static string GetRandomNoun()
        {
            List<string> allNouns = ReadAllLinesFromFile("Nouns.txt");
            int randomIndex = new Random().Next(allNouns.Count);
            return allNouns[randomIndex];
        }
    }
}
