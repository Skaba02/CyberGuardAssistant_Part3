// DatabaseService.cs - Handles all MySQL database operations for task storage
using MySql.Data.MySqlClient;
using CyberGuardAssistant.Models;

namespace CyberGuardAssistant.Data
{
    // Handles all communication with the MySQL database
    // Used by the Task Assistant feature (Part 3 - Task 1)
    public class DatabaseService
    {
        // Connection string - update the password to match your MySQL setup
        private const string ConnectionString =
            "Server=localhost;Database=cyberguard;Uid=root;Pwd=root;";

        // ----------------------------------------------------------------
        // Sets up the database and table if they do not exist yet
        // Call this once when the app starts
        // ----------------------------------------------------------------
        public static void Initialise()
        {
            try
            {
                // First create the database if it does not exist
                string createDb =
                    "Server=localhost;Uid=root;Pwd=root;";

                using var conn = new MySqlConnection(createDb);
                conn.Open();

                string sql = "CREATE DATABASE IF NOT EXISTS cyberguard;";
                new MySqlCommand(sql, conn).ExecuteNonQuery();
                conn.Close();

                // Now create the tasks table inside cyberguard database
                using var conn2 = new MySqlConnection(ConnectionString);
                conn2.Open();

                string createTable = @"
                    CREATE TABLE IF NOT EXISTS tasks (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        title VARCHAR(200) NOT NULL,
                        description VARCHAR(500),
                        is_completed TINYINT(1) DEFAULT 0,
                        reminder_date DATETIME NULL,
                        created_at DATETIME DEFAULT CURRENT_TIMESTAMP
                    );";

                new MySqlCommand(createTable, conn2).ExecuteNonQuery();
            }
            catch
            {
                // If database is not available, the app still runs
                // Task features will show a friendly message instead
            }
        }

        // ----------------------------------------------------------------
        // Add a new task to the database
        // ----------------------------------------------------------------
        public static bool AddTask(CyberTask task)
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();

                string sql = @"INSERT INTO tasks (title, description, reminder_date)
                               VALUES (@title, @desc, @reminder);";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@title", task.Title);
                cmd.Parameters.AddWithValue("@desc", task.Description);
                cmd.Parameters.AddWithValue("@reminder",
                    task.ReminderDate.HasValue ? (object)task.ReminderDate.Value : DBNull.Value);

                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ----------------------------------------------------------------
        // Get all tasks from the database
        // ----------------------------------------------------------------
        public static List<CyberTask> GetAllTasks()
        {
            var tasks = new List<CyberTask>();

            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();

                string sql = "SELECT id, title, description, is_completed, reminder_date FROM tasks ORDER BY id DESC;";
                using var cmd = new MySqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var task = new CyberTask
                    {
                        Id = reader.GetInt32("id"),
                        Title = reader.GetString("title"),
                        Description = reader.IsDBNull(reader.GetOrdinal("description"))
                            ? "" : reader.GetString("description"),
                        IsCompleted = reader.GetBoolean("is_completed"),
                        ReminderDate = reader.IsDBNull(reader.GetOrdinal("reminder_date"))
                            ? null : reader.GetDateTime("reminder_date")
                    };
                    tasks.Add(task);
                }
            }
            catch
            {
                // Return empty list if database is unavailable
            }

            return tasks;
        }

        // ----------------------------------------------------------------
        // Mark a task as completed
        // ----------------------------------------------------------------
        public static bool CompleteTask(int taskId)
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();

                string sql = "UPDATE tasks SET is_completed = 1 WHERE id = @id;";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", taskId);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ----------------------------------------------------------------
        // Delete a task from the database
        // ----------------------------------------------------------------
        public static bool DeleteTask(int taskId)
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();

                string sql = "DELETE FROM tasks WHERE id = @id;";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", taskId);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
