using System;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CaroLAN
{
    public static class SoundManager
    {
        private static bool _sfxEnabled = true;
        private static bool _musicEnabled = true;
        
        private static string SoundFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");
        
        private const string MUSIC_LOBBY = "background.wav";
        private const string MUSIC_GAME = "background.wav";
        private const string SFX_CLICK = "button_click.wav";
        private const string SFX_MOVE = "piece_click.wav";
        private const string SFX_WIN = "game_win.wav";
        private const string SFX_LOSE = "game_lose.wav";
        
        private static SoundPlayer? _sfxPlayer;
        
        private static SoundPlayer? _musicPlayer;
        
        private static bool _isMusicPlaying = false;
        private static string _currentMusicFile = string.Empty;

        public static bool SfxEnabled
        {
            get => _sfxEnabled;
            set => _sfxEnabled = value;
        }

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
                    PlayMusic(_currentMusicFile);
                }
            }
        }

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

                StopMusicInternal();

                _musicPlayer = new SoundPlayer(filePath);
                _musicPlayer.PlayLooping();

                _isMusicPlaying = true;
                _currentMusicFile = fileName;
                
                System.Diagnostics.Debug.WriteLine($"Music playing: {fileName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PlayMusic error: {ex.Message}");
            }
        }

        public static void PlayLobbyMusic()
        {
            PlayMusic(MUSIC_LOBBY);
        }

        public static void PlayGameMusic()
        {
            PlayMusic(MUSIC_GAME);
        }

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

                _sfxPlayer?.Dispose();
                _sfxPlayer = new SoundPlayer(filePath);
                _sfxPlayer.Play();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PlaySfx error: {ex.Message}");
            }
        }

        public static void PlayClickSound()
        {
            PlaySfx(SFX_CLICK);
        }

        public static void PlayMoveSound()
        {
            PlaySfx(SFX_MOVE);
        }

        public static void PlayWinSound()
        {
            PlaySfx(SFX_WIN);
        }

        public static void PlayLoseSound()
        {
            PlaySfx(SFX_LOSE);
        }

        public static bool ToggleSfx()
        {
            SfxEnabled = !SfxEnabled;
            return SfxEnabled;
        }

        public static bool ToggleMusic()
        {
            MusicEnabled = !MusicEnabled;
            return MusicEnabled;
        }

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
