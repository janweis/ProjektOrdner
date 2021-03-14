namespace ProjektOrdner.Forms
{
    partial class ManagerForm
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
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Berechtigungen - Manager", 1, 1);
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Berechtigungen - Lesen & Schreiben", 1, 1);
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Berechtigungen - Nur Lesen", 1, 1);
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Einstellungen");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Beispiel Projekt", 6, 6, new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManagerForm));
            this.ContextMenu2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.umbenennenToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.EntfernenItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.ManagePermissionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UpdatePermissionsItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.AusklappenItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EinklappenItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.EigenschaftenItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusProjektZahl = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripProjektStatusMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.projektOrnderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.anlegenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.umbenennenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.entfernenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.updateAufV2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.mehrfachAuswahlModusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.beendenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.berechtigungenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hinzufügenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.umwandelnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lesenSchreibenNurLesenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nurLesenLesenSchreibenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aktualisiereAlleBerechtigungenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projektRootToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.test1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.test2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.verwaltenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projektRootAnlegenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ansichtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aktualisierenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.zeigeDefekteProjekteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nachUpdatesSuchenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.infoToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ProjektsTree = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.projekteErneutEinlesenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.MainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ContextMenu2
            // 
            this.ContextMenu2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ContextMenu2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.umbenennenToolStripMenuItem1,
            this.EntfernenItem,
            this.toolStripSeparator4,
            this.ManagePermissionsMenuItem,
            this.UpdatePermissionsItem,
            this.toolStripSeparator1,
            this.AusklappenItem,
            this.EinklappenItem,
            this.toolStripSeparator5,
            this.EigenschaftenItem});
            this.ContextMenu2.Name = "contextMenuStrip1";
            this.ContextMenu2.Size = new System.Drawing.Size(235, 176);
            // 
            // umbenennenToolStripMenuItem1
            // 
            this.umbenennenToolStripMenuItem1.Name = "umbenennenToolStripMenuItem1";
            this.umbenennenToolStripMenuItem1.Size = new System.Drawing.Size(234, 22);
            this.umbenennenToolStripMenuItem1.Text = "Umbenennen";
            this.umbenennenToolStripMenuItem1.Click += new System.EventHandler(this.umbenennenToolStripMenuItem1_Click);
            // 
            // EntfernenItem
            // 
            this.EntfernenItem.Name = "EntfernenItem";
            this.EntfernenItem.Size = new System.Drawing.Size(234, 22);
            this.EntfernenItem.Text = "Entfernen";
            this.EntfernenItem.Click += new System.EventHandler(this.EntfernenItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(231, 6);
            // 
            // ManagePermissionsMenuItem
            // 
            this.ManagePermissionsMenuItem.Name = "ManagePermissionsMenuItem";
            this.ManagePermissionsMenuItem.Size = new System.Drawing.Size(234, 22);
            this.ManagePermissionsMenuItem.Text = "Berechtigungen verwalten";
            this.ManagePermissionsMenuItem.Click += new System.EventHandler(this.ManagePermissionsMenuItem_Click);
            // 
            // UpdatePermissionsItem
            // 
            this.UpdatePermissionsItem.Name = "UpdatePermissionsItem";
            this.UpdatePermissionsItem.Size = new System.Drawing.Size(234, 22);
            this.UpdatePermissionsItem.Text = "Berechtigungsabgleich starten";
            this.UpdatePermissionsItem.Click += new System.EventHandler(this.UpdatePermissionsItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(231, 6);
            // 
            // AusklappenItem
            // 
            this.AusklappenItem.Name = "AusklappenItem";
            this.AusklappenItem.Size = new System.Drawing.Size(234, 22);
            this.AusklappenItem.Text = "Alles Ausklappen";
            this.AusklappenItem.Click += new System.EventHandler(this.AusklappenItem_Click);
            // 
            // EinklappenItem
            // 
            this.EinklappenItem.Name = "EinklappenItem";
            this.EinklappenItem.Size = new System.Drawing.Size(234, 22);
            this.EinklappenItem.Text = "Alles Einklappen";
            this.EinklappenItem.Click += new System.EventHandler(this.EinklappenItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(231, 6);
            // 
            // EigenschaftenItem
            // 
            this.EigenschaftenItem.Name = "EigenschaftenItem";
            this.EigenschaftenItem.Size = new System.Drawing.Size(234, 22);
            this.EigenschaftenItem.Text = "Projekt im Explorer öffnen";
            this.EigenschaftenItem.Click += new System.EventHandler(this.EigenschaftenItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusProjektZahl,
            this.ToolStripProjektStatusMessage});
            this.statusStrip1.Location = new System.Drawing.Point(0, 453);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(526, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusProjektZahl
            // 
            this.toolStripStatusProjektZahl.Name = "toolStripStatusProjektZahl";
            this.toolStripStatusProjektZahl.Size = new System.Drawing.Size(59, 17);
            this.toolStripStatusProjektZahl.Text = "0 Projekte";
            // 
            // ToolStripProjektStatusMessage
            // 
            this.ToolStripProjektStatusMessage.Name = "ToolStripProjektStatusMessage";
            this.ToolStripProjektStatusMessage.Size = new System.Drawing.Size(48, 17);
            this.ToolStripProjektStatusMessage.Text = "Status...";
            // 
            // MainMenu
            // 
            this.MainMenu.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projektOrnderToolStripMenuItem,
            this.berechtigungenToolStripMenuItem,
            this.projektRootToolStripMenuItem,
            this.ansichtToolStripMenuItem,
            this.infoToolStripMenuItem});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(526, 25);
            this.MainMenu.TabIndex = 7;
            this.MainMenu.Text = "Menü";
            // 
            // projektOrnderToolStripMenuItem
            // 
            this.projektOrnderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.anlegenToolStripMenuItem,
            this.umbenennenToolStripMenuItem,
            this.entfernenToolStripMenuItem,
            this.toolStripSeparator7,
            this.updateAufV2ToolStripMenuItem,
            this.toolStripMenuItem3,
            this.mehrfachAuswahlModusToolStripMenuItem,
            this.toolStripSeparator2,
            this.beendenToolStripMenuItem});
            this.projektOrnderToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.projektOrnderToolStripMenuItem.Name = "projektOrnderToolStripMenuItem";
            this.projektOrnderToolStripMenuItem.Size = new System.Drawing.Size(102, 21);
            this.projektOrnderToolStripMenuItem.Text = "ProjektOrdner";
            // 
            // anlegenToolStripMenuItem
            // 
            this.anlegenToolStripMenuItem.Name = "anlegenToolStripMenuItem";
            this.anlegenToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.anlegenToolStripMenuItem.Text = "Anlegen";
            this.anlegenToolStripMenuItem.Click += new System.EventHandler(this.anlegenToolStripMenuItem_Click);
            // 
            // umbenennenToolStripMenuItem
            // 
            this.umbenennenToolStripMenuItem.Name = "umbenennenToolStripMenuItem";
            this.umbenennenToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.umbenennenToolStripMenuItem.Text = "Umbenennen";
            this.umbenennenToolStripMenuItem.Click += new System.EventHandler(this.umbenennenToolStripMenuItem_Click);
            // 
            // entfernenToolStripMenuItem
            // 
            this.entfernenToolStripMenuItem.Name = "entfernenToolStripMenuItem";
            this.entfernenToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.entfernenToolStripMenuItem.Text = "Entfernen";
            this.entfernenToolStripMenuItem.Click += new System.EventHandler(this.entfernenToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(225, 6);
            // 
            // updateAufV2ToolStripMenuItem
            // 
            this.updateAufV2ToolStripMenuItem.Enabled = false;
            this.updateAufV2ToolStripMenuItem.Name = "updateAufV2ToolStripMenuItem";
            this.updateAufV2ToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.updateAufV2ToolStripMenuItem.Text = "Upgrade auf V2";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(225, 6);
            // 
            // mehrfachAuswahlModusToolStripMenuItem
            // 
            this.mehrfachAuswahlModusToolStripMenuItem.Name = "mehrfachAuswahlModusToolStripMenuItem";
            this.mehrfachAuswahlModusToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.mehrfachAuswahlModusToolStripMenuItem.Text = "Mehrfach Auswahl-Modus";
            this.mehrfachAuswahlModusToolStripMenuItem.Click += new System.EventHandler(this.MehrfachAuswahlModusToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(225, 6);
            // 
            // beendenToolStripMenuItem
            // 
            this.beendenToolStripMenuItem.Name = "beendenToolStripMenuItem";
            this.beendenToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.beendenToolStripMenuItem.Text = "Beenden";
            this.beendenToolStripMenuItem.Click += new System.EventHandler(this.beendenToolStripMenuItem_Click);
            // 
            // berechtigungenToolStripMenuItem
            // 
            this.berechtigungenToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hinzufügenToolStripMenuItem,
            this.umwandelnToolStripMenuItem,
            this.toolStripSeparator3,
            this.updateToolStripMenuItem,
            this.aktualisiereAlleBerechtigungenToolStripMenuItem});
            this.berechtigungenToolStripMenuItem.Name = "berechtigungenToolStripMenuItem";
            this.berechtigungenToolStripMenuItem.Size = new System.Drawing.Size(110, 21);
            this.berechtigungenToolStripMenuItem.Text = "Berechtigungen";
            // 
            // hinzufügenToolStripMenuItem
            // 
            this.hinzufügenToolStripMenuItem.Name = "hinzufügenToolStripMenuItem";
            this.hinzufügenToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.hinzufügenToolStripMenuItem.Text = "Projektberechtigung verwalten";
            this.hinzufügenToolStripMenuItem.Click += new System.EventHandler(this.HinzufügenToolStripMenuItem_Click);
            // 
            // umwandelnToolStripMenuItem
            // 
            this.umwandelnToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lesenSchreibenNurLesenToolStripMenuItem,
            this.nurLesenLesenSchreibenToolStripMenuItem});
            this.umwandelnToolStripMenuItem.Enabled = false;
            this.umwandelnToolStripMenuItem.Name = "umwandelnToolStripMenuItem";
            this.umwandelnToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.umwandelnToolStripMenuItem.Text = "Quick-Switch";
            // 
            // lesenSchreibenNurLesenToolStripMenuItem
            // 
            this.lesenSchreibenNurLesenToolStripMenuItem.Name = "lesenSchreibenNurLesenToolStripMenuItem";
            this.lesenSchreibenNurLesenToolStripMenuItem.Size = new System.Drawing.Size(265, 22);
            this.lesenSchreibenNurLesenToolStripMenuItem.Text = "Lesen && Schreiben -> Nur Lesen";
            // 
            // nurLesenLesenSchreibenToolStripMenuItem
            // 
            this.nurLesenLesenSchreibenToolStripMenuItem.Name = "nurLesenLesenSchreibenToolStripMenuItem";
            this.nurLesenLesenSchreibenToolStripMenuItem.Size = new System.Drawing.Size(265, 22);
            this.nurLesenLesenSchreibenToolStripMenuItem.Text = "Nur Lesen -> Lesen && Schreiben";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(291, 6);
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.updateToolStripMenuItem.Text = "Abgleich der Projekt-Berechtigungen";
            this.updateToolStripMenuItem.Click += new System.EventHandler(this.updateToolStripMenuItem_Click);
            // 
            // aktualisiereAlleBerechtigungenToolStripMenuItem
            // 
            this.aktualisiereAlleBerechtigungenToolStripMenuItem.Name = "aktualisiereAlleBerechtigungenToolStripMenuItem";
            this.aktualisiereAlleBerechtigungenToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.aktualisiereAlleBerechtigungenToolStripMenuItem.Text = "Abgleich aller Projekt-Berechtigungen";
            this.aktualisiereAlleBerechtigungenToolStripMenuItem.Click += new System.EventHandler(this.aktualisiereAlleBerechtigungenToolStripMenuItem_Click);
            // 
            // projektRootToolStripMenuItem
            // 
            this.projektRootToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.test1ToolStripMenuItem,
            this.test2ToolStripMenuItem,
            this.toolStripSeparator6,
            this.verwaltenToolStripMenuItem,
            this.projektRootAnlegenToolStripMenuItem});
            this.projektRootToolStripMenuItem.Enabled = false;
            this.projektRootToolStripMenuItem.Name = "projektRootToolStripMenuItem";
            this.projektRootToolStripMenuItem.Size = new System.Drawing.Size(141, 21);
            this.projektRootToolStripMenuItem.Text = "ProjektOrdners-Root";
            // 
            // test1ToolStripMenuItem
            // 
            this.test1ToolStripMenuItem.Name = "test1ToolStripMenuItem";
            this.test1ToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.test1ToolStripMenuItem.Text = "Test1";
            // 
            // test2ToolStripMenuItem
            // 
            this.test2ToolStripMenuItem.Name = "test2ToolStripMenuItem";
            this.test2ToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.test2ToolStripMenuItem.Text = "Test2";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(220, 6);
            // 
            // verwaltenToolStripMenuItem
            // 
            this.verwaltenToolStripMenuItem.Name = "verwaltenToolStripMenuItem";
            this.verwaltenToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.verwaltenToolStripMenuItem.Text = "Pfade verwalten";
            this.verwaltenToolStripMenuItem.Click += new System.EventHandler(this.verwaltenToolStripMenuItem_Click);
            // 
            // projektRootAnlegenToolStripMenuItem
            // 
            this.projektRootAnlegenToolStripMenuItem.Name = "projektRootAnlegenToolStripMenuItem";
            this.projektRootAnlegenToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.projektRootAnlegenToolStripMenuItem.Text = "Neuen Rootpfad anlegen";
            // 
            // ansichtToolStripMenuItem
            // 
            this.ansichtToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aktualisierenToolStripMenuItem,
            this.toolStripSeparator8,
            this.projekteErneutEinlesenToolStripMenuItem,
            this.zeigeDefekteProjekteToolStripMenuItem});
            this.ansichtToolStripMenuItem.Name = "ansichtToolStripMenuItem";
            this.ansichtToolStripMenuItem.Size = new System.Drawing.Size(61, 21);
            this.ansichtToolStripMenuItem.Text = "Ansicht";
            // 
            // aktualisierenToolStripMenuItem
            // 
            this.aktualisierenToolStripMenuItem.Name = "aktualisierenToolStripMenuItem";
            this.aktualisierenToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.aktualisierenToolStripMenuItem.Text = "Aktualisieren";
            this.aktualisierenToolStripMenuItem.Click += new System.EventHandler(this.aktualisierenToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(212, 6);
            // 
            // zeigeDefekteProjekteToolStripMenuItem
            // 
            this.zeigeDefekteProjekteToolStripMenuItem.Name = "zeigeDefekteProjekteToolStripMenuItem";
            this.zeigeDefekteProjekteToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.zeigeDefekteProjekteToolStripMenuItem.Text = "Zeige defekte Projekte";
            this.zeigeDefekteProjekteToolStripMenuItem.Click += new System.EventHandler(this.zeigeDefekteProjekteToolStripMenuItem_Click);
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nachUpdatesSuchenToolStripMenuItem,
            this.toolStripMenuItem1,
            this.infoToolStripMenuItem1});
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(42, 21);
            this.infoToolStripMenuItem.Text = "Info";
            // 
            // nachUpdatesSuchenToolStripMenuItem
            // 
            this.nachUpdatesSuchenToolStripMenuItem.Enabled = false;
            this.nachUpdatesSuchenToolStripMenuItem.Name = "nachUpdatesSuchenToolStripMenuItem";
            this.nachUpdatesSuchenToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.nachUpdatesSuchenToolStripMenuItem.Text = "Nach Updates suchen";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(200, 6);
            // 
            // infoToolStripMenuItem1
            // 
            this.infoToolStripMenuItem1.Name = "infoToolStripMenuItem1";
            this.infoToolStripMenuItem1.Size = new System.Drawing.Size(203, 22);
            this.infoToolStripMenuItem1.Text = "Info";
            this.infoToolStripMenuItem1.Click += new System.EventHandler(this.infoToolStripMenuItem1_Click);
            // 
            // ProjektsTree
            // 
            this.ProjektsTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProjektsTree.ImageIndex = 0;
            this.ProjektsTree.ImageList = this.imageList1;
            this.ProjektsTree.Location = new System.Drawing.Point(12, 93);
            this.ProjektsTree.Name = "ProjektsTree";
            treeNode6.ImageIndex = 1;
            treeNode6.Name = "Knoten1";
            treeNode6.SelectedImageIndex = 1;
            treeNode6.Text = "Berechtigungen - Manager";
            treeNode7.ImageIndex = 1;
            treeNode7.Name = "Knoten3";
            treeNode7.SelectedImageIndex = 1;
            treeNode7.Text = "Berechtigungen - Lesen & Schreiben";
            treeNode8.ImageIndex = 1;
            treeNode8.Name = "Knoten4";
            treeNode8.SelectedImageIndex = 1;
            treeNode8.Text = "Berechtigungen - Nur Lesen";
            treeNode9.Name = "Knoten5";
            treeNode9.SelectedImageIndex = 0;
            treeNode9.Text = "Einstellungen";
            treeNode10.ContextMenuStrip = this.ContextMenu2;
            treeNode10.ImageIndex = 6;
            treeNode10.Name = "Knoten0";
            treeNode10.SelectedImageIndex = 6;
            treeNode10.Text = "Beispiel Projekt";
            this.ProjektsTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode10});
            this.ProjektsTree.SelectedImageIndex = 0;
            this.ProjektsTree.Size = new System.Drawing.Size(502, 327);
            this.ProjektsTree.TabIndex = 8;
            this.ProjektsTree.DoubleClick += new System.EventHandler(this.ProjektsTree_DoubleClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "konstruktion-und-werkzeuge.png");
            this.imageList1.Images.SetKeyName(1, "smart-key.png");
            this.imageList1.Images.SetKeyName(2, "liste.png");
            this.imageList1.Images.SetKeyName(3, "kalender.png");
            this.imageList1.Images.SetKeyName(4, "kalender (1).png");
            this.imageList1.Images.SetKeyName(5, "seite-nicht-gefunden.png");
            this.imageList1.Images.SetKeyName(6, "projects_folder_20300.ico");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "Projektfilter";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 62);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(257, 25);
            this.textBox1.TabIndex = 10;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(275, 62);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(164, 25);
            this.button1.TabIndex = 11;
            this.button1.Text = "Filter löschen";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // projekteErneutEinlesenToolStripMenuItem
            // 
            this.projekteErneutEinlesenToolStripMenuItem.Name = "projekteErneutEinlesenToolStripMenuItem";
            this.projekteErneutEinlesenToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.projekteErneutEinlesenToolStripMenuItem.Text = "Projekte erneut einlesen";
            this.projekteErneutEinlesenToolStripMenuItem.Click += new System.EventHandler(this.projekteErneutEinlesenToolStripMenuItem_Click);
            // 
            // ManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 475);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ProjektsTree);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.MainMenu);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MainMenu;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ManagerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ProjektOrdner - Manager";
            this.ContextMenu2.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem projektOrnderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem anlegenToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem beendenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem berechtigungenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hinzufügenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nachUpdatesSuchenToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem1;
        private System.Windows.Forms.TreeView ProjektsTree;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem umwandelnToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip ContextMenu2;
        private System.Windows.Forms.ToolStripMenuItem EntfernenItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem AusklappenItem;
        private System.Windows.Forms.ToolStripMenuItem EinklappenItem;
        private System.Windows.Forms.ToolStripMenuItem ansichtToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mehrfachAuswahlModusToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem entfernenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lesenSchreibenNurLesenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nurLesenLesenSchreibenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem umbenennenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem umbenennenToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem UpdatePermissionsItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem EigenschaftenItem;
        private System.Windows.Forms.ToolStripMenuItem projektRootToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem test1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem test2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem verwaltenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aktualisiereAlleBerechtigungenToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem updateAufV2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusProjektZahl;
        private System.Windows.Forms.ToolStripMenuItem aktualisierenToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripProjektStatusMessage;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem zeigeDefekteProjekteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ManagePermissionsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem projektRootAnlegenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem projekteErneutEinlesenToolStripMenuItem;
    }
}