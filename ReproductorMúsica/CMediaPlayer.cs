using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WMPLib;

namespace ReproductorMúsica
{
    // Coordinador principal del reproductor multimedia y las visualizaciones
    // Envuelve el control COM de Windows Media Player y orquesta todas las
    // clases de visualización (SpectrumData, BeatDetector, ParticleEngine,
    // BarVisualization, CircleVisualization, PolygonVisualization)
    internal class CMediaPlayer
    {
        // Referencia al reproductor multimedia de Windows
        private WindowsMediaPlayer player;
        // Temporizador que dispara el bucle de animación (25 FPS ≈ 40ms)
        private Timer animationTimer;
        // Contador de animación que se incrementa en cada frame (para movimiento continuo)
        private int offset = 0;
        // Estilo de visualización activo: 0 = Barras, 1 = Círculos, 2 = Polígonos
        private int style = 0;
        // Líneas dibujadas para la animación de polígonos (aumenta progresivamente)
        private int linesToDraw = 0;
        private const int totalLines = 36;
        // PictureBox donde se renderizan las visualizaciones
        private PictureBox canvas;
        // Analizador de audio FFT (opcional, captura del micrófono)
        private AudioAnalyzer analyzer;

        // Componentes separados de la visualización
        private SpectrumData spectrumData;
        private BeatDetector beatDetector;
        private ParticleEngine particleEngine;
        private BarVisualization barVis;
        private CircleVisualization circleVis;
        private PolygonVisualization polygonVis;

        // Colores del tema actual (neon por defecto)
        private Color themeBarStart = Color.FromArgb(255, 128, 255, 0);
        private Color themeBarEnd = Color.FromArgb(255, 0, 200, 80);
        private Color themeCircleInner = Color.Magenta;
        private Color themeCircleOuter = Color.Cyan;

        // Evita repetir el mismo estilo aleatorio en canciones consecutivas
        private int previousStyle = -1;

        public CMediaPlayer(WindowsMediaPlayer mediaPlayer, Timer timer, PictureBox canvas, AudioAnalyzer analyzer = null)
        {
            this.player = mediaPlayer;
            this.animationTimer = timer;
            this.canvas = canvas;
            this.analyzer = analyzer;

            // Crear los componentes especializados de visualización
            spectrumData = new SpectrumData(analyzer);
            beatDetector = new BeatDetector();
            particleEngine = new ParticleEngine();
            barVis = new BarVisualization();
            circleVis = new CircleVisualization();
            polygonVis = new PolygonVisualization();

            if (this.animationTimer != null)
                this.animationTimer.Tick += Timer_Tick;
        }

        // Devuelve el nombre del estilo actual para mostrarlo en pantalla
        public string GetCurrentStyleName()
        {
            switch (style)
            {
                case 0: return "Barras";
                case 1: return "Círculos";
                case 2: return "Polígonos";
                default: return "Desconocido";
            }
        }

        // Cambia la paleta de colores del tema (0 = neón, 1 = azules fríos, 2 = naranjas cálidos)
        public void SetTheme(int themeIndex)
        {
            switch (themeIndex)
            {
                case 0:
                    themeBarStart = Color.FromArgb(255, 128, 255, 0);
                    themeBarEnd = Color.FromArgb(255, 0, 200, 80);
                    themeCircleInner = Color.Magenta;
                    themeCircleOuter = Color.Cyan;
                    break;
                case 1:
                    themeBarStart = Color.FromArgb(255, 100, 200, 255);
                    themeBarEnd = Color.FromArgb(255, 0, 120, 220);
                    themeCircleInner = Color.FromArgb(255, 120, 180, 255);
                    themeCircleOuter = Color.FromArgb(255, 0, 120, 200);
                    break;
                case 2:
                    themeBarStart = Color.FromArgb(255, 255, 160, 80);
                    themeBarEnd = Color.FromArgb(255, 200, 60, 20);
                    themeCircleInner = Color.FromArgb(255, 255, 120, 180);
                    themeCircleOuter = Color.FromArgb(255, 200, 80, 40);
                    break;
                default:
                    SetTheme(0);
                    break;
            }
        }

        // Carga una canción y prepara la visualización
        // - path: ruta del archivo MP3
        // - styleIndex: -1 para aleatorio, 0-2 para estilo específico
        public void LoadTrack(string path, int styleIndex)
        {
            try
            {
                player.URL = path;
            }
            catch { }

            // Elegir estilo aleatorio evitando repetir el anterior
            if (styleIndex < 0 || styleIndex > 2)
            {
                int newStyle = previousStyle;
                int attempts = 0;
                while (newStyle == previousStyle && attempts < 10)
                {
                    newStyle = new Random().Next(0, 3);
                    attempts++;
                }
                style = newStyle;
                previousStyle = style;
            }
            else
            {
                style = styleIndex;
                previousStyle = styleIndex;
            }

            SetStyle(style);

            // Reiniciar contadores de animación
            offset = 0;
            linesToDraw = 0;

            beatDetector.Reset();
            particleEngine.Clear();
        }

        // Cambia el estilo de visualización actual (0 = Barras, 1 = Círculos, 2 = Polígonos)
        public void SetStyle(int styleIndex)
        {
            style = styleIndex;
        }

        // Inicia la reproducción y el bucle de animación
        public void Play()
        {
            try { player.controls.play(); } catch { }
            try { animationTimer?.Start(); } catch { }
            try { analyzer?.Start(); } catch { }
        }

        // Pausa la reproducción y la animación
        public void Pause()
        {
            try { player.controls.pause(); } catch { }
            try { animationTimer?.Stop(); } catch { }
            try { analyzer?.Stop(); } catch { }
        }

        // Detiene la reproducción y reinicia todo el estado
        public void Stop()
        {
            try { player.controls.stop(); } catch { }
            try { animationTimer?.Stop(); } catch { }
            try
            {
                canvas.Image?.Dispose();
            }
            catch { }
            canvas.Image = null;
            offset = 0;
            linesToDraw = 0;

            spectrumData.Reset();
            beatDetector.Reset();

            try { analyzer?.Stop(); } catch { }
            particleEngine.Clear();
        }

        // Bucle principal de animación (se ejecuta cada ~40ms = 25 FPS)
        // Este método coordina todo el pipeline de visualización:
        // 1. Actualizar datos del espectro
        // 2. Detectar beats
        // 3. Generar partículas en los golpes
        // 4. Renderizar el estilo visual seleccionado
        // 5. Dibujar partículas y overlay
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (canvas == null) return;
            if (canvas.Width == 0 || canvas.Height == 0) return;

            // Obtener la posición actual de la canción para sincronizar la simulación
            double position = 0.0;
            try { position = player.controls.currentPosition; } catch { }

            // Paso 1: actualizar el espectro (FFT real o simulación)
            spectrumData.Update(position, offset);

            // Paso 2: analizar el espectro para detectar beats
            beatDetector.Analyze(spectrumData.SpectrumSmooth, offset);

            // Paso 3: si hay un beat y el cooldown lo permite, generar partículas
            if (beatDetector.Delta > 0.06f && beatDetector.CanSpawn)
            {
                int cx = canvas.Width / 2;
                int cy = canvas.Height / 2;
                particleEngine.SpawnParticles(
                    cx + beatDetector.CenterOffsetX,
                    cy + beatDetector.CenterOffsetY,
                    Math.Min(1.0f, beatDetector.Delta * 8f + beatDetector.LowEnergy),
                    themeCircleInner, themeCircleOuter
                );
                beatDetector.ResetCooldown();
            }

            // Crear un bitmap para el frame actual
            Bitmap bmp = new Bitmap(canvas.Width, canvas.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Black);

                int centerX = canvas.Width / 2;
                int centerY = canvas.Height / 2;
                int radius = Math.Min(canvas.Width, canvas.Height) / 2 - 10;

                // Paso 4: renderizar según el estilo seleccionado
                switch (style)
                {
                    case 0:
                        barVis.Draw(g, canvas.Width, canvas.Height, spectrumData.SpectrumSmooth,
                            beatDetector.BeatPulse, beatDetector.LowEnergy, themeBarStart, themeBarEnd);
                        break;

                    case 1:
                        circleVis.Draw(g, offset, spectrumData.SpectrumSmooth,
                            beatDetector.BeatPulse, beatDetector.LowEnergy,
                            centerX + beatDetector.CenterOffsetX, centerY + beatDetector.CenterOffsetY,
                            radius, themeCircleInner, themeCircleOuter);
                        break;

                    case 2:
                        if (linesToDraw < totalLines)
                            linesToDraw++;
                        polygonVis.Draw(g, offset, linesToDraw, centerX, centerY, radius,
                            spectrumData.SpectrumSmooth, beatDetector.BeatPulse);
                        break;

                    default:
                        barVis.Draw(g, canvas.Width, canvas.Height, spectrumData.SpectrumSmooth,
                            beatDetector.BeatPulse, beatDetector.LowEnergy, themeBarStart, themeBarEnd);
                        break;
                }

                // Paso 5: dibujar partículas y overlay de depuración
                particleEngine.UpdateAndDrawParticles(g);
                DrawDebugOverlay(g);
            }

            // Asignar el bitmap renderizado al PictureBox
            try { canvas.Image?.Dispose(); } catch { }
            canvas.Image = bmp;

            // Avanzar contadores de animación
            offset += 3;
            beatDetector.Decay();
        }

        // Muestra información de depuración en la esquina superior izquierda
        // Indica el estilo actual y si se usa analizador real o simulación
        private void DrawDebugOverlay(Graphics g)
        {
            try
            {
                string status = GetCurrentStyleName() + (analyzer != null && analyzer.IsAvailable ? " (Analyzer)" : " (Simulado)");
                using (Font f = new Font("Segoe UI", 12, FontStyle.Bold))
                using (SolidBrush br = new SolidBrush(Color.FromArgb(220, 255, 255, 255)))
                {
                    g.DrawString(status, f, br, 10, 10);
                }
            }
            catch { }
        }

        // === Métodos de control del reproductor ===
        // Estos métodos envuelven las funciones básicas de Windows Media Player

        public void SetVolume(int vol)
        {
            try { player.settings.volume = Math.Max(0, Math.Min(100, vol)); } catch { }
        }

        public void Seek(double seconds)
        {
            try { player.controls.currentPosition = seconds; } catch { }
        }

        public double GetDuration()
        {
            try
            {
                var m = player.currentMedia;
                if (m != null) return m.duration;
            }
            catch { }
            return double.NaN;
        }

        public double GetPosition()
        {
            try { return player.controls.currentPosition; } catch { return 0; }
        }

        public void SetLoop(bool loop)
        {
            try { player.settings.setMode("loop", loop); } catch { }
        }

        public void Forward(double seconds)
        {
            try { Seek(GetPosition() + seconds); } catch { }
        }

        public void Backward(double seconds)
        {
            try { Seek(Math.Max(0.0, GetPosition() - seconds)); } catch { }
        }
    }
}
