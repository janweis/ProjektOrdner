namespace ProjektOrdner.Forms
{
    partial class RootFolderAssistentForm
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
            this.OkButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.EditRootRadio = new System.Windows.Forms.RadioButton();
            this.CreateRootRadio = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.PfadTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.DurchsuchenButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // OkButton
            // 
            this.OkButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Enabled = false;
            this.OkButton.Location = new System.Drawing.Point(265, 254);
            this.OkButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(180, 34);
            this.OkButton.TabIndex = 0;
            this.OkButton.Text = "Weiter";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(23, 254);
            this.CancelButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(180, 34);
            this.CancelButton.TabIndex = 1;
            this.CancelButton.Text = "Abbrechen";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Was möchten Sie tun?";
            // 
            // EditRootRadio
            // 
            this.EditRootRadio.AutoSize = true;
            this.EditRootRadio.Location = new System.Drawing.Point(23, 69);
            this.EditRootRadio.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.EditRootRadio.Name = "EditRootRadio";
            this.EditRootRadio.Size = new System.Drawing.Size(302, 21);
            this.EditRootRadio.TabIndex = 3;
            this.EditRootRadio.TabStop = true;
            this.EditRootRadio.Text = "Einen vorhandenes Stammverzeichnis eintragen";
            this.EditRootRadio.UseVisualStyleBackColor = true;
            this.EditRootRadio.CheckedChanged += new System.EventHandler(this.EditRootRadio_CheckedChanged);
            // 
            // CreateRootRadio
            // 
            this.CreateRootRadio.AutoSize = true;
            this.CreateRootRadio.Location = new System.Drawing.Point(23, 99);
            this.CreateRootRadio.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CreateRootRadio.Name = "CreateRootRadio";
            this.CreateRootRadio.Size = new System.Drawing.Size(312, 21);
            this.CreateRootRadio.TabIndex = 4;
            this.CreateRootRadio.TabStop = true;
            this.CreateRootRadio.Text = "Ein neues Stammverzeichnis für Projekte erstellen";
            this.CreateRootRadio.UseVisualStyleBackColor = true;
            this.CreateRootRadio.CheckedChanged += new System.EventHandler(this.CreateRootRadio_CheckedChanged);
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(23, 241);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(423, 3);
            this.label2.TabIndex = 5;
            // 
            // PfadTextBox
            // 
            this.PfadTextBox.Enabled = false;
            this.PfadTextBox.Location = new System.Drawing.Point(41, 153);
            this.PfadTextBox.Name = "PfadTextBox";
            this.PfadTextBox.Size = new System.Drawing.Size(404, 25);
            this.PfadTextBox.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(38, 133);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Pfad";
            // 
            // DurchsuchenButton
            // 
            this.DurchsuchenButton.Enabled = false;
            this.DurchsuchenButton.Location = new System.Drawing.Point(41, 184);
            this.DurchsuchenButton.Name = "DurchsuchenButton";
            this.DurchsuchenButton.Size = new System.Drawing.Size(145, 26);
            this.DurchsuchenButton.TabIndex = 8;
            this.DurchsuchenButton.Text = "Durchsuchen";
            this.DurchsuchenButton.UseVisualStyleBackColor = true;
            this.DurchsuchenButton.Click += new System.EventHandler(this.DurchsuchenButton_Click);
            // 
            // RootFolderAssistentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 301);
            this.Controls.Add(this.DurchsuchenButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.PfadTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CreateRootRadio);
            this.Controls.Add(this.EditRootRadio);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OkButton);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "RootFolderAssistentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stammverzeichnis Assistent";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton EditRootRadio;
        private System.Windows.Forms.RadioButton CreateRootRadio;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PfadTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button DurchsuchenButton;
    }
}