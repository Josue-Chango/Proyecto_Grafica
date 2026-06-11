using System;

namespace ReproductorMúsica
{
    // Gestiona los datos del espectro de audio para la visualización
    // Puede obtener datos reales mediante FFT (AudioAnalyzer) o generar una simulación matemática
    internal class SpectrumData
    {
        // Cantidad de bandas de frecuencia (barras) que se usarán en la visualización
        private int barCount = 64;

        // Valores instantáneos del espectro (sin suavizar)
        private float[] spectrum;

        // Valores suavizados del espectro usando media exponencial (EMA)
        // Esto evita que las barras brinquen erráticamente
        private float[] spectrumSmooth;

        // Factor de suavizado: 0 = sin suavizar, 1 = completamente suave
        private float spectrumSmoothing = 0.4f;

        private Random rng = new Random();

        // Analizador FFT real (captura del micrófono vía NAudio)
        private AudioAnalyzer analyzer;

        // Propiedad pública para acceder al espectro suavizado
        public float[] SpectrumSmooth => spectrumSmooth;

        public SpectrumData(AudioAnalyzer analyzer = null)
        {
            this.analyzer = analyzer;
            spectrum = new float[barCount];
            spectrumSmooth = new float[barCount];
        }

        // Actualiza los arrays del espectro cada frame
        // - Si hay analizador real, obtiene datos FFT del micrófono
        // - Siempre genera una simulación de respaldo (solo se usa si no hay analizador)
        // - position: posición actual de la canción (para sincronizar la simulación)
        // - offset: contador de animación (para variación temporal)
        public void Update(double position, int offset)
        {
            // Obtener datos del analizador FFT real si está disponible
            if (analyzer != null)
            {
                try
                {
                    var real = analyzer.GetSpectrum(barCount);
                    for (int i = 0; i < barCount && i < real.Length; i++)
                    {
                        spectrum[i] = real[i];
                        // Aplicar suavizado exponencial
                        spectrumSmooth[i] = spectrumSmooth[i] * spectrumSmoothing + spectrum[i] * (1f - spectrumSmoothing);
                    }
                }
                catch { }
            }

            // Generar simulación matemática del espectro usando senos y cosenos
            // Esto sirve como respaldo cuando no hay micrófono disponible
            int seed = (int)(DateTime.Now.Ticks & 0xFFFFFF);
            var localRng = new Random(seed ^ offset);

            for (int i = 0; i < barCount; i++)
            {
                // Combinar posición de la canción + índice de banda + offset para variación
                double t = position * 2.0 + i * 0.13 + offset * 0.01 + localRng.NextDouble() * 0.5;
                float value = (float)((Math.Abs(Math.Sin(t + i)) * 0.8) + (localRng.NextDouble() * 0.2));
                // Las frecuencias altas (índice i grande) tienen menor amplitud (falloff)
                float falloff = 1.0f - (i / (float)barCount);
                value *= (0.3f + 0.7f * falloff);

                // Solo asignar simulación si no hay analizador real
                if (analyzer == null)
                {
                    spectrum[i] = value;
                    spectrumSmooth[i] = spectrumSmooth[i] * spectrumSmoothing + spectrum[i] * (1f - spectrumSmoothing);
                }
            }
        }

        // Reinicia todos los valores del espectro a cero
        public void Reset()
        {
            for (int i = 0; i < spectrum.Length; i++)
                spectrum[i] = spectrumSmooth[i] = 0f;
        }
    }
}
