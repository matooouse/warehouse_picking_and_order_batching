using System.Windows.Forms;

namespace warehouse_picking
{
    partial class MainGui
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
            this.generate = new System.Windows.Forms.Button();
            this.DummySolver = new System.Windows.Forms.Button();
            this.distanceLastSolutionLabel = new System.Windows.Forms.Label();
            this.distanceLastSolution = new System.Windows.Forms.TextBox();
            this.SShapeSolver = new System.Windows.Forms.Button();
            this.ReturnSolver = new System.Windows.Forms.Button();
            this.LargestGapSolver = new System.Windows.Forms.Button();
            this.CompositeSolver = new System.Windows.Forms.Button();
            this.clear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // generate
            // 
            this.generate.Location = new System.Drawing.Point(15, 350);
            this.generate.Name = "generate";
            this.generate.Size = new System.Drawing.Size(105, 23);
            this.generate.TabIndex = 0;
            this.generate.Text = "Generate";
            this.generate.UseVisualStyleBackColor = true;
            this.generate.Click += new System.EventHandler(this.generate_Click);
            // 
            // DummySolver
            // 
            this.DummySolver.Location = new System.Drawing.Point(15, 395);
            this.DummySolver.Name = "DummySolver";
            this.DummySolver.Size = new System.Drawing.Size(105, 23);
            this.DummySolver.TabIndex = 1;
            this.DummySolver.Text = "Dummy solve";
            this.DummySolver.UseVisualStyleBackColor = true;
            this.DummySolver.Click += new System.EventHandler(this.DummySolver_Click);
            // 
            // distanceLastSolutionLabel
            // 
            this.distanceLastSolutionLabel.AutoSize = true;
            this.distanceLastSolutionLabel.Location = new System.Drawing.Point(12, 446);
            this.distanceLastSolutionLabel.Name = "distanceLastSolutionLabel";
            this.distanceLastSolutionLabel.Size = new System.Drawing.Size(123, 13);
            this.distanceLastSolutionLabel.TabIndex = 2;
            this.distanceLastSolutionLabel.Text = "distance of last solution :";
            // 
            // distanceLastSolution
            // 
            this.distanceLastSolution.Enabled = false;
            this.distanceLastSolution.Location = new System.Drawing.Point(158, 443);
            this.distanceLastSolution.Name = "distanceLastSolution";
            this.distanceLastSolution.Size = new System.Drawing.Size(100, 20);
            this.distanceLastSolution.TabIndex = 3;
            // 
            // SShapeSolver
            // 
            this.SShapeSolver.Location = new System.Drawing.Point(126, 395);
            this.SShapeSolver.Name = "SShapeSolver";
            this.SShapeSolver.Size = new System.Drawing.Size(105, 23);
            this.SShapeSolver.TabIndex = 4;
            this.SShapeSolver.Text = "S-shape solve";
            this.SShapeSolver.UseVisualStyleBackColor = true;
            this.SShapeSolver.Click += new System.EventHandler(this.SShapeSolver_Click);
            // 
            // ReturnSolver
            // 
            this.ReturnSolver.Location = new System.Drawing.Point(348, 395);
            this.ReturnSolver.Name = "ReturnSolver";
            this.ReturnSolver.Size = new System.Drawing.Size(105, 23);
            this.ReturnSolver.TabIndex = 6;
            this.ReturnSolver.Text = "Return solve";
            this.ReturnSolver.UseVisualStyleBackColor = true;
            this.ReturnSolver.Click += new System.EventHandler(this.ReturnSolver_Click);
            // 
            // LargestGapSolver
            // 
            this.LargestGapSolver.Location = new System.Drawing.Point(237, 395);
            this.LargestGapSolver.Name = "LargestGapSolver";
            this.LargestGapSolver.Size = new System.Drawing.Size(105, 23);
            this.LargestGapSolver.TabIndex = 5;
            this.LargestGapSolver.Text = "Largest gap solve";
            this.LargestGapSolver.UseVisualStyleBackColor = true;
            this.LargestGapSolver.Click += new System.EventHandler(this.LargestGapSolver_Click);
            // 
            // CompositeSolver
            // 
            this.CompositeSolver.Location = new System.Drawing.Point(459, 395);
            this.CompositeSolver.Name = "CompositeSolver";
            this.CompositeSolver.Size = new System.Drawing.Size(105, 23);
            this.CompositeSolver.TabIndex = 7;
            this.CompositeSolver.Text = "Composite solve";
            this.CompositeSolver.UseVisualStyleBackColor = true;
            this.CompositeSolver.Click += new System.EventHandler(this.CompositeSolver_Click);
            // 
            // clear
            // 
            this.clear.Location = new System.Drawing.Point(126, 350);
            this.clear.Name = "clear";
            this.clear.Size = new System.Drawing.Size(75, 23);
            this.clear.TabIndex = 8;
            this.clear.Text = "Clear";
            this.clear.UseVisualStyleBackColor = true;
            this.clear.Click += new System.EventHandler(this.clear_Click);
            // 
            // MainGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 500);
            this.Controls.Add(this.clear);
            this.Controls.Add(this.CompositeSolver);
            this.Controls.Add(this.ReturnSolver);
            this.Controls.Add(this.LargestGapSolver);
            this.Controls.Add(this.SShapeSolver);
            this.Controls.Add(this.distanceLastSolution);
            this.Controls.Add(this.distanceLastSolutionLabel);
            this.Controls.Add(this.DummySolver);
            this.Controls.Add(this.generate);
            this.Name = "MainGui";
            this.Text = "MainGui";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button generate;
        private Button DummySolver;
        private Label distanceLastSolutionLabel;
        private TextBox distanceLastSolution;
        private Button SShapeSolver;
        private Button ReturnSolver;
        private Button LargestGapSolver;
        private Button CompositeSolver;
        private Button clear;
    }
}

