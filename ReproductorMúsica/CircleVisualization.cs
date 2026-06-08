using System;
using System.Drawing;

namespace ReproductorMúsica
{
    // Renderizador de la visualización de tipo "Círculos"
    // Dibuja anillos concéntricos, un núcleo brillante, anillos laterales y chispas radiales
    // Todos los elementos reaccionan al espectro de audio y a los beats
    internal class CircleVisualization
    {
        // Dibuja la visualización completa de círculos
        // - offset: contador de animación (produce movimiento continuo)
        // - spectrumSmooth: datos del espectro suavizados
        // - beatPulse: pulso rítmico para expandir/iluminar elementos
        // - lowEnergy: energía de graves para color del núcleo y vibración
        // - cx, cy: centro del canvas (incluye vibración por graves)
        // - radius: radio máximo disponible para dibujar
        // - innerColor, outerColor: colores del tema
        public void Draw(Graphics g, int offset, float[] spectrumSmooth, float beatPulse, float lowEnergy, float cx, float cy, int radius, Color innerColor, Color outerColor)
        {
            // Amplitud promedio del espectro (volumen general)
            float avg = 0f;
            for (int i = 0; i < spectrumSmooth.Length; i++) avg += spectrumSmooth[i];
            avg /= spectrumSmooth.Length;

            // Factor de pulso: amplifica el tamaño con el beat
            float pulse = 1f + beatPulse * 1.2f;

            // Escala del centro (más grande que el radio base)
            float centerScale = 1.25f;

            // Cantidad de anillos concéntricos
            int rings = 34;

            // Radio base del primer anillo (crece con el pulso)
            float baseRadius = radius * 0.16f * pulse * centerScale;

            // === NÚCLEO BRILLANTE ===
            // Círculos rellenos concéntricos en el centro que late con la música
            try
            {
                // Color del núcleo: mezcla entre inner y outer según energía de graves
                Color coreColor = ColorUtils.BlendColors(innerColor, outerColor, Math.Max(0f, Math.Min(1f, lowEnergy)));
                // Tamaño del núcleo: base + volumen + pulso del beat
                float coreBase = baseRadius * 0.28f + avg * radius * 0.18f + beatPulse * 18f;

                // 6 capas del núcleo con transparencia decreciente
                for (int gstep = 5; gstep >= 0; gstep--)
                {
                    int alpha = ColorUtils.ClampAlpha((int)(200 * (1.0f - gstep / 6.0f)));
                    float rr = coreBase * (1.0f + gstep * 0.45f);
                    if (rr > 0.5f)
                    {
                        try
                        {
                            using (SolidBrush br = new SolidBrush(Color.FromArgb(alpha, coreColor.R, coreColor.G, coreColor.B)))
                            {
                                g.FillEllipse(br, cx - rr, cy - rr, rr * 2, rr * 2);
                            }
                        }
                        catch { }
                    }
                }

                // Líneas de estrella/rayos que parten del centro
                int starLines = 40;
                using (Pen starPen = new Pen(Color.FromArgb(240, innerColor.R, innerColor.G, innerColor.B), Math.Max(1f, 2f + beatPulse * 3f)))
                {
                    for (int i = 0; i < starLines; i++)
                    {
                        double ang = (i * 2 * Math.PI / starLines) + offset * 0.04;
                        float len = coreBase * 0.6f + avg * radius * 0.45f * pulse + beatPulse * 12f;
                        if (len > 0.5f)
                        {
                            float x = cx + (float)(Math.Cos(ang) * len);
                            float y = cy + (float)(Math.Sin(ang) * len);
                            try { g.DrawLine(starPen, cx, cy, x, y); } catch { }
                        }
                    }
                }
            }
            catch { }

            // === ANILLOS CONCÉNTRICOS ===
            // Cada anillo pulsa hacia afuera según el volumen y el beat
            for (int i = 0; i < rings; i++)
            {
                float t = (i / (float)rings);
                // Radio: base + avance + oscilación sinusoidal modulada por volumen
                float rad = baseRadius + t * (radius * 0.9f) + (float)(Math.Sin((offset + i * 8) * 0.07) * avg * 28 * pulse);

                // Transparencia: los anillos exteriores son más tenues
                int alpha = ColorUtils.ClampAlpha((int)(Math.Max(0, 200 - i * (180 / rings))));
                Color c = ColorUtils.BlendColors(innerColor, outerColor, t);

                if (rad > 0.5f)
                {
                    try
                    {
                        using (Pen p = new Pen(Color.FromArgb(alpha, c.R, c.G, c.B), Math.Max(1f, 3f - t * 2.6f)))
                        {
                            g.DrawEllipse(p, cx - rad, cy - rad, rad * 2, rad * 2);
                        }
                    }
                    catch { }
                }

                // Brillo extra en anillos cada 5 para resaltar el ritmo
                if (i % 5 == 0)
                {
                    int glowAlpha = ColorUtils.ClampAlpha((int)(20 + beatPulse * 90));
                    using (SolidBrush glow = new SolidBrush(Color.FromArgb(glowAlpha, c.R, c.G, c.B)))
                    {
                        if (rad > 0.5f) try { g.FillEllipse(glow, cx - rad - 4, cy - rad - 4, (rad + 4) * 2, (rad + 4) * 2); } catch { }
                    }
                }
            }

            // === ANILLOS LATERALES ===
            // Dos conjuntos de anillos a izquierda y derecha que reaccionan fuertemente al beat
            int sideRings = 18;
            float horizontalOffset = radius * 0.85f;
            int leftCx = (int)(cx - horizontalOffset);
            int rightCx = (int)(cx + horizontalOffset);

            for (int side = 0; side < 2; side++)
            {
                int scx = side == 0 ? leftCx : rightCx;
                // Colores ligeramente distintos para cada lado
                Color sideA = side == 0
                    ? ColorUtils.BlendColors(innerColor, Color.FromArgb(255, 0, 200, 255), 0.3f)
                    : ColorUtils.BlendColors(outerColor, Color.FromArgb(255, 255, 100, 50), 0.3f);
                Color sideB = side == 0
                    ? ColorUtils.BlendColors(outerColor, Color.FromArgb(255, 0, 255, 150), 0.3f)
                    : ColorUtils.BlendColors(innerColor, Color.FromArgb(255, 255, 200, 100), 0.3f);

                for (int i = 0; i < sideRings; i++)
                {
                    float t = (i / (float)sideRings);
                    // Radio más pequeño que los anillos centrales, pero se expande con el beat
                    float rad = baseRadius * 0.6f + t * (radius * 0.45f) + (float)(Math.Sin((offset + i * 6 + side * 10) * 0.08) * avg * 14 * (1f + beatPulse));

                    int alpha = (int)(Math.Max(0, 160 - i * (140 / sideRings)));
                    int beatAlphaBoost = (int)(Math.Min(120, beatPulse * 120));
                    int drawAlpha = ColorUtils.ClampAlpha(Math.Min(255, alpha + beatAlphaBoost));
                    Color c = ColorUtils.BlendColors(sideA, sideB, t);
                    using (Pen p = new Pen(Color.FromArgb(drawAlpha, c.R, c.G, c.B), Math.Max(1f, 2f - t * 1.8f)))
                    {
                        g.DrawEllipse(p, scx - rad, cy - rad, rad * 2, rad * 2);
                    }

                    // Púas radiales que conectan los anillos laterales para marcar el ritmo
                    if (i % 6 == 0)
                    {
                        double ang = (side == 0 ? -Math.PI / 2 : -Math.PI / 2) + offset * 0.01 * (side == 0 ? 1 : -1);
                        float len = rad * 0.3f + beatPulse * 10f;
                        float sx = scx + (float)(Math.Cos(ang) * (rad - len));
                        float sy = cy + (float)(Math.Sin(ang) * (rad - len));
                        float ex = scx + (float)(Math.Cos(ang) * (rad + len));
                        float ey = cy + (float)(Math.Sin(ang) * (rad + len));
                        int spikeAlpha = ColorUtils.ClampAlpha((int)(80 + beatPulse * 120));
                        using (Pen spike = new Pen(Color.FromArgb(spikeAlpha, c.R, c.G, c.B), 1f))
                        {
                            g.DrawLine(spike, sx, sy, ex, ey);
                        }
                    }
                }
            }

            // === CHISPAS RADIALES ===
            // Líneas desde el centro cuya longitud varía con el volumen
            int sparks = 32;
            for (int i = 0; i < sparks; i++)
            {
                double ang = (i * 2 * Math.PI / sparks) + offset * 0.03 * pulse;
                float len = baseRadius * 0.5f + avg * radius * 0.8f * (float)Math.Abs(Math.Cos(i + offset * 0.03)) * pulse;
                float x = cx + (float)(Math.Cos(ang) * len);
                float y = cy + (float)(Math.Sin(ang) * len);
                using (Pen p = new Pen(Color.FromArgb(200, 255, 100, 220), 1 + (float)(Math.Abs(Math.Sin(offset * 0.05)) * 1.8f)))
                {
                    g.DrawLine(p, cx, cy, x, y);
                }
            }
        }
    }
}
