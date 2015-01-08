namespace Funwap
{
	partial class Stdin
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
			this.InputLabel = new System.Windows.Forms.Label();
			this.Input = new System.Windows.Forms.TextBox();
			this.OKButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// InputLabel
			// 
			this.InputLabel.AutoSize = true;
			this.InputLabel.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.InputLabel.Location = new System.Drawing.Point(1, 3);
			this.InputLabel.Name = "InputLabel";
			this.InputLabel.Size = new System.Drawing.Size(228, 18);
			this.InputLabel.TabIndex = 4;
			this.InputLabel.Text = "Insert the input text:";
			// 
			// Input
			// 
			this.Input.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Input.Location = new System.Drawing.Point(4, 21);
			this.Input.Name = "Input";
			this.Input.Size = new System.Drawing.Size(376, 26);
			this.Input.TabIndex = 9;
			this.Input.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OKButton_KeyPress);
			// 
			// OKButton
			// 
			this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OKButton.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.OKButton.Location = new System.Drawing.Point(154, 50);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(75, 34);
			this.OKButton.TabIndex = 10;
			this.OKButton.Text = "OK";
			this.OKButton.UseVisualStyleBackColor = true;
			this.OKButton.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OKButton_KeyPress);
			// 
			// Stdin
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 86);
			this.Controls.Add(this.OKButton);
			this.Controls.Add(this.Input);
			this.Controls.Add(this.InputLabel);
			this.MaximumSize = new System.Drawing.Size(400, 125);
			this.MinimumSize = new System.Drawing.Size(400, 125);
			this.Name = "Stdin";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Stdin";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label InputLabel;
		private System.Windows.Forms.Button OKButton;
		/// <summary>The Input <see cref="System.Windows.Forms.TextBox"/> used to acquire the User input.</summary>
		public System.Windows.Forms.TextBox Input;

	}
}