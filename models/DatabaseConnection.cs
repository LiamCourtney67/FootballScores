using MySqlConnector;
using System.Data;

namespace FootballScoresUI.models
{
    /// <summary>
    /// Database connection class for the FootballScores database.
    /// </summary>
    public static class DatabaseConnection
    {
        private static MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder();
        private static MySqlConnection connection;

        /// <summary>
        /// Opening a connection to the database.
        /// </summary>
        /// <returns>True if connection opened, otherwise an exception.</returns>
        public static bool OpenConnection()
        {
            connectionStringBuilder.Server = "localhost";
            connectionStringBuilder.UserID = "root";
            connectionStringBuilder.Database = "FootballScores";
            connectionStringBuilder.CharacterSet = "utf8";
            connection = new MySqlConnection(connectionStringBuilder.ToString());

            try
            {
                if (connection.State == ConnectionState.Closed) { connection.Open(); }
                return true;
            }
            catch (MySqlException) { throw; }
        }

        /// <summary>
        /// Closing the connection to the database.
        /// </summary>
        /// <returns>True if connection closed, otherwise an exception.</returns>
        public static bool CloseConnection()
        {
            try
            {
                if (connection.State == ConnectionState.Open) { connection.Close(); }
                return true;
            }
            catch (MySqlException) { throw; }
        }

        /// <summary>
        /// Getter for the connection.
        /// </summary>
        /// <returns>MySql database connection.</returns>
        public static MySqlConnection GetConnection() { return connection; }
    }
}
