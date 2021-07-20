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
            this.SucheLabel = new System.Windows.Forms.Label();
            this.SuchKriteriumLabel = new System.Windows.Forms.Label();
            this.AddUserButton = new System.Windows.Forms.Button();
            this.SuchTypLabel = new System.Windows.Forms.Label();
            this.SuchTypCombo = new System.Windows.Forms.ComboBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusStripLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.button1 = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // FilterArtCombo
            // 
            this.FilterArtCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FilterArtCombo.FormattingEnabled = true;
            this.FilterArtCombo.Items.AddRange(new object[] {
            "Name",
            "E-Mail Adresse",
            "Matrikelnummer"});
            this.FilterArtCombo.Location = new System.Drawing.Point(15, 89);
            this.FilterArtCombo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.FilterArtCombo.Name = "FilterArtCombo";
            this.FilterArtCombo.Size = new System.Drawing.Size(241, 25);
            this.FilterArtCombo.TabIndex = 20;
            this.FilterArtCombo.SelectedIndexChanged += new System.EventHandler(this.FilterArtCombo_SelectedIndexChanged);
            // 
            // SuchenButton
            // 
            this.SuchenButton.Location = new System.Drawing.Point(14, 441);
            this.SuchenButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.SuchenButton.Name = "SuchenButton";
            this.SuchenButton.Size = new System.Drawing.Size(279, 31);
            this.SuchenButton.TabIndex = 19;
            this.SuchenButton.Text = "Im Active Directory suchen...";
            this.SuchenButton.UseVisualStyleBackColor = true;
            this.SuchenButton.Click += new System.EventHandler(this.SuchenButton_Click);
            // 
            // UserTreeView
            // 
            this.UserTreeView.AllowDrop = true;
            this.UserTreeView.Location = new System.Drawing.Point(15, 187);
            this.UserTreeView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.UserTreeView.Name = "UserTreeView";
            this.UserTreeView.Size = new System.Drawing.Size(278, 246);
            this.UserTreeView.TabIndex = 18;
            // 
            // FilterBox
            // 
            this.FilterBox.Location = new System.Drawing.Point(15, 154);
            this.FilterBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.FilterBox.Name = "FilterBox";
            this.FilterBox.Size = new System.Drawing.Size(278, 25);
            this.FilterBox.TabIndex = 17;
            // 
            // SucheLabel
            // 
            this.SucheLabel.AutoSize = true;
            this.SucheLabel.Location = new System.Drawing.Point(12, 133);
            this.SucheLabel.Name = "SucheLabel";
            this.SucheLabel.Size = new System.Drawing.Size(80, 17);
            this.SucheLabel.TabIndex = 16;
            this.SucheLabel.Text = "Suchen nach";
            // 
            // SuchKriteriumLabel
            // 
            this.SuchKriteriumLabel.AutoSize = true;
            this.SuchKriteriumLabel.Location = new System.Drawing.Point(12, 68);
            this.SuchKriteriumLabel.Name = "SuchKriteriumLabel";
            this.SuchKriteriumLabel.Size = new System.Drawing.Size(130, 17);
            this.SuchKriteriumLabel.TabIndex = 23;
            this.SuchKriteriumLabel.Text = "Suche über Kriterium";
            // 
            // AddUserButton
            // 
            this.AddUserButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.AddUserButton.Location = new System.Drawing.Point(15, 495);
            this.AddUserButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.AddUserButton.Name = "AddUserButton";
            this.AddUserButton.Size = new System.Drawing.Size(278, 39);
            this.AddUserButton.TabIndex = 25;
            this.AddUserButton.Text = "Ergebnis übernehmen";
            this.AddUserButton.UseVisualStyleBackColor = true;
            this.AddUserButton.Click += new System.EventHandler(this.AddUserButton_Click);
            // 
            // SuchTypLabel
            // 
            this.SuchTypLabel.AutoSize = true;
            this.SuchTypLabel.Location = new System.Drawing.Point(12, 9);
            this.SuchTypLabel.Name = "SuchTypLabel";
            this.SuchTypLabel.Size = new System.Drawing.Size(28, 17);
            this.SuchTypLabel.TabIndex = 26;
            this.SuchTypLabel.Text = "Typ";
            // 
            // SuchTypCombo
            // 
            this.SuchTypCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SuchTypCombo.FormattingEnabled = true;
            this.SuchTypCombo.Items.AddRange(new object[] {
            "Benutzer",
            "Gruppe",
            "Teams (mehrere Gruppen)"});
            this.SuchTypCombo.Location = new System.Drawing.Point(15, 29);
            this.SuchTypCombo.Name = "SuchTypCombo";
            this.SuchTypCombo.Size = new System.Drawing.Size(241, 25);
            this.SuchTypCombo.TabIndex = 27;
            this.SuchTypCombo.SelectedIndexChanged += new System.EventHandler(this.SuchTypCombo_SelectedIndexChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusStripLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 559);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(316, 22);
            this.statusStrip1.TabIndex = 28;
            this.statusStrip1.Text = "StatusStrip";
            // 
            // StatusStripLabel
            // 
            this.StatusStripLabel.Name = "StatusStripLabel";
            this.StatusStripLabel.Size = new System.Drawing.Size(91, 17);
            this.StatusStripLabel.Text = "StatusStripLabel";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(270, 29);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(34, 25);
            this.button1.TabIndex = 29;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FindAdUserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 581);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.SuchTypCombo);
            this.Controls.Add(this.SuchTypLabel);
            this.Controls.Add(this.AddUserButton);
            this.Controls.Add(this.SuchKriteriumLabel);
            this.Controls.Add(this.FilterArtCombo);
            this.Controls.Add(this.SuchenButton);
            this.Controls.Add(this.UserTreeView);
            this.Controls.Add(this.FilterBox);
            this.Controls.Add(this.SucheLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FindAdUserForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Active Directory Suche";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox FilterArtCombo;
        private System.Windows.Forms.Button SuchenButton;
        private System.Windows.Forms.TreeView UserTreeView;
        private System.Windows.Forms.TextBox FilterBox;
        private System.Windows.Forms.Label SucheLabel;
        private System.Windows.Forms.Label SuchKriteriumLabel;
        private System.Windows.Forms.Button AddUserButton;
        private System.Windows.Forms.Label SuchTypLabel;
        private System.Windows.Forms.ComboBox SuchTypCombo;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusStripLabel;
        private System.Windows.Forms.Button button1;
    }
}