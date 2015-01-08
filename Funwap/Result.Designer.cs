namespace Funwap
{
	partial class Result
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
			this.Stdout = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// Stdout
			// 
			this.Stdout.BackColor = System.Drawing.Color.Black;
			this.Stdout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Stdout.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Stdout.ForeColor = System.Drawing.Color.White;
			this.Stdout.Location = new System.Drawing.Point(0, 0);
			this.Stdout.Multiline = true;
			this.Stdout.Name = "Stdout";
			this.Stdout.ReadOnly = true;
			this.Stdout.Size = new System.Drawing.Size(784, 561);
			this.Stdout.TabIndex = 0;
			this.Stdout.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Stdout_KeyPress);
			// 
			// Result
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 561);
			this.Controls.Add(this.Stdout);
			this.MinimumSize = new System.Drawing.Size(200, 200);
			this.Name = "Result";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Result";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		/// <summary>The Standard Output <see cref="System.Windows.Forms.TextBox"/> used to write the results of the code interpretation.</summary>
		public System.Windows.Forms.TextBox Stdout;
	}
}