using System;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CaroLAN
{
    /// <summary>
    /// Quản lý âm thanh cho game Caro
    /// </summary>
    public static class SoundManager
    {
        // Cờ bật/tắt âm thanh
        private static bool _sfxEnabled = true;
        private static bool _musicEnabled = true;
        
        // Đường dẫn thư mục âm thanh
        private static string SoundFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");
        
        // Các file âm thanh - sử dụng tên file thực tế
        private const string MUSIC_LOBBY = "background.wav";
        private const string MUSIC_GAME = "background.wav";
        private const string SFX_CLICK = "button_click.wav";
        private const string SFX_MOVE = "piece_click.wav";
        private const string SFX_WIN = "game_win.wav";
        private const string SFX_LOSE = "game_lose.wav";
        
        // SoundPlayer cho SFX
        private static SoundPlayer? _sfxPlayer;
        
        // SoundPlayer cho nhạc nền (loop)
        private static SoundPlayer? _musicPlayer;
        
        // Trạng thái nhạc nền
        private static bool _isMusicPlaying = false;
        private static string _currentMusicFile = string.Empty;

        /// <summary>
        /// Bật/tắt âm thanh hiệu ứng
        /// </summary>
        public static bool SfxEnabled
        {
            get => _sfxEnabled;
            set => _sfxEnabled = value;
        }

        /// <summary>
        /// Bật/tắt nhạc nền
        /// </summary>
        public static bool MusicEnabled
        {
            get => _musicEnabled;
            set
            {
                _musicEnabled = value;
                if (!value)
                {
                    StopMusic();
                }
                else if (!string.IsNullOrEmpty(_currentMusicFile))
                {
                    // Tiếp tục phát nhạc nếu đã có
                    PlayMusic(_currentMusicFile);
                }
            }
        }

        /// <summary>
        /// Khởi tạo SoundManager - tạo thư mục Sounds nếu chưa có
        /// </summary>
        public static void Initialize()
        {
            try
            {
                if (!Directory.Exists(SoundFolder))
                {
                    Directory.CreateDirectory(SoundFolder);
                }
                
                System.Diagnostics.Debug.WriteLine($"SoundManager initialized. Sound folder: {SoundFolder}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SoundManager Initialize error: {ex.Message}");
            }
        }

        /// <summary>
        /// Phát nhạc nền (loop) sử dụng SoundPlayer.PlayLooping()
        /// </summary>
        public static void PlayMusic(string fileName)
        {
            if (!_musicEnabled) return;
            
            try
            {
                string filePath = Path.Combine(SoundFolder, fileName);
                System.Diagnostics.Debug.WriteLine($"Trying to play music: {filePath}");
                
                if (!File.Exists(filePath))
                {
                    System.Diagnostics.Debug.WriteLine($"Music file not found: {filePath}");
                    return;
                }

                // Dừng nhạc hiện tại
                StopMusicInternal();

                // Tạo SoundPlayer mới và phát loop
                _musicPlayer = new SoundPlayer(filePath);
                _musicPlayer.PlayLooping(); // Phát lặp lại liên tục

                _isMusicPlaying = true;
                _currentMusicFile = fileName;
                
                System.Diagnostics.Debug.WriteLine($"Music playing: {fileName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PlayMusic error: {ex.Message}");
            }
        }

        /// <summary>
        /// Phát nhạc lobby
        /// </summary>
        public static void PlayLobbyMusic()
        {
            PlayMusic(MUSIC_LOBBY);
        }

        /// <summary>
        /// Phát nhạc game
        /// </summary>
        public static void PlayGameMusic()
        {
            PlayMusic(MUSIC_GAME);
        }

        /// <summary>
        /// Dừng nhạc nền
        /// </summary>
        public static void StopMusic()
        {
            StopMusicInternal();
            _currentMusicFile = string.Empty;
        }

        private static void StopMusicInternal()
        {
            try
            {
                if (_isMusicPlaying && _musicPlayer != null)
                {
                    _musicPlayer.Stop();
                    _musicPlayer.Dispose();
                    _musicPlayer = null;
                    _isMusicPlaying = false;
                    System.Diagnostics.Debug.WriteLine("Music stopped");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"StopMusic error: {ex.Message}");
            }
        }

        /// <summary>
        /// Phát âm thanh hiệu ứng
        /// </summary>
        public static void PlaySfx(string fileName)
        {
            if (!_sfxEnabled) return;

            try
            {
                string filePath = Path.Combine(SoundFolder, fileName);
                if (!File.Exists(filePath))
                {
                    System.Diagnostics.Debug.WriteLine($"SFX file not found: {filePath}");
                    return;
                }

                // Sử dụng SoundPlayer để phát SFX
                _sfxPlayer?.Dispose();
                _sfxPlayer = new SoundPlayer(filePath);
                _sfxPlayer.Play();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PlaySfx error: {ex.Message}");
            }
        }

        /// <summary>
        /// Phát âm thanh click button
        /// </summary>
        public static void PlayClickSound()
        {
            PlaySfx(SFX_CLICK);
        }

        /// <summary>
        /// Phát âm thanh đặt cờ
        /// </summary>
        public static void PlayMoveSound()
        {
            PlaySfx(SFX_MOVE);
        }

        /// <summary>
        /// Phát âm thanh thắng
        /// </summary>
        public static void PlayWinSound()
        {
            PlaySfx(SFX_WIN);
        }

        /// <summary>
        /// Phát âm thanh thua
        /// </summary>
        public static void PlayLoseSound()
        {
            PlaySfx(SFX_LOSE);
        }

        /// <summary>
        /// Toggle SFX on/off
        /// </summary>
        public static bool ToggleSfx()
        {
            SfxEnabled = !SfxEnabled;
            return SfxEnabled;
        }

        /// <summary>
        /// Toggle Music on/off
        /// </summary>
        public static bool ToggleMusic()
        {
            MusicEnabled = !MusicEnabled;
            return MusicEnabled;
        }

        /// <summary>
        /// Dọn dẹp tài nguyên
        /// </summary>
        public static void Cleanup()
        {
            try
            {
                StopMusic();
                _sfxPlayer?.Dispose();
                _sfxPlayer = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Cleanup error: {ex.Message}");
            }
        }
    }
}
