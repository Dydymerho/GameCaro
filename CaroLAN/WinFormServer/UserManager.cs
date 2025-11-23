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

        // Kiểm tra và tạo database nếu chưa có
        public static bool InitializeDatabase(string server, string database, string userId, string password, Action<string>? logAction = null)
        {
            try
            {
                // Kết nối đến MySQL server (không chỉ định database)
                string serverConnectionString = $"Server={server};Uid={userId};Pwd={password};CharSet=utf8mb4;";
                
                using (MySqlConnection conn = new MySqlConnection(serverConnectionString))
                {
                    conn.Open();
                    logAction?.Invoke("Đã kết nối đến MySQL server.");

                    // Kiểm tra xem database có tồn tại không
                    string checkDbQuery = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{database}'";
                    using (MySqlCommand cmd = new MySqlCommand(checkDbQuery, conn))
                    {
                        object? result = cmd.ExecuteScalar();
                        
                        if (result == null)
                        {
                            // Database chưa tồn tại, tạo mới
                            logAction?.Invoke($"Database '{database}' chưa tồn tại. Đang tạo database...");
                            string createDbQuery = $"CREATE DATABASE IF NOT EXISTS {database} CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci";
                            using (MySqlCommand createCmd = new MySqlCommand(createDbQuery, conn))
                            {
                                createCmd.ExecuteNonQuery();
                                logAction?.Invoke($"Đã tạo database '{database}' thành công.");
                            }
                        }
                        else
                        {
                            logAction?.Invoke($"Database '{database}' đã tồn tại.");
                        }
                    }

                    // Kết nối đến database vừa tạo/kiểm tra để tạo tables
                    string dbConnectionString = $"Server={server};Database={database};Uid={userId};Pwd={password};CharSet=utf8mb4;";
                    using (MySqlConnection dbConn = new MySqlConnection(dbConnectionString))
                    {
                        dbConn.Open();
                        
                        // Tạo bảng users nếu chưa có
                        string createUsersTable = @"
                            CREATE TABLE IF NOT EXISTS users (
                                id INT AUTO_INCREMENT PRIMARY KEY,
                                username VARCHAR(50) UNIQUE NOT NULL,
                                password_hash VARCHAR(255) NOT NULL,
                                email VARCHAR(100),
                                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                                last_login TIMESTAMP NULL,
                                total_games INT DEFAULT 0,
                                wins INT DEFAULT 0,
                                losses INT DEFAULT 0,
                                INDEX idx_username (username)
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci";
                        
                        using (MySqlCommand cmd = new MySqlCommand(createUsersTable, dbConn))
                        {
                            cmd.ExecuteNonQuery();
                            logAction?.Invoke("Đã kiểm tra/tạo bảng 'users'.");
                        }

                        // Tạo bảng game_history nếu chưa có
                        string createGameHistoryTable = @"
                            CREATE TABLE IF NOT EXISTS game_history (
                                id INT AUTO_INCREMENT PRIMARY KEY,
                                room_id VARCHAR(20),
                                player1_id INT,
                                player2_id INT,
                                winner_id INT,
                                started_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                                ended_at TIMESTAMP NULL,
                                FOREIGN KEY (player1_id) REFERENCES users(id),
                                FOREIGN KEY (player2_id) REFERENCES users(id),
                                FOREIGN KEY (winner_id) REFERENCES users(id)
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci";
                        
                        using (MySqlCommand cmd = new MySqlCommand(createGameHistoryTable, dbConn))
                        {
                            cmd.ExecuteNonQuery();
                            logAction?.Invoke("Đã kiểm tra/tạo bảng 'game_history'.");
                        }
                    }

                    logAction?.Invoke("Khởi tạo database hoàn tất.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Lỗi khi khởi tạo database: {ex.Message}");
                return false;
            }
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

