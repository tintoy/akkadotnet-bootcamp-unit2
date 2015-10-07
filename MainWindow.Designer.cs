﻿namespace ChartApp
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
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
			this.sysChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.btnAddSeries = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.sysChart)).BeginInit();
			this.SuspendLayout();
			// 
			// sysChart
			// 
			chartArea1.Name = "ChartArea1";
			this.sysChart.ChartAreas.Add(chartArea1);
			this.sysChart.Dock = System.Windows.Forms.DockStyle.Fill;
			legend1.Name = "Legend1";
			this.sysChart.Legends.Add(legend1);
			this.sysChart.Location = new System.Drawing.Point(0, 0);
			this.sysChart.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.sysChart.Name = "sysChart";
			series1.ChartArea = "ChartArea1";
			series1.Legend = "Legend1";
			series1.Name = "Series1";
			this.sysChart.Series.Add(series1);
			this.sysChart.Size = new System.Drawing.Size(1026, 686);
			this.sysChart.TabIndex = 0;
			this.sysChart.Text = "sysChart";
			// 
			// btnAddSeries
			// 
			this.btnAddSeries.Location = new System.Drawing.Point(891, 576);
			this.btnAddSeries.Name = "btnAddSeries";
			this.btnAddSeries.Size = new System.Drawing.Size(123, 42);
			this.btnAddSeries.TabIndex = 1;
			this.btnAddSeries.Text = "&Add Series";
			this.btnAddSeries.UseVisualStyleBackColor = true;
			this.btnAddSeries.Click += new System.EventHandler(this.btnAddSeries_Click);
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1026, 686);
			this.Controls.Add(this.btnAddSeries);
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
		private System.Windows.Forms.Button btnAddSeries;
	}
}

