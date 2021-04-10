namespace ProjektOrdner.Forms
{
    partial class ExpandProjektExpirationForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.VerlängernButton = new System.Windows.Forms.Button();
            this.NameLabel = new System.Windows.Forms.Label();
            this.AblaufdatumLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ablaufdatum:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Projektname:";
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.FirstDayOfWeek = System.Windows.Forms.Day.Monday;
            this.monthCalendar1.Location = new System.Drawing.Point(12, 103);
            this.monthCalendar1.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.monthCalendar1.MaxSelectionCount = 1;
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.ShowWeekNumbers = true;
            this.monthCalendar1.TabIndex = 2;
            this.monthCalendar1.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateChanged);
            this.monthCalendar1.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateSelected);
            // 
            // VerlängernButton
            // 
            this.VerlängernButton.Location = new System.Drawing.Point(12, 281);
            this.VerlängernButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.VerlängernButton.Name = "VerlängernButton";
            this.VerlängernButton.Size = new System.Drawing.Size(243, 30);
            this.VerlängernButton.TabIndex = 3;
            this.VerlängernButton.Text = "Verlängern";
            this.VerlängernButton.UseVisualStyleBackColor = true;
            this.VerlängernButton.Click += new System.EventHandler(this.VerlängernButton_Click);
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(113, 13);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(43, 17);
            this.NameLabel.TabIndex = 5;
            this.NameLabel.Text = "Name";
            // 
            // AblaufdatumLabel
            // 
            this.AblaufdatumLabel.AutoSize = true;
            this.AblaufdatumLabel.Location = new System.Drawing.Point(113, 36);
            this.AblaufdatumLabel.Name = "AblaufdatumLabel";
            this.AblaufdatumLabel.Size = new System.Drawing.Size(82, 17);
            this.AblaufdatumLabel.TabIndex = 6;
            this.AblaufdatumLabel.Text = "Ablaufdatum";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Verlängern auf:";
            // 
            // ExpandProjektExpirationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(267, 325);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.AblaufdatumLabel);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.VerlängernButton);
            this.Controls.Add(this.monthCalendar1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ExpandProjektExpirationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Projekt verlängern";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.Button VerlängernButton;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.Label AblaufdatumLabel;
        private System.Windows.Forms.Label label3;
    }
}