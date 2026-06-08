using System;
using System.Linq;
using System.Threading;
using NAudio.Wave;
using NAudio.Dsp;

namespace ReproductorMúsica
{
    internal class AudioAnalyzer : IDisposable
    {
        private IWaveIn waveIn;
        private readonly int fftSize;
        private readonly int fftM;
        private Complex[] fftBuffer;
        private float[] sampleBuffer;
        private int sampleOffset = 0;
        private readonly float[] window;
        private readonly object lockObj = new object();
        private float[] latestMagnitudes;
        private bool started = false;

        public bool IsAvailable => waveIn != null;

        public AudioAnalyzer(int fftSize = 2048)
        {
            if (fftSize <= 0 || (fftSize & (fftSize - 1)) != 0)
                throw new ArgumentException("fftSize must be a power of two");

            this.fftSize = fftSize;
            this.fftM = (int)Math.Log(fftSize, 2);
            fftBuffer = new Complex[fftSize];
            sampleBuffer = new float[fftSize];
            window = new float[fftSize];
            latestMagnitudes = new float[fftSize / 2];

            // Hann window
            for (int i = 0; i < fftSize; i++) window[i] = (float)(0.5 * (1 - Math.Cos(2 * Math.PI * i / (fftSize - 1))));

            // use WaveInEvent (microphone) as reliable fallback for compilation
            try
            {
                var win = new WaveInEvent();
                win.WaveFormat = new WaveFormat(44100, 16, 2);
                waveIn = win;
                waveIn.DataAvailable += OnDataAvailable;
            }
            catch
            {
                waveIn = null;
            }
        }

        public void Start()
        {
            if (waveIn == null) return;
            if (started) return;
            sampleOffset = 0;
            try { waveIn.StartRecording(); started = true; } catch { }
        }

        public void Stop()
        {
            if (waveIn == null) return;
            if (!started) return;
            try { waveIn.StopRecording(); started = false; } catch { }
        }

        private void OnDataAvailable(object s, WaveInEventArgs e)
        {
            if (waveIn == null) return;

            int bytesPerSample = waveIn.WaveFormat.BitsPerSample / 8;
            int channels = waveIn.WaveFormat.Channels;
            int idx = 0;
            int availableSamples = e.BytesRecorded / bytesPerSample / channels;

            for (int n = 0; n < availableSamples; n++)
            {
                float sample = 0f;
                if (waveIn.WaveFormat.BitsPerSample == 32)
                {
                    sample = BitConverter.ToSingle(e.Buffer, idx);
                    idx += 4 * channels;
                }
                else if (waveIn.WaveFormat.BitsPerSample == 16)
                {
                    short s0 = BitConverter.ToInt16(e.Buffer, idx);
                    sample = s0 / 32768f;
                    idx += 2 * channels;
                }
                else
                {
                    idx += bytesPerSample * channels;
                }

                lock (lockObj)
                {
                    sampleBuffer[sampleOffset++] = sample;
                    if (sampleOffset >= fftSize)
                    {
                        ProcessFft();
                        sampleOffset = 0;
                    }
                }
            }
        }

        private void ProcessFft()
        {
            for (int i = 0; i < fftSize; i++)
            {
                fftBuffer[i].X = sampleBuffer[i] * window[i];
                fftBuffer[i].Y = 0f;
            }

            try
            {
                FastFourierTransform.FFT(true, fftM, fftBuffer);
            }
            catch
            {
                return;
            }

            int half = fftSize / 2;
            float[] mags = new float[half];
            float max = 1e-9f;
            for (int i = 0; i < half; i++)
            {
                float mag = (float)Math.Sqrt(fftBuffer[i].X * fftBuffer[i].X + fftBuffer[i].Y * fftBuffer[i].Y);
                mags[i] = mag;
                if (mag > max) max = mag;
            }

            lock (lockObj)
            {
                for (int i = 0; i < half; i++) latestMagnitudes[i] = mags[i] / max;
            }
        }

        public float[] GetMagnitudes()
        {
            lock (lockObj)
            {
                return (float[])latestMagnitudes.Clone();
            }
        }

        public float[] GetSpectrum(int targetBands)
        {
            var mags = GetMagnitudes();
            int len = mags.Length;
            if (len == 0 || targetBands <= 0) return new float[targetBands];
            float[] result = new float[targetBands];
            for (int b = 0; b < targetBands; b++)
            {
                int start = (int)((b / (float)targetBands) * len);
                int end = (int)(((b + 1) / (float)targetBands) * len);
                if (end <= start) end = start + 1;
                float sum = 0f;
                for (int k = start; k < end && k < len; k++) sum += mags[k];
                result[b] = sum / (end - start);
            }
            return result;
        }

        public void Dispose()
        {
            try { if (waveIn != null) { waveIn.DataAvailable -= OnDataAvailable; waveIn.Dispose(); } } catch { }
        }
    }
}
