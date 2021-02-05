
namespace GUITest
{
    partial class FormMain
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            Process?.Dispose();
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.buttonFps10 = new System.Windows.Forms.Button();
            this.buttonFps60 = new System.Windows.Forms.Button();
            this.buttonFps144 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.buttonFps144);
            this.splitContainer1.Panel1.Controls.Add(this.buttonFps60);
            this.splitContainer1.Panel1.Controls.Add(this.buttonFps10);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.ClientSizeChanged += new System.EventHandler(this.splitContainer1_Panel2_ClientSizeChanged);
            this.splitContainer1.Size = new System.Drawing.Size(800, 450);
            this.splitContainer1.SplitterDistance = 266;
            this.splitContainer1.TabIndex = 0;
            // 
            // buttonFps10
            // 
            this.buttonFps10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFps10.Location = new System.Drawing.Point(12, 12);
            this.buttonFps10.Name = "buttonFps10";
            this.buttonFps10.Size = new System.Drawing.Size(251, 63);
            this.buttonFps10.TabIndex = 0;
            this.buttonFps10.Text = "10";
            this.buttonFps10.UseVisualStyleBackColor = true;
            this.buttonFps10.Click += new System.EventHandler(this.buttonFps10_Click);
            // 
            // buttonFps60
            // 
            this.buttonFps60.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFps60.Location = new System.Drawing.Point(12, 81);
            this.buttonFps60.Name = "buttonFps60";
            this.buttonFps60.Size = new System.Drawing.Size(251, 63);
            this.buttonFps60.TabIndex = 0;
            this.buttonFps60.Text = "60";
            this.buttonFps60.UseVisualStyleBackColor = true;
            this.buttonFps60.Click += new System.EventHandler(this.buttonFps60_Click);
            // 
            // buttonFps144
            // 
            this.buttonFps144.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFps144.Location = new System.Drawing.Point(12, 150);
            this.buttonFps144.Name = "buttonFps144";
            this.buttonFps144.Size = new System.Drawing.Size(251, 63);
            this.buttonFps144.TabIndex = 0;
            this.buttonFps144.Text = "144";
            this.buttonFps144.UseVisualStyleBackColor = true;
            this.buttonFps144.Click += new System.EventHandler(this.buttonFps144_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonFps10;
        private System.Windows.Forms.Button buttonFps60;
        private System.Windows.Forms.Button buttonFps144;
    }
}

