namespace Tools
{
    partial class Form1
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
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtFilterFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnBrowseFilter = new System.Windows.Forms.Button();
            this.txtResultFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBrowseResult = new System.Windows.Forms.Button();
            this.txtSourceFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowseSource = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.txtWasteFile = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnBrowseWaste = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.radioButton2);
            this.tabPage1.Controls.Add(this.radioButton1);
            this.tabPage1.Controls.Add(this.btnStart);
            this.tabPage1.Controls.Add(this.txtLog);
            this.tabPage1.Controls.Add(this.txtWasteFile);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.btnBrowseWaste);
            this.tabPage1.Controls.Add(this.txtFilterFile);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.btnBrowseFilter);
            this.tabPage1.Controls.Add(this.txtResultFile);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.btnBrowseResult);
            this.tabPage1.Controls.Add(this.txtSourceFile);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.btnBrowseSource);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(681, 358);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Filtering";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtFilterFile
            // 
            this.txtFilterFile.Location = new System.Drawing.Point(122, 53);
            this.txtFilterFile.Name = "txtFilterFile";
            this.txtFilterFile.Size = new System.Drawing.Size(304, 20);
            this.txtFilterFile.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(59, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Filter file";
            // 
            // btnBrowseFilter
            // 
            this.btnBrowseFilter.Location = new System.Drawing.Point(432, 51);
            this.btnBrowseFilter.Name = "btnBrowseFilter";
            this.btnBrowseFilter.Size = new System.Drawing.Size(28, 23);
            this.btnBrowseFilter.TabIndex = 6;
            this.btnBrowseFilter.Text = "...";
            this.btnBrowseFilter.UseVisualStyleBackColor = true;
            // 
            // txtResultFile
            // 
            this.txtResultFile.Location = new System.Drawing.Point(122, 94);
            this.txtResultFile.Name = "txtResultFile";
            this.txtResultFile.Size = new System.Drawing.Size(304, 20);
            this.txtResultFile.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(59, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Result file";
            // 
            // btnBrowseResult
            // 
            this.btnBrowseResult.Location = new System.Drawing.Point(432, 92);
            this.btnBrowseResult.Name = "btnBrowseResult";
            this.btnBrowseResult.Size = new System.Drawing.Size(28, 23);
            this.btnBrowseResult.TabIndex = 3;
            this.btnBrowseResult.Text = "...";
            this.btnBrowseResult.UseVisualStyleBackColor = true;
            // 
            // txtSourceFile
            // 
            this.txtSourceFile.Location = new System.Drawing.Point(122, 14);
            this.txtSourceFile.Name = "txtSourceFile";
            this.txtSourceFile.Size = new System.Drawing.Size(304, 20);
            this.txtSourceFile.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Source file";
            // 
            // btnBrowseSource
            // 
            this.btnBrowseSource.Location = new System.Drawing.Point(432, 12);
            this.btnBrowseSource.Name = "btnBrowseSource";
            this.btnBrowseSource.Size = new System.Drawing.Size(28, 23);
            this.btnBrowseSource.TabIndex = 0;
            this.btnBrowseSource.Text = "...";
            this.btnBrowseSource.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(689, 384);
            this.tabControl1.TabIndex = 0;
            // 
            // txtWasteFile
            // 
            this.txtWasteFile.Location = new System.Drawing.Point(122, 137);
            this.txtWasteFile.Name = "txtWasteFile";
            this.txtWasteFile.Size = new System.Drawing.Size(304, 20);
            this.txtWasteFile.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(59, 140);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Waste file";
            // 
            // btnBrowseWaste
            // 
            this.btnBrowseWaste.Location = new System.Drawing.Point(432, 135);
            this.btnBrowseWaste.Name = "btnBrowseWaste";
            this.btnBrowseWaste.Size = new System.Drawing.Size(28, 23);
            this.btnBrowseWaste.TabIndex = 9;
            this.btnBrowseWaste.Text = "...";
            this.btnBrowseWaste.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtLog.Location = new System.Drawing.Point(3, 216);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(675, 139);
            this.txtLog.TabIndex = 12;
            this.txtLog.Text = "";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(385, 187);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 13;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(147, 190);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(74, 17);
            this.radioButton1.TabIndex = 14;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "sentences";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(243, 190);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(57, 17);
            this.radioButton2.TabIndex = 15;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "tokens";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(689, 384);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox txtSourceFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowseSource;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TextBox txtFilterFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnBrowseFilter;
        private System.Windows.Forms.TextBox txtResultFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBrowseResult;
        private System.Windows.Forms.TextBox txtWasteFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnBrowseWaste;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
    }
}

