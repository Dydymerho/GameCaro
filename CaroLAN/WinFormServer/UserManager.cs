using System;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;

namespace WinFormServer
{
    public class UserManager
    {
        private string connectionString;

        public UserManager(string server, string database, string userId, string password)
        {
            connectionString = $"Server={server};Database={database};Uid={userId};Pwd={password};CharSet=utf8mb4;";
        }

        // Hash password
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Đăng ký người dùng mới
        public bool Register(string username, string password, string email = "")
        {
            try
            {
                // Kiểm tra username đã tồn tại chưa
                if (UserExists(username))
                {
                    return false;
                }

                string passwordHash = HashPassword(password);

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO users (username, password_hash, email) VALUES (@username, @password_hash, @email)";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password_hash", passwordHash);
                        cmd.Parameters.AddWithValue("@email", string.IsNullOrEmpty(email) ? DBNull.Value : email);
                        
                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi đăng ký: {ex.Message}");
                return false;
            }
        }

        // Đăng nhập
        public User? Login(string username, string password)
        {
            try
            {
                string passwordHash = HashPassword(password);

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT id, username, email, created_at, last_login, total_games, wins, losses " +
                                   "FROM users WHERE username = @username AND password_hash = @password_hash";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password_hash", passwordHash);
                        
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                User user = new User
                                {
                                    Id = reader.GetInt32("id"),
                                    Username = reader.GetString("username"),
                                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? "" : reader.GetString("email"),
                                    CreatedAt = reader.GetDateTime("created_at"),
                                    LastLogin = reader.IsDBNull(reader.GetOrdinal("last_login")) ? null : reader.GetDateTime("last_login"),
                                    TotalGames = reader.GetInt32("total_games"),
                                    Wins = reader.GetInt32("wins"),
                                    Losses = reader.GetInt32("losses")
                                };

                                // Cập nhật last_login
                                reader.Close();
                                UpdateLastLogin(user.Id);

                                return user;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi đăng nhập: {ex.Message}");
                return null;
            }
        }

        private bool UserExists(string username)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE username = @username";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        object result = cmd.ExecuteScalar();
                        return Convert.ToInt32(result) > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private void UpdateLastLogin(int userId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE users SET last_login = NOW() WHERE id = @id";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", userId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi cập nhật last_login: {ex.Message}");
            }
        }

        public void UpdateGameStats(int userId, bool isWin)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE users SET total_games = total_games + 1, " +
                                   (isWin ? "wins = wins + 1" : "losses = losses + 1") +
                                   " WHERE id = @id";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", userId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi cập nhật thống kê: {ex.Message}");
            }
        }

        public User? GetUserById(int userId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT id, username, email, created_at, last_login, total_games, wins, losses " +
                                   "FROM users WHERE id = @id";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", userId);
                        
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    Id = reader.GetInt32("id"),
                                    Username = reader.GetString("username"),
                                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? "" : reader.GetString("email"),
                                    CreatedAt = reader.GetDateTime("created_at"),
                                    LastLogin = reader.IsDBNull(reader.GetOrdinal("last_login")) ? null : reader.GetDateTime("last_login"),
                                    TotalGames = reader.GetInt32("total_games"),
                                    Wins = reader.GetInt32("wins"),
                                    Losses = reader.GetInt32("losses")
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy thông tin user: {ex.Message}");
                return null;
            }
        }
    }
}

