namespace Rocket.Launcher
{
    partial class MainWindow
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
            this.lServerDashboard = new MetroFramework.Controls.MetroLabel();
            this.pDashboardContainer = new System.Windows.Forms.Panel();
            this.msmMain = new MetroFramework.Components.MetroStyleManager(this.components);
            this.lInstances = new MetroFramework.Controls.MetroLabel();
            this.flpServiceContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.msmMain)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lServerDashboard
            // 
            this.lServerDashboard.Dock = System.Windows.Forms.DockStyle.Top;
            this.lServerDashboard.Location = new System.Drawing.Point(0, 0);
            this.lServerDashboard.Name = "lServerDashboard";
            this.lServerDashboard.Size = new System.Drawing.Size(697, 31);
            this.lServerDashboard.TabIndex = 2;
            this.lServerDashboard.Text = "Server Dashboard";
            this.lServerDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pDashboardContainer
            // 
            this.pDashboardContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.pDashboardContainer.Location = new System.Drawing.Point(0, 31);
            this.pDashboardContainer.Name = "pDashboardContainer";
            this.pDashboardContainer.Size = new System.Drawing.Size(697, 352);
            this.pDashboardContainer.TabIndex = 3;
            // 
            // msmMain
            // 
            this.msmMain.Owner = this;
            this.msmMain.Style = MetroFramework.MetroColorStyle.Teal;
            this.msmMain.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // lInstances
            // 
            this.lInstances.Dock = System.Windows.Forms.DockStyle.Top;
            this.lInstances.Location = new System.Drawing.Point(0, 0);
            this.lInstances.Name = "lInstances";
            this.lInstances.Size = new System.Drawing.Size(176, 31);
            this.lInstances.TabIndex = 1;
            this.lInstances.Text = "Instances";
            this.lInstances.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flpServiceContainer
            // 
            this.flpServiceContainer.Dock = System.Windows.Forms.DockStyle.Left;
            this.flpServiceContainer.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpServiceContainer.Location = new System.Drawing.Point(0, 31);
            this.flpServiceContainer.Name = "flpServiceContainer";
            this.flpServiceContainer.Size = new System.Drawing.Size(176, 358);
            this.flpServiceContainer.TabIndex = 0;
            this.flpServiceContainer.WrapContents = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.pDashboardContainer);
            this.panel1.Controls.Add(this.lServerDashboard);
            this.panel1.Location = new System.Drawing.Point(202, 60);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(697, 389);
            this.panel1.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.flpServiceContainer);
            this.panel2.Controls.Add(this.lInstances);
            this.panel2.Location = new System.Drawing.Point(20, 60);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(176, 389);
            this.panel2.TabIndex = 0;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(922, 469);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "MainWindow";
            this.Text = "Rocket Launcher";
            ((System.ComponentModel.ISupportInitialize)(this.msmMain)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MetroFramework.Controls.MetroLabel lServerDashboard;
        private System.Windows.Forms.Panel pDashboardContainer;
        private MetroFramework.Components.MetroStyleManager msmMain;
        private MetroFramework.Controls.MetroLabel lInstances;
        private System.Windows.Forms.FlowLayoutPanel flpServiceContainer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}