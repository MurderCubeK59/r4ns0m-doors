using NAudio.Wave;
using System;
using System.IO;

namespace r4ns0m
{
    public class SoundHandle
    {
        private readonly WaveOut _waveOut;
        private readonly WaveFileReader _reader;
        private readonly Stream _stream;

        private bool _disposed;
        private bool _looping;

        public SoundHandle(
            WaveOut waveOut,
            WaveFileReader reader,
            Stream stream)
        {
            _waveOut = waveOut;
            _reader = reader;
            _stream = stream;

            _waveOut.PlaybackStopped += OnPlaybackStopped;
        }

        public void Play()
        {
            if (_disposed)
                return;

            _waveOut.Play();
        }

        public void PlayLooping()
        {
            if (_disposed)
                return;

            _looping = true;
            _waveOut.Play();
        }

        private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
        {
            if (_looping && !_disposed)
            {
                _reader.Position = 0;
                _waveOut.Play();
                return;
            }

            DisposeOnce();
        }

        public void Stop()
        {
            if (_disposed)
                return;

            _looping = false;

            try
            {
                _waveOut.Stop();
            }
            catch
            {
            }

            DisposeOnce();
        }

        private void DisposeOnce()
        {
            if (_disposed)
                return;

            _disposed = true;

            _waveOut.Dispose();
            _reader.Dispose();
            _stream.Dispose();
        }
    }

    public class SoundHelper
    {
        public static SoundHandle Create(Stream wavStream)
        {
            WaveFileReader reader = new WaveFileReader(wavStream);
            WaveOut waveOut = new WaveOut();

            waveOut.Init(reader);

            return new SoundHandle(
                waveOut,
                reader,
                wavStream);
        }
    }
}