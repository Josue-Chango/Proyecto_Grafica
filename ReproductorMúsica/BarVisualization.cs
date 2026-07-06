using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ReproductorMúsica
{
    // Renderizador de la visualización de tipo "Barras" (ecualizador)
    // Dibuja barras verticales en la parte inferior cuya altura varía con el espectro de audio
    internal class BarVisualization
    {
        // Escala de altura de las barras (mayor valor = barras más altas)
        private float barScale = 0.6f;

        // Dibuja las barras del ecualizador en el canvas
        // - canvasWidth, canvasHeight: dimensiones del área de dibujo
        // - spectrumSmooth: datos del espectro suavizados (64 bandas)
        // - beatPulse: pulso rítmico actual (0 cuando no hay beat)
        // - lowEnergy: energía de frecuencias graves (refuerza las barras)
        // - themeStart, themeEnd: colores del degradado de las barras
        public void Draw(Graphics g, int canvasWidth, int canvasHeight, float[] spectrumSmooth, float beatPulse, float lowEnergy, Color themeStart, Color themeEnd)
        {
            int count = spectrumSmooth.Length;
            if (count <= 0) return;
            float barWidth = (float)canvasWidth / count;

            for (int i = 0; i < count; i++)
            {
                float x = i * barWidth;
                float amp = spectrumSmooth[i];

                // Transformación no lineal: comprime la dinámica para que valores pequeños también se vean
                float nonlinear = (float)Math.Pow(Math.Max(0f, amp), 0.6);

                // Refuerzo por beat: las barras bajas (i grande) reciben más impulso
                float ampBoost = 1f + beatPulse * (0.35f + (1f - i / (float)count) * 0.65f);

                // Refuerzo por graves: la energía baja infla todas las barras
                float energyBoost = 1f + lowEnergy * 1.8f;

                // Altura final combinando todos los factores
                float barHeight = nonlinear * canvasHeight * 0.6f * barScale * ampBoost * energyBoost;

                // Si la barra es demasiado pequeña, dibujar solo una línea mínima
                if (!(barHeight > 1f))
                {
                    float minCapH = Math.Min(2f, canvasHeight * 0.02f);
                    try { g.FillRectangle(Brushes.Transparent, x + 1, canvasHeight - minCapH, Math.Max(1f, barWidth - 2), minCapH); } catch { }
                    continue;
                }

                RectangleF rect = new RectangleF(x + 1, canvasHeight - barHeight, barWidth - 2, barHeight);

                // Dibujar la barra con degradado vertical (arriba: themeStart, abajo: themeEnd)
                try
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(rect, themeStart, themeEnd, LinearGradientMode.Vertical))
                    {
                        ColorBlend cb = new ColorBlend(3);
                        cb.Colors = new Color[] { Color.FromArgb(220, themeStart.R, themeStart.G, themeStart.B), themeStart, themeEnd };
                        cb.Positions = new float[] { 0f, 0.5f, 1f };
                        brush.InterpolationColors = cb;
                        g.FillRectangle(brush, rect);
                    }
                }
                catch { }

                // Pequeño borde blanco semitransparente en la parte superior de cada barra
                using (Brush cap = new SolidBrush(Color.FromArgb(255, 165, 0, 0)))
                {
                    float capH = Math.Min(10, barHeight);
                    if (capH > 0.5f)
                    {
                        try { g.FillRectangle(cap, x + 1, canvasHeight - barHeight - capH, Math.Max(1f, barWidth - 2), capH); } catch { }
                    }
                }

                // --- Barra superior (marco) ---
                RectangleF rectTop = new RectangleF(x + 1, 0, barWidth - 2, barHeight);

                try
                {
                    using (LinearGradientBrush brushTop = new LinearGradientBrush(rectTop, themeEnd, themeStart, LinearGradientMode.Vertical))
                    {
                        ColorBlend cb = new ColorBlend(3);
                        cb.Colors = new Color[] { Color.FromArgb(220, themeStart.R, themeStart.G, themeStart.B), themeEnd, themeStart };
                        cb.Positions = new float[] { 0f, 0.5f, 1f };
                        brushTop.InterpolationColors = cb;
                        g.FillRectangle(brushTop, rectTop);
                    }
                }
                catch { }

                // Cap blanco en la parte inferior de la barra superior
                using (Brush cap = new SolidBrush(Color.FromArgb(240, 255, 255, 255)))
                {
                    float capH = Math.Min(10, barHeight);
                    if (capH > 0.5f)
                    {
                        try { g.FillRectangle(cap, x + 1, barHeight, Math.Max(1f, barWidth - 2), capH); } catch { }
                    }
                }
            }
        }
    }
}
