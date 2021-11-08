namespace ScreenshotFinder
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
            this.mMainProgressTXT = new System.Windows.Forms.TextBox();
            this.mOutputTXT = new System.Windows.Forms.TextBox();
            this.mMainProgressBAR = new System.Windows.Forms.ProgressBar();
            this.mDirectoryTXT = new System.Windows.Forms.TextBox();
            this.mBrowseBTN = new System.Windows.Forms.Button();
            this.mStartBTN = new System.Windows.Forms.Button();
            this.mUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.mGroupByLBL = new System.Windows.Forms.Label();
            this.mGroupByCMB = new System.Windows.Forms.ComboBox();
            this.mThreadCountLBL = new System.Windows.Forms.Label();
            this.mThreadCountNUD = new System.Windows.Forms.NumericUpDown();
            this.mOutputSplitCont = new System.Windows.Forms.SplitContainer();
            this.mFolderProgressTXT = new System.Windows.Forms.TextBox();
            this.mPauseBTN = new System.Windows.Forms.Button();
            this.mStopBTN = new System.Windows.Forms.Button();
            this.mThreadPriorityCMB = new System.Windows.Forms.ComboBox();
            this.mThreadPriorityLBL = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mThreadCountNUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mOutputSplitCont)).BeginInit();
            this.mOutputSplitCont.Panel1.SuspendLayout();
            this.mOutputSplitCont.Panel2.SuspendLayout();
            this.mOutputSplitCont.SuspendLayout();
            this.SuspendLayout();
            // 
            // mMainProgressTXT
            // 
            this.mMainProgressTXT.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mMainProgressTXT.Enabled = false;
            this.mMainProgressTXT.Location = new System.Drawing.Point(12, 529);
            this.mMainProgressTXT.Name = "mMainProgressTXT";
            this.mMainProgressTXT.Size = new System.Drawing.Size(760, 20);
            this.mMainProgressTXT.TabIndex = 11;
            // 
            // mOutputTXT
            // 
            this.mOutputTXT.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mOutputTXT.Location = new System.Drawing.Point(4, 3);
            this.mOutputTXT.Multiline = true;
            this.mOutputTXT.Name = "mOutputTXT";
            this.mOutputTXT.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.mOutputTXT.Size = new System.Drawing.Size(753, 208);
            this.mOutputTXT.TabIndex = 10;
            // 
            // mMainProgressBAR
            // 
            this.mMainProgressBAR.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mMainProgressBAR.Location = new System.Drawing.Point(12, 500);
            this.mMainProgressBAR.Name = "mMainProgressBAR";
            this.mMainProgressBAR.Size = new System.Drawing.Size(760, 23);
            this.mMainProgressBAR.TabIndex = 9;
            // 
            // mDirectoryTXT
            // 
            this.mDirectoryTXT.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mDirectoryTXT.Enabled = false;
            this.mDirectoryTXT.Location = new System.Drawing.Point(12, 14);
            this.mDirectoryTXT.Name = "mDirectoryTXT";
            this.mDirectoryTXT.Size = new System.Drawing.Size(598, 20);
            this.mDirectoryTXT.TabIndex = 8;
            // 
            // mBrowseBTN
            // 
            this.mBrowseBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mBrowseBTN.Location = new System.Drawing.Point(616, 12);
            this.mBrowseBTN.Name = "mBrowseBTN";
            this.mBrowseBTN.Size = new System.Drawing.Size(75, 23);
            this.mBrowseBTN.TabIndex = 7;
            this.mBrowseBTN.Text = "Browse";
            this.mBrowseBTN.UseVisualStyleBackColor = true;
            this.mBrowseBTN.Click += new System.EventHandler(this.BrowseButtonClicked);
            // 
            // mStartBTN
            // 
            this.mStartBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mStartBTN.Location = new System.Drawing.Point(697, 12);
            this.mStartBTN.Name = "mStartBTN";
            this.mStartBTN.Size = new System.Drawing.Size(75, 23);
            this.mStartBTN.TabIndex = 6;
            this.mStartBTN.Text = "Start";
            this.mStartBTN.UseVisualStyleBackColor = true;
            this.mStartBTN.Click += new System.EventHandler(this.StartButtonClicked);
            // 
            // mUpdateTimer
            // 
            this.mUpdateTimer.Tick += new System.EventHandler(this.UpdateTimerTick);
            // 
            // mGroupByLBL
            // 
            this.mGroupByLBL.AutoSize = true;
            this.mGroupByLBL.Location = new System.Drawing.Point(13, 44);
            this.mGroupByLBL.Name = "mGroupByLBL";
            this.mGroupByLBL.Size = new System.Drawing.Size(54, 13);
            this.mGroupByLBL.TabIndex = 12;
            this.mGroupByLBL.Text = "Group By:";
            // 
            // mGroupByCMB
            // 
            this.mGroupByCMB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mGroupByCMB.FormattingEnabled = true;
            this.mGroupByCMB.Items.AddRange(new object[] {
            "All",
            "Codec",
            "Extension"});
            this.mGroupByCMB.Location = new System.Drawing.Point(73, 40);
            this.mGroupByCMB.Name = "mGroupByCMB";
            this.mGroupByCMB.Size = new System.Drawing.Size(121, 21);
            this.mGroupByCMB.TabIndex = 13;
            this.mGroupByCMB.SelectedIndexChanged += new System.EventHandler(this.GroupByIndexChanged);
            // 
            // mThreadCountLBL
            // 
            this.mThreadCountLBL.AutoSize = true;
            this.mThreadCountLBL.Location = new System.Drawing.Point(200, 43);
            this.mThreadCountLBL.Name = "mThreadCountLBL";
            this.mThreadCountLBL.Size = new System.Drawing.Size(107, 13);
            this.mThreadCountLBL.TabIndex = 14;
            this.mThreadCountLBL.Text = "Image Thread Count:";
            // 
            // mThreadCountNUD
            // 
            this.mThreadCountNUD.Location = new System.Drawing.Point(313, 40);
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
            this.mThreadCountNUD.TabIndex = 15;
            this.mThreadCountNUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mThreadCountNUD.ValueChanged += new System.EventHandler(this.ThreadCountValueChanged);
            // 
            // mOutputSplitCont
            // 
            this.mOutputSplitCont.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mOutputSplitCont.Location = new System.Drawing.Point(12, 65);
            this.mOutputSplitCont.Name = "mOutputSplitCont";
            this.mOutputSplitCont.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mOutputSplitCont.Panel1
            // 
            this.mOutputSplitCont.Panel1.Controls.Add(this.mOutputTXT);
            // 
            // mOutputSplitCont.Panel2
            // 
            this.mOutputSplitCont.Panel2.Controls.Add(this.mFolderProgressTXT);
            this.mOutputSplitCont.Size = new System.Drawing.Size(760, 429);
            this.mOutputSplitCont.SplitterDistance = 214;
            this.mOutputSplitCont.TabIndex = 16;
            // 
            // mFolderProgressTXT
            // 
            this.mFolderProgressTXT.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mFolderProgressTXT.Enabled = false;
            this.mFolderProgressTXT.Location = new System.Drawing.Point(3, 3);
            this.mFolderProgressTXT.Multiline = true;
            this.mFolderProgressTXT.Name = "mFolderProgressTXT";
            this.mFolderProgressTXT.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.mFolderProgressTXT.Size = new System.Drawing.Size(754, 205);
            this.mFolderProgressTXT.TabIndex = 0;
            // 
            // mPauseBTN
            // 
            this.mPauseBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mPauseBTN.Enabled = false;
            this.mPauseBTN.Location = new System.Drawing.Point(616, 39);
            this.mPauseBTN.Name = "mPauseBTN";
            this.mPauseBTN.Size = new System.Drawing.Size(75, 23);
            this.mPauseBTN.TabIndex = 17;
            this.mPauseBTN.Text = "Pause";
            this.mPauseBTN.UseVisualStyleBackColor = true;
            this.mPauseBTN.Click += new System.EventHandler(this.PauseButtonClicked);
            // 
            // mStopBTN
            // 
            this.mStopBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mStopBTN.Enabled = false;
            this.mStopBTN.Location = new System.Drawing.Point(697, 39);
            this.mStopBTN.Name = "mStopBTN";
            this.mStopBTN.Size = new System.Drawing.Size(75, 23);
            this.mStopBTN.TabIndex = 18;
            this.mStopBTN.Text = "Stop";
            this.mStopBTN.UseVisualStyleBackColor = true;
            this.mStopBTN.Click += new System.EventHandler(this.StopButtonClicked);
            // 
            // mThreadPriorityCMB
            // 
            this.mThreadPriorityCMB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mThreadPriorityCMB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mThreadPriorityCMB.FormattingEnabled = true;
            this.mThreadPriorityCMB.Items.AddRange(new object[] {
            "Lower",
            "Normal",
            "Higher"});
            this.mThreadPriorityCMB.Location = new System.Drawing.Point(489, 40);
            this.mThreadPriorityCMB.Name = "mThreadPriorityCMB";
            this.mThreadPriorityCMB.Size = new System.Drawing.Size(121, 21);
            this.mThreadPriorityCMB.TabIndex = 19;
            this.mThreadPriorityCMB.SelectedIndexChanged += new System.EventHandler(this.ThreadPriorityIndexChanged);
            // 
            // mThreadPriorityLBL
            // 
            this.mThreadPriorityLBL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mThreadPriorityLBL.AutoSize = true;
            this.mThreadPriorityLBL.Location = new System.Drawing.Point(405, 44);
            this.mThreadPriorityLBL.Name = "mThreadPriorityLBL";
            this.mThreadPriorityLBL.Size = new System.Drawing.Size(78, 13);
            this.mThreadPriorityLBL.TabIndex = 21;
            this.mThreadPriorityLBL.Text = "Thread Priority:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.mThreadPriorityLBL);
            this.Controls.Add(this.mThreadPriorityCMB);
            this.Controls.Add(this.mStopBTN);
            this.Controls.Add(this.mPauseBTN);
            this.Controls.Add(this.mOutputSplitCont);
            this.Controls.Add(this.mThreadCountNUD);
            this.Controls.Add(this.mThreadCountLBL);
            this.Controls.Add(this.mGroupByCMB);
            this.Controls.Add(this.mGroupByLBL);
            this.Controls.Add(this.mMainProgressTXT);
            this.Controls.Add(this.mMainProgressBAR);
            this.Controls.Add(this.mDirectoryTXT);
            this.Controls.Add(this.mBrowseBTN);
            this.Controls.Add(this.mStartBTN);
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "MainForm";
            this.Text = "Duplicate Screenshot Finder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.mThreadCountNUD)).EndInit();
            this.mOutputSplitCont.Panel1.ResumeLayout(false);
            this.mOutputSplitCont.Panel1.PerformLayout();
            this.mOutputSplitCont.Panel2.ResumeLayout(false);
            this.mOutputSplitCont.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mOutputSplitCont)).EndInit();
            this.mOutputSplitCont.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mMainProgressTXT;
        private System.Windows.Forms.TextBox mOutputTXT;
        private System.Windows.Forms.ProgressBar mMainProgressBAR;
        private System.Windows.Forms.TextBox mDirectoryTXT;
        private System.Windows.Forms.Button mBrowseBTN;
        private System.Windows.Forms.Button mStartBTN;
        private System.Windows.Forms.Timer mUpdateTimer;
        private System.Windows.Forms.Label mGroupByLBL;
        private System.Windows.Forms.ComboBox mGroupByCMB;
        private System.Windows.Forms.Label mThreadCountLBL;
        private System.Windows.Forms.NumericUpDown mThreadCountNUD;
        private System.Windows.Forms.SplitContainer mOutputSplitCont;
        private System.Windows.Forms.TextBox mFolderProgressTXT;
        private System.Windows.Forms.Button mPauseBTN;
        private System.Windows.Forms.Button mStopBTN;
        private System.Windows.Forms.ComboBox mThreadPriorityCMB;
        private System.Windows.Forms.Label mThreadPriorityLBL;
    }
}

