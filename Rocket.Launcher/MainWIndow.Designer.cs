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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lInstances = new MetroFramework.Controls.MetroLabel();
            this.flpServiceContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.servicePanel = new System.Windows.Forms.Panel();
            this.pDashboardContainer = new System.Windows.Forms.Panel();
            this.lServerDashboard = new MetroFramework.Controls.MetroLabel();
            this.msmMain = new MetroFramework.Components.MetroStyleManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.servicePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.msmMain)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(20, 60);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lInstances);
            this.splitContainer1.Panel1.Controls.Add(this.flpServiceContainer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.servicePanel);
            this.splitContainer1.Size = new System.Drawing.Size(882, 371);
            this.splitContainer1.SplitterDistance = 133;
            this.splitContainer1.TabIndex = 2;
            // 
            // lInstances
            // 
            this.lInstances.Dock = System.Windows.Forms.DockStyle.Top;
            this.lInstances.Location = new System.Drawing.Point(0, 0);
            this.lInstances.Name = "lInstances";
            this.lInstances.Size = new System.Drawing.Size(133, 31);
            this.lInstances.TabIndex = 1;
            this.lInstances.Text = "Instances";
            this.lInstances.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flpServiceContainer
            // 
            this.flpServiceContainer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flpServiceContainer.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpServiceContainer.Location = new System.Drawing.Point(0, 31);
            this.flpServiceContainer.Name = "flpServiceContainer";
            this.flpServiceContainer.Size = new System.Drawing.Size(133, 340);
            this.flpServiceContainer.TabIndex = 0;
            this.flpServiceContainer.WrapContents = false;
            // 
            // servicePanel
            // 
            this.servicePanel.Controls.Add(this.pDashboardContainer);
            this.servicePanel.Controls.Add(this.lServerDashboard);
            this.servicePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.servicePanel.Location = new System.Drawing.Point(0, 0);
            this.servicePanel.Name = "servicePanel";
            this.servicePanel.Size = new System.Drawing.Size(745, 371);
            this.servicePanel.TabIndex = 0;
            // 
            // pDashboardContainer
            // 
            this.pDashboardContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pDashboardContainer.Location = new System.Drawing.Point(0, 31);
            this.pDashboardContainer.Name = "pDashboardContainer";
            this.pDashboardContainer.Size = new System.Drawing.Size(745, 340);
            this.pDashboardContainer.TabIndex = 3;
            // 
            // lServerDashboard
            // 
            this.lServerDashboard.Dock = System.Windows.Forms.DockStyle.Top;
            this.lServerDashboard.Location = new System.Drawing.Point(0, 0);
            this.lServerDashboard.Name = "lServerDashboard";
            this.lServerDashboard.Size = new System.Drawing.Size(745, 31);
            this.lServerDashboard.TabIndex = 2;
            this.lServerDashboard.Text = "Server Dashboard";
            this.lServerDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // msmMain
            // 
            this.msmMain.Owner = this;
            this.msmMain.Style = MetroFramework.MetroColorStyle.Teal;
            this.msmMain.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(922, 451);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainWindow";
            this.Text = "Rocket Launcher";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.servicePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.msmMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.FlowLayoutPanel flpServiceContainer;
        private System.Windows.Forms.Panel servicePanel;
        private MetroFramework.Controls.MetroLabel lInstances;
        private MetroFramework.Controls.MetroLabel lServerDashboard;
        private System.Windows.Forms.Panel pDashboardContainer;
        private MetroFramework.Components.MetroStyleManager msmMain;
    }
}