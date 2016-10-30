namespace Rocket.Launcher
{
    partial class Service
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
            this.cbInstance = new MetroFramework.Controls.MetroCheckBox();
            this.SuspendLayout();
            // 
            // cbInstance
            // 
            this.cbInstance.AutoSize = true;
            this.cbInstance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbInstance.Location = new System.Drawing.Point(0, 0);
            this.cbInstance.Name = "cbInstance";
            this.cbInstance.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.cbInstance.Size = new System.Drawing.Size(130, 26);
            this.cbInstance.TabIndex = 0;
            this.cbInstance.Text = "checkBox1";
            this.cbInstance.UseVisualStyleBackColor = true;
            this.cbInstance.CheckedChanged += new System.EventHandler(this.cbInstance_CheckedChanged);
            // 
            // Service
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbInstance);
            this.Name = "Service";
            this.Size = new System.Drawing.Size(130, 26);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroCheckBox cbInstance;
    }
}
