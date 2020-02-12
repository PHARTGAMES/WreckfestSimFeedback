namespace WFSTTelemetry
{
    partial class ScanDialog
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
            this.ScanButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.ExecutableText = new System.Windows.Forms.TextBox();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ScanButton
            // 
            this.ScanButton.Location = new System.Drawing.Point(302, 329);
            this.ScanButton.Name = "ScanButton";
            this.ScanButton.Size = new System.Drawing.Size(186, 62);
            this.ScanButton.TabIndex = 0;
            this.ScanButton.Text = "Initialize";
            this.ScanButton.UseVisualStyleBackColor = true;
            this.ScanButton.Click += new System.EventHandler(this.ScanButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(94, 224);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(603, 52);
            this.progressBar1.TabIndex = 1;
            // 
            // ExecutableText
            // 
            this.ExecutableText.Location = new System.Drawing.Point(197, 66);
            this.ExecutableText.Name = "ExecutableText";
            this.ExecutableText.ReadOnly = true;
            this.ExecutableText.Size = new System.Drawing.Size(408, 31);
            this.ExecutableText.TabIndex = 2;
            this.ExecutableText.Text = "WAITING";
            this.ExecutableText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Location = new System.Drawing.Point(332, 147);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(133, 25);
            this.StatusLabel.TabIndex = 3;
            this.StatusLabel.Text = "Click Initialize!";
            this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.ExecutableText);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.ScanButton);
            this.Name = "Form1";
            this.Text = "Wreckfest Telemetry";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button ScanButton;
        public System.Windows.Forms.ProgressBar progressBar1;
        public System.Windows.Forms.TextBox ExecutableText;
        public System.Windows.Forms.Label StatusLabel;
    }
}