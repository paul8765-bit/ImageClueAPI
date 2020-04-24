using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        public static int GetTeams(List<Tuple<string, string>> playersAndPhoneNumbers)
        {

            List<List<Tuple<string, string>>> teams = GetTeamsFromPlayers(playersAndPhoneNumbers);

            // So now at this stage, we have:
            // 1. The players object (passed in as arg, just need to convert to JSON)
            string playersJson = JsonConvert.SerializeObject(playersAndPhoneNumbers);

            // 2. The teams object (just need to convert to JSON)
            string teamsJson = JsonConvert.SerializeObject(teams);

            // 3. Let's get the formatted Teams string
            string formattedTeamsString = GetUserFriendlyTeams(teams);

            // Store the above in SQL and return the TeamID below
            bool success = PersistTeamInSQL(playersJson, teamsJson, formattedTeamsString, teams.Count);
            if (success)
            {
                int teamId = GetTeamIdOfPersistedTeam();
                return teamId;
            }

            return -1;
        }

        private static int GetTeamIdOfPersistedTeam()
        {
            string sqlQuery = "select top 1 TeamID from Teams order by TeamID desc";
            Dictionary<string, string> sqlQueryParams = new Dictionary<string, string>();
            SQLHelper sqlHelper = new SQLHelper();
            List<List<object>> response = sqlHelper.RunSQLQuery(sqlQuery, sqlQueryParams);
            return (int)response[0][0];
        }

        private static bool PersistTeamInSQL(string playersJson, string teamsJson, 
                                        string formattedTeamsString, int teamsCount)
        {
            string sqlQuery = GetPersistTeamSQLQuery();
            Dictionary<string, string> sqlQueryParameters = 
                                        GetPersistTeamQueryParams(playersJson, teamsJson, 
                                                                formattedTeamsString, teamsCount);
            SQLHelper sqlHelper = new SQLHelper();
            List<List<object>> sqlResponse = sqlHelper.RunSQLQuery(sqlQuery, sqlQueryParameters);
            return true;
        }

        public static string GetCluesDetails(int cluesID)
        {
            // We have the cluesID. Just need to query SQL to get the formatted clues string
            string sqlQuery = GetCluesDetailsSQLQuery();
            Dictionary<string, string> sqlParams = GetCluesDetailsSQLParams(cluesID);
            SQLHelper sqlHelper = new SQLHelper();
            List<List<object>> sqlResponse = sqlHelper.RunSQLQuery(sqlQuery, sqlParams);
            if (sqlResponse.Count == 0)
            {
                throw new Exception(string.Format("Unable to find Clue {0}", cluesID));
            }
            return sqlResponse[0][0].ToString();
        }

        public static Dictionary<string, string> GetCluesDetailsSQLParams(int cluesID)
        {
            Dictionary<string, string> sqlParams = new Dictionary<string, string>(1);
            sqlParams.Add("@cluesID", cluesID.ToString());
            return sqlParams;
        }

        public static string GetCluesDetailsSQLQuery()
        {
            return "use ImageClueDB; select CluesFormatted from Clues where CluesID = @cluesID";
        }

        public static string GetTeamsDetails(int teamID)
        {
            // We have the teamID. Just need to query SQL to get the formatted team string
            string sqlQuery = GetTeamDetailsSQLQuery();
            Dictionary<string, string> sqlParams = GetTeamDetailsSQLParams(teamID);
            SQLHelper sqlHelper = new SQLHelper();
            List<List<object>> sqlResponse = sqlHelper.RunSQLQuery(sqlQuery, sqlParams);
            if (sqlResponse.Count == 0)
            {
                return string.Format("Unable to find team with ID {0}", teamID);
            }
            return sqlResponse[0][0].ToString();
        }

        public static Dictionary<string, string> GetTeamDetailsSQLParams(int teamID)
        {
            Dictionary<string, string> sqlParams = new Dictionary<string, string>(1);
            sqlParams.Add("@teamID", teamID.ToString());
            return sqlParams;
        }

        public static string GetTeamDetailsSQLQuery()
        {
            return "use ImageClueDB; select TeamsFormatted from Teams where TeamID=@teamID";
        }

        private static Dictionary<string, string> GetPersistTeamQueryParams(
                                        string playersJson, string teamsJson, 
                                        string formattedTeamsString, int teamsCount)
        {
            Dictionary<string, string> sqlQueryParameters = new Dictionary<string, string>(3);
            sqlQueryParameters.Add("@PlayersJson", playersJson);
            sqlQueryParameters.Add("@TeamsJson", teamsJson);
            sqlQueryParameters.Add("@TeamsFormatted", formattedTeamsString);
            sqlQueryParameters.Add("@TeamsCount", teamsCount.ToString());
            return sqlQueryParameters;
        }

        private static string GetPersistTeamSQLQuery()
        {
            return "USE ImageClueDB; INSERT INTO Teams (PlayersJson, TeamsJson, TeamsFormatted, TeamsCount) VALUES (@PlayersJson,@TeamsJson,@TeamsFormatted,@TeamsCount)";
        }

        private static string GetUserFriendlyTeams(List<List<Tuple<string, string>>> teamsArray)
        {
            StringBuilder userFriendlyTeams = new StringBuilder();
            for (int teamIndex = 0; teamIndex < teamsArray.Count; teamIndex++)
            {
                List<Tuple<string, string>> currentTeam = teamsArray[teamIndex];
                userFriendlyTeams.Append(string.Format("Team {0} has {1} members\n", teamIndex + 1, currentTeam.Count));
                for (int teamMemberIndex = 0; teamMemberIndex < currentTeam.Count; teamMemberIndex++)
                {
                    Tuple<string, string> currentTeamMember = currentTeam[teamMemberIndex];
                    userFriendlyTeams.Append(string.Format("    {0}\n", currentTeamMember.Item1));
                }
            }
            return userFriendlyTeams.ToString();
        }

        public static List<List<Tuple<string, string>>> GetTeamsFromPlayers(List<Tuple<string, string>> playersAndPhoneNumbers)
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

        public static int GetClues(int teamsID)
        {
            List<Clue> clues = GetCluesForTeam(teamsID);

            // Now we have what we need to persist to SQL:
            // 1. The clues JSON
            string cluesJson = JsonConvert.SerializeObject(clues);

            // 2. Clues formatted (nick this from JS)
            string formattedClues = GetCluesFormatted(clues);

            // 3. TeamID 

            // Now need to push this into SQL, and get the Clue ID
            bool success = PersistCluesInSQL(cluesJson, formattedClues, teamsID);
            if (success)
            {
                return GetCluesIdOfPersistedClues();
            }
            return -1;
        }

        private static int GetCluesIdOfPersistedClues()
        {
            string sqlQuery = GetCluesIdSQLQuery();
            Dictionary<string, string> blankParams = new Dictionary<string, string>();
            SQLHelper sqlHelper = new SQLHelper();
            List<List<object>> sqlResponse = sqlHelper.RunSQLQuery(sqlQuery, blankParams);
            return (int)sqlResponse[0][0];
        }

        public static string GetCluesIdSQLQuery()
        {
            return "use ImageClueDB; select top 1 CluesID from Clues order by CluesID desc;";
        }

        private static bool PersistCluesInSQL(string cluesJson, string formattedClues, int teamsID)
        {
            string sqlQuery = GetPersistCluesSQLQuery();
            Dictionary<string, string> sqlParams = GetPersistCluesQueryParams(cluesJson, formattedClues, teamsID);
            SQLHelper sqlHelper = new SQLHelper();
            sqlHelper.RunSQLQuery(sqlQuery, sqlParams);
            return true;
        }

        public static Dictionary<string, string> GetPersistCluesQueryParams(string cluesJson, string formattedClues, int teamsID)
        {
            Dictionary<string, string> sqlParams = new Dictionary<string, string>(3);
            sqlParams.Add("@CluesJson", cluesJson);
            sqlParams.Add("@CluesFormatted", formattedClues);
            sqlParams.Add("@TeamID", teamsID.ToString());
            return sqlParams;
        }

        public static string GetPersistCluesSQLQuery()
        {
            return "use ImageClueDB; insert into Clues (CluesJson, CluesFormatted, TeamID) VALUES (@CluesJson, @CluesFormatted, @TeamID)";
        }

        public static string GetCluesFormatted(List<Clue> cluesArray)
        {
            StringBuilder userFriendlyClues = new StringBuilder();
            for (int clueIndex = 0; clueIndex < cluesArray.Count; clueIndex++)
            {
                Clue currentClue = cluesArray[clueIndex];
                userFriendlyClues.Append(string.Format("Team {0}: please draw a {1} {2}", clueIndex + 1, currentClue.Adjective, currentClue.Noun));
                if (clueIndex + 1 < cluesArray.Count)
                {
                    userFriendlyClues.Append("\n");
                }
            }
            return userFriendlyClues.ToString();
        }

        private static List<Clue> GetCluesForTeam(int teamsID)
        {
            // Find out how many teams there are by querying SQL
            int teamsCount = GetTeamsCountByID(teamsID);
            string sharedNoun = GetRandomNoun();
            List<string> adjectivePerTeam = GetAdjectivePerTeam(teamsCount);
            return GenerateClues(sharedNoun, adjectivePerTeam, teamsCount);
        }

        private static int GetTeamsCountByID(int teamsID)
        {
            string sqlQuery = GetTeamsCountSQLQuery();
            Dictionary<string, string> sqlParams = GetTeamsCountSQLParams(teamsID);
            SQLHelper sqlHelper = new SQLHelper();
            List<List<object>> response = sqlHelper.RunSQLQuery(sqlQuery, sqlParams);
            if (response.Count == 0)
            {
                throw new Exception(string.Format("Unable to find team {0}", teamsID));
            }
            return (int)response[0][0];
        }

        public static Dictionary<string, string> GetTeamsCountSQLParams(int teamsID)
        {
            Dictionary<string, string> sqlParams = new Dictionary<string, string>(1);
            sqlParams.Add("@teamsID", teamsID.ToString());
            return sqlParams;
        }

        public static string GetTeamsCountSQLQuery()
        {
            return "use ImageClueDB; select TeamsCount from Teams where TeamID = @teamsID";
        }

        public static List<Clue> GenerateClues(string sharedNoun, List<string> adjectivePerTeam, int teamsCount)
        {
            List<Clue> clueForEachTeam = new List<Clue>();
            for (int teamIndex = 0; teamIndex < teamsCount; teamIndex++)
            {
                clueForEachTeam.Add(new Clue(adjectivePerTeam[teamIndex], sharedNoun));
            }
            return clueForEachTeam;
        }

        public static List<string> GetAdjectivePerTeam(int teamCount)
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

        public static bool SendSMS(int teamsId, int cluesId)
        {
            // Load from SQL!
            List<List<Tuple<string, string>>> teams = GetTeamsJsonByID(teamsId);
            List<Clue> cluesList = GetCluesJsonById(cluesId);

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

        private static List<Clue> GetCluesJsonById(int cluesId)
        {
            string sqlQuery = GetCluesJsonSqlQuery();
            Dictionary<string, string> sqlParams = GetCluesJsonSqlParams(cluesId);
            SQLHelper sqlHelper = new SQLHelper();
            List<List<object>> response = sqlHelper.RunSQLQuery(sqlQuery, sqlParams);
            if (response.Count == 0)
            {
                throw new Exception(string.Format("Unable to find Clue with ID {0}", cluesId));
            }
            return JsonConvert.DeserializeObject<List<Clue>>(response[0][0].ToString());
        }

        public static Dictionary<string, string> GetCluesJsonSqlParams(int cluesId)
        {
            Dictionary<string, string> sqlParams = new Dictionary<string, string>(1);
            sqlParams.Add("@clueID", cluesId.ToString());
            return sqlParams;
        }

        public static string GetCluesJsonSqlQuery()
        {
            return "use ImageClueDB; select CluesJson from Clues where CluesID = @clueID";
        }

        private static List<List<Tuple<string, string>>> GetTeamsJsonByID(int teamsId)
        {
            string sqlQuery = GetTeamsJsonSqlQuery();
            Dictionary<string, string> sqlParams = GetTeamsJsonSqlParams(teamsId);
            SQLHelper sqlHelper = new SQLHelper();
            List<List<object>> response = sqlHelper.RunSQLQuery(sqlQuery, sqlParams);
            if (response.Count == 0)
            {
                throw new Exception(string.Format("Unable to find Team with ID {0}", teamsId));
            }
            return JsonConvert.DeserializeObject<List<List<Tuple<string, string>>>>(response[0][0].ToString());
        }

        public static Dictionary<string, string> GetTeamsJsonSqlParams(int teamsId)
        {
            Dictionary<string, string> sqlParams = new Dictionary<string, string>(1);
            sqlParams.Add("@teamID", teamsId.ToString());
            return sqlParams;
        }

        public static string GetTeamsJsonSqlQuery()
        {
            return "use ImageClueDB; select TeamsJson from Teams where TeamID = @teamID";
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
            string authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH");
            if (string.IsNullOrWhiteSpace(authToken))
            {
                throw new ApiException("No Auth token defined for Twilio - TWILIO_AUTH");
            }

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
