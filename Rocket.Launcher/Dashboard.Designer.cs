namespace Rocket.Launcher
{
    partial class Dashboard
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tcDashboard = new MetroFramework.Controls.MetroTabControl();
            this.tabPage1 = new MetroFramework.Controls.MetroTabPage();
            this.textBox1 = new MetroFramework.Controls.MetroTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new MetroFramework.Controls.MetroTabPage();
            this.tcDashboard.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcDashboard
            // 
            this.tcDashboard.Controls.Add(this.tabPage1);
            this.tcDashboard.Controls.Add(this.tabPage2);
            this.tcDashboard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcDashboard.Location = new System.Drawing.Point(0, 0);
            this.tcDashboard.Name = "tcDashboard";
            this.tcDashboard.SelectedIndex = 0;
            this.tcDashboard.Size = new System.Drawing.Size(778, 398);
            this.tcDashboard.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.richTextBox1);
            this.tabPage1.HorizontalScrollbarBarColor = true;
            this.tabPage1.Location = new System.Drawing.Point(4, 35);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(770, 359);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Console";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.VerticalScrollbarBarColor = true;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.Black;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(3, 337);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(764, 18);
            this.textBox1.TabIndex = 1;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.Black;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.richTextBox1.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.ForeColor = System.Drawing.Color.White;
            this.richTextBox1.Location = new System.Drawing.Point(3, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.richTextBox1.Size = new System.Drawing.Size(764, 334);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // tabPage2
            // 
            this.tabPage2.HorizontalScrollbarBarColor = true;
            this.tabPage2.Location = new System.Drawing.Point(4, 35);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(770, 359);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Players";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.VerticalScrollbarBarColor = true;
            // 
            // Dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcDashboard);
            this.Name = "Dashboard";
            this.Size = new System.Drawing.Size(778, 398);
            this.tcDashboard.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroTabControl tcDashboard;
        private MetroFramework.Controls.MetroTabPage tabPage1;
        private MetroFramework.Controls.MetroTextBox textBox1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private MetroFramework.Controls.MetroTabPage tabPage2;
    }
}
