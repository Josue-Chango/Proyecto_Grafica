using System;
using System.Collections.Generic;
using System.Drawing;

namespace ReproductorMúsica
{
    // Sistema de partículas que se activan en cada golpe rítmico (beat)
    // Las partículas nacen desde el centro y se expanden con física simple
    internal class ParticleEngine
    {
        // Cada partícula tiene posición, velocidad, tamaño, transparencia y color
        private class Particle
        {
            public float X, Y;          // Posición actual
            public float VX, VY;        // Velocidad en cada eje
            public float Radius;         // Radio (crece con el tiempo)
            public int Alpha;            // Transparencia (de 200 a 0)
            public Color Color;          // Color (mezcla entre inner y outer)
        }

        private List<Particle> particles = new List<Particle>();
        private int maxParticles = 220;  // Límite para evitar problemas de rendimiento
        private Random rng = new Random();

        // Genera partículas desde una posición central al detectar un beat
        // - cx, cy: centro de explosión (generalmente el centro del canvas + vibración)
        // - intensity: fuerza del beat (0-1), afecta velocidad y tamaño
        // - innerColor, outerColor: colores del tema para mezclar en cada partícula
        public void SpawnParticles(float cx, float cy, float intensity, Color innerColor, Color outerColor)
        {
            int count = 6 + rng.Next(6);  // Entre 6 y 12 partículas por beat
            for (int i = 0; i < count; i++)
            {
                if (particles.Count >= maxParticles) break;

                // Dirección aleatoria (ángulo) y velocidad según intensidad
                double ang = rng.NextDouble() * Math.PI * 2.0;
                float speed = (float)(2.0 + rng.NextDouble() * 6.0 * intensity);

                Particle p = new Particle();
                // Posición inicial con pequeño desplazamiento aleatorio
                p.X = cx + (float)(Math.Cos(ang) * rng.NextDouble() * 8f);
                p.Y = cy + (float)(Math.Sin(ang) * rng.NextDouble() * 8f);
                p.VX = (float)(Math.Cos(ang) * speed);
                p.VY = (float)(Math.Sin(ang) * speed);
                // Tamaño: base 2-6 píxeles + refuerzo por intensidad
                p.Radius = (float)(2.0 + rng.NextDouble() * 4.0 + intensity * 4.0f);
                p.Alpha = 200;  // Opacidad inicial alta
                // Color aleatorio entre los dos colores del tema
                p.Color = ColorUtils.BlendColors(innerColor, outerColor, (float)rng.NextDouble());
                particles.Add(p);
            }
        }

        // Actualiza y dibuja todas las partículas activas
        // Física: velocidad con fricción (0.98), radio crece (1.02), alpha decrece (8/frame)
        public void UpdateAndDrawParticles(Graphics g)
        {
            if (particles.Count == 0) return;

            // Recorrer en orden inverso para poder eliminar partículas muertas
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                var p = particles[i];

                // Actualizar física
                p.X += p.VX;           // Movimiento
                p.Y += p.VY;
                p.VX *= 0.98f;         // Fricción (desaceleración)
                p.VY *= 0.98f;
                p.Radius *= 1.02f;     // La partícula se expande
                p.Alpha -= 8;          // Se desvanece gradualmente

                // Dibujar la partícula como un círculo relleno
                try
                {
                    int a = ColorUtils.ClampAlpha(p.Alpha);
                    using (SolidBrush br = new SolidBrush(Color.FromArgb(a, p.Color.R, p.Color.G, p.Color.B)))
                    {
                        float rr = Math.Max(1f, p.Radius);
                        g.FillEllipse(br, p.X - rr, p.Y - rr, rr * 2, rr * 2);
                    }
                }
                catch { }

                // Eliminar partículas muertas (transparencia total o demasiado grandes)
                if (p.Alpha <= 0 || p.Radius > 5000)
                {
                    particles.RemoveAt(i);
                }
            }
        }

        // Limpia todas las partículas (al detener o cargar una nueva canción)
        public void Clear()
        {
            particles.Clear();
        }
    }
}
