namespace Astar
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnStartPoint = new System.Windows.Forms.Button();
            this.btnEndPoint = new System.Windows.Forms.Button();
            this.btnNavgate = new System.Windows.Forms.Button();
            this.btnClearPath = new System.Windows.Forms.Button();
            this.btnCreateObs = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(14, 16);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(440, 394);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            // 
            // btnStartPoint
            // 
            this.btnStartPoint.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnStartPoint.Location = new System.Drawing.Point(494, 28);
            this.btnStartPoint.Name = "btnStartPoint";
            this.btnStartPoint.Size = new System.Drawing.Size(106, 37);
            this.btnStartPoint.TabIndex = 1;
            this.btnStartPoint.Text = "设置起点";
            this.btnStartPoint.UseVisualStyleBackColor = true;
            this.btnStartPoint.Click += new System.EventHandler(this.btnStartPoint_Click);
            // 
            // btnEndPoint
            // 
            this.btnEndPoint.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnEndPoint.Location = new System.Drawing.Point(494, 101);
            this.btnEndPoint.Name = "btnEndPoint";
            this.btnEndPoint.Size = new System.Drawing.Size(106, 37);
            this.btnEndPoint.TabIndex = 2;
            this.btnEndPoint.Text = "设置终点";
            this.btnEndPoint.UseVisualStyleBackColor = true;
            this.btnEndPoint.Click += new System.EventHandler(this.btnEndPoint_Click);
            // 
            // btnNavgate
            // 
            this.btnNavgate.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnNavgate.Location = new System.Drawing.Point(494, 258);
            this.btnNavgate.Name = "btnNavgate";
            this.btnNavgate.Size = new System.Drawing.Size(106, 37);
            this.btnNavgate.TabIndex = 3;
            this.btnNavgate.Text = "开始规划";
            this.btnNavgate.UseVisualStyleBackColor = true;
            this.btnNavgate.Click += new System.EventHandler(this.btnNavgate_Click);
            // 
            // btnClearPath
            // 
            this.btnClearPath.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnClearPath.AutoSize = true;
            this.btnClearPath.Location = new System.Drawing.Point(494, 338);
            this.btnClearPath.Name = "btnClearPath";
            this.btnClearPath.Size = new System.Drawing.Size(106, 37);
            this.btnClearPath.TabIndex = 4;
            this.btnClearPath.Text = "清除轨迹";
            this.btnClearPath.UseVisualStyleBackColor = true;
            this.btnClearPath.Click += new System.EventHandler(this.btnClearPath_Click);
            // 
            // btnCreateObs
            // 
            this.btnCreateObs.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnCreateObs.Location = new System.Drawing.Point(494, 177);
            this.btnCreateObs.Name = "btnCreateObs";
            this.btnCreateObs.Size = new System.Drawing.Size(106, 37);
            this.btnCreateObs.TabIndex = 5;
            this.btnCreateObs.Text = "生成障碍物";
            this.btnCreateObs.UseVisualStyleBackColor = true;
            this.btnCreateObs.Click += new System.EventHandler(this.btnCreateObs_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 419);
            this.Controls.Add(this.btnCreateObs);
            this.Controls.Add(this.btnClearPath);
            this.Controls.Add(this.btnNavgate);
            this.Controls.Add(this.btnEndPoint);
            this.Controls.Add(this.btnStartPoint);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnStartPoint;
        private System.Windows.Forms.Button btnEndPoint;
        private System.Windows.Forms.Button btnNavgate;
        private System.Windows.Forms.Button btnClearPath;
        private System.Windows.Forms.Button btnCreateObs;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

