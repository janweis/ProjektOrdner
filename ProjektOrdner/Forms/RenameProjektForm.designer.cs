namespace ProjektOrdner.Forms
{
    partial class RenameProjektForm
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
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.AbbrechenButton = new System.Windows.Forms.Button();
            this.RenameProjektButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.NameLbl = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 63);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(245, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Geben Sie einen neuen Namen ein";
            // 
            // NameTextBox
            // 
            this.NameTextBox.Location = new System.Drawing.Point(10, 83);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.Size = new System.Drawing.Size(309, 25);
            this.NameTextBox.TabIndex = 1;
            // 
            // AbbrechenButton
            // 
            this.AbbrechenButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.AbbrechenButton.Location = new System.Drawing.Point(208, 145);
            this.AbbrechenButton.Name = "AbbrechenButton";
            this.AbbrechenButton.Size = new System.Drawing.Size(111, 30);
            this.AbbrechenButton.TabIndex = 2;
            this.AbbrechenButton.Text = "Abbrechen";
            this.AbbrechenButton.UseVisualStyleBackColor = true;
            this.AbbrechenButton.Click += new System.EventHandler(this.AbbrechenButton_Click);
            // 
            // RenameProjektButton
            // 
            this.RenameProjektButton.Location = new System.Drawing.Point(10, 145);
            this.RenameProjektButton.Name = "RenameProjektButton";
            this.RenameProjektButton.Size = new System.Drawing.Size(175, 30);
            this.RenameProjektButton.TabIndex = 3;
            this.RenameProjektButton.Text = "Projekt Umbenennen";
            this.RenameProjektButton.UseVisualStyleBackColor = true;
            this.RenameProjektButton.Click += new System.EventHandler(this.RenameProjektButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Name:";
            // 
            // NameLbl
            // 
            this.NameLbl.AutoSize = true;
            this.NameLbl.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameLbl.Location = new System.Drawing.Point(7, 28);
            this.NameLbl.Name = "NameLbl";
            this.NameLbl.Size = new System.Drawing.Size(122, 17);
            this.NameLbl.TabIndex = 5;
            this.NameLbl.Text = "HierStehtDerName";
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(10, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(309, 3);
            this.label3.TabIndex = 6;
            // 
            // RenameProjektForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 190);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.NameLbl);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.RenameProjektButton);
            this.Controls.Add(this.AbbrechenButton);
            this.Controls.Add(this.NameTextBox);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "RenameProjektForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Projekt Umbenennen";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.Button AbbrechenButton;
        private System.Windows.Forms.Button RenameProjektButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label NameLbl;
        private System.Windows.Forms.Label label3;
    }
}