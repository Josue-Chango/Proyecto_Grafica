using System;
using System.Drawing;

namespace ReproductorMúsica
{
    // Clase de utilidades para operaciones con colores
    // Se usa en los renderizados de visualización y en el sistema de partículas
    internal static class ColorUtils
    {
        // Limita un valor alpha (0-255) para evitar errores en GDI+
        public static int ClampAlpha(int a)
        {
            if (a < 0) return 0;
            if (a > 255) return 255;
            return a;
        }

        // Interpola entre dos colores usando el factor t (0 = color a, 1 = color b)
        // Se usa para degradados y mezcla de colores en círculos y partículas
        public static Color BlendColors(Color a, Color b, float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            return Color.FromArgb(
                (int)(a.A + (b.A - a.A) * t),
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t));
        }

        // Convierte un ángulo HSL (hue en grados 0-360) a Color RGB
        // Saturación y luminosidad fijas (90% y 55%) para colores vibrantes
        // Se usa en la visualización de polígonos para el ciclo de colores
        public static Color ColorFromHue(int hue)
        {
            float h = (hue % 360) / 360f;
            float s = 0.9f;
            float l = 0.55f;

            float r = 0, g = 0, b = 0;
            if (s == 0)
            {
                r = g = b = l;
            }
            else
            {
                Func<float, float, float, float> hue2rgb = (pa, pb, tc) =>
                {
                    if (tc < 0) tc += 1;
                    if (tc > 1) tc -= 1;
                    if (tc < 1f / 6f) return pa + (pb - pa) * 6f * tc;
                    if (tc < 1f / 2f) return pb;
                    if (tc < 2f / 3f) return pa + (pb - pa) * (2f / 3f - tc) * 6f;
                    return pa;
                };

                float qq = l < 0.5f ? l * (1 + s) : l + s - l * s;
                float p2 = 2 * l - qq;
                r = hue2rgb(p2, qq, h + 1f / 3f);
                g = hue2rgb(p2, qq, h);
                b = hue2rgb(p2, qq, h - 1f / 3f);
            }

            return Color.FromArgb(255, (int)(r * 255), (int)(g * 255), (int)(b * 255));
        }
    }
}
