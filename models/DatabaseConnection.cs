using MySqlConnector;
using System;
using System.Data;

namespace FootballScoresUI.models
{

    public static class DatabaseConnection
    {
        private static MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder();
        private static MySqlConnection connection;

        public static bool OpenConnection()
        {
            connectionStringBuilder.Server = "localhost";
            connectionStringBuilder.UserID = "root";
            connectionStringBuilder.Database = "FootballScores";
            connectionStringBuilder.CharacterSet = "utf8";
            connection = new MySqlConnection(connectionStringBuilder.ToString());

            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return true;
            }
            catch (MySqlException e)
            {
                throw new Exception("Error: " + e.Message);
            }
        }

        public static bool CloseConnection()
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                return true;
            }
            catch (MySqlException e)
            {
                throw new Exception("Error: " + e.Message);
            }
        }

        public static MySqlConnection GetConnection()
        {
            return connection;
        }
    }
}
