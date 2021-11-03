using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ScreenshotFinder
{
    public partial class MainForm : Form
    {
        private StringBuilder mOutputBuilder;
        private StringAndFileWriter mOutputWriter;
        private int mOutputVersion;

        private Thread mSearchThread;
        private bool bSearchRunning;
        private bool bSearchPaused;

        private DirectoryInfo mRootDir;
        private Separator.GroupImgBy mGroupBy;
        private int mImageThreadCount;

        private Separator[] mSeparators;
        private int mTotalImageCount;
        private bool bRefreshProgressBar;
        private DateTime mSearchStartTime;
        private DateTime mLastTickTime;
        private int mLastTickProgress;
        private long mAvgCompTicks;

        public MainForm()
        {
            InitializeComponent();

            string logFilePath = "DupFinder_"
                + DateTime.Now.ToString(Separator.FileNameTimeFormat)
                + "_Log.txt";
            logFilePath = Path.Combine(Environment.CurrentDirectory, logFilePath);

            mOutputBuilder = new StringBuilder();
            mOutputWriter = new StringAndFileWriter(mOutputBuilder, logFilePath);
            mOutputVersion = mOutputWriter.GetVersion();
            Console.SetOut(mOutputWriter);

            mSearchThread = new Thread(StartSearch)
            {
                Priority = Separator.DefaultPriority
            };
            bSearchRunning = false;

            mRootDir = new DirectoryInfo(Environment.CurrentDirectory);
            UpdateDirectoryDisplay();

            mGroupBy = Separator.GroupImgBy.Codec;
            mGroupByCMB.SelectedIndex = (int)mGroupBy;

            mImageThreadCount = 1;
            mThreadCountNUD.Value = mImageThreadCount;

            bRefreshProgressBar = false;

            mUpdateTimer.Enabled = true;
        }

        private void UpdateDirectoryDisplay()
        {
            mDirectoryTXT.Text = mRootDir.FullName;

            if (mRootDir.Name.EndsWith(Separator.DupDirSuffix))
            {
                mStartBTN.Text = "Start ⚠️";
            }
            else
            {
                mStartBTN.Text = "Start 🆗";
            }
        }

        private void BrowseButtonClicked(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a Directory to scan for duplicate "
                    + "images from sequential screenshots taken automatically.";
                dialog.ShowNewFolderButton = false;
                dialog.SelectedPath = mRootDir.FullName;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    mRootDir = new DirectoryInfo(dialog.SelectedPath);
                    UpdateDirectoryDisplay();
                }
            }
        }

        private void StartButtonClicked(object sender, EventArgs e)
        {
            mSearchThread.Start();
        }

        private void PauseButtonClicked(object sender, EventArgs e)
        {
            int count = mSeparators == null ? 0 : mSeparators.Length;
            if (count > 0 && bSearchRunning)
            {
                bSearchPaused = !bSearchPaused;
                for (int i = 0; i < count; i++)
                {
                    mSeparators[i].Paused = bSearchPaused;
                }
                mPauseButton.Text = bSearchPaused ? "Play" : "Pause";
            }
        }

        private void StopButtonClicked(object sender, EventArgs e)
        {
            StopSearching();
        }

        private void GroupByIndexChanged(object sender, EventArgs e)
        {
            int index = mGroupByCMB.SelectedIndex;
            if (index < 0 || index > 2)
            {
                mGroupBy = Separator.GroupImgBy.Codec;
            }
            else
            {
                mGroupBy = (Separator.GroupImgBy)index;
            }
        }

        private void ThreadCountValueChanged(object sender, EventArgs e)
        {
            mImageThreadCount = (int)mThreadCountNUD.Value;
        }

        private void MainFormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void MainFormClosed(object sender, FormClosedEventArgs e)
        {
            StopSearching();
        }

        private void UpdateTimerTick(object sender, EventArgs e)
        {
            // Update Output Text Box
            int version = mOutputWriter.GetVersion();
            if (version != mOutputVersion)
            {
                try
                {
                    mOutputTXT.Text = mOutputBuilder.ToString();
                    mOutputTXT.Select(mOutputTXT.TextLength, 0);
                    mOutputTXT.ScrollToCaret();
                    mOutputVersion = version;
                }
                catch (Exception)
                {
                    // StringBuilder.ToString() sometimes throws an
                    // ArgumentOutOfRangeException for some reason.
                    // Maybe it has to do with the multi-threading.
                }
            }
            // Update User Control Elements
            {
                bool enabled = !bSearchRunning;
                mStartBTN.Enabled = enabled;
                mBrowseBTN.Enabled = enabled;
                mGroupByCMB.Enabled = enabled;
                mThreadCountNUD.Enabled = enabled;
                mPauseButton.Enabled = !enabled;
                mStopButton.Enabled = !enabled;
            }
            // Update Progress Bar and Message
            if (bRefreshProgressBar)
            {
                mMainProgressBAR.Value = 0;
                mMainProgressBAR.Maximum = mTotalImageCount;
                bRefreshProgressBar = false;
            }
            if (bSearchPaused)
            {
                mLastTickTime = DateTime.Now;
            }
            else if (bSearchRunning)
            {
                StringBuilder sb = new StringBuilder();
                DateTime dt = DateTime.Now;
                TimeSpan ts;
                Separator sep;
                long avgCompTicks = 0L;
                int avgSearchIndex = 0;
                int avgSearchCount = 0;
                int avgDuplicates = 0;
                long avgDupBytes = 0L;
                double percent = 0.0;
                int runningCount = 0;
                int currentProgress = 0;
                int dupCount = 0;
                long dupBytes = 0L;

                int count = mSeparators == null ? 0 : mSeparators.Length;
                for (int i = 0; i < count; i++)
                {
                    sep = mSeparators[i];
                    if (sep.IsSeparating)
                    {
                        sep.AppendDisplayString(sb, false, true);
                        sb.AppendLine();
                        avgCompTicks += sep.AverageCompareTime.Ticks;
                        avgSearchIndex += sep.SearchIndex;
                        avgSearchCount += sep.SearchCount;
                        avgDuplicates += sep.DuplicateCount;
                        avgDupBytes += sep.DuplicateBytes;
                        percent += sep.SearchFraction;
                        runningCount++;
                    }
                    currentProgress += sep.SearchIndex;
                    dupCount += sep.DuplicateCount;
                    dupBytes += sep.DuplicateBytes;
                }
                if (runningCount == 0)
                    runningCount = 1;
                avgCompTicks /= runningCount;
                avgSearchIndex /= runningCount;
                avgSearchCount /= runningCount;
                avgDuplicates /= runningCount;
                avgDupBytes /= runningCount;
                percent /= runningCount;

                sb.Append("AVERAGE : ALL : ");
                sb.Append(percent.ToString("P"));
                sb.Append(" ( ");
                sb.Append(avgSearchIndex.ToString("N0"));
                sb.Append(" / ");
                sb.Append(avgSearchCount.ToString("N0"));
                sb.Append(" )");
                sb.Append(" | Dups: ");
                sb.Append(avgDuplicates.ToString("N0"));
                sb.Append(" ( ");
                sb.Append(Separator.GetSizeString(avgDupBytes));
                sb.Append(" ) ( ");
                percent = avgSearchIndex == 0 
                    ? 0.0 : (double)avgDuplicates / avgSearchIndex;
                sb.Append(percent.ToString("P"));
                sb.Append(" / ");
                percent = avgSearchCount == 0 
                    ? 0.0 : (double)avgDuplicates / avgSearchCount;
                sb.Append(percent.ToString("P"));
                sb.Append(" ) ");
                sb.Append(" | Avg: ");
                ts = new TimeSpan(avgCompTicks);
                sb.Append(ts.ToString(Separator.AverageTimeFormat));
                sb.Append(" s / Img");

                ts = mLastTickTime - dt;
                mLastTickTime = dt;
                mAvgCompTicks = mAvgCompTicks * mLastTickProgress + ts.Ticks;
                mAvgCompTicks /= currentProgress == 0 ? 1 : currentProgress;
                mLastTickProgress = currentProgress;

                mFolderProgressTXT.Text = sb.ToString();
                mFolderProgressTXT.Select(sb.Length, 0);

                mMainProgressBAR.Value = currentProgress;

                sb.Clear();
                sb.Append("Overall : ");
                percent = (double)currentProgress / mTotalImageCount;
                sb.Append(percent.ToString("P"));
                sb.Append(" ( ");
                sb.Append(currentProgress.ToString("N0"));
                sb.Append(" / ");
                sb.Append(mTotalImageCount.ToString("N0"));
                sb.Append(" )");
                sb.Append(" | Dups: ");
                sb.Append(dupCount.ToString("N0"));
                sb.Append(" ( ");
                sb.Append(Separator.GetSizeString(dupBytes));
                sb.Append(" ) ( ");
                percent = currentProgress == 0 
                    ? 0.0 : (double)dupCount / currentProgress;
                sb.Append(percent.ToString("P"));
                sb.Append(" / ");
                percent = mTotalImageCount == 0 
                    ? 0.0 : (double)dupCount / mTotalImageCount;
                sb.Append(percent.ToString("P"));
                sb.Append(" ) ");
                sb.Append(" | Elapsed: ");
                ts = dt - mSearchStartTime;
                sb.Append(ts.ToString(Separator.ElapsedTimeFormat));
                sb.Append(" | Remaining: ");
                int diff = mTotalImageCount - currentProgress;
                ts = new TimeSpan(diff * mAvgCompTicks);
                sb.Append(ts.ToString(Separator.ElapsedTimeFormat));
                sb.Append(" | Avg: ");
                ts = new TimeSpan(mAvgCompTicks);
                sb.Append(ts.ToString(Separator.AverageTimeFormat));
                sb.Append(" s / Img");

                mMainProgressTXT.Text = sb.ToString();
            }
        }

        private void StopSearching()
        {
            // Stop creating any new Separator instances.
            Separator.CreationAllowed = false;
            // Stop any Separator instances still running.
            if (mSeparators != null && mSeparators.Length > 0)
            {
                for (int i = mSeparators.Length - 1; i >= 0; i--)
                {
                    mSeparators[i].StopSeparating();
                }
            }
        }

        private void StartSearch()
        {
            if (mRootDir.Name.EndsWith(Separator.DupDirSuffix))
            {
                // Root Directory ends with the duplicate directory suffix.
                Console.Write(DateTime.Now.ToString(Separator.TimeLogPrefixFormat));
                Console.Write("Error: ");
                Console.WriteLine("Screenshot Directory's Name cannot end with '{0}'.",
                    Separator.DupDirSuffix);
                Console.Write(DateTime.Now.ToString(Separator.TimeLogPrefixFormat));
                Console.WriteLine("Please select a different Directory to search.");
                
                return;
            }

            bSearchRunning = true;

            int i, count = 0;

            mTotalImageCount = 0;
            bRefreshProgressBar = true;

            mSeparators = Separator.CreateSeparators(
                mRootDir, mImageThreadCount, mGroupBy);

            count = mSeparators == null ? 0 : mSeparators.Length;

            for (i = 0; i < count; i++)
            {
                mTotalImageCount += mSeparators[i].SearchCount;
            }
            bRefreshProgressBar = true;

            Separator.WriteLog("Found {0:N0} Screenshots in total.", mTotalImageCount);

            mSearchStartTime = DateTime.Now;
            mLastTickTime = mSearchStartTime;
            mLastTickProgress = 0;
            mAvgCompTicks = 0L;

            bSearchRunning = count > 0;

            if (bSearchRunning)
            {
                for (i = 0; i < count; i++)
                {
                    mSeparators[i].StartSeparating();
                }
                Thread.Sleep(1000);
            }

            while (bSearchRunning)
            {
                bSearchRunning = false;
                for (i = 0; i < count; i++)
                {
                    bSearchRunning = bSearchRunning || mSeparators[i].IsSeparating;
                }
                Thread.Sleep(1000);
            }

            int dupCount = 0;
            long dupBytes = 0;

            for (i = 0; i < count; i++)
            {
                dupCount += mSeparators[i].DuplicateCount;
                dupBytes += mSeparators[i].DuplicateBytes;
            }

            double percent = mTotalImageCount == 0 ? 1 : mTotalImageCount;
            percent = dupCount / percent;
            Separator.WriteLog("Found {0:N0} Duplicates ( {1} ) " 
                + "out of {2:N0} Images ( {3:P} ) in total.", 
                dupCount, Separator.GetSizeString(dupBytes), 
                mTotalImageCount, percent);

            mOutputWriter.Flush();

            bSearchRunning = false;
        }
    }
}
