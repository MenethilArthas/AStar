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
            this.btnImportMap = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.rplidar = new System.IO.Ports.SerialPort(this.components);
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.cbSerialName = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Gray;
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
            this.btnStartPoint.Enabled = false;
            this.btnStartPoint.Location = new System.Drawing.Point(536, 14);
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
            this.btnEndPoint.Location = new System.Drawing.Point(536, 87);
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
            this.btnNavgate.Location = new System.Drawing.Point(536, 306);
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
            this.btnClearPath.Location = new System.Drawing.Point(534, 379);
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
            this.btnCreateObs.Location = new System.Drawing.Point(536, 233);
            this.btnCreateObs.Name = "btnCreateObs";
            this.btnCreateObs.Size = new System.Drawing.Size(106, 37);
            this.btnCreateObs.TabIndex = 5;
            this.btnCreateObs.Text = "生成障碍物";
            this.btnCreateObs.UseVisualStyleBackColor = true;
            this.btnCreateObs.Click += new System.EventHandler(this.btnCreateObs_Click);
            // 
            // btnImportMap
            // 
            this.btnImportMap.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnImportMap.Location = new System.Drawing.Point(536, 160);
            this.btnImportMap.Name = "btnImportMap";
            this.btnImportMap.Size = new System.Drawing.Size(106, 37);
            this.btnImportMap.TabIndex = 6;
            this.btnImportMap.Text = "导入地图";
            this.btnImportMap.UseVisualStyleBackColor = true;
            this.btnImportMap.Click += new System.EventHandler(this.btnImportMap_Click);
            // 
            // btnTest
            // 
            this.btnTest.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnTest.Location = new System.Drawing.Point(536, 441);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(106, 37);
            this.btnTest.TabIndex = 7;
            this.btnTest.Text = "test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // rplidar
            // 
            this.rplidar.BaudRate = 115200;
            this.rplidar.PortName = "COM3";
            // 
            // serialPort
            // 
            this.serialPort.BaudRate = 115200;
            // 
            // cbSerialName
            // 
            this.cbSerialName.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cbSerialName.FormattingEnabled = true;
            this.cbSerialName.Location = new System.Drawing.Point(167, 442);
            this.cbSerialName.Name = "cbSerialName";
            this.cbSerialName.Size = new System.Drawing.Size(96, 23);
            this.cbSerialName.TabIndex = 8;
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnConnect.Location = new System.Drawing.Point(307, 436);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(82, 32);
            this.btnConnect.TabIndex = 9;
            this.btnConnect.Text = "连接串口";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 509);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.cbSerialName);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnImportMap);
            this.Controls.Add(this.btnCreateObs);
            this.Controls.Add(this.btnClearPath);
            this.Controls.Add(this.btnNavgate);
            this.Controls.Add(this.btnEndPoint);
            this.Controls.Add(this.btnStartPoint);
            this.Controls.Add(this.pictureBox1);
            this.Location = new System.Drawing.Point(-20, -20);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
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
        private System.Windows.Forms.Button btnImportMap;
        private System.Windows.Forms.Button btnTest;
        private System.IO.Ports.SerialPort rplidar;
        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.ComboBox cbSerialName;
        private System.Windows.Forms.Button btnConnect;
    }
}

