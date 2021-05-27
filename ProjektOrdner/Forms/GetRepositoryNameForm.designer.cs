namespace ProjektOrdner.Forms
{
    partial class GetRepositoryNameForm
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
            this.label4 = new System.Windows.Forms.Label();
            this.NameTextbox = new System.Windows.Forms.TextBox();
            this.EndDate = new System.Windows.Forms.DateTimePicker();
            this.AnlegenButton = new System.Windows.Forms.Button();
            this.AbbrechenButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SetupPermissionsBox = new System.Windows.Forms.CheckBox();
            this.DateInfinityCheck = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name des Projekts";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(179, 17);
            this.label4.TabIndex = 3;
            this.label4.Text = "Voraussichtliches Projektende";
            // 
            // NameTextbox
            // 
            this.NameTextbox.Location = new System.Drawing.Point(15, 31);
            this.NameTextbox.Name = "NameTextbox";
            this.NameTextbox.Size = new System.Drawing.Size(253, 25);
            this.NameTextbox.TabIndex = 4;
            // 
            // EndDate
            // 
            this.EndDate.Location = new System.Drawing.Point(15, 95);
            this.EndDate.Name = "EndDate";
            this.EndDate.Size = new System.Drawing.Size(253, 25);
            this.EndDate.TabIndex = 6;
            // 
            // AnlegenButton
            // 
            this.AnlegenButton.Location = new System.Drawing.Point(15, 237);
            this.AnlegenButton.Name = "AnlegenButton";
            this.AnlegenButton.Size = new System.Drawing.Size(157, 35);
            this.AnlegenButton.TabIndex = 7;
            this.AnlegenButton.Text = "Projekt anlegen";
            this.AnlegenButton.UseVisualStyleBackColor = true;
            this.AnlegenButton.Click += new System.EventHandler(this.AnlegenButton_Click);
            // 
            // AbbrechenButton
            // 
            this.AbbrechenButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.AbbrechenButton.Location = new System.Drawing.Point(179, 237);
            this.AbbrechenButton.Name = "AbbrechenButton";
            this.AbbrechenButton.Size = new System.Drawing.Size(157, 35);
            this.AbbrechenButton.TabIndex = 8;
            this.AbbrechenButton.Text = "Abbrechen";
            this.AbbrechenButton.UseVisualStyleBackColor = true;
            this.AbbrechenButton.Click += new System.EventHandler(this.AbbrechenButton_Click);
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(15, 220);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(321, 2);
            this.label3.TabIndex = 9;
            // 
            // SetupPermissionsBox
            // 
            this.SetupPermissionsBox.Checked = true;
            this.SetupPermissionsBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SetupPermissionsBox.Location = new System.Drawing.Point(15, 176);
            this.SetupPermissionsBox.Name = "SetupPermissionsBox";
            this.SetupPermissionsBox.Size = new System.Drawing.Size(321, 41);
            this.SetupPermissionsBox.TabIndex = 10;
            this.SetupPermissionsBox.Text = "Berechtigungsassistent nach der Projektanlage starten";
            this.SetupPermissionsBox.UseVisualStyleBackColor = true;
            this.SetupPermissionsBox.CheckedChanged += new System.EventHandler(this.SetupPermissionsBox_CheckedChanged);
            // 
            // DateInfinityCheck
            // 
            this.DateInfinityCheck.AutoSize = true;
            this.DateInfinityCheck.Location = new System.Drawing.Point(15, 126);
            this.DateInfinityCheck.Name = "DateInfinityCheck";
            this.DateInfinityCheck.Size = new System.Drawing.Size(145, 21);
            this.DateInfinityCheck.TabIndex = 11;
            this.DateInfinityCheck.Text = "∞ Kein Ablaufdatum";
            this.DateInfinityCheck.UseVisualStyleBackColor = true;
            this.DateInfinityCheck.CheckedChanged += new System.EventHandler(this.DateInfinityCheck_CheckedChanged);
            // 
            // GetRepositoryNameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 288);
            this.Controls.Add(this.DateInfinityCheck);
            this.Controls.Add(this.SetupPermissionsBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.AbbrechenButton);
            this.Controls.Add(this.AnlegenButton);
            this.Controls.Add(this.EndDate);
            this.Controls.Add(this.NameTextbox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "GetRepositoryNameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Projekt anlegen";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox NameTextbox;
        private System.Windows.Forms.DateTimePicker EndDate;
        private System.Windows.Forms.Button AnlegenButton;
        private System.Windows.Forms.Button AbbrechenButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox SetupPermissionsBox;
        private System.Windows.Forms.CheckBox DateInfinityCheck;
    }
}