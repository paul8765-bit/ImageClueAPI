using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security;

namespace PairAndImagesLibrary
{
    public class SQLHelper
    {
        public SQLHelper()
        {
            // Create the Command and Parameter objects.
            connection = new SqlConnection(GetSQLConnectionString());
            AddAuthToConnection(connection);
        }

        private SecureString GetEnvironmentVariableSecure(string envName)
        {
            string envValueRaw = Environment.GetEnvironmentVariable(envName);
            if (string.IsNullOrWhiteSpace(envValueRaw))
            {
                throw new Exception("Please add env variable " + envName);
            }
            SecureString envValueSecure = new SecureString();
            foreach (char currentLetter in envValueRaw)
            {
                envValueSecure.AppendChar(currentLetter);
            }
            envValueSecure.MakeReadOnly();
            return envValueSecure;
        }

        private void AddAuthToConnection(SqlConnection connection)
        {
            string sqlUser = "sa";
            SecureString sqlPassword = GetEnvironmentVariableSecure("SQL_PASSWORD");
            SqlCredential creds = new SqlCredential(sqlUser, sqlPassword);
            connection.Credential = creds;
        }

        private string GetSQLConnectionString()
        {
            string sqlServerIP = Environment.GetEnvironmentVariable("SQL_SERVER_IP");
            if (string.IsNullOrWhiteSpace(sqlServerIP))
            {
                throw new Exception("Please add env variable SQL_SERVER_IP");
            }
            return string.Format("Data Source={0};Initial Catalog=ImageClueDB;"
            + "Integrated Security=false", sqlServerIP);
        }

        public List<List<object>> RunSQLQuery(string sqlQuery, Dictionary<string, string> sqlQueryParameters)
        {
            List<List<object>> resultTable = null;
            using (connection)
            {
                // Set the content of a SQL query
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                foreach (KeyValuePair<string, string> currentQueryParam in sqlQueryParameters)
                {
                    command.Parameters.AddWithValue(currentQueryParam.Key, currentQueryParam.Value);
                }

                // Open the connection in a try/catch block.
                // Create and execute the DataReader, writing the result
                // set to the console window.
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // For each row in the result
                resultTable = new List<List<object>>();
                while (reader.Read())
                {
                    List<object> currentRow = new List<object>(reader.FieldCount);
                    // For each column in the current row
                    for (int fieldIndex = 0; fieldIndex < reader.VisibleFieldCount; fieldIndex++)
                    {
                        currentRow.Add(reader[fieldIndex]);
                    }
                    resultTable.Add(currentRow);
                }
                reader.Close();
            }
            return resultTable;
        }

        private SqlConnection connection;
    }
}