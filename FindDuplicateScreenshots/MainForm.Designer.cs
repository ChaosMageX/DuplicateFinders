namespace FindDuplicateScreenshots
{
    partial class MainForm
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
            this.mStartBTN = new System.Windows.Forms.Button();
            this.mBrowseBTN = new System.Windows.Forms.Button();
            this.mDirectoryTXT = new System.Windows.Forms.TextBox();
            this.mMainProgressBAR = new System.Windows.Forms.ProgressBar();
            this.mOutputTXT = new System.Windows.Forms.TextBox();
            this.mUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.mMainProgressTXT = new System.Windows.Forms.TextBox();
            this.mThreadCountNUD = new System.Windows.Forms.NumericUpDown();
            this.mThreadCountLBL = new System.Windows.Forms.Label();
            this.mGroupByCMB = new System.Windows.Forms.ComboBox();
            this.mGroupByLBL = new System.Windows.Forms.Label();
            this.mPauseBTN = new System.Windows.Forms.Button();
            this.mStopBTN = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.mThreadCountNUD)).BeginInit();
            this.SuspendLayout();
            // 
            // mStartBTN
            // 
            this.mStartBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mStartBTN.Location = new System.Drawing.Point(697, 12);
            this.mStartBTN.Name = "mStartBTN";
            this.mStartBTN.Size = new System.Drawing.Size(75, 23);
            this.mStartBTN.TabIndex = 0;
            this.mStartBTN.Text = "Start";
            this.mStartBTN.UseVisualStyleBackColor = true;
            this.mStartBTN.Click += new System.EventHandler(this.StartButtonClicked);
            // 
            // mBrowseBTN
            // 
            this.mBrowseBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mBrowseBTN.Location = new System.Drawing.Point(616, 12);
            this.mBrowseBTN.Name = "mBrowseBTN";
            this.mBrowseBTN.Size = new System.Drawing.Size(75, 23);
            this.mBrowseBTN.TabIndex = 1;
            this.mBrowseBTN.Text = "Browse";
            this.mBrowseBTN.UseVisualStyleBackColor = true;
            this.mBrowseBTN.Click += new System.EventHandler(this.BrowseButtonClicked);
            // 
            // mDirectoryTXT
            // 
            this.mDirectoryTXT.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mDirectoryTXT.Enabled = false;
            this.mDirectoryTXT.Location = new System.Drawing.Point(12, 14);
            this.mDirectoryTXT.Name = "mDirectoryTXT";
            this.mDirectoryTXT.Size = new System.Drawing.Size(598, 20);
            this.mDirectoryTXT.TabIndex = 2;
            // 
            // mMainProgressBAR
            // 
            this.mMainProgressBAR.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mMainProgressBAR.Location = new System.Drawing.Point(12, 300);
            this.mMainProgressBAR.Name = "mMainProgressBAR";
            this.mMainProgressBAR.Size = new System.Drawing.Size(760, 23);
            this.mMainProgressBAR.TabIndex = 3;
            // 
            // mOutputTXT
            // 
            this.mOutputTXT.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mOutputTXT.Location = new System.Drawing.Point(12, 67);
            this.mOutputTXT.Multiline = true;
            this.mOutputTXT.Name = "mOutputTXT";
            this.mOutputTXT.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.mOutputTXT.Size = new System.Drawing.Size(760, 227);
            this.mOutputTXT.TabIndex = 4;
            // 
            // mUpdateTimer
            // 
            this.mUpdateTimer.Tick += new System.EventHandler(this.UpdateTimerTick);
            // 
            // mMainProgressTXT
            // 
            this.mMainProgressTXT.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mMainProgressTXT.Enabled = false;
            this.mMainProgressTXT.Location = new System.Drawing.Point(12, 329);
            this.mMainProgressTXT.Name = "mMainProgressTXT";
            this.mMainProgressTXT.Size = new System.Drawing.Size(760, 20);
            this.mMainProgressTXT.TabIndex = 5;
            // 
            // mThreadCountNUD
            // 
            this.mThreadCountNUD.Location = new System.Drawing.Point(312, 41);
            this.mThreadCountNUD.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.mThreadCountNUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mThreadCountNUD.Name = "mThreadCountNUD";
            this.mThreadCountNUD.Size = new System.Drawing.Size(44, 20);
            this.mThreadCountNUD.TabIndex = 19;
            this.mThreadCountNUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mThreadCountNUD.ValueChanged += new System.EventHandler(this.ThreadCountValueChanged);
            // 
            // mThreadCountLBL
            // 
            this.mThreadCountLBL.AutoSize = true;
            this.mThreadCountLBL.Location = new System.Drawing.Point(199, 43);
            this.mThreadCountLBL.Name = "mThreadCountLBL";
            this.mThreadCountLBL.Size = new System.Drawing.Size(107, 13);
            this.mThreadCountLBL.TabIndex = 18;
            this.mThreadCountLBL.Text = "Image Thread Count:";
            // 
            // mGroupByCMB
            // 
            this.mGroupByCMB.FormattingEnabled = true;
            this.mGroupByCMB.Items.AddRange(new object[] {
            "All",
            "Codec",
            "Extension"});
            this.mGroupByCMB.Location = new System.Drawing.Point(72, 40);
            this.mGroupByCMB.Name = "mGroupByCMB";
            this.mGroupByCMB.Size = new System.Drawing.Size(121, 21);
            this.mGroupByCMB.TabIndex = 17;
            this.mGroupByCMB.SelectedIndexChanged += new System.EventHandler(this.GroupByIndexChanged);
            // 
            // mGroupByLBL
            // 
            this.mGroupByLBL.AutoSize = true;
            this.mGroupByLBL.Location = new System.Drawing.Point(12, 43);
            this.mGroupByLBL.Name = "mGroupByLBL";
            this.mGroupByLBL.Size = new System.Drawing.Size(54, 13);
            this.mGroupByLBL.TabIndex = 16;
            this.mGroupByLBL.Text = "Group By:";
            // 
            // mPauseBTN
            // 
            this.mPauseBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mPauseBTN.Enabled = false;
            this.mPauseBTN.Location = new System.Drawing.Point(616, 38);
            this.mPauseBTN.Name = "mPauseBTN";
            this.mPauseBTN.Size = new System.Drawing.Size(75, 23);
            this.mPauseBTN.TabIndex = 20;
            this.mPauseBTN.Text = "Pause";
            this.mPauseBTN.UseVisualStyleBackColor = true;
            this.mPauseBTN.Click += new System.EventHandler(this.PauseButtonClicked);
            // 
            // mStopBTN
            // 
            this.mStopBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mStopBTN.Enabled = false;
            this.mStopBTN.Location = new System.Drawing.Point(697, 38);
            this.mStopBTN.Name = "mStopBTN";
            this.mStopBTN.Size = new System.Drawing.Size(75, 23);
            this.mStopBTN.TabIndex = 21;
            this.mStopBTN.Text = "Stop";
            this.mStopBTN.UseVisualStyleBackColor = true;
            this.mStopBTN.Click += new System.EventHandler(this.StopButtonClicked);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 361);
            this.Controls.Add(this.mStopBTN);
            this.Controls.Add(this.mPauseBTN);
            this.Controls.Add(this.mThreadCountNUD);
            this.Controls.Add(this.mThreadCountLBL);
            this.Controls.Add(this.mGroupByCMB);
            this.Controls.Add(this.mGroupByLBL);
            this.Controls.Add(this.mMainProgressTXT);
            this.Controls.Add(this.mOutputTXT);
            this.Controls.Add(this.mMainProgressBAR);
            this.Controls.Add(this.mDirectoryTXT);
            this.Controls.Add(this.mBrowseBTN);
            this.Controls.Add(this.mStartBTN);
            this.MinimumSize = new System.Drawing.Size(400, 200);
            this.Name = "MainForm";
            this.Text = "Find Duplicate Screenshots";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.mThreadCountNUD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mStartBTN;
        private System.Windows.Forms.Button mBrowseBTN;
        private System.Windows.Forms.TextBox mDirectoryTXT;
        private System.Windows.Forms.ProgressBar mMainProgressBAR;
        private System.Windows.Forms.TextBox mOutputTXT;
        private System.Windows.Forms.Timer mUpdateTimer;
        private System.Windows.Forms.TextBox mMainProgressTXT;
        private System.Windows.Forms.NumericUpDown mThreadCountNUD;
        private System.Windows.Forms.Label mThreadCountLBL;
        private System.Windows.Forms.ComboBox mGroupByCMB;
        private System.Windows.Forms.Label mGroupByLBL;
        private System.Windows.Forms.Button mPauseBTN;
        private System.Windows.Forms.Button mStopBTN;
    }
}

