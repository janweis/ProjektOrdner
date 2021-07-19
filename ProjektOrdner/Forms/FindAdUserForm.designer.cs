namespace ProjektOrdner.Forms
{
    partial class FindAdUserForm
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
            this.FilterArtCombo = new System.Windows.Forms.ComboBox();
            this.SuchenButton = new System.Windows.Forms.Button();
            this.UserTreeView = new System.Windows.Forms.TreeView();
            this.FilterBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.AddUserButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // FilterArtCombo
            // 
            this.FilterArtCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FilterArtCombo.FormattingEnabled = true;
            this.FilterArtCombo.Items.AddRange(new object[] {
            "Benutzername",
            "E-Mail Adresse",
            "Matrikelnummer",
            "Domänen-Gruppe"});
            this.FilterArtCombo.Location = new System.Drawing.Point(15, 34);
            this.FilterArtCombo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.FilterArtCombo.Name = "FilterArtCombo";
            this.FilterArtCombo.Size = new System.Drawing.Size(278, 25);
            this.FilterArtCombo.TabIndex = 20;
            this.FilterArtCombo.SelectedIndexChanged += new System.EventHandler(this.FilterArtCombo_SelectedIndexChanged);
            // 
            // SuchenButton
            // 
            this.SuchenButton.Location = new System.Drawing.Point(14, 416);
            this.SuchenButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.SuchenButton.Name = "SuchenButton";
            this.SuchenButton.Size = new System.Drawing.Size(279, 39);
            this.SuchenButton.TabIndex = 19;
            this.SuchenButton.Text = "Suchen";
            this.SuchenButton.UseVisualStyleBackColor = true;
            this.SuchenButton.Click += new System.EventHandler(this.SuchenButton_Click);
            // 
            // UserTreeView
            // 
            this.UserTreeView.AllowDrop = true;
            this.UserTreeView.Location = new System.Drawing.Point(15, 162);
            this.UserTreeView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.UserTreeView.Name = "UserTreeView";
            this.UserTreeView.Size = new System.Drawing.Size(278, 246);
            this.UserTreeView.TabIndex = 18;
            // 
            // FilterBox
            // 
            this.FilterBox.Location = new System.Drawing.Point(15, 94);
            this.FilterBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.FilterBox.Name = "FilterBox";
            this.FilterBox.Size = new System.Drawing.Size(278, 25);
            this.FilterBox.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 17);
            this.label1.TabIndex = 16;
            this.label1.Text = "Suche";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 17);
            this.label2.TabIndex = 23;
            this.label2.Text = "Suchfilter";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 141);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 17);
            this.label3.TabIndex = 24;
            this.label3.Text = "Ergebnis";
            // 
            // AddUserButton
            // 
            this.AddUserButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.AddUserButton.Location = new System.Drawing.Point(15, 471);
            this.AddUserButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.AddUserButton.Name = "AddUserButton";
            this.AddUserButton.Size = new System.Drawing.Size(278, 39);
            this.AddUserButton.TabIndex = 25;
            this.AddUserButton.Text = "Suchergebnis übernehmen";
            this.AddUserButton.UseVisualStyleBackColor = true;
            this.AddUserButton.Click += new System.EventHandler(this.AddUserButton_Click);
            // 
            // FindAdUserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 523);
            this.Controls.Add(this.AddUserButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.FilterArtCombo);
            this.Controls.Add(this.SuchenButton);
            this.Controls.Add(this.UserTreeView);
            this.Controls.Add(this.FilterBox);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FindAdUserForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Active Directory Suche";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox FilterArtCombo;
        private System.Windows.Forms.Button SuchenButton;
        private System.Windows.Forms.TreeView UserTreeView;
        private System.Windows.Forms.TextBox FilterBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button AddUserButton;
    }
}