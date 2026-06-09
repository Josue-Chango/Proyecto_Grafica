using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using WMPLib; 

namespace ReproductorMúsica
{
    public partial class frmMusicPlayer : Form
    {
        // Use strongly-typed Windows Media Player COM object
        private WindowsMediaPlayer player = null;
        private CMediaPlayer mediaHelper = null;
        private bool isPlaying = false;
        private bool isLooping = false;

        // new field to store selected visualization style
        private int visualStyle = 0; // 0=barras,1=circulos,2=poligonos
        private ToolTip canvasToolTip;

        // new field: audio analyzer for real FFT spectrum
        private AudioAnalyzer analyzer = null;

        // Use a fixed scale for the progress bar so we update by percentage (smoother and independent of duration)
        private const int ProgressScale = 1000;

        public frmMusicPlayer()
        {
            InitializeComponent();

            // Aplicar tema oscuro a la interfaz
            this.BackColor = Color.FromArgb(28, 28, 28);
            this.ForeColor = Color.White;
            lblTimer.ForeColor = Color.White;
            txtFileName.BackColor = Color.FromArgb(45, 45, 45);
            txtFileName.ForeColor = Color.White;
            trbVolume.BackColor = Color.FromArgb(45, 45, 45);
            foreach (Button btn in new Button[] { btnPlayPause, btnForward, btnBackward, btnStop, btnReplay, btnUpload, btnVolume, btnStyleBars, btnStyleCircles, btnStylePolygons })
            {
                btn.BackColor = Color.FromArgb(60, 60, 60);
                btn.ForeColor = Color.White;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
            }

            // Generar iconos por c�digo GDI+ (sin archivos PNG externos)
            GenerateButtonIcons();

            // NOTE: click events are wired in the Designer; avoid wiring them here to prevent double-invocation
            // NOTE: click events for upload/playpause are wired in the Designer.
            // Wire additional UI events here.
            var pb = GetProgressBar();
            if (pb != null)
            {
                // allow seeking by clicking
                pb.MouseDown += progressBar_MouseDown;
                // smoother visual updates
                try { pb.Style = ProgressBarStyle.Continuous; } catch { }
                try { pb.Minimum = 0; pb.Value = 0; pb.Maximum = ProgressScale; pb.Enabled = true; } catch { }
            }

            // Initialize volume control defaults (designer declares trbVolume)
            try
            {
                if (this.trbVolume != null)
                {
                    this.trbVolume.Minimum = 0;
                    this.trbVolume.Maximum = 100;
                    if (this.trbVolume.Value < this.trbVolume.Minimum || this.trbVolume.Value > this.trbVolume.Maximum)
                        this.trbVolume.Value = 50;
                }
            }
            catch { }

            this.btnForward.Click += btnForward_Click;
            this.btnBackward.Click += btnBackward_Click;
            this.btnStop.Click += btnStop_Click;
            this.btnReplay.Click += btnReplay_Click;

            // Configure timer to update progress
            this.timer1.Interval = 250; // faster updates
            this.timer1.Tick += timer1_Tick;

            // Ensure animation timer has a reasonable default
            try { this.timer2.Interval = 40; } catch { }

            // Setup tooltip for canvas and hook mouse click to change visualization
            try
            {
                canvasToolTip = new ToolTip();
                canvasToolTip.IsBalloon = false;
                canvasToolTip.ShowAlways = true;

                if (this.picCanvas != null)
                {
                    this.picCanvas.MouseClick += PicCanvas_MouseClick;
                }
            }
            catch { }
        }

        private void PicCanvas_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    // cycle visualization style
                    visualStyle = (visualStyle + 1) % 3;

                    // apply to helper if present
                    if (mediaHelper != null)
                    {
                        mediaHelper.SetStyle(visualStyle);
                    }

                    // show quick tip
                    string name = GetStyleName(visualStyle);
                    var p = picCanvas.PointToScreen(new Point(e.X, e.Y));
                    canvasToolTip.Show("Visual: " + name, this.picCanvas, e.X + 10, e.Y + 10, 1000);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    // optional: left click can toggle pause/play
                    try
                    {
                        btnPlayPause.PerformClick();
                    }
                    catch { }
                }
            }
            catch { }
        }

        private string GetStyleName(int s)
        {
            switch (s)
            {
                case 0: return "Barras";
                case 1: return "Círculos";
                case 2: return "Polígonos";
                default: return "Desconocido";
            }
        }

        private ProgressBar GetProgressBar()
        {
            // use designer-named progress bar directly
            return this.pgProgress;
        }

        private void frmMusicPlayer_Load(object sender, EventArgs e)
        {
            HighlightStyleButton(btnStyleBars);
        }

        private void EnsurePlayerAndHelper()
        {
            if (player == null)
            {
                try
                {
                    player = new WindowsMediaPlayer();
                    player.PlayStateChange += Player_PlayStateChange;
                }
                catch (Exception)
                {
                    MessageBox.Show("Windows Media Player no está disponible en este equipo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (mediaHelper == null && player != null)
            {
                // create helper using player, animation timer and canvas
                try
                {
                    // create analyzer if available
                    try
                    {
                        if (analyzer == null)
                        {
                            analyzer = new AudioAnalyzer(2048); // FFT size 2048
                        }
                    }
                    catch { analyzer = null; }

                    mediaHelper = new CMediaPlayer(player, this.timer2, this.picCanvas, analyzer);
                    // apply current visual style
                    mediaHelper.SetStyle(visualStyle);
                }
                catch { }
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "MP3 files (*.mp3)|*.mp3";
                ofd.Multiselect = false;
                ofd.Title = "Seleccionar archivo MP3";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string file = ofd.FileName;

                    try
                    {
                        // Show filename in textbox
                        this.txtFileName.Text = Path.GetFileName(file);

                        // initialize label and progress immediately
                        try { lblTimer.Text = "00:00 / 00:00"; } catch { }
                        var pbInit = GetProgressBar();
                        if (pbInit != null) { try { pbInit.Minimum = 0; pbInit.Value = 0; pbInit.Maximum = ProgressScale; pbInit.Enabled = true; } catch { } }

                        // Create Windows Media Player instance and helper if needed
                        EnsurePlayerAndHelper();

                        // Reset looping to current setting
                        try { if (player != null) player.settings.setMode("loop", isLooping); } catch { }

                        // Apply volume from UI if available
                        try
                        {
                            if (this.trbVolume != null && mediaHelper != null)
                            {
                                int vol = Math.Max(0, Math.Min(100, this.trbVolume.Value));
                                try { mediaHelper.SetVolume(vol); } catch { }
                            }
                        }
                        catch { }

                        // Load track into helper and play
                        if (mediaHelper != null)
                        {
                            // use selected visual style when loading
                            mediaHelper.LoadTrack(file, -1);
                            // Reaplicar el estilo seleccionado por el usuario
                            mediaHelper.SetStyle(visualStyle);
                            mediaHelper.Play();
                            isPlaying = true;
                        }
                        else if (player != null)
                        {
                            // fallback to direct player
                            player.URL = file;
                            player.controls.play();
                            isPlaying = true;
                        }

                        // Start timer (play state event will also manage it)
                        try { this.Invoke((Action)(() => this.timer1.Start())); } catch { try { this.timer1.Start(); } catch { } }

                        // Try to initialize progress bar (duration may not be available yet)
                        TryInitializeProgressBar();

                        // Poll for duration in background and initialize UI as soon as it's available
                        WaitForMediaReady();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("No se pudo reproducir el archivo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            try
            {
                if (player == null && mediaHelper == null)
                    return; // no file loaded

                if (isPlaying)
                {
                    if (mediaHelper != null)
                        mediaHelper.Pause();
                    else
                        player?.controls.pause();

                    isPlaying = false;
                    try { this.Invoke((Action)(() => this.timer1.Stop())); } catch { try { this.timer1.Stop(); } catch { } }
                }
                else
                {
                    if (mediaHelper != null)
                        mediaHelper.Play();
                    else
                        player?.controls.play();

                    isPlaying = true;
                    try { this.Invoke((Action)(() => this.timer1.Start())); } catch { try { this.timer1.Start(); } catch { } }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al reproducir/pausar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Timer tick: update progress bar and time label
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                UpdateProgressUI();
            }
            catch
            {
                // swallow exceptions to avoid timer crash
            }
        }

        private double GetMediaDuration()
        {
            try
            {
                if (player == null) return double.NaN;

                // Try currentMedia
                try
                {
                    var m = player.currentMedia;
                    if (m != null)
                    {
                        double d = m.duration;
                        if (!double.IsNaN(d) && d > 0) return d;
                    }
                }
                catch { }

                // Try controls.currentItem
                try
                {
                    var ci = player.controls.currentItem;
                    if (ci != null)
                    {
                        double d2 = ci.duration;
                        if (!double.IsNaN(d2) && d2 > 0) return d2;
                    }
                }
                catch { }

                // As fallback, try to read the duration from currentMedia again
                try
                {
                    var m2 = player.currentMedia;
                    if (m2 != null) return m2.duration;
                }
                catch { }

                return double.NaN;
            }
            catch { return double.NaN; }
        }

        private double GetCurrentPosition()
        {
            try
            {
                if (player == null) return 0;
                try { return player.controls.currentPosition; } catch { return 0; }
            }
            catch { return 0; }
        }

        private void UpdateProgressUI()
        {
            if (player == null)
                return;

            double duration = GetMediaDuration();
            double position = GetCurrentPosition();

            var pb = GetProgressBar();

            // If duration not yet valid, set basic UI and exit
            if (double.IsNaN(duration) || duration <= 0)
            {
                if (pb != null)
                {
                    try { pb.Value = 0; } catch { }
                    try { pb.Maximum = ProgressScale; } catch { }
                }
                try { lblTimer.Text = "00:00 / 00:00"; } catch { }
                return;
            }

            if (pb != null)
            {
                // Use percentage of duration on a fixed scale for smoother and consistent updates
                int val = 0;
                try
                {
                    double ratio = position / duration;
                    ratio = Math.Max(0.0, Math.Min(1.0, ratio));
                    val = (int)Math.Round(ratio * ProgressScale);
                    if (val < 0) val = 0;
                    if (val > ProgressScale) val = ProgressScale;
                    // ensure enabled so value shows
                    try { if (!pb.Enabled) pb.Enabled = true; } catch { }
                    pb.Value = val;
                    pb.Refresh();
                    pb.Invalidate();
                    pb.Update();
                }
                catch { }
            }

            try { lblTimer.Text = FormatTime(position) + " / " + FormatTime(duration); } catch { }
        }

        private string FormatTime(double seconds)
        {
            if (double.IsNaN(seconds) || seconds <= 0)
                return "00:00";

            int s = (int)Math.Floor(seconds);
            int minutes = s / 60;
            int secs = s % 60;
            return string.Format("{0:D2}:{1:D2}", minutes, secs);
        }

        // Poll until the media object reports a valid duration, then initialize UI
        private async void WaitForMediaReady()
        {
            if (player == null)
                return;

            for (int i = 0; i < 50; i++) // try for ~10 seconds
            {
                try
                {
                    double d = GetMediaDuration();
                    if (!double.IsNaN(d) && d > 0)
                    {
                        // initialize on UI thread
                        try { this.Invoke((Action)TryInitializeProgressBar); } catch { TryInitializeProgressBar(); }
                        try { this.Invoke((Action)UpdateProgressUI); } catch { UpdateProgressUI(); }
                        return;
                    }
                }
                catch
                {
                    // ignore COM errors while media is loading
                }

                await Task.Delay(200);
            }

            // fallback: if not available, set basic label
            try { lblTimer.Text = "00:00 / 00:00"; } catch { }
        }

        // Seek when user clicks on progress bar
        private void progressBar_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (player == null)
                    return;

                double duration = GetMediaDuration();
                if (double.IsNaN(duration) || duration <= 0)
                    return;

                var pb = sender as ProgressBar ?? GetProgressBar();
                if (pb == null)
                    return;

                double ratio = e.X / (double)Math.Max(1, pb.Width);
                ratio = Math.Max(0.0, Math.Min(1.0, ratio));
                double newPos = ratio * duration;

                if (mediaHelper != null)
                    mediaHelper.Seek(newPos);
                else
                    player.controls.currentPosition = newPos;

                // Update UI immediately using the same scaled value
                try
                {
                    int val = (int)Math.Round(ratio * ProgressScale);
                    if (val < 0) val = 0;
                    if (val > ProgressScale) val = ProgressScale;
                    pb.Value = val;
                    pb.Refresh();
                }
                catch { }

                lblTimer.Text = FormatTime(newPos) + " / " + FormatTime(duration);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mover la barra de progreso: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TryInitializeProgressBar()
        {
            try
            {
                double duration = GetMediaDuration();
                if (double.IsNaN(duration) || duration <= 0)
                    return;

                var pb = GetProgressBar();
                if (pb == null)
                    return;

                try { pb.Minimum = 0; } catch { }
                try { pb.Maximum = ProgressScale; } catch { }
                try { pb.Value = 0; } catch { }
                try { pb.Enabled = true; } catch { }
                try { lblTimer.Text = "00:00 / " + FormatTime(duration); } catch { }
            }
            catch
            {
            }
        }

        private void Player_PlayStateChange(int NewState)
        {
            try
            {
                var state = (WMPPlayState)NewState;
                var pb = GetProgressBar();
                if (state == WMPPlayState.wmppsPlaying)
                {
                    // When playback actually starts, duration should be available; try initializing
                    try { this.Invoke((Action)TryInitializeProgressBar); } catch { TryInitializeProgressBar(); }
                    try { this.Invoke((Action)(() => this.timer1.Start())); } catch { try { this.timer1.Start(); } catch { } }
                    try { this.Invoke((Action)UpdateProgressUI); } catch { UpdateProgressUI(); }
                    isPlaying = true;
                }
                else if (state == WMPPlayState.wmppsPaused)
                {
                    try { this.Invoke((Action)(() => this.timer1.Stop())); } catch { try { this.timer1.Stop(); } catch { } }
                    isPlaying = false;
                }
                else if (state == WMPPlayState.wmppsStopped || state == WMPPlayState.wmppsMediaEnded)
                {
                    try { this.Invoke((Action)(() => this.timer1.Stop())); } catch { try { this.timer1.Stop(); } catch { } }
                    isPlaying = false;

                    // when media ends, reset progress to end (or 0 depending on desired behavior)
                    try
                    {
                        if (player != null && !double.IsNaN(GetMediaDuration()) && pb != null)
                        {
                            double duration = GetMediaDuration();
                            if (duration > 0)
                            {
                                try { pb.Value = ProgressScale; } catch { }
                                try { lblTimer.Text = FormatTime(duration) + " / " + FormatTime(duration); } catch { }
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        private void pgProgress_Click(object sender, EventArgs e)
        {

        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            try
            {
                if (player == null || double.IsNaN(GetMediaDuration()))
                    return;

                if (mediaHelper != null)
                {
                    mediaHelper.Forward(10.0);
                }
                else
                {
                    double duration = GetMediaDuration();
                    double pos = GetCurrentPosition();
                    double newPos = Math.Min(duration, pos + 10.0); // jump forward 10 seconds
                    player.controls.currentPosition = newPos;
                }

                try { UpdateProgressUI(); } catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al avanzar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBackward_Click(object sender, EventArgs e)
        {
            try
            {
                if (player == null)
                    return;

                if (mediaHelper != null)
                {
                    mediaHelper.Backward(10.0);
                }
                else
                {
                    double pos = GetCurrentPosition();
                    double newPos = Math.Max(0.0, pos - 10.0); // jump back 10 seconds
                    player.controls.currentPosition = newPos;
                }

                try { UpdateProgressUI(); } catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al retroceder: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                // Stop playback and clear loaded file so user can upload another
                try
                {
                    if (mediaHelper != null)
                    {
                        mediaHelper.Stop();
                        mediaHelper = null;
                    }
                    else if (player != null)
                    {
                        try { player.controls.stop(); } catch { }
                        try { player.close(); } catch { }
                    }
                }
                catch { }

                isPlaying = false;

                try { this.Invoke((Action)(() => this.timer1.Stop())); } catch { try { this.timer1.Stop(); } catch { } }

                // Clear UI and state
                try
                {
                    // clear loaded file info
                    this.txtFileName.Text = string.Empty;

                    // reset progress bar
                    var pb = GetProgressBar();
                    if (pb != null)
                    {
                        try { pb.Value = 0; } catch { }
                        try { pb.Enabled = false; } catch { }
                    }

                    // reset timer label
                    try { lblTimer.Text = "00:00"; } catch { }

                    // release player instance so a new file can be loaded cleanly
                    try
                    {
                        if (player != null)
                        {
                            try { player.PlayStateChange -= Player_PlayStateChange; } catch { }
                            try { player = null; } catch { }
                        }
                    }
                    catch { }

                    // dispose analyzer
                    try { if (analyzer != null) { analyzer.Stop(); analyzer.Dispose(); analyzer = null; } } catch { }
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al detener y limpiar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReplay_Click(object sender, EventArgs e)
        {
            try
            {
                // Toggle looping mode
                isLooping = !isLooping;
                if (player != null)
                {
                    try { player.settings.setMode("loop", isLooping); } catch { }
                }

                if (mediaHelper != null)
                    mediaHelper.SetLoop(isLooping);

                // Provide immediate feedback: if enabled and media loaded, restart playback from beginning
                if (isLooping)
                {
                    // ensure the mode is set; if media loaded restart
                    if ((mediaHelper != null || player != null) && !double.IsNaN(GetMediaDuration()))
                    {
                        try
                        {
                            if (mediaHelper != null) mediaHelper.Seek(0);
                            else if (player != null) player.controls.currentPosition = 0;
                        }
                        catch { }

                        try { if (mediaHelper != null) mediaHelper.Play(); else if (player != null) player.controls.play(); isPlaying = true; } catch { }
                        try { this.Invoke((Action)(() => this.timer1.Start())); } catch { try { this.timer1.Start(); } catch { } }
                    }
                }
                else
                {
                    // looping disabled; no further action required
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al alternar replay: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void trbVolume_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (player == null && mediaHelper == null)
                    return;

                // Set the player volume based on the track bar value
                try
                {
                    int vol = Math.Max(0, Math.Min(100, this.trbVolume.Value));
                    if (mediaHelper != null) mediaHelper.SetVolume(vol);
                    else player.settings.volume = vol;
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al ajustar el volumen: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnVolume_Click(object sender, EventArgs e)
        {
            try
            {
                // Set volume to 0 (mute)
                try
                {
                    if (mediaHelper != null)
                    {
                        mediaHelper.SetVolume(0);
                    }
                    else if (player != null)
                    {
                        player.settings.volume = 0;
                    }
                }
                catch { }

                // Update trackbar UI if present
                try { if (this.trbVolume != null) this.trbVolume.Value = 0; } catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al silenciar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // Botones de selecci�n de estilo visual
        // ============================================================
        private void btnStyleBars_Click(object sender, EventArgs e)
        {
            visualStyle = 0;
            if (mediaHelper != null) mediaHelper.SetStyle(0);
            HighlightStyleButton(btnStyleBars);
        }

        private void btnStyleCircles_Click(object sender, EventArgs e)
        {
            visualStyle = 1;
            if (mediaHelper != null) mediaHelper.SetStyle(1);
            HighlightStyleButton(btnStyleCircles);
        }

        private void btnStylePolygons_Click(object sender, EventArgs e)
        {
            visualStyle = 2;
            if (mediaHelper != null) mediaHelper.SetStyle(2);
            HighlightStyleButton(btnStylePolygons);
        }

        private void HighlightStyleButton(Button active)
        {
            foreach (Button btn in new Button[] { btnStyleBars, btnStyleCircles, btnStylePolygons })
            {
                btn.BackColor = btn == active ? Color.FromArgb(90, 90, 100) : Color.FromArgb(60, 60, 60);
            }
        }

        // ============================================================
        // Generar iconos de botones con GDI+ (sin archivos PNG)
        // ============================================================
        private void GenerateButtonIcons()
        {
            try { btnStop.Image = MakeStopIcon(40, 40); } catch { }
            try { btnBackward.Image = MakeBackwardIcon(40, 40); } catch { }
            try { btnForward.Image = MakeForwardIcon(40, 40); } catch { }
            try { btnPlayPause.Image = MakePlayPauseIcon(64, 64); } catch { }
            try { btnReplay.Image = MakeReplayIcon(40, 40); } catch { }
            try { btnVolume.Image = MakeVolumeIcon(40, 40); } catch { }
            try { btnUpload.Image = MakeUploadIcon(40, 40); } catch { }
        }

        private Bitmap MakeStopIcon(int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                int s = Math.Min(w, h) / 3;
                int cx = w / 2, cy = h / 2;
                using (SolidBrush br = new SolidBrush(Color.FromArgb(255, 80, 80)))
                    g.FillRectangle(br, cx - s / 2, cy - s / 2, s, s);
            }
            return bmp;
        }

        private Bitmap MakePlayPauseIcon(int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                int cx = w / 2, cy = h / 2;
                int r = Math.Min(w, h) / 3;
                PointF[] tri = {
                    new PointF(cx - r * 0.6f, cy - r),
                    new PointF(cx - r * 0.6f, cy + r),
                    new PointF(cx + r * 0.8f, cy)
                };
                using (SolidBrush br = new SolidBrush(Color.FromArgb(100, 200, 255)))
                    g.FillPolygon(br, tri);
            }
            return bmp;
        }

        private Bitmap MakeBackwardIcon(int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                int cy = h / 2, s = Math.Min(w, h) / 4;
                // Triangles pointing LEFT (◀◀)
                PointF[] tri = {
                    new PointF(w / 2 - s * 0.7f, cy),
                    new PointF(w / 2 + s * 0.5f, cy - s),
                    new PointF(w / 2 + s * 0.5f, cy + s)
                };
                PointF[] tri2 = {
                    new PointF(w / 2 - s * 1.7f, cy),
                    new PointF(w / 2 - s * 0.5f, cy - s),
                    new PointF(w / 2 - s * 0.5f, cy + s)
                };
                using (SolidBrush br = new SolidBrush(Color.FromArgb(255, 200, 80)))
                {
                    g.FillPolygon(br, tri);
                    g.FillPolygon(br, tri2);
                }
            }
            return bmp;
        }

        private Bitmap MakeForwardIcon(int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                int cy = h / 2, s = Math.Min(w, h) / 4;
                // Triangles pointing RIGHT (▶▶)
                PointF[] tri = {
                    new PointF(w / 2 + s * 0.7f, cy),
                    new PointF(w / 2 - s * 0.5f, cy - s),
                    new PointF(w / 2 - s * 0.5f, cy + s)
                };
                PointF[] tri2 = {
                    new PointF(w / 2 + s * 1.7f, cy),
                    new PointF(w / 2 + s * 0.5f, cy - s),
                    new PointF(w / 2 + s * 0.5f, cy + s)
                };
                using (SolidBrush br = new SolidBrush(Color.FromArgb(100, 255, 150)))
                {
                    g.FillPolygon(br, tri);
                    g.FillPolygon(br, tri2);
                }
            }
            return bmp;
        }

        private Bitmap MakeReplayIcon(int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                int cx = w / 2, cy = h / 2, r = Math.Min(w, h) / 3;
                // Arc from top, clockwise 270° → ends on the left
                using (Pen p = new Pen(Color.FromArgb(150, 120, 255), 2))
                {
                    g.DrawArc(p, cx - r, cy - r, r * 2, r * 2, -90, 270);
                }
                // Arrow at the END of the arc (left side, pointing UP to complete the loop)
                PointF[] arrow = {
                    new PointF(cx - r, cy - 6),       // tip up
                    new PointF(cx - r - 5, cy + 2),   // left fin
                    new PointF(cx - r + 5, cy + 2)    // right fin
                };
                using (SolidBrush br = new SolidBrush(Color.FromArgb(150, 120, 255)))
                    g.FillPolygon(br, arrow);
            }
            return bmp;
        }

        private Bitmap MakeVolumeIcon(int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                int cx = w / 2, cy = h / 2;
                PointF[] speaker = {
                    new PointF(cx - 6, cy - 4),
                    new PointF(cx - 2, cy - 4),
                    new PointF(cx + 3, cy - 8),
                    new PointF(cx + 3, cy + 8),
                    new PointF(cx - 2, cy + 4),
                    new PointF(cx - 6, cy + 4)
                };
                using (SolidBrush br = new SolidBrush(Color.FromArgb(200, 200, 200)))
                    g.FillPolygon(br, speaker);
                using (Pen p = new Pen(Color.FromArgb(200, 200, 200), 1.5f))
                {
                    g.DrawArc(p, cx + 1, cy - 6, 6, 12, -60, 120);
                }
            }
            return bmp;
        }

        private Bitmap MakeUploadIcon(int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                int cx = w / 2, cy = h / 2;
                PointF[] arrow = {
                    new PointF(cx, cy - 8),
                    new PointF(cx - 6, cy - 2),
                    new PointF(cx + 6, cy - 2)
                };
                using (SolidBrush br = new SolidBrush(Color.FromArgb(255, 220, 80)))
                    g.FillPolygon(br, arrow);
                using (Pen p = new Pen(Color.FromArgb(255, 220, 80), 2))
                {
                    g.DrawLine(p, cx, cy - 8, cx, cy + 6);
                    g.DrawLine(p, cx - 6, cy + 6, cx + 6, cy + 6);
                }
            }
            return bmp;
        }

        private void txtFileName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
