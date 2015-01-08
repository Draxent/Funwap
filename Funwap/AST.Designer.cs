namespace Funwap
{
	partial class AST
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
			this.Panel = new System.Windows.Forms.Panel();
			this.DrawingBox = new System.Windows.Forms.Panel();
			this.Panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// Panel
			// 
			this.Panel.Controls.Add(this.DrawingBox);
			this.Panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Panel.Location = new System.Drawing.Point(0, 0);
			this.Panel.Name = "Panel";
			this.Panel.Size = new System.Drawing.Size(1184, 661);
			this.Panel.TabIndex = 1;
			// 
			// DrawingBox
			// 
			this.DrawingBox.Location = new System.Drawing.Point(12, 12);
			this.DrawingBox.Name = "DrawingBox";
			this.DrawingBox.Size = new System.Drawing.Size(200, 100);
			this.DrawingBox.TabIndex = 0;
			this.DrawingBox.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawingBox_Paint);
			// 
			// AST
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1184, 661);
			this.Controls.Add(this.Panel);
			this.MaximumSize = new System.Drawing.Size(1200, 700);
			this.MinimumSize = new System.Drawing.Size(300, 200);
			this.Name = "AST";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Abstract Syntax Tree";
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AST_KeyPress);
			this.Panel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel Panel;
		private System.Windows.Forms.Panel DrawingBox;


	}
}