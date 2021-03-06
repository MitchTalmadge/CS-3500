﻿using System.IO;
using NAudio.Wave;

namespace SpaceWars
{
    /// <summary>
    /// Plays mp3 music files from an array of bytes in a loop.
    /// </summary>
    /// <authors>Jiahui Chen, Mitch Talmadge</authors>
    public class Mp3Player
    {
        private readonly byte[] _mp3Bytes;
        private MemoryStream _mp3Stream;
        private Mp3FileReader _mp3Reader;
        private LoopStream _loopStream;
        private WaveOutEvent _player;

        /// <summary>
        /// Loads an mp3 file for playing.
        /// </summary>
        /// <param name="mp3Bytes">The bytes of the mp3 file to play.</param>
        public Mp3Player(byte[] mp3Bytes)
        {
            _mp3Bytes = (byte[]) mp3Bytes.Clone();
        }

        /// <summary>
        /// Plays the loaded mp3 audio.
        /// </summary>
        /// <param name="loop">Determines if the audio should loop. Defaults to true.</param>
        /// <returns>True if the music was started, false if it could not start.</returns>
        public bool StartPlaying(bool loop = true)
        {
            StopPlaying();

            try
            {
                _player = new WaveOutEvent();

                _mp3Stream = new MemoryStream(_mp3Bytes);
                _mp3Reader = new Mp3FileReader(_mp3Stream);

                // Only use loop stream if looping is enabled.
                if (loop)
                {
                    _loopStream = new LoopStream(_mp3Reader);
                    _player.Init(_loopStream);
                }
                else
                {
                    _player.Init(_mp3Reader);
                }

                _player.Play();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Stops the currently playing mp3.
        /// </summary>
        public void StopPlaying()
        {
            if (_player == null)
                return;

            _player.Stop();
            _player.Dispose();
            _player = null;

            _mp3Stream.Dispose();
            _mp3Reader.Dispose();
            _loopStream?.Dispose();
        }
    }
}