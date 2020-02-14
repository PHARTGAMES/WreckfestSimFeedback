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
            this.playerComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ScanButton
            // 
            this.ScanButton.Location = new System.Drawing.Point(295, 338);
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
            this.StatusLabel.Location = new System.Drawing.Point(321, 147);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(149, 25);
            this.StatusLabel.TabIndex = 3;
            this.StatusLabel.Text = "Click Initialize!";
            this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // playerComboBox
            // 
            this.playerComboBox.FormattingEnabled = true;
            this.playerComboBox.Items.AddRange(new object[] {
            "00",
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23"});
            this.playerComboBox.Location = new System.Drawing.Point(99, 354);
            this.playerComboBox.Name = "playerComboBox";
            this.playerComboBox.Size = new System.Drawing.Size(121, 33);
            this.playerComboBox.TabIndex = 4;
            this.playerComboBox.Text = "00";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(89, 326);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 25);
            this.label1.TabIndex = 5;
            this.label1.Text = "Choose player";
            // 
            // ScanDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.playerComboBox);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.ExecutableText);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.ScanButton);
            this.Name = "ScanDialog";
            this.Text = "Wreckfest Telemetry";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button ScanButton;
        public System.Windows.Forms.ProgressBar progressBar1;
        public System.Windows.Forms.TextBox ExecutableText;
        public System.Windows.Forms.Label StatusLabel;
        public System.Windows.Forms.ComboBox playerComboBox;
        private System.Windows.Forms.Label label1;
    }
}