namespace ProjektOrdner.Forms
{
    partial class EditRootPathsForm
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
            this.RootPathsTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.DurchsuchenButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.AbbrechenButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RootPathsTextBox
            // 
            this.RootPathsTextBox.Location = new System.Drawing.Point(17, 39);
            this.RootPathsTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.RootPathsTextBox.Multiline = true;
            this.RootPathsTextBox.Name = "RootPathsTextBox";
            this.RootPathsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.RootPathsTextBox.Size = new System.Drawing.Size(443, 63);
            this.RootPathsTextBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(322, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Geben Sie die Pfade getrennt durch ein Semikolon an:";
            // 
            // DurchsuchenButton
            // 
            this.DurchsuchenButton.Location = new System.Drawing.Point(468, 39);
            this.DurchsuchenButton.Margin = new System.Windows.Forms.Padding(4);
            this.DurchsuchenButton.Name = "DurchsuchenButton";
            this.DurchsuchenButton.Size = new System.Drawing.Size(99, 64);
            this.DurchsuchenButton.TabIndex = 2;
            this.DurchsuchenButton.Text = "Durchsuchen";
            this.DurchsuchenButton.UseVisualStyleBackColor = true;
            this.DurchsuchenButton.Click += new System.EventHandler(this.DurchsuchenButton_Click);
            // 
            // OkButton
            // 
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Location = new System.Drawing.Point(17, 126);
            this.OkButton.Margin = new System.Windows.Forms.Padding(4);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(196, 30);
            this.OkButton.TabIndex = 3;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // AbbrechenButton
            // 
            this.AbbrechenButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.AbbrechenButton.Location = new System.Drawing.Point(375, 126);
            this.AbbrechenButton.Margin = new System.Windows.Forms.Padding(4);
            this.AbbrechenButton.Name = "AbbrechenButton";
            this.AbbrechenButton.Size = new System.Drawing.Size(192, 30);
            this.AbbrechenButton.TabIndex = 4;
            this.AbbrechenButton.Text = "Abbrechen";
            this.AbbrechenButton.UseVisualStyleBackColor = true;
            this.AbbrechenButton.Click += new System.EventHandler(this.AbbrechenButton_Click);
            // 
            // RootPathManageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 168);
            this.Controls.Add(this.AbbrechenButton);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.DurchsuchenButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RootPathsTextBox);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "RootPathManageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rootpfade verwalten";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox RootPathsTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button DurchsuchenButton;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Button AbbrechenButton;
    }
}