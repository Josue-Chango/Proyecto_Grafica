using System;

namespace ReproductorMúsica
{
    // Detecta golpes rítmicos (beats) analizando cambios en la amplitud del audio
    // Cuando la energía sube bruscamente, se dispara un pulso que las visualizaciones usan para animarse
    internal class BeatDetector
    {
        // Amplitud promedio del frame anterior (para calcular el cambio)
        private float prevAvg = 0f;

        // Contador para evitar múltiples beats en muy poco tiempo (6 frames ≈ 240ms)
        private int beatCooldown = 0;

        // Pulso del beat (0 a ~2.0). Se incrementa al detectar un golpe y decae gradualmente
        public float BeatPulse { get; private set; }

        // Energía promedio de las frecuencias bajas (graves)
        // Se usa para vibración del centro y refuerzo de visualización
        public float LowEnergy { get; private set; }

        // Desplazamiento del centro por vibración de graves
        // Simula el "bass shake" moviendo ligeramente el centro de los círculos
        public float CenterOffsetX { get; private set; }
        public float CenterOffsetY { get; private set; }

        // Diferencia de amplitud entre el frame actual y el anterior
        // Un valor positivo grande indica un golpe rítmico
        public float Delta { get; private set; }

        // Indica si se pueden generar nuevas partículas (cooldown terminado)
        public bool CanSpawn => beatCooldown == 0;

        // Analiza el espectro suavizado para detectar beats
        // 1. Calcula el promedio general de amplitud
        // 2. Calcula la energía de graves (primeras bandas)
        // 3. Actualiza el desplazamiento del centro según las graves
        // 4. Compara con el frame anterior para detectar cambios bruscos
        public void Analyze(float[] spectrumSmooth, int offset)
        {
            // Promedio general de amplitud
            float avg = 0f;
            for (int i = 0; i < spectrumSmooth.Length; i++)
                avg += spectrumSmooth[i];
            avg /= spectrumSmooth.Length;

            // Energía de las frecuencias bajas (primeras bandas)
            int lowBands = Math.Max(2, spectrumSmooth.Length / 12);
            float lowSum = 0f;
            for (int i = 0; i < lowBands; i++)
                lowSum += spectrumSmooth[i];
            LowEnergy = lowSum / lowBands;

            // Vibración del centro basada en graves: movimiento sinusoidal
            CenterOffsetX = (float)(Math.Sin(offset * 0.02) * LowEnergy * 28.0);
            CenterOffsetY = (float)(Math.Cos(offset * 0.017) * LowEnergy * 18.0);

            // Calcular cambio de amplitud respecto al frame anterior
            Delta = avg - prevAvg;

            // Si la amplitud subió más del umbral (0.06), hay un beat
            if (Delta > 0.06f)
            {
                // Acumular pulso (máximo 2.0) para que múltiples beats seguidos sumen fuerza
                BeatPulse = Math.Min(2.0f, BeatPulse + Delta * 8f);
            }

            prevAvg = avg;
        }

        // Reinicia el cooldown de partículas (6 frames de espera)
        public void ResetCooldown()
        {
            beatCooldown = 6;
        }

        // Decaimiento del pulso: se llama cada frame después del renderizado
        // El pulso se reduce un 8% por frame (factor 0.92)
        public void Decay()
        {
            BeatPulse *= 0.92f;
            if (BeatPulse < 0.01f) BeatPulse = 0f;
            if (beatCooldown > 0) beatCooldown--;
        }

        // Reinicia todo el estado del detector
        public void Reset()
        {
            prevAvg = 0f;
            BeatPulse = 0f;
            beatCooldown = 0;
        }
    }
}
