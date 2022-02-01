
namespace ReClassNET.Forms
{
	partial class RecursiveSearchForm
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
            this.valueTextBox = new System.Windows.Forms.TextBox();
            this.levelNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.valueLabel = new System.Windows.Forms.Label();
            this.levelLabel = new System.Windows.Forms.Label();
            this.searchButton = new System.Windows.Forms.Button();
            this.typeLabel = new System.Windows.Forms.Label();
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.offsetLabel = new System.Windows.Forms.Label();
            this.offsetNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.gridLabel = new System.Windows.Forms.Label();
            this.gridNumericUpDown = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.levelNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.offsetNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // valueTextBox
            // 
            this.valueTextBox.Location = new System.Drawing.Point(12, 33);
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.Size = new System.Drawing.Size(352, 22);
            this.valueTextBox.TabIndex = 0;
            // 
            // levelNumericUpDown
            // 
            this.levelNumericUpDown.Location = new System.Drawing.Point(12, 170);
            this.levelNumericUpDown.Name = "levelNumericUpDown";
            this.levelNumericUpDown.Size = new System.Drawing.Size(352, 22);
            this.levelNumericUpDown.TabIndex = 1;
            this.levelNumericUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // valueLabel
            // 
            this.valueLabel.AutoSize = true;
            this.valueLabel.Location = new System.Drawing.Point(13, 13);
            this.valueLabel.Name = "valueLabel";
            this.valueLabel.Size = new System.Drawing.Size(48, 17);
            this.valueLabel.TabIndex = 2;
            this.valueLabel.Text = "Value:";
            // 
            // levelLabel
            // 
            this.levelLabel.AutoSize = true;
            this.levelLabel.Location = new System.Drawing.Point(13, 150);
            this.levelLabel.Name = "levelLabel";
            this.levelLabel.Size = new System.Drawing.Size(70, 17);
            this.levelLabel.TabIndex = 3;
            this.levelLabel.Text = "Max level:";
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(12, 245);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(351, 41);
            this.searchButton.TabIndex = 4;
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // typeLabel
            // 
            this.typeLabel.AutoSize = true;
            this.typeLabel.Location = new System.Drawing.Point(13, 58);
            this.typeLabel.Name = "typeLabel";
            this.typeLabel.Size = new System.Drawing.Size(44, 17);
            this.typeLabel.TabIndex = 5;
            this.typeLabel.Text = "Type:";
            // 
            // typeComboBox
            // 
            this.typeComboBox.FormattingEnabled = true;
            this.typeComboBox.Location = new System.Drawing.Point(11, 78);
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.Size = new System.Drawing.Size(351, 24);
            this.typeComboBox.TabIndex = 6;
            // 
            // offsetLabel
            // 
            this.offsetLabel.AutoSize = true;
            this.offsetLabel.Location = new System.Drawing.Point(13, 105);
            this.offsetLabel.Name = "offsetLabel";
            this.offsetLabel.Size = new System.Drawing.Size(76, 17);
            this.offsetLabel.TabIndex = 7;
            this.offsetLabel.Text = "Max offset:";
            // 
            // offsetNumericUpDown
            // 
            this.offsetNumericUpDown.Location = new System.Drawing.Point(12, 125);
            this.offsetNumericUpDown.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.offsetNumericUpDown.Name = "offsetNumericUpDown";
            this.offsetNumericUpDown.Size = new System.Drawing.Size(350, 22);
            this.offsetNumericUpDown.TabIndex = 8;
            this.offsetNumericUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // gridLabel
            // 
            this.gridLabel.AutoSize = true;
            this.gridLabel.Location = new System.Drawing.Point(13, 195);
            this.gridLabel.Name = "gridLabel";
            this.gridLabel.Size = new System.Drawing.Size(39, 17);
            this.gridLabel.TabIndex = 9;
            this.gridLabel.Text = "Grid:";
            // 
            // gridNumericUpDown
            // 
            this.gridNumericUpDown.Location = new System.Drawing.Point(11, 215);
            this.gridNumericUpDown.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.gridNumericUpDown.Name = "gridNumericUpDown";
            this.gridNumericUpDown.Size = new System.Drawing.Size(351, 22);
            this.gridNumericUpDown.TabIndex = 10;
            this.gridNumericUpDown.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // RecursiveSearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(376, 298);
            this.Controls.Add(this.gridNumericUpDown);
            this.Controls.Add(this.gridLabel);
            this.Controls.Add(this.offsetNumericUpDown);
            this.Controls.Add(this.offsetLabel);
            this.Controls.Add(this.typeComboBox);
            this.Controls.Add(this.typeLabel);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.levelLabel);
            this.Controls.Add(this.valueLabel);
            this.Controls.Add(this.levelNumericUpDown);
            this.Controls.Add(this.valueTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RecursiveSearchForm";
            this.Text = "Recursive Search";
            ((System.ComponentModel.ISupportInitialize)(this.levelNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.offsetNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox valueTextBox;
		private System.Windows.Forms.NumericUpDown levelNumericUpDown;
		private System.Windows.Forms.Label valueLabel;
		private System.Windows.Forms.Label levelLabel;
		private System.Windows.Forms.Button searchButton;
		private System.Windows.Forms.Label typeLabel;
		private System.Windows.Forms.ComboBox typeComboBox;
		private System.Windows.Forms.Label offsetLabel;
		private System.Windows.Forms.NumericUpDown offsetNumericUpDown;
		private System.Windows.Forms.Label gridLabel;
		private System.Windows.Forms.NumericUpDown gridNumericUpDown;
	}
}