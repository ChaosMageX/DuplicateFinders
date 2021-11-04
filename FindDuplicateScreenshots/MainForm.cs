using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Windows.Forms;

namespace FindDuplicateScreenshots
{
    public partial class MainForm : Form
    {
        public const string TimePrefixFormat = "yyyy/MM/dd HH:mm:ss.fff: ";

        public const string FileNameTimeFormat = "yyyy-MM-dd_HH-mm-ss";

        public const string DupDirSuffix = "-Duplicates";

        public const ThreadPriority DefaultPriority = ThreadPriority.Normal;

        public enum GroupImgBy
        {
            All,
            Codec,
            Ext
        }

        private struct ImgPattern
        {
            public string MimeType;
            public string[] Exts;

            public ImgPattern(string mimeType, string[] exts)
            {
                MimeType = mimeType;
                Exts = exts;
            }

            public override string ToString()
            {
                return MimeType + " : " + string.Join(", ", Exts);
            }
        }

        private static ImgPattern[] sSearchPatterns;

        static MainForm()
        {
            char[] sep = new char[] { ';' };

            string[] exts;
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            sSearchPatterns = new ImgPattern[codecs.Length];
            for (int i = codecs.Length - 1; i >= 0; i--)
            {
                exts = codecs[i].FilenameExtension.Split(sep,
                    StringSplitOptions.RemoveEmptyEntries);
                for (int j = exts.Length - 1; j >= 0; j--)
                {
                    exts[j] = exts[j].ToLower();
                }
                sSearchPatterns[i] = new ImgPattern(codecs[i].MimeType, exts);
            }

            /*sSearchPatterns = new string[9][];
            // Bmp
            sSearchPatterns[0] = new string[] { "*.bmp" };
            // Emf
            sSearchPatterns[1] = new string[] { "*.emf" };
            // Exif
            sSearchPatterns[2] = new string[] { "*.exif" };
            // Gif
            sSearchPatterns[3] = new string[] { "*.gif" };
            // Icon
            sSearchPatterns[4] = new string[] { "*.ico" };
            // Jpeg
            sSearchPatterns[5] = new string[] { "*.jpe", "*.jpeg", "*.jpg" };
            // Png
            sSearchPatterns[6] = new string[] { "*.png" };
            // Tiff
            sSearchPatterns[7] = new string[] { "*.tiff" };
            // Wmf
            sSearchPatterns[8] = new string[] { "*.wmf" };/* */
        }

        private const double cKBsize = 1024.0;
        private const double cMBsize = 1048576.0;
        private const double cGBsize = 1073741824.0;
        private const double cTBsize = 1099511627776.0;
        private const double cPBsize = 1125899906842624.0;
        private const long cKBcutoff = 1000L;
        private const long cMBcutoff = 1024000L;
        private const long cGBcutoff = 1048576000L;
        private const long cTBcutoff = 1073741824000L;
        private const long cPBcutoff = 1099511627776000L;

        public static string GetSizeString(long byteCount)
        {
            if (byteCount < cKBcutoff)
            {
                return byteCount.ToString("N0") + " B";
            }
            if (byteCount < cMBcutoff)
            {
                return (byteCount / cKBsize).ToString("G3") + " KB";
            }
            if (byteCount < cGBcutoff)
            {
                return (byteCount / cMBsize).ToString("G3") + " MB";
            }
            if (byteCount < cTBcutoff)
            {
                return (byteCount / cGBsize).ToString("G3") + " GB";
            }
            if (byteCount < cPBcutoff)
            {
                return (byteCount / cTBsize).ToString("G3") + " TB";
            }
            return (byteCount / cPBsize).ToString("G3") + " PB";
        }

        private StringBuilder mOutputBuilder;
        private StringAndFileWriter mOutputWriter;
        private int mOutputVersion;

        private Thread mSearchThread;
        private bool bCountRunning;
        private bool bSearchRunning;
        private bool bSearchPaused = false;

        private bool bNeedsUpdate;
        private bool bSearchStopped = false;

        private DirectoryInfo mRootDir;
        private GroupImgBy mGroupBy;
        private int mImageThreadCount;

        // Total number of image files found.
        private int mTotalImageCount;
        // Total number of image files processed.
        private int mCurrentProgress;
        // Total number of duplicate images found.
        private int mDuplicateCount;
        private long mDuplicateBytes;

        private string mCurrentSearchFolder = "";
        private int mCurrentSearchIndex = 0;
        private int mCurrentSearchCount = 0;
        private int mCurrentDupCount = 0;
        private long mCurrentDupBytes = 0L;

        private DateTime mSearchStartTime;
        private TimeSpan mImgCompTime;
        private TimeSpan mAvgCompTime;

        public MainForm()
        {
            InitializeComponent();

            string logFilePath = "FindDups_"
                + DateTime.Now.ToString(FileNameTimeFormat)
                + "_Log.txt";
            logFilePath = Path.Combine(Environment.CurrentDirectory, logFilePath);

            mOutputBuilder = new StringBuilder();
            mOutputWriter = new StringAndFileWriter(mOutputBuilder, logFilePath);
            mOutputVersion = mOutputWriter.GetVersion();
            Console.SetOut(mOutputWriter);

            mSearchThread = new Thread(SearchForDuplicateImages)
            {
                Priority = DefaultPriority
            };

            mRootDir = new DirectoryInfo(Environment.CurrentDirectory);
            UpdateDirectoryDisplay();

            mGroupBy = GroupImgBy.Codec;
            mGroupByCMB.SelectedIndex = (int)mGroupBy;

            mImageThreadCount = 1;
            mThreadCountNUD.Value = mImageThreadCount;

            mUpdateTimer.Enabled = true;
        }

        private void WriteLog(string format, object arg0)
        {
            Console.Write(DateTime.Now.ToString(TimePrefixFormat));
            Console.WriteLine(format, arg0);
        }

        private void WriteLog(string format, object arg0, object arg1)
        {
            Console.Write(DateTime.Now.ToString(TimePrefixFormat));
            Console.WriteLine(format, arg0, arg1);
        }

        private void WriteLog(string format, params object[] args)
        {
            Console.Write(DateTime.Now.ToString(TimePrefixFormat));
            Console.WriteLine(format, args);
        }

        private void StartButtonClicked(object sender, EventArgs e)
        {
            mSearchThread.Start();
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

        private void PauseButtonClicked(object sender, EventArgs e)
        {
            if (bSearchRunning)
            {
                bSearchPaused = !bSearchPaused;
                mPauseBTN.Text = bSearchPaused ? "Play" : "Pause";
            }
        }

        private void StopButtonClicked(object sender, EventArgs e)
        {
            bSearchStopped = true;
        }

        private void GroupByIndexChanged(object sender, EventArgs e)
        {
            mGroupBy = (GroupImgBy)mGroupByCMB.SelectedIndex;
        }

        private void ThreadCountValueChanged(object sender, EventArgs e)
        {
            mImageThreadCount = (int)mThreadCountNUD.Value;
        }

        private void MainFormClosed(object sender, FormClosedEventArgs e)
        {
            bSearchStopped = true;
        }

        private void UpdateTimerTick(object sender, EventArgs e)
        {
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
            if (bNeedsUpdate)
            {
                bool enabled = !(bCountRunning || bSearchRunning);
                mStartBTN.Enabled = enabled;
                mBrowseBTN.Enabled = enabled;
                mGroupByCMB.Enabled = enabled;
                mThreadCountNUD.Enabled = enabled;
                mPauseBTN.Enabled = !enabled;
                mStopBTN.Enabled = !enabled;
                mMainProgressBAR.Maximum = mTotalImageCount;
                bNeedsUpdate = false;
            }
            if (bSearchRunning)
            {
                mMainProgressBAR.Value = mCurrentProgress;

                TimeSpan ts;
                double percent;
                int progress = mCurrentProgress == 0 ? 1 : mCurrentProgress;
                StringBuilder sb = new StringBuilder();
                sb.Append("Overall : ");
                percent = (double)mCurrentProgress / mTotalImageCount;
                sb.Append(percent.ToString("P"));
                sb.Append(" ( ");
                sb.Append(mCurrentProgress.ToString("N0"));
                sb.Append(" / ");
                sb.Append(mTotalImageCount.ToString("N0"));
                sb.Append(" )");
                sb.Append(" | Dups: ");
                sb.Append(mDuplicateCount);
                sb.Append(" ( ");
                sb.Append(GetSizeString(mDuplicateBytes));
                sb.Append(" ) ( ");
                percent = (double)mDuplicateCount / progress;
                sb.Append(percent.ToString("P"));
                sb.Append(" / ");
                percent = (double)mDuplicateCount / mTotalImageCount;
                sb.Append(percent.ToString("P"));
                sb.Append(" ) | In Folder ");
                sb.Append(mCurrentSearchFolder);
                sb.Append(" : ");
                percent = (double)mCurrentSearchIndex / mCurrentSearchCount;
                sb.Append(percent.ToString("P"));
                sb.Append(" ( ");
                sb.Append(mCurrentSearchIndex.ToString("N0"));
                sb.Append(" / ");
                sb.Append(mCurrentSearchCount.ToString("N0"));
                sb.Append(" )");
                sb.Append(" | Dups: ");
                sb.Append(mCurrentDupCount);
                sb.Append(" ( ");
                sb.Append(GetSizeString(mCurrentDupBytes));
                sb.Append(" ) ( ");
                progress = mCurrentSearchIndex == 0 ? 1 : mCurrentSearchIndex;
                percent = (double)mCurrentDupCount / progress;
                sb.Append(percent.ToString("P"));
                sb.Append(" / ");
                percent = (double)mCurrentDupCount / mCurrentSearchCount;
                sb.Append(percent.ToString("P"));
                sb.Append(" ) | Elapsed: ");
                ts = DateTime.Now - mSearchStartTime;
                sb.Append(ts.ToString(@"d\.hh\:mm\:ss"));
                sb.Append(" | Remaining: ");
                if (mCurrentProgress > 1)
                {
                    long ticks = (mCurrentProgress - 1) * mAvgCompTime.Ticks;
                    ticks = (ticks + mImgCompTime.Ticks) / mCurrentProgress;
                    mAvgCompTime = new TimeSpan(ticks);
                }
                else
                {
                    mAvgCompTime = mImgCompTime;
                }
                int diff = mTotalImageCount - mCurrentProgress;
                ts = new TimeSpan(diff * mAvgCompTime.Ticks);
                sb.Append(ts.ToString(@"d\.hh\:mm\:ss"));
                sb.Append(" | Avg: ");
                sb.Append(mAvgCompTime.ToString(@"ss\.fffffff"));
                sb.Append(" / Img");

                mMainProgressTXT.Text = sb.ToString();
            }
        }

        private void UpdateDirectoryDisplay()
        {
            mDirectoryTXT.Text = mRootDir.FullName;

            if (mRootDir.Name.EndsWith(DupDirSuffix))
            {
                mStartBTN.Text = "Start ⚠️";
            }
            else
            {
                mStartBTN.Text = "Start 🆗";
            }
        }

        private void SearchForDuplicateImages()
        {
            bCountRunning = true;
            bNeedsUpdate = true;

            mTotalImageCount = 0;
            mCurrentProgress = 0;
            CountTotalImages(mRootDir);
            WriteLog("Found {0} Screenshots in total.", mTotalImageCount);

            bCountRunning = false;
            bSearchRunning = true;
            mSearchStartTime = DateTime.Now;
            bNeedsUpdate = true;

            DirectoryInfo parentDir = mRootDir.Parent;
            if (parentDir == null)
            {
                parentDir = new DirectoryInfo(Path.Combine(
                    mRootDir.FullName, "Screenshot" + DupDirSuffix));
            }
            if (!parentDir.Exists)
            {
                parentDir.Create();
            }
            if (!SequesterDuplicates(mRootDir, parentDir))
            {
                // Root Directory ends with the duplicate directory suffix.
                Console.Write(DateTime.Now.ToString(TimePrefixFormat));
                Console.Write("Error: ");
                Console.WriteLine("Screenshot Directory's Name cannot end with '{0}'.", 
                    DupDirSuffix);
                Console.Write(DateTime.Now.ToString(TimePrefixFormat));
                Console.WriteLine("Please select a different Directory to search.");
            }
            bSearchRunning = false;
            bNeedsUpdate = true;
        }

        private bool CountTotalImages(DirectoryInfo currDir)
        {
            if (bSearchStopped)
            {
                return false;
            }
            WriteLog("Counting Screenshots in {0}...", currDir.Name);
            int i, j, fCount;
            ImgPattern patterns;
            string pattern;
            FileInfo[] files;
            for (i = sSearchPatterns.Length - 1; i >= 0; i--)
            {
                patterns = sSearchPatterns[i];
                WriteLog("Counting {0} Screenshots...", patterns.MimeType);
                for (j = patterns.Exts.Length - 1; j >= 0; j--)
                {
                    pattern = patterns.Exts[j];
                    WriteLog("Counting {0} Screenshots...", pattern);
                    files = currDir.GetFiles(pattern,
                        SearchOption.TopDirectoryOnly);
                    fCount = files.Length;
                    WriteLog("Found {0} {1} Screenshots!", fCount, pattern);

                    mTotalImageCount += fCount;

                    if (bSearchStopped) break;
                }
                if (bSearchStopped) break;
            }
            if (bSearchStopped)
            {
                return false;
            }
            DirectoryInfo[] childDirs = currDir.GetDirectories("*",
                SearchOption.TopDirectoryOnly);
            WriteLog("Counting Screenshots in {0} Child Directories...",
                childDirs.Length);
            for (i = 0; i < childDirs.Length; i++)
            {
                CountTotalImages(childDirs[i]);
            }
            return true;
        }

        private bool SequesterDuplicates(DirectoryInfo currDir, DirectoryInfo parentDupDir)
        {
            if (bSearchStopped)
            {
                return false;
            }
            string currDirName = currDir.Name;
            if (currDirName.EndsWith(DupDirSuffix))
            {
                return false;
            }
            WriteLog("Finding Duplicates in {0}...", currDirName);

            string dupDirName = currDirName + DupDirSuffix;
            DirectoryInfo dupDir = new DirectoryInfo(Path.Combine(
                parentDupDir.FullName, dupDirName));
            if (!dupDir.Exists)
                dupDir.Create();

            DateTime mTime = currDir.LastWriteTimeUtc;

            mDuplicateCount = 0;
            mDuplicateBytes = 0L;
            switch (mGroupBy)
            {
                case GroupImgBy.All:
                    SequesterByAll(currDir, dupDir);
                    break;
                case GroupImgBy.Codec:
                    SequesterByCodec(currDir, dupDir);
                    break;
                case GroupImgBy.Ext:
                    SequesterByExt(currDir, dupDir);
                    break;
            }

            currDir.LastWriteTimeUtc = mTime;

            if (bSearchStopped)
            {
                return false;
            }
            DirectoryInfo[] childDirs = currDir.GetDirectories("*",
                SearchOption.TopDirectoryOnly);
            WriteLog("Finding Duplicates in {0} Child Directories...",
                childDirs.Length);
            for (int i = 0; i < childDirs.Length; i++)
            {
                SequesterDuplicates(childDirs[i], dupDir);
            }
            return true;
        }

        private void SequesterByAll(DirectoryInfo currDir, DirectoryInfo dupDir)
        {
            int i, j, dupCount;
            ImgPattern patterns;
            string pattern;
            FileInfo[] files;
            List<FileInfo> fileList = new List<FileInfo>();
            for (i = sSearchPatterns.Length - 1; i >= 0; i--)
            {
                patterns = sSearchPatterns[i];
                for (j = patterns.Exts.Length - 1; j >= 0; j--)
                {
                    pattern = patterns.Exts[j];
                    files = currDir.GetFiles(pattern,
                        SearchOption.TopDirectoryOnly);
                    fileList.AddRange(files);
                    if (bSearchStopped) break;
                }
                if (bSearchStopped) break;
            }
            if (!bSearchStopped)
            {
                dupCount = SequesterCore(currDir, dupDir, fileList.ToArray());

                WriteLog("Found {0} Duplicates!", dupCount);
            }
        }

        private void SequesterByCodec(DirectoryInfo currDir, DirectoryInfo dupDir)
        {
            int i, j, dupCount;
            ImgPattern patterns;
            string pattern;
            FileInfo[] files;
            List<FileInfo> fileList = new List<FileInfo>();
            for (i = sSearchPatterns.Length - 1; i >= 0; i--)
            {
                fileList.Clear();
                patterns = sSearchPatterns[i];
                WriteLog("Finding {0} Duplicates...", patterns.MimeType);
                for (j = patterns.Exts.Length - 1; j >= 0; j--)
                {
                    pattern = patterns.Exts[j];
                    files = currDir.GetFiles(pattern,
                        SearchOption.TopDirectoryOnly);
                    fileList.AddRange(files);
                    if (bSearchStopped) break;
                }
                if (bSearchStopped) break;

                dupCount = SequesterCore(currDir, dupDir, fileList.ToArray());

                WriteLog("Found {0} {1} Duplicates!", dupCount, patterns.MimeType);
            }
        }

        private void SequesterByExt(DirectoryInfo currDir, DirectoryInfo dupDir)
        {
            int i, j, dupCount;
            ImgPattern patterns;
            string pattern;
            FileInfo[] files;
            for (i = sSearchPatterns.Length - 1; i >= 0; i--)
            {
                patterns = sSearchPatterns[i];
                WriteLog("Finding {0} Duplicates...", patterns.MimeType);
                for (j = patterns.Exts.Length - 1; j >= 0; j--)
                {
                    pattern = patterns.Exts[j];
                    WriteLog("Finding {0} Duplicates...", pattern);
                    files = currDir.GetFiles(pattern,
                        SearchOption.TopDirectoryOnly);
                    if (bSearchStopped) break;

                    dupCount = SequesterCore(currDir, dupDir, files);

                    WriteLog("Found {0} {1} Duplicates!", dupCount, pattern);
                }
                if (bSearchStopped) break;
            }
        }

        private int SequesterCore(DirectoryInfo currDir, DirectoryInfo dupDir, FileInfo[] files)
        {
            int fileCount = files.Length;
            if (fileCount < 2)
            {
                return 0;
            }
            mCurrentSearchFolder = currDir.Name;
            mCurrentSearchCount = fileCount;
            mCurrentSearchIndex = 0;
            mCurrentDupCount = 0;
            mCurrentDupBytes = 0L;

            FileAttributes attr;
            DateTime cTime, mTime, aTime;

            FileAttributes currDirAttr = currDir.Attributes;
            DateTime currDirCTime = currDir.CreationTimeUtc;
            DateTime currDirMTime = currDir.LastWriteTimeUtc;
            DateTime currDirATime = currDir.LastAccessTimeUtc;

            FileInfo file;
            DateTime startTime;
            int attempts, dupCount = 0;
            Bitmap currPic, lastPic = null;
            for (int k = 0; !bSearchStopped && k < fileCount; k++)
            {
                while (bSearchPaused)
                {
                    Thread.Sleep(1000);
                }
                file = files[k];
                startTime = DateTime.Now;
                attempts = 0;
                currPic = null;
                while (currPic == null && attempts < 3)
                {
                    try { currPic = new Bitmap(file.FullName); }
                    catch (Exception ex)
                    {
                        attempts++;
                        WriteLog("Error opening file \"{0}\""
                            + "\r\nAttempt {1} : {2} : {3}",
                            file.FullName, attempts,
                            ex.GetType().Name, ex.Message);
                        Thread.Sleep(10);
                    }
                }
                if (lastPic == null)
                {
                    // It's the very first image in the folder.
                    // Set lastPic and move on to the next image.
                    lastPic = currPic;

                    // If the first image didn't successfully open, 
                    // lastPic will still be null on the next loop,
                    // so it will keep looping to this block until it 
                    // successfully opens an image for lastPic, and then 
                    // successfully opens another image for currPic.
                }
                else if (currPic != null)
                {
                    if (ImagesDifferUnsafe(currPic, lastPic))
                    {
                        lastPic.Dispose();
                        lastPic = currPic;
                    }
                    else
                    {
                        currPic.Dispose();
                        mDuplicateCount++;
                        mDuplicateBytes += file.Length;
                        mCurrentDupCount++;
                        mCurrentDupBytes += file.Length;
                        dupCount++;

                        // Preserve file info before moving.
                        attr = file.Attributes;
                        cTime = file.CreationTimeUtc;
                        mTime = file.LastWriteTimeUtc;
                        aTime = file.LastAccessTimeUtc;

                        // Move the duplicate image file.
                        file.MoveTo(Path.Combine(
                            dupDir.FullName, file.Name));

                        // Apply saved file info after moving.
                        file.Attributes = attr;
                        file.CreationTimeUtc = cTime;
                        file.LastWriteTimeUtc = mTime;
                        file.LastAccessTimeUtc = aTime;
                    }
                }
                mImgCompTime = DateTime.Now - startTime;
                mCurrentProgress++;
                mCurrentSearchIndex++;
            }
            if (lastPic != null)
                lastPic.Dispose();
            currDir.Attributes = currDirAttr;
            currDir.CreationTimeUtc = currDirCTime;
            currDir.LastWriteTimeUtc = currDirMTime;
            currDir.LastAccessTimeUtc = currDirATime;
            return dupCount;
        }

        private bool ImagesDifferSafe(Bitmap currPic, Bitmap lastPic)
        {
            if (lastPic.Height != currPic.Height)
                return true;
            if (lastPic.Width != currPic.Width)
                return true;
            if (lastPic.PixelFormat != currPic.PixelFormat)
                return true;

            int height = lastPic.Height;
            int width = lastPic.Width;
            Rectangle rect = new Rectangle(0, 0, width, height);
            PixelFormat format = lastPic.PixelFormat;

            BitmapData lastData = lastPic.LockBits(rect, ImageLockMode.ReadOnly, format);
            BitmapData currData = currPic.LockBits(rect, ImageLockMode.ReadOnly, format);

            int stride = lastData.Stride;
            int pixLen = Image.GetPixelFormatSize(format) / 8;
            int length = height * stride;

            byte[] lastScan = new byte[length];
            byte[] currScan = new byte[length];

            System.Runtime.InteropServices.Marshal.Copy(lastData.Scan0, lastScan, 0, length);
            System.Runtime.InteropServices.Marshal.Copy(currData.Scan0, currScan, 0, length);

            bool isMatch = false;
            int div = mImageThreadCount;

            if (div == 1)
            {
                /*int i, j, k;
                int lastPix, currPix;
                for (i = 0; isMatch && i < height; i++)
                {
                    for (j = 0; isMatch && j < width; j++)
                    {
                        lastPix = i * stride + j * pixLen;
                        currPix = i * stride + j * pixLen;
                        for (k = 0; k < pixLen; k++)
                        {
                            if (lastScan[k + lastPix] != currScan[k + currPix])
                            {
                                isMatch = false;
                            }
                        }
                    }
                }/* */
                for (int i = 0; isMatch && i < length; i++)
                {
                    if (lastScan[i] != currScan[i])
                    {
                        isMatch = false;
                    }
                }/* */
            }
            else
            {
                int i, count;
                int len = length / div;
                int mod = length % div;
                int start = 0;
                FractalSafe fractal;
                FractalSafe[] fractals = new FractalSafe[div];
                for (i = 0; i < div; i++)
                {
                    count = len;
                    if (mod > 0)
                    {
                        count++;
                        mod--;
                    }
                    fractals[i] = new FractalSafe(lastScan, currScan, start, count);
                    start += count;
                }
                bool allDone = false;
                while (isMatch && !allDone)
                {
                    allDone = true;
                    for (i = 0; isMatch && i < div; i++)
                    {
                        fractal = fractals[i];
                        if (fractal.Different)
                        {
                            isMatch = false;
                        }
                        allDone = allDone && fractal.Done;
                    }
                    if (!isMatch)
                    {
                        for (i = 0; i < div; i++)
                        {
                            fractals[i].ForceStop();
                        }
                    }
                }
            }

            lastPic.UnlockBits(lastData);
            currPic.UnlockBits(currData);
            return !isMatch;
        }

        private class FractalSafe
        {
            private readonly byte[] mLastScan;
            private readonly byte[] mCurrScan;
            private readonly int mStart;
            private readonly int mCount;

            private readonly Thread mThread;

            public bool Done;
            public bool Different;

            public FractalSafe(byte[] lastScan, byte[] currScan, int start, int count)
            {
                mLastScan = lastScan;
                mCurrScan = currScan;
                mStart = start;
                mCount = count;

                Done = false;
                Different = false;
                mThread = new Thread(Compute)
                {
                    Priority = DefaultPriority
                };
                mThread.Start();
            }

            private void Compute()
            {
                for (int i = mStart; !Done && i < mCount; i++)
                {
                    if (mLastScan[i] != mCurrScan[i])
                    {
                        Different = true;
                        Done = true;
                    }
                }
                Done = true;
            }

            public void ForceStop() { Done = true; }

            public override string ToString()
            {
                return "S: " + mStart
                    + " C: " + mCount
                    + " Done: " + (Done ? "Y" : "N")
                    + " Diff: " + (Different ? "Y" : "N");
            }
        }

        private unsafe bool ImagesDifferUnsafe(Bitmap currPic, Bitmap lastPic)
        {
            if (lastPic.Height != currPic.Height)
                return true;
            if (lastPic.Width != currPic.Width)
                return true;
            if (lastPic.PixelFormat != currPic.PixelFormat)
                return true;

            int height = lastPic.Height;
            int width = lastPic.Width;
            Rectangle rect = new Rectangle(0, 0, width, height);
            PixelFormat format = lastPic.PixelFormat;

            BitmapData lastData = lastPic.LockBits(rect, ImageLockMode.ReadOnly, format);
            BitmapData currData = currPic.LockBits(rect, ImageLockMode.ReadOnly, format);

            int stride = lastData.Stride;
            int pixLen = Image.GetPixelFormatSize(format) / 8;
            int length = height * stride;

            byte* lastScan = (byte*)lastData.Scan0.ToPointer();
            byte* currScan = (byte*)currData.Scan0.ToPointer();

            bool isMatch = true;
            int div = mImageThreadCount;

            if (div == 1)
            {
                /*int i, j, k;
                byte* lastPix, currPix;
                for (i = 0; isMatch && i < height; i++)
                {
                    for (j = 0; isMatch && j < width; j++)
                    {
                        lastPix = lastScan + i * stride + j * pixLen;
                        currPix = currScan + i * stride + j * pixLen;
                        for (k = 0; k < pixLen; k++)
                        {
                            if (lastPix[k] != currPix[k])
                            {
                                isMatch = false;
                            }
                        }
                    }
                }/* */
                for (int i = 0; isMatch && i < length; i++)
                {
                    if (lastScan[i] != currScan[i])
                    {
                        isMatch = false;
                    }
                }/* */
            }
            else
            {
                int i, count, start = 0;
                int len = length / div;
                int mod = length % div;
                FractalUnsafe fractal;
                FractalUnsafe[] fractals = new FractalUnsafe[div];
                for (i = 0; i < div; i++)
                {
                    count = len;
                    if (mod > 0)
                    {
                        count++;
                        mod--;
                    }
                    fractals[i] = new FractalUnsafe(lastScan, currScan, start, count);
                    start += count;
                }
                bool allDone = false;
                while (isMatch && !allDone)
                {
                    allDone = true;
                    for (i = 0; isMatch && i < div; i++)
                    {
                        fractal = fractals[i];
                        if (fractal.Different)
                        {
                            isMatch = false;
                        }
                        allDone = allDone && fractal.Done;
                    }
                    if (!isMatch)
                    {
                        for (i = 0; i < div; i++)
                        {
                            fractals[i].ForceStop();
                        }
                    }
                }
            }

            lastPic.UnlockBits(lastData);
            currPic.UnlockBits(currData);
            return !isMatch;
        }

        private unsafe class FractalUnsafe
        {
            private readonly byte* mLastScan;
            private readonly byte* mCurrScan;
            private readonly int mStart;
            private readonly int mCount;

            private readonly Thread mThread;

            public bool Done;
            public bool Different;

            public FractalUnsafe(byte* lastScan, byte* currScan, int start, int count)
            {
                mLastScan = lastScan;
                mCurrScan = currScan;
                mStart = start;
                mCount = count;

                Done = false;
                Different = false;
                mThread = new Thread(Compute)
                {
                    Priority = DefaultPriority
                };
                mThread.Start();
            }

            private void Compute()
            {
                for (int i = mStart; !Done && i < mCount; i++)
                {
                    if (mLastScan[i] != mCurrScan[i])
                    {
                        Different = true;
                        Done = true;
                    }
                }
                Done = true;
            }

            public void ForceStop() { Done = true; }

            public override string ToString()
            {
                return "S: " + mStart
                    + " C: " + mCount
                    + " Done: " + (Done ? "Y" : "N")
                    + " Diff: " + (Different ? "Y" : "N");
            }
        }
    }
}
