using System;
using System.Collections.Generic;
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

        // Lưu lịch sử đấu
        public bool SaveMatchHistory(string roomId, int player1Id, int player2Id, int? winnerId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO game_history (room_id, player1_id, player2_id, winner_id, ended_at) 
                                     VALUES (@room_id, @player1_id, @player2_id, @winner_id, NOW())";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@room_id", roomId);
                        cmd.Parameters.AddWithValue("@player1_id", player1Id);
                        cmd.Parameters.AddWithValue("@player2_id", player2Id);
                        cmd.Parameters.AddWithValue("@winner_id", winnerId == null ? DBNull.Value : winnerId);
                        
                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lưu lịch sử đấu: {ex.Message}");
                return false;
            }
        }

        // Lấy lịch sử đấu của một user cụ thể
        public List<MatchHistory> GetUserMatchHistory(int userId, int limit = 100)
        {
            List<MatchHistory> history = new List<MatchHistory>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT gh.id, gh.room_id, gh.player1_id, gh.player2_id, gh.winner_id, 
                                            gh.started_at, gh.ended_at,
                                            u1.username as player1_username,
                                            u2.username as player2_username,
                                            uw.username as winner_username
                                     FROM game_history gh
                                     LEFT JOIN users u1 ON gh.player1_id = u1.id
                                     LEFT JOIN users u2 ON gh.player2_id = u2.id
                                     LEFT JOIN users uw ON gh.winner_id = uw.id
                                     WHERE gh.player1_id = @user_id OR gh.player2_id = @user_id
                                     ORDER BY gh.ended_at DESC
                                     LIMIT @limit";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        cmd.Parameters.AddWithValue("@limit", limit);
                        
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                history.Add(new MatchHistory
                                {
                                    Id = reader.GetInt32("id"),
                                    RoomId = reader.GetString("room_id"),
                                    Player1Id = reader.GetInt32("player1_id"),
                                    Player1Username = reader.GetString("player1_username"),
                                    Player2Id = reader.GetInt32("player2_id"),
                                    Player2Username = reader.GetString("player2_username"),
                                    WinnerId = reader.IsDBNull(reader.GetOrdinal("winner_id")) ? null : reader.GetInt32("winner_id"),
                                    WinnerUsername = reader.IsDBNull(reader.GetOrdinal("winner_username")) ? "Hòa" : reader.GetString("winner_username"),
                                    StartedAt = reader.GetDateTime("started_at"),
                                    EndedAt = reader.IsDBNull(reader.GetOrdinal("ended_at")) ? null : reader.GetDateTime("ended_at")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy lịch sử đấu của user: {ex.Message}");
            }
            return history;
        }
    }
}

