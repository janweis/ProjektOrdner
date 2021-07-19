namespace ProjektOrdner.Forms
{
    partial class ManagePermissionsForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManagePermissionsForm));
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Unbestimmt", 1, 1);
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Manager", 2, 2);
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Lesen & Schreiben", 2, 2);
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Nur Lesen", 2, 2);
            this.ImageCustomList = new System.Windows.Forms.ImageList(this.components);
            this.PermissionTreeView = new System.Windows.Forms.TreeView();
            this.ÜbernehmenButton = new System.Windows.Forms.Button();
            this.AbbrechenButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.EntfernenButton = new System.Windows.Forms.Button();
            this.ResetButton = new System.Windows.Forms.Button();
            this.SimpleViewRadio = new System.Windows.Forms.RadioButton();
            this.AdvancedViewRadio = new System.Windows.Forms.RadioButton();
            this.AddUserButton = new System.Windows.Forms.Button();
            this.SetUpButton = new System.Windows.Forms.Button();
            this.SetDownButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.MehrfachauswahlCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ImageCustomList
            // 
            this.ImageCustomList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageCustomList.ImageStream")));
            this.ImageCustomList.TransparentColor = System.Drawing.Color.Transparent;
            this.ImageCustomList.Images.SetKeyName(0, "profil.png");
            this.ImageCustomList.Images.SetKeyName(1, "name.png");
            this.ImageCustomList.Images.SetKeyName(2, "smart-key.png");
            // 
            // PermissionTreeView
            // 
            this.PermissionTreeView.AllowDrop = true;
            this.PermissionTreeView.ImageIndex = 2;
            this.PermissionTreeView.ImageList = this.ImageCustomList;
            this.PermissionTreeView.Location = new System.Drawing.Point(14, 42);
            this.PermissionTreeView.Name = "PermissionTreeView";
            treeNode5.ImageIndex = 1;
            treeNode5.Name = "UndefinedKnoten";
            treeNode5.SelectedImageIndex = 1;
            treeNode5.Text = "Unbestimmt";
            treeNode6.ImageIndex = 2;
            treeNode6.Name = "ManagerKnoten";
            treeNode6.SelectedImageIndex = 2;
            treeNode6.Text = "Manager";
            treeNode7.ImageIndex = 2;
            treeNode7.Name = "ChangeKnoten";
            treeNode7.SelectedImageIndex = 2;
            treeNode7.Text = "Lesen & Schreiben";
            treeNode8.ImageIndex = 2;
            treeNode8.Name = "ReadOnlyKnoten";
            treeNode8.SelectedImageIndex = 2;
            treeNode8.Text = "Nur Lesen";
            this.PermissionTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8});
            this.PermissionTreeView.SelectedImageIndex = 0;
            this.PermissionTreeView.Size = new System.Drawing.Size(416, 279);
            this.PermissionTreeView.TabIndex = 4;
            // 
            // ÜbernehmenButton
            // 
            this.ÜbernehmenButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ÜbernehmenButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ÜbernehmenButton.Location = new System.Drawing.Point(17, 450);
            this.ÜbernehmenButton.Name = "ÜbernehmenButton";
            this.ÜbernehmenButton.Size = new System.Drawing.Size(182, 35);
            this.ÜbernehmenButton.TabIndex = 11;
            this.ÜbernehmenButton.Text = "Änderungen übernehmen";
            this.ÜbernehmenButton.UseVisualStyleBackColor = true;
            this.ÜbernehmenButton.Click += new System.EventHandler(this.ÜbernehmenButton_Click);
            // 
            // AbbrechenButton
            // 
            this.AbbrechenButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.AbbrechenButton.Location = new System.Drawing.Point(396, 450);
            this.AbbrechenButton.Name = "AbbrechenButton";
            this.AbbrechenButton.Size = new System.Drawing.Size(149, 35);
            this.AbbrechenButton.TabIndex = 12;
            this.AbbrechenButton.Text = "Abbrechen";
            this.AbbrechenButton.UseVisualStyleBackColor = true;
            this.AbbrechenButton.Click += new System.EventHandler(this.AbbrechenButton_Click);
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(18, 431);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(528, 3);
            this.label3.TabIndex = 13;
            // 
            // EntfernenButton
            // 
            this.EntfernenButton.Location = new System.Drawing.Point(245, 327);
            this.EntfernenButton.Name = "EntfernenButton";
            this.EntfernenButton.Size = new System.Drawing.Size(185, 35);
            this.EntfernenButton.TabIndex = 16;
            this.EntfernenButton.Text = "Benutzer entfernen";
            this.EntfernenButton.UseVisualStyleBackColor = true;
            this.EntfernenButton.Click += new System.EventHandler(this.EntfernenButton_Click);
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(208, 450);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(182, 35);
            this.ResetButton.TabIndex = 17;
            this.ResetButton.Text = "Ansicht zurücksetzen";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // SimpleViewRadio
            // 
            this.SimpleViewRadio.AutoSize = true;
            this.SimpleViewRadio.Checked = true;
            this.SimpleViewRadio.Location = new System.Drawing.Point(179, 14);
            this.SimpleViewRadio.Name = "SimpleViewRadio";
            this.SimpleViewRadio.Size = new System.Drawing.Size(119, 21);
            this.SimpleViewRadio.TabIndex = 18;
            this.SimpleViewRadio.TabStop = true;
            this.SimpleViewRadio.Text = "Einfache Ansicht";
            this.SimpleViewRadio.UseVisualStyleBackColor = true;
            this.SimpleViewRadio.CheckedChanged += new System.EventHandler(this.SimpleViewRadio_CheckedChanged);
            // 
            // AdvancedViewRadio
            // 
            this.AdvancedViewRadio.AutoSize = true;
            this.AdvancedViewRadio.Location = new System.Drawing.Point(304, 14);
            this.AdvancedViewRadio.Name = "AdvancedViewRadio";
            this.AdvancedViewRadio.Size = new System.Drawing.Size(129, 21);
            this.AdvancedViewRadio.TabIndex = 19;
            this.AdvancedViewRadio.Text = "Erweiterte Ansicht";
            this.AdvancedViewRadio.UseVisualStyleBackColor = true;
            this.AdvancedViewRadio.CheckedChanged += new System.EventHandler(this.AdvancedViewRadio_CheckedChanged);
            // 
            // AddUserButton
            // 
            this.AddUserButton.Location = new System.Drawing.Point(14, 327);
            this.AddUserButton.Name = "AddUserButton";
            this.AddUserButton.Size = new System.Drawing.Size(185, 35);
            this.AddUserButton.TabIndex = 20;
            this.AddUserButton.Text = "Benutzer hinzufügen";
            this.AddUserButton.UseVisualStyleBackColor = true;
            this.AddUserButton.Click += new System.EventHandler(this.AddUserButton_Click);
            // 
            // SetUpButton
            // 
            this.SetUpButton.Location = new System.Drawing.Point(436, 42);
            this.SetUpButton.Name = "SetUpButton";
            this.SetUpButton.Size = new System.Drawing.Size(109, 37);
            this.SetUpButton.TabIndex = 21;
            this.SetUpButton.Text = "Hoch";
            this.SetUpButton.UseVisualStyleBackColor = true;
            this.SetUpButton.Click += new System.EventHandler(this.SetUpButton_Click);
            // 
            // SetDownButton
            // 
            this.SetDownButton.Location = new System.Drawing.Point(436, 85);
            this.SetDownButton.Name = "SetDownButton";
            this.SetDownButton.Size = new System.Drawing.Size(109, 38);
            this.SetDownButton.TabIndex = 22;
            this.SetDownButton.Text = "Runter";
            this.SetDownButton.UseVisualStyleBackColor = true;
            this.SetDownButton.Click += new System.EventHandler(this.SetDownButton_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 391);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(530, 40);
            this.label1.TabIndex = 23;
            this.label1.Text = "Hinweis: Nachdem Berechtigungen hinzugefügt oder entfernt wurden, müssen sich die" +
    " Benutzer erneut an Windows anmelden, um die Änderungen zu sychronisieren.";
            // 
            // MehrfachauswahlCheckBox
            // 
            this.MehrfachauswahlCheckBox.AutoSize = true;
            this.MehrfachauswahlCheckBox.Location = new System.Drawing.Point(14, 15);
            this.MehrfachauswahlCheckBox.Name = "MehrfachauswahlCheckBox";
            this.MehrfachauswahlCheckBox.Size = new System.Drawing.Size(128, 21);
            this.MehrfachauswahlCheckBox.TabIndex = 24;
            this.MehrfachauswahlCheckBox.Text = "Mehrfachauswahl";
            this.MehrfachauswahlCheckBox.UseVisualStyleBackColor = true;
            this.MehrfachauswahlCheckBox.CheckedChanged += new System.EventHandler(this.MehrfachauswahlCheckBox_CheckedChanged);
            // 
            // ManagePermissionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 500);
            this.Controls.Add(this.MehrfachauswahlCheckBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SetDownButton);
            this.Controls.Add(this.SetUpButton);
            this.Controls.Add(this.AddUserButton);
            this.Controls.Add(this.AdvancedViewRadio);
            this.Controls.Add(this.SimpleViewRadio);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.EntfernenButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.AbbrechenButton);
            this.Controls.Add(this.ÜbernehmenButton);
            this.Controls.Add(this.PermissionTreeView);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "ManagePermissionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Projektberechtigungen verwalten";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ÜbernehmenButton;
        private System.Windows.Forms.Button AbbrechenButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TreeView PermissionTreeView;
        private System.Windows.Forms.ImageList ImageCustomList;
        private System.Windows.Forms.Button EntfernenButton;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.RadioButton SimpleViewRadio;
        private System.Windows.Forms.RadioButton AdvancedViewRadio;
        private System.Windows.Forms.Button AddUserButton;
        private System.Windows.Forms.Button SetUpButton;
        private System.Windows.Forms.Button SetDownButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox MehrfachauswahlCheckBox;
    }
}