using MySql.Data.MySqlClient;
using OnlineTestSystem.Models;
using System.Data;

namespace OnlineTestSystem.DataAccess
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public User GetUserByCredentials(string username, string password)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            var query = "SELECT * FROM Users WHERE (RollNumber = @Username OR Name = @Username) AND Password = @Password";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    UserId = reader.GetInt32("UserId"),
                    RollNumber = reader.IsDBNull("RollNumber") ? null : reader.GetString("RollNumber"),
                    Name = reader.GetString("Name"),
                    Class = reader.IsDBNull("Class") ? null : reader.GetString("Class"),
                    Course = reader.IsDBNull("Course") ? null : reader.GetString("Course"),
                    Password = reader.GetString("Password"),
                    Role = reader.GetString("Role")
                };
            }
            return null;
        }

        public List<User> GetStudents()
        {
            var students = new List<User>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            var query = "SELECT * FROM Users WHERE Role = 'Student'";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                students.Add(new User
                {
                    UserId = reader.GetInt32("UserId"),
                    RollNumber = reader.IsDBNull("RollNumber") ? null : reader.GetString("RollNumber"),
                    Name = reader.GetString("Name"),
                    Class = reader.IsDBNull("Class") ? null : reader.GetString("Class"),
                    Course = reader.IsDBNull("Course") ? null : reader.GetString("Course"),
                    Password = reader.GetString("Password"),
                    Role = reader.GetString("Role")
                });
            }
            return students;
        }

        public void AddStudent(User student)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                var query = "INSERT INTO Users (RollNumber, Name, Class, Course, Password, Role) VALUES (@RollNumber, @Name, @Class, @Course, @Password, 'Student')";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@RollNumber", string.IsNullOrEmpty(student.RollNumber) ? DBNull.Value : student.RollNumber);
                command.Parameters.AddWithValue("@Name", student.Name ?? throw new ArgumentNullException(nameof(student.Name)));
                command.Parameters.AddWithValue("@Class", string.IsNullOrEmpty(student.Class) ? DBNull.Value : student.Class);
                command.Parameters.AddWithValue("@Course", string.IsNullOrEmpty(student.Course) ? DBNull.Value : student.Course);
                command.Parameters.AddWithValue("@Password", student.Password ?? throw new ArgumentNullException(nameof(student.Password)));
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddStudent: {ex.Message}");
                throw;
            }
        }

        public void DeleteStudent(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            var query = "DELETE FROM Users WHERE UserId = @Id AND Role = 'Student'";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }

        public void UpdateStudent(User student)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                var query = "UPDATE Users SET RollNumber = @RollNumber, Name = @Name, Class = @Class, Course = @Course, Password = @Password WHERE UserId = @UserId AND Role = 'Student'";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@RollNumber", string.IsNullOrEmpty(student.RollNumber) ? DBNull.Value : student.RollNumber);
                command.Parameters.AddWithValue("@Name", student.Name ?? throw new ArgumentNullException(nameof(student.Name)));
                command.Parameters.AddWithValue("@Class", string.IsNullOrEmpty(student.Class) ? DBNull.Value : student.Class);
                command.Parameters.AddWithValue("@Course", string.IsNullOrEmpty(student.Course) ? DBNull.Value : student.Course);
                command.Parameters.AddWithValue("@Password", student.Password ?? throw new ArgumentNullException(nameof(student.Password)));
                command.Parameters.AddWithValue("@UserId", student.UserId);
                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                    throw new Exception("No student found with the specified UserId or the user is not a student.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateStudent: {ex.Message}");
                throw;
            }
        }

        public List<Question> GetQuestions()
        {
            var questions = new List<Question>();
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                var query = "SELECT * FROM Questions";
                using var command = new MySqlCommand(query, connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    questions.Add(new Question
                    {
                        QuestionId = reader.GetInt32("QuestionId"),
                        QuestionText = reader.GetString("QuestionText"),
                        Option1 = reader.GetString("Option1"),
                        Option2 = reader.GetString("Option2"),
                        Option3 = reader.GetString("Option3"),
                        Option4 = reader.GetString("Option4"),
                        CorrectOption = reader.GetInt32("CorrectOption")
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetQuestions: {ex.Message}");
                // Return empty list instead of throwing
            }
            return questions;
        }

        public Question GetQuestionById(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            var query = "SELECT * FROM Questions WHERE QuestionId = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Question
                {
                    QuestionId = reader.GetInt32("QuestionId"),
                    QuestionText = reader.GetString("QuestionText"),
                    Option1 = reader.GetString("Option1"),
                    Option2 = reader.GetString("Option2"),
                    Option3 = reader.GetString("Option3"),
                    Option4 = reader.GetString("Option4"),
                    CorrectOption = reader.GetInt32("CorrectOption")
                };
            }
            return null;
        }

        public void AddQuestion(Question question)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            var query = "INSERT INTO Questions (QuestionText, Option1, Option2, Option3, Option4, CorrectOption) VALUES (@Text, @Opt1, @Opt2, @Opt3, @Opt4, @Correct)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Text", question.QuestionText);
            command.Parameters.AddWithValue("@Opt1", question.Option1);
            command.Parameters.AddWithValue("@Opt2", question.Option2);
            command.Parameters.AddWithValue("@Opt3", question.Option3);
            command.Parameters.AddWithValue("@Opt4", question.Option4);
            command.Parameters.AddWithValue("@Correct", question.CorrectOption);
            command.ExecuteNonQuery();
        }

        public void DeleteQuestion(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            var query = "DELETE FROM Questions WHERE QuestionId = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }

        public void SaveTestResult(int userId, int marks)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            var query = "INSERT INTO TestResults (UserId, Marks, TestDate) VALUES (@UserId, @Marks, @TestDate)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@Marks", marks);
            command.Parameters.AddWithValue("@TestDate", DateTime.Now);
            command.ExecuteNonQuery();
        }

        public List<object> GetMarks()
        {
            var results = new List<object>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            var query = "SELECT u.RollNumber, u.Name, t.Marks FROM TestResults t JOIN Users u ON t.UserId = u.UserId";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                results.Add(new
                {
                    RollNumber = reader.IsDBNull("RollNumber") ? null : reader.GetString("RollNumber"),
                    Name = reader.GetString("Name"),
                    Marks = reader.GetInt32("Marks")
                });
            }
            return results;
        }
    }
}