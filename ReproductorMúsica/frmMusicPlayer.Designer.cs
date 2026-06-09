namespace ReproductorMúsica
{
    partial class frmMusicPlayer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMusicPlayer));
            this.pgProgress = new System.Windows.Forms.ProgressBar();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblTimer = new System.Windows.Forms.Label();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.trbVolume = new System.Windows.Forms.TrackBar();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnReplay = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnBackward = new System.Windows.Forms.Button();
            this.btnForward = new System.Windows.Forms.Button();
            this.btnPlayPause = new System.Windows.Forms.Button();
            this.picCanvas = new System.Windows.Forms.PictureBox();
            this.btnVolume = new System.Windows.Forms.Button();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.tlpBottom = new System.Windows.Forms.TableLayoutPanel();
            this.flpControls = new System.Windows.Forms.FlowLayoutPanel();
            this.flpRight = new System.Windows.Forms.FlowLayoutPanel();
            this.flpStyle = new System.Windows.Forms.FlowLayoutPanel();
            this.btnStyleBars = new System.Windows.Forms.Button();
            this.btnStyleCircles = new System.Windows.Forms.Button();
            this.btnStylePolygons = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trbVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCanvas)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.tlpBottom.SuspendLayout();
            this.flpControls.SuspendLayout();
            this.flpRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // pgProgress
            // 
            this.tlpBottom.SetColumnSpan(this.pgProgress, 5);
            this.pgProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgProgress.Location = new System.Drawing.Point(11, 14);
            this.pgProgress.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.pgProgress.Name = "pgProgress";
            this.pgProgress.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.pgProgress.Size = new System.Drawing.Size(1004, 8);
            this.pgProgress.Step = 1;
            this.pgProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pgProgress.TabIndex = 1;
            // 
            // txtFileName
            // 
            this.txtFileName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.txtFileName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFileName.Enabled = false;
            this.txtFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFileName.ForeColor = System.Drawing.Color.White;
            this.txtFileName.Location = new System.Drawing.Point(11, 31);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(448, 21);
            this.txtFileName.TabIndex = 8;
            this.txtFileName.TextChanged += new System.EventHandler(this.txtFileName_TextChanged);
            // 
            // lblTimer
            // 
            this.lblTimer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTimer.AutoSize = true;
            this.lblTimer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimer.Location = new System.Drawing.Point(980, 10);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(38, 15);
            this.lblTimer.TabIndex = 9;
            this.lblTimer.Text = "00:00";
            // 
            // trbVolume
            // 
            this.trbVolume.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.trbVolume.Location = new System.Drawing.Point(766, 51);
            this.trbVolume.Margin = new System.Windows.Forms.Padding(2);
            this.trbVolume.Name = "trbVolume";
            this.trbVolume.Size = new System.Drawing.Size(122, 45);
            this.trbVolume.TabIndex = 10;
            this.trbVolume.Scroll += new System.EventHandler(this.trbVolume_Scroll);
            // 
            // btnUpload
            // 
            this.btnUpload.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnUpload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.btnUpload.FlatAppearance.BorderSize = 0;
            this.btnUpload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpload.Image = ((System.Drawing.Image)(resources.GetObject("btnUpload.Image")));
            this.btnUpload.Location = new System.Drawing.Point(58, 6);
            this.btnUpload.Margin = new System.Windows.Forms.Padding(6);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(40, 40);
            this.btnUpload.TabIndex = 7;
            this.btnUpload.UseVisualStyleBackColor = false;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnReplay
            // 
            this.btnReplay.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnReplay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.btnReplay.FlatAppearance.BorderSize = 0;
            this.btnReplay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReplay.Image = global::ReproductorMúsica.Properties.Resources.replay;
            this.btnReplay.Location = new System.Drawing.Point(238, 18);
            this.btnReplay.Margin = new System.Windows.Forms.Padding(6);
            this.btnReplay.Name = "btnReplay";
            this.btnReplay.Size = new System.Drawing.Size(40, 40);
            this.btnReplay.TabIndex = 6;
            this.btnReplay.UseVisualStyleBackColor = false;
            this.btnReplay.Click += new System.EventHandler(this.btnReplay_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Image = global::ReproductorMúsica.Properties.Resources.stop;
            this.btnStop.Location = new System.Drawing.Point(6, 18);
            this.btnStop.Margin = new System.Windows.Forms.Padding(6);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(40, 40);
            this.btnStop.TabIndex = 5;
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnBackward
            // 
            this.btnBackward.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnBackward.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.btnBackward.FlatAppearance.BorderSize = 0;
            this.btnBackward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackward.Image = global::ReproductorMúsica.Properties.Resources.backward;
            this.btnBackward.Location = new System.Drawing.Point(58, 18);
            this.btnBackward.Margin = new System.Windows.Forms.Padding(6);
            this.btnBackward.Name = "btnBackward";
            this.btnBackward.Size = new System.Drawing.Size(40, 40);
            this.btnBackward.TabIndex = 4;
            this.btnBackward.UseVisualStyleBackColor = false;
            this.btnBackward.Click += new System.EventHandler(this.btnBackward_Click);
            // 
            // btnForward
            // 
            this.btnForward.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnForward.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.btnForward.FlatAppearance.BorderSize = 0;
            this.btnForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnForward.Image = global::ReproductorMúsica.Properties.Resources.forward;
            this.btnForward.Location = new System.Drawing.Point(186, 18);
            this.btnForward.Margin = new System.Windows.Forms.Padding(6);
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(40, 40);
            this.btnForward.TabIndex = 3;
            this.btnForward.UseVisualStyleBackColor = false;
            this.btnForward.Click += new System.EventHandler(this.btnForward_Click);
            // 
            // btnPlayPause
            // 
            this.btnPlayPause.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPlayPause.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.btnPlayPause.FlatAppearance.BorderSize = 0;
            this.btnPlayPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlayPause.Image = global::ReproductorMúsica.Properties.Resources.pause_play;
            this.btnPlayPause.Location = new System.Drawing.Point(110, 6);
            this.btnPlayPause.Margin = new System.Windows.Forms.Padding(6);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(64, 64);
            this.btnPlayPause.TabIndex = 2;
            this.btnPlayPause.UseVisualStyleBackColor = false;
            this.btnPlayPause.Click += new System.EventHandler(this.btnPlayPause_Click);
            // 
            // picCanvas
            // 
            this.picCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picCanvas.Location = new System.Drawing.Point(0, 0);
            this.picCanvas.Name = "picCanvas";
            this.picCanvas.Size = new System.Drawing.Size(1028, 479);
            this.picCanvas.TabIndex = 0;
            this.picCanvas.TabStop = false;
            // 
            // btnVolume
            // 
            this.btnVolume.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnVolume.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.btnVolume.FlatAppearance.BorderSize = 0;
            this.btnVolume.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolume.Image = global::ReproductorMúsica.Properties.Resources.volumen;
            this.btnVolume.Location = new System.Drawing.Point(6, 6);
            this.btnVolume.Margin = new System.Windows.Forms.Padding(6);
            this.btnVolume.Name = "btnVolume";
            this.btnVolume.Size = new System.Drawing.Size(40, 40);
            this.btnVolume.TabIndex = 11;
            this.btnVolume.UseVisualStyleBackColor = false;
            this.btnVolume.Click += new System.EventHandler(this.btnVolume_Click);
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.pnlBottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlBottom.Controls.Add(this.tlpBottom);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 479);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(1028, 130);
            this.pnlBottom.TabIndex = 1;
            // 
            // tlpBottom
            // 
            this.tlpBottom.ColumnCount = 5;
            this.tlpBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tlpBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tlpBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tlpBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlpBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            // Wrap txtFileName + flpStyle in a panel (vertical stack)
            this.pnlFileRow = new System.Windows.Forms.Panel();
            this.pnlFileRow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFileName.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtFileName.Height = 22;
            this.flpStyle.Dock = System.Windows.Forms.DockStyle.Fill;
            // Add flpStyle FIRST so txtFileName (added second) takes the top
            this.pnlFileRow.Controls.Add(this.flpStyle);
            this.pnlFileRow.Controls.Add(this.txtFileName);

            this.tlpBottom.Controls.Add(this.pgProgress, 0, 0);
            this.tlpBottom.Controls.Add(this.pnlFileRow, 0, 1);
            this.tlpBottom.Controls.Add(this.flpControls, 1, 1);
            this.tlpBottom.Controls.Add(this.trbVolume, 3, 1);
            this.tlpBottom.Controls.Add(this.flpRight, 4, 1);
            this.tlpBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpBottom.Location = new System.Drawing.Point(0, 0);
            this.tlpBottom.Name = "tlpBottom";
            this.tlpBottom.Padding = new System.Windows.Forms.Padding(8);
            this.tlpBottom.RowCount = 2;
            this.tlpBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpBottom.Size = new System.Drawing.Size(1026, 128);
            this.tlpBottom.TabIndex = 0;
            // 
            // flpControls
            // 
            this.flpControls.BackColor = System.Drawing.Color.Transparent;
            this.tlpBottom.SetColumnSpan(this.flpControls, 2);
            this.flpControls.Controls.Add(this.btnStop);
            this.flpControls.Controls.Add(this.btnBackward);
            this.flpControls.Controls.Add(this.btnPlayPause);
            this.flpControls.Controls.Add(this.btnForward);
            this.flpControls.Controls.Add(this.btnReplay);
            this.flpControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpControls.Location = new System.Drawing.Point(465, 31);
            this.flpControls.Name = "flpControls";
            this.flpControls.Size = new System.Drawing.Size(296, 86);
            this.flpControls.TabIndex = 9;
            this.flpControls.WrapContents = false;
            // 
            // flpStyle
            // 
            this.flpStyle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.flpStyle.BackColor = System.Drawing.Color.Transparent;
            this.flpStyle.Controls.Add(this.btnStyleBars);
            this.flpStyle.Controls.Add(this.btnStyleCircles);
            this.flpStyle.Controls.Add(this.btnStylePolygons);
            this.flpStyle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpStyle.Location = new System.Drawing.Point(3, 35);
            this.flpStyle.Name = "flpStyle";
            this.flpStyle.Size = new System.Drawing.Size(142, 80);
            this.flpStyle.TabIndex = 12;
            this.flpStyle.WrapContents = false;
            // 
            // btnStyleBars
            // 
            this.btnStyleBars.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnStyleBars.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.btnStyleBars.FlatAppearance.BorderSize = 0;
            this.btnStyleBars.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStyleBars.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btnStyleBars.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(136)))));
            this.btnStyleBars.Location = new System.Drawing.Point(4, 20);
            this.btnStyleBars.Margin = new System.Windows.Forms.Padding(4);
            this.btnStyleBars.Name = "btnStyleBars";
            this.btnStyleBars.Size = new System.Drawing.Size(40, 40);
            this.btnStyleBars.TabIndex = 12;
            this.btnStyleBars.Text = "≡";
            this.btnStyleBars.UseVisualStyleBackColor = false;
            this.btnStyleBars.Click += new System.EventHandler(this.btnStyleBars_Click);
            // 
            // btnStyleCircles
            // 
            this.btnStyleCircles.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnStyleCircles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.btnStyleCircles.FlatAppearance.BorderSize = 0;
            this.btnStyleCircles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStyleCircles.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnStyleCircles.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.btnStyleCircles.Location = new System.Drawing.Point(52, 20);
            this.btnStyleCircles.Margin = new System.Windows.Forms.Padding(4);
            this.btnStyleCircles.Name = "btnStyleCircles";
            this.btnStyleCircles.Size = new System.Drawing.Size(40, 40);
            this.btnStyleCircles.TabIndex = 13;
            this.btnStyleCircles.Text = "◎";
            this.btnStyleCircles.UseVisualStyleBackColor = false;
            this.btnStyleCircles.Click += new System.EventHandler(this.btnStyleCircles_Click);
            // 
            // btnStylePolygons
            // 
            this.btnStylePolygons.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnStylePolygons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.btnStylePolygons.FlatAppearance.BorderSize = 0;
            this.btnStylePolygons.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStylePolygons.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnStylePolygons.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.btnStylePolygons.Location = new System.Drawing.Point(100, 20);
            this.btnStylePolygons.Margin = new System.Windows.Forms.Padding(4);
            this.btnStylePolygons.Name = "btnStylePolygons";
            this.btnStylePolygons.Size = new System.Drawing.Size(40, 40);
            this.btnStylePolygons.TabIndex = 14;
            this.btnStylePolygons.Text = "⬡";
            this.btnStylePolygons.UseVisualStyleBackColor = false;
            this.btnStylePolygons.Click += new System.EventHandler(this.btnStylePolygons_Click);
            // 
            // flpRight
            // 
            this.flpRight.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.flpRight.BackColor = System.Drawing.Color.Transparent;
            this.flpRight.Controls.Add(this.btnVolume);
            this.flpRight.Controls.Add(this.btnUpload);
            this.flpRight.Location = new System.Drawing.Point(893, 31);
            this.flpRight.Name = "flpRight";
            this.flpRight.Size = new System.Drawing.Size(122, 86);
            this.flpRight.TabIndex = 11;
            this.flpRight.WrapContents = false;
            // 
            // frmMusicPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(23)))));
            this.ClientSize = new System.Drawing.Size(1028, 609);
            this.Controls.Add(this.picCanvas);
            this.Controls.Add(this.pnlBottom);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMusicPlayer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Windows Media Player";
            this.Load += new System.EventHandler(this.frmMusicPlayer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trbVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCanvas)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.tlpBottom.ResumeLayout(false);
            this.tlpBottom.PerformLayout();
            this.flpControls.ResumeLayout(false);
            this.flpRight.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picCanvas;
        private System.Windows.Forms.ProgressBar pgProgress;
        private System.Windows.Forms.Button btnPlayPause;
        private System.Windows.Forms.Button btnForward;
        private System.Windows.Forms.Button btnBackward;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnReplay;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.TrackBar trbVolume;
        private System.Windows.Forms.Button btnVolume;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.TableLayoutPanel tlpBottom;
        private System.Windows.Forms.FlowLayoutPanel flpControls;
        private System.Windows.Forms.FlowLayoutPanel flpRight;
        private System.Windows.Forms.FlowLayoutPanel flpStyle;
        private System.Windows.Forms.Button btnStyleBars;
        private System.Windows.Forms.Button btnStyleCircles;
        private System.Windows.Forms.Button btnStylePolygons;
        private System.Windows.Forms.Panel pnlFileRow;
    }
}
