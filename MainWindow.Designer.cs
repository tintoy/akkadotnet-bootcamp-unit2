namespace ChartApp
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
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
			this.sysChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.btnDisk = new System.Windows.Forms.Button();
			this.btnMemory = new System.Windows.Forms.Button();
			this.btnProcessor = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.sysChart)).BeginInit();
			this.SuspendLayout();
			// 
			// sysChart
			// 
			chartArea4.Name = "ChartArea1";
			this.sysChart.ChartAreas.Add(chartArea4);
			this.sysChart.Dock = System.Windows.Forms.DockStyle.Fill;
			legend4.Name = "Legend1";
			this.sysChart.Legends.Add(legend4);
			this.sysChart.Location = new System.Drawing.Point(0, 0);
			this.sysChart.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.sysChart.Name = "sysChart";
			series4.ChartArea = "ChartArea1";
			series4.Legend = "Legend1";
			series4.Name = "Series1";
			this.sysChart.Series.Add(series4);
			this.sysChart.Size = new System.Drawing.Size(1026, 686);
			this.sysChart.TabIndex = 0;
			this.sysChart.Text = "sysChart";
			// 
			// btnDisk
			// 
			this.btnDisk.Location = new System.Drawing.Point(867, 568);
			this.btnDisk.Name = "btnDisk";
			this.btnDisk.Size = new System.Drawing.Size(147, 45);
			this.btnDisk.TabIndex = 1;
			this.btnDisk.Text = "DISK (OFF)";
			this.btnDisk.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnDisk.UseVisualStyleBackColor = true;
			this.btnDisk.Click += new System.EventHandler(this.btnDisk_Click);
			// 
			// btnMemory
			// 
			this.btnMemory.Location = new System.Drawing.Point(867, 517);
			this.btnMemory.Name = "btnMemory";
			this.btnMemory.Size = new System.Drawing.Size(147, 45);
			this.btnMemory.TabIndex = 2;
			this.btnMemory.Text = "MEMORY (OFF)";
			this.btnMemory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnMemory.UseVisualStyleBackColor = true;
			this.btnMemory.Click += new System.EventHandler(this.btnMemory_Click);
			// 
			// btnProcessor
			// 
			this.btnProcessor.Location = new System.Drawing.Point(867, 466);
			this.btnProcessor.Name = "btnProcessor";
			this.btnProcessor.Size = new System.Drawing.Size(147, 45);
			this.btnProcessor.TabIndex = 3;
			this.btnProcessor.Text = "CPU (ON)";
			this.btnProcessor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnProcessor.UseVisualStyleBackColor = true;
			this.btnProcessor.Click += new System.EventHandler(this.btnProcessor_Click);
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1026, 686);
			this.Controls.Add(this.btnProcessor);
			this.Controls.Add(this.btnMemory);
			this.Controls.Add(this.btnDisk);
			this.Controls.Add(this.sysChart);
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "MainWindow";
			this.Text = "System Metrics";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.Main_Load);
			((System.ComponentModel.ISupportInitialize)(this.sysChart)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart sysChart;
		private System.Windows.Forms.Button btnDisk;
		private System.Windows.Forms.Button btnMemory;
		private System.Windows.Forms.Button btnProcessor;
	}
}

