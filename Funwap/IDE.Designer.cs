namespace Funwap
{
    partial class IDE
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Liberare le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IDE));
			this.MenuStrip = new System.Windows.Forms.MenuStrip();
			this.MenuItem_File = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItem_OpenFile = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItem_SaveFile = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItem_EditFile = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItem_Exit = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItem_Execute = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItem_ShowAST = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItem_Compile = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItem_Interpret = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItem_QM = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItem_Help = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuItem_About = new System.Windows.Forms.ToolStripMenuItem();
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.PanelContainer = new System.Windows.Forms.SplitContainer();
			this.Editor = new System.Windows.Forms.RichTextBox();
			this.Console = new System.Windows.Forms.TextBox();
			this.ConsoleTitle = new System.Windows.Forms.Label();
			this.MenuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PanelContainer)).BeginInit();
			this.PanelContainer.Panel1.SuspendLayout();
			this.PanelContainer.Panel2.SuspendLayout();
			this.PanelContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// MenuStrip
			// 
			this.MenuStrip.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_File,
            this.MenuItem_Execute,
            this.MenuItem_QM});
			this.MenuStrip.Location = new System.Drawing.Point(0, 0);
			this.MenuStrip.Name = "MenuStrip";
			this.MenuStrip.Size = new System.Drawing.Size(784, 29);
			this.MenuStrip.TabIndex = 0;
			this.MenuStrip.Text = "menuStrip1";
			// 
			// MenuItem_File
			// 
			this.MenuItem_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_OpenFile,
            this.MenuItem_SaveFile,
            this.MenuItem_EditFile,
            this.MenuItem_Exit});
			this.MenuItem_File.Name = "MenuItem_File";
			this.MenuItem_File.Size = new System.Drawing.Size(50, 25);
			this.MenuItem_File.Text = "FILE";
			// 
			// MenuItem_OpenFile
			// 
			this.MenuItem_OpenFile.Image = ((System.Drawing.Image)(resources.GetObject("MenuItem_OpenFile.Image")));
			this.MenuItem_OpenFile.Name = "MenuItem_OpenFile";
			this.MenuItem_OpenFile.Size = new System.Drawing.Size(146, 26);
			this.MenuItem_OpenFile.Text = "Open File";
			this.MenuItem_OpenFile.Click += new System.EventHandler(this.MenuItem_OpenFile_Click);
			// 
			// MenuItem_SaveFile
			// 
			this.MenuItem_SaveFile.Image = ((System.Drawing.Image)(resources.GetObject("MenuItem_SaveFile.Image")));
			this.MenuItem_SaveFile.Name = "MenuItem_SaveFile";
			this.MenuItem_SaveFile.Size = new System.Drawing.Size(146, 26);
			this.MenuItem_SaveFile.Text = "Save File";
			this.MenuItem_SaveFile.Click += new System.EventHandler(this.MenuItem_SaveFile_Click);
			// 
			// MenuItem_EditFile
			// 
			this.MenuItem_EditFile.Image = ((System.Drawing.Image)(resources.GetObject("MenuItem_EditFile.Image")));
			this.MenuItem_EditFile.Name = "MenuItem_EditFile";
			this.MenuItem_EditFile.Size = new System.Drawing.Size(146, 26);
			this.MenuItem_EditFile.Text = "Edit File";
			this.MenuItem_EditFile.Click += new System.EventHandler(this.MenuItem_EditFile_Click);
			// 
			// MenuItem_Exit
			// 
			this.MenuItem_Exit.Image = ((System.Drawing.Image)(resources.GetObject("MenuItem_Exit.Image")));
			this.MenuItem_Exit.Name = "MenuItem_Exit";
			this.MenuItem_Exit.Size = new System.Drawing.Size(146, 26);
			this.MenuItem_Exit.Text = "Exit";
			this.MenuItem_Exit.Click += new System.EventHandler(this.MenuItem_Exit_Click);
			// 
			// MenuItem_Execute
			// 
			this.MenuItem_Execute.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_ShowAST,
            this.MenuItem_Compile,
            this.MenuItem_Interpret});
			this.MenuItem_Execute.Name = "MenuItem_Execute";
			this.MenuItem_Execute.Size = new System.Drawing.Size(84, 25);
			this.MenuItem_Execute.Text = "EXECUTE";
			// 
			// MenuItem_ShowAST
			// 
			this.MenuItem_ShowAST.Image = ((System.Drawing.Image)(resources.GetObject("MenuItem_ShowAST.Image")));
			this.MenuItem_ShowAST.Name = "MenuItem_ShowAST";
			this.MenuItem_ShowAST.Size = new System.Drawing.Size(264, 26);
			this.MenuItem_ShowAST.Text = "Show Abstract Syntax Tree";
			this.MenuItem_ShowAST.Click += new System.EventHandler(this.MenuItem_ShowAST_Click);
			// 
			// MenuItem_Compile
			// 
			this.MenuItem_Compile.Image = ((System.Drawing.Image)(resources.GetObject("MenuItem_Compile.Image")));
			this.MenuItem_Compile.Name = "MenuItem_Compile";
			this.MenuItem_Compile.Size = new System.Drawing.Size(264, 26);
			this.MenuItem_Compile.Text = "Compile";
			this.MenuItem_Compile.Click += new System.EventHandler(this.MenuItem_Compile_Click);
			// 
			// MenuItem_Interpret
			// 
			this.MenuItem_Interpret.Image = ((System.Drawing.Image)(resources.GetObject("MenuItem_Interpret.Image")));
			this.MenuItem_Interpret.Name = "MenuItem_Interpret";
			this.MenuItem_Interpret.Size = new System.Drawing.Size(264, 26);
			this.MenuItem_Interpret.Text = "Interpret";
			this.MenuItem_Interpret.Click += new System.EventHandler(this.MenuItem_Interpret_Click);
			// 
			// MenuItem_QM
			// 
			this.MenuItem_QM.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_Help,
            this.MenuItem_About});
			this.MenuItem_QM.Name = "MenuItem_QM";
			this.MenuItem_QM.Size = new System.Drawing.Size(29, 25);
			this.MenuItem_QM.Text = "?";
			// 
			// MenuItem_Help
			// 
			this.MenuItem_Help.Image = ((System.Drawing.Image)(resources.GetObject("MenuItem_Help.Image")));
			this.MenuItem_Help.Name = "MenuItem_Help";
			this.MenuItem_Help.Size = new System.Drawing.Size(122, 26);
			this.MenuItem_Help.Text = "Help";
			this.MenuItem_Help.Click += new System.EventHandler(this.MenuItem_Help_Click);
			// 
			// MenuItem_About
			// 
			this.MenuItem_About.Image = ((System.Drawing.Image)(resources.GetObject("MenuItem_About.Image")));
			this.MenuItem_About.Name = "MenuItem_About";
			this.MenuItem_About.Size = new System.Drawing.Size(122, 26);
			this.MenuItem_About.Text = "About";
			this.MenuItem_About.Click += new System.EventHandler(this.MenuItem_About_Click);
			// 
			// PanelContainer
			// 
			this.PanelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelContainer.Location = new System.Drawing.Point(0, 29);
			this.PanelContainer.Name = "PanelContainer";
			this.PanelContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// PanelContainer.Panel1
			// 
			this.PanelContainer.Panel1.Controls.Add(this.Editor);
			this.PanelContainer.Panel1MinSize = 100;
			// 
			// PanelContainer.Panel2
			// 
			this.PanelContainer.Panel2.Controls.Add(this.Console);
			this.PanelContainer.Panel2.Controls.Add(this.ConsoleTitle);
			this.PanelContainer.Panel2.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PanelContainer.Size = new System.Drawing.Size(784, 532);
			this.PanelContainer.SplitterDistance = 399;
			this.PanelContainer.TabIndex = 1;
			// 
			// Editor
			// 
			this.Editor.BackColor = System.Drawing.Color.White;
			this.Editor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Editor.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Editor.Location = new System.Drawing.Point(0, 0);
			this.Editor.Name = "Editor";
			this.Editor.Size = new System.Drawing.Size(784, 399);
			this.Editor.TabIndex = 8;
			this.Editor.Text = "";
			this.Editor.WordWrap = false;
			this.Editor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Editor_KeyDown);
			// 
			// Console
			// 
			this.Console.BackColor = System.Drawing.Color.White;
			this.Console.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Console.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Console.Location = new System.Drawing.Point(0, 16);
			this.Console.Multiline = true;
			this.Console.Name = "Console";
			this.Console.ReadOnly = true;
			this.Console.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.Console.Size = new System.Drawing.Size(784, 113);
			this.Console.TabIndex = 1;
			// 
			// ConsoleTitle
			// 
			this.ConsoleTitle.AutoSize = true;
			this.ConsoleTitle.Dock = System.Windows.Forms.DockStyle.Top;
			this.ConsoleTitle.Font = new System.Drawing.Font("Courier New", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ConsoleTitle.Location = new System.Drawing.Point(0, 0);
			this.ConsoleTitle.Name = "ConsoleTitle";
			this.ConsoleTitle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.ConsoleTitle.Size = new System.Drawing.Size(76, 16);
			this.ConsoleTitle.TabIndex = 0;
			this.ConsoleTitle.Text = "Console :";
			// 
			// IDE
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 561);
			this.Controls.Add(this.PanelContainer);
			this.Controls.Add(this.MenuStrip);
			this.MainMenuStrip = this.MenuStrip;
			this.MinimumSize = new System.Drawing.Size(200, 200);
			this.Name = "IDE";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Funw@p Compiler";
			this.MenuStrip.ResumeLayout(false);
			this.MenuStrip.PerformLayout();
			this.PanelContainer.Panel1.ResumeLayout(false);
			this.PanelContainer.Panel2.ResumeLayout(false);
			this.PanelContainer.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.PanelContainer)).EndInit();
			this.PanelContainer.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_File;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_OpenFile;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Exit;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Execute;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Compile;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_QM;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Help;
		private System.Windows.Forms.ToolStripMenuItem MenuItem_About;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_ShowAST;
		private System.Windows.Forms.SplitContainer PanelContainer;
		/// <summary>The Console <see cref="System.Windows.Forms.TextBox"/> used to report all the actions and errors.</summary>
		public System.Windows.Forms.TextBox Console;
		private System.Windows.Forms.Label ConsoleTitle;
		private System.Windows.Forms.ToolStripMenuItem MenuItem_Interpret;
		private System.Windows.Forms.ToolStripMenuItem MenuItem_SaveFile;
		private System.Windows.Forms.ToolStripMenuItem MenuItem_EditFile;
		private System.Windows.Forms.RichTextBox Editor;
    }
}

