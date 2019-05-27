namespace Spotitoast
{
    partial class Form1
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
            this.trackLabel = new System.Windows.Forms.Label();
            this.loveButton = new System.Windows.Forms.Button();
            this.unloveButton = new System.Windows.Forms.Button();
            this.playPauseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // trackLabel
            // 
            this.trackLabel.AutoSize = true;
            this.trackLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackLabel.Location = new System.Drawing.Point(287, 196);
            this.trackLabel.Name = "trackLabel";
            this.trackLabel.Size = new System.Drawing.Size(171, 73);
            this.trackLabel.TabIndex = 0;
            this.trackLabel.Text = "track";
            // 
            // loveButton
            // 
            this.loveButton.Location = new System.Drawing.Point(109, 332);
            this.loveButton.Name = "loveButton";
            this.loveButton.Size = new System.Drawing.Size(75, 23);
            this.loveButton.TabIndex = 1;
            this.loveButton.Text = "💖";
            this.loveButton.UseVisualStyleBackColor = true;
            this.loveButton.Click += new System.EventHandler(this.LoveButton_Click);
            // 
            // unloveButton
            // 
            this.unloveButton.Location = new System.Drawing.Point(544, 332);
            this.unloveButton.Name = "unloveButton";
            this.unloveButton.Size = new System.Drawing.Size(75, 23);
            this.unloveButton.TabIndex = 2;
            this.unloveButton.Text = "💔";
            this.unloveButton.UseVisualStyleBackColor = true;
            this.unloveButton.Click += new System.EventHandler(this.UnloveButton_Click);
            // 
            // playPauseButton
            // 
            this.playPauseButton.Location = new System.Drawing.Point(316, 332);
            this.playPauseButton.Name = "playPauseButton";
            this.playPauseButton.Size = new System.Drawing.Size(75, 23);
            this.playPauseButton.TabIndex = 3;
            this.playPauseButton.Text = "⏯️";
            this.playPauseButton.UseVisualStyleBackColor = true;
            this.playPauseButton.Click += new System.EventHandler(this.PlayPauseButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.playPauseButton);
            this.Controls.Add(this.unloveButton);
            this.Controls.Add(this.loveButton);
            this.Controls.Add(this.trackLabel);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label trackLabel;
        private System.Windows.Forms.Button loveButton;
        private System.Windows.Forms.Button unloveButton;
        private System.Windows.Forms.Button playPauseButton;
    }
}

