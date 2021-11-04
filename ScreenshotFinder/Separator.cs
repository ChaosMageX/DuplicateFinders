using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ScreenshotFinder
{
    public class Separator
    {
        public const string TimeLogPrefixFormat = "yyyy/MM/dd HH:mm:ss.fff: ";

        public const string FileNameTimeFormat = "yyyy-MM-dd_HH-mm-ss";

        public const string ElapsedTimeFormat = @"d\.hh\:mm\:ss";
        public const string AverageTimeFormat = @"ss\.fffffff";

        public const string DupDirSuffix = "-Duplicates";

        public const string AllImagesGroupName = "All";

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

            public string GetMimeName()
            {
                string[] split = MimeType.Split('/');
                return split == null ? MimeType : split[split.Length - 1];
            }
        }

        private static ImgPattern[] sSearchPatterns;

        static Separator()
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

        public static void WriteLog(string format, object arg0)
        {
            Console.Write(DateTime.Now.ToString(TimeLogPrefixFormat));
            Console.WriteLine(format, arg0);
        }

        public static void WriteLog(string format, object arg0, object arg1)
        {
            Console.Write(DateTime.Now.ToString(TimeLogPrefixFormat));
            Console.WriteLine(format, arg0, arg1);
        }

        public static void WriteLog(string format, params object[] args)
        {
            Console.Write(DateTime.Now.ToString(TimeLogPrefixFormat));
            Console.WriteLine(format, args);
        }

        #region Separator Factory
        /// <summary>
        /// Whether the creation of new Separators is allowed. 
        /// Set to <c>false</c> when the application is closed, 
        /// in order to halt any currently running creation process.
        /// </summary>
        public static bool CreationAllowed = true;
        /// <summary>
        /// Creates an array of <see cref="Separator"/>s corresponding to 
        /// the <paramref name="rootDir"/> folder and all of its sub-folders.
        /// </summary>
        /// <param name="rootDir"></param>
        /// <param name="imgThreadCount"></param>
        /// <param name="groupBy"></param>
        /// <returns></returns>
        public static Separator[] CreateSeparators(DirectoryInfo rootDir, 
            int imgThreadCount, GroupImgBy groupBy)
        {
            if (!CreationAllowed)
            {
                return new Separator[0];
            }
            string rootDirName = rootDir.Name;
            if (rootDirName.EndsWith(DupDirSuffix))
            {
                return new Separator[0];
            }
            DirectoryInfo parentDir = rootDir.Parent;
            if (parentDir == null)
            {
                parentDir = new DirectoryInfo(Path.Combine(
                    rootDir.FullName, "Screenshot" + DupDirSuffix));
            }
            if (!parentDir.Exists)
            {
                parentDir.Create();
            }
            DirectoryInfo currDir = new DirectoryInfo(rootDir.FullName);

            List<Separator> seps = new List<Separator>();
            switch (groupBy)
            {
                case GroupImgBy.All:
                    seps = CreateSepsByAll(currDir, parentDir, imgThreadCount);
                    break;
                case GroupImgBy.Codec:
                    seps = CreateSepsByCodec(currDir, parentDir, imgThreadCount);
                    break;
                case GroupImgBy.Ext:
                    seps = CreateSepsByExt(currDir, parentDir, imgThreadCount);
                    break;
            }
            return seps.ToArray();
        }

        private static List<Separator> CreateSepsByAll(
            DirectoryInfo currDir, DirectoryInfo parentDupDir, 
            int imgThreadCount)
        {
            if (!CreationAllowed)
            {
                return new List<Separator>();
            }
            WriteLog("Counting Screenshots in {0}...", currDir.Name);
            Separator sep;
            List<Separator> seps = new List<Separator>();
            List<string> imgGroupPatterns = new List<string>();
            int i, j;
            ImgPattern patterns;
            string pattern;
            for (i = sSearchPatterns.Length - 1; i >= 0; i--)
            {
                patterns = sSearchPatterns[i];
                for (j = patterns.Exts.Length - 1; j >= 0; j--)
                {
                    pattern = patterns.Exts[j];
                    imgGroupPatterns.Add(pattern);
                }
            }
            if (!CreationAllowed)
            {
                return seps;
            }
            sep = new Separator(currDir, parentDupDir,
                imgThreadCount, AllImagesGroupName, imgGroupPatterns);
            WriteLog("Found {0:N0} Screenshots!", 
                sep.mImageFiles.Length);
            seps.Add(sep);

            string dupDirName = currDir.Name + DupDirSuffix;
            DirectoryInfo dupDir = new DirectoryInfo(Path.Combine(
                parentDupDir.FullName, dupDirName));
            if (!dupDir.Exists)
                dupDir.Create();
            DirectoryInfo[] childDirs = currDir.GetDirectories("*",
                SearchOption.TopDirectoryOnly);
            for (i = 0; CreationAllowed && i < childDirs.Length; i++)
            {
                seps.AddRange(CreateSepsByAll(
                    childDirs[i], dupDir, imgThreadCount));
            }
            return seps;
        }

        private static List<Separator> CreateSepsByCodec(
            DirectoryInfo currDir, DirectoryInfo parentDupDir,
            int imgThreadCount)
        {
            if (!CreationAllowed)
            {
                return new List<Separator>();
            }
            WriteLog("Counting Screenshots in {0}...", currDir.Name);
            Separator sep;
            List<Separator> seps = new List<Separator>();
            List<string> imgGroupPatterns;
            int i, j;
            ImgPattern patterns;
            string pattern, groupName;
            for (i = sSearchPatterns.Length - 1; CreationAllowed && i >= 0; i--)
            {
                patterns = sSearchPatterns[i];
                groupName = patterns.GetMimeName();
                WriteLog("Counting {0} Screenshots...", groupName);
                imgGroupPatterns = new List<string>(patterns.Exts.Length);
                for (j = patterns.Exts.Length - 1; j >= 0; j--)
                {
                    pattern = patterns.Exts[j];
                    imgGroupPatterns.Add(pattern);
                }
                sep = new Separator(currDir, parentDupDir,
                    imgThreadCount, groupName, imgGroupPatterns);
                WriteLog("Found {0:N0} {1} Screenshots!", 
                    sep.mImageFiles.Length, groupName);
                seps.Add(sep);
            }
            if (!CreationAllowed)
            {
                return seps;
            }
            string dupDirName = currDir.Name + DupDirSuffix;
            DirectoryInfo dupDir = new DirectoryInfo(Path.Combine(
                parentDupDir.FullName, dupDirName));
            if (!dupDir.Exists)
                dupDir.Create();
            DirectoryInfo[] childDirs = currDir.GetDirectories("*",
                SearchOption.TopDirectoryOnly);
            for (i = 0; CreationAllowed && i < childDirs.Length; i++)
            {
                seps.AddRange(CreateSepsByCodec(
                    childDirs[i], dupDir, imgThreadCount));
            }
            return seps;
        }

        private static List<Separator> CreateSepsByExt(
            DirectoryInfo currDir, DirectoryInfo parentDupDir,
            int imgThreadCount)
        {
            if (!CreationAllowed)
            {
                return new List<Separator>();
            }
            WriteLog("Counting Screenshots in {0}...", currDir.Name);
            Separator sep;
            List<Separator> seps = new List<Separator>();
            List<string> imgGroupPatterns;
            int i, j;
            ImgPattern patterns;
            string pattern;
            for (i = sSearchPatterns.Length - 1; CreationAllowed && i >= 0; i--)
            {
                patterns = sSearchPatterns[i];
                WriteLog("Counting {0} Screenshots...", patterns.GetMimeName());
                for (j = patterns.Exts.Length - 1; CreationAllowed && j >= 0; j--)
                {
                    pattern = patterns.Exts[j];
                    WriteLog("Counting {0} Screenshots...", pattern);
                    imgGroupPatterns = new List<string>(1) { pattern };
                    sep = new Separator(currDir, parentDupDir,
                        imgThreadCount, pattern, imgGroupPatterns);
                    WriteLog("Found {0:N0} {1} Screenshots!",
                        sep.mImageFiles.Length, pattern);
                    seps.Add(sep);
                }
            }
            if (!CreationAllowed)
            {
                return seps;
            }
            string dupDirName = currDir.Name + DupDirSuffix;
            DirectoryInfo dupDir = new DirectoryInfo(Path.Combine(
                parentDupDir.FullName, dupDirName));
            if (!dupDir.Exists)
                dupDir.Create();
            DirectoryInfo[] childDirs = currDir.GetDirectories("*",
                SearchOption.TopDirectoryOnly);
            for (i = 0; CreationAllowed && i < childDirs.Length; i++)
            {
                seps.AddRange(CreateSepsByExt(
                    childDirs[i], dupDir, imgThreadCount));
            }
            return seps;
        }
        #endregion

        private readonly DirectoryInfo mCurrentDir;
        private readonly DirectoryInfo mDuplicateDir;

        private readonly FileAttributes mCurrDirAttr;
        private readonly DateTime mCurrDirCTime;
        private readonly DateTime mCurrDirMTime;
        private readonly DateTime mCurrDirATime;

        private readonly int mImageThreadCount;

        private readonly string mImageGroupName;
        private readonly FileInfo[] mImageFiles;

        private readonly Thread mComputeThread;
        private bool bAllDone;
        private bool bPaused;
        private int mSearchIndex;
        private DateTime mSearchStartTime;
        private TimeSpan mAvgCompTime;
        private int mDuplicateCount;
        private long mDuplicateBytes;

        private Separator(DirectoryInfo currDir, DirectoryInfo parentDupDir,
            int imgThreadCount, string imgGroupName, List<string> imgGroupPatterns)
        {
            mCurrentDir = currDir;

            mCurrDirAttr = currDir.Attributes;
            mCurrDirCTime = currDir.CreationTimeUtc;
            mCurrDirMTime = currDir.LastWriteTimeUtc;
            mCurrDirATime = currDir.LastAccessTimeUtc;

            string dupDirName = currDir.Name + DupDirSuffix;
            mDuplicateDir = new DirectoryInfo(Path.Combine(
                parentDupDir.FullName, dupDirName));
            if (!mDuplicateDir.Exists)
                mDuplicateDir.Create();

            mImageThreadCount = imgThreadCount;

            mImageGroupName = imgGroupName;

            if (imgGroupPatterns.Count > 1)
            {
                FileInfo[] files;
                List<FileInfo> fileList = new List<FileInfo>();
                for (int i = imgGroupPatterns.Count - 1; i >= 0; i--)
                {
                    files = currDir.GetFiles(imgGroupPatterns[i],
                        SearchOption.TopDirectoryOnly);
                    fileList.AddRange(files);
                }
                mImageFiles = fileList.ToArray();
            }
            else
            {
                mImageFiles = currDir.GetFiles(imgGroupPatterns[0],
                    SearchOption.TopDirectoryOnly);
            }

            mComputeThread = new Thread(Compute)
            {
                Priority = DefaultPriority
            };
            // If there are not enough image files to compare, 
            // there's no point in running the thread.
            // Avoid calling Thread.Start() if it leads nowhere. 
            // Otherwise, it can really bog down the app at startup.
            bAllDone = mImageFiles == null || mImageFiles.Length < 2;
            bPaused = false;
            mSearchIndex = 0;
            mSearchStartTime = DateTime.MaxValue;
            mAvgCompTime = new TimeSpan(0L);
            mDuplicateCount = 0;
        }
        /// <summary>
        /// Restores the folder attributes, modification time, access time 
        /// and other properties of this instance's directory to what they 
        /// were prior to removing any found duplicate images from them.
        /// </summary>
        public void RestoreFolderProperties()
        {
            mCurrentDir.Attributes = mCurrDirAttr;
            mCurrentDir.CreationTimeUtc = mCurrDirCTime;
            mCurrentDir.LastWriteTimeUtc = mCurrDirMTime;
            mCurrentDir.LastAccessTimeUtc = mCurrDirATime;
        }

        #region Thread Control
        /// <summary>
        /// Whether the internal image comparison thread is running.
        /// </summary>
        public bool IsSeparating => !bAllDone;
        /// <summary>
        /// Whether the internal image comparison thread is paused.
        /// </summary>
        public bool Paused
        {
            get { return bPaused; }
            set { bPaused = value; }
        }
        /// <summary>
        /// Starts the internal image comparison thread.
        /// </summary>
        public void StartSeparating()
        {
            if (!bAllDone)
            {
                mSearchStartTime = DateTime.Now;
                mComputeThread.Start();
            }
        }
        /// <summary>
        /// Stops the internal image comparison thread.
        /// </summary>
        public void StopSeparating()
        {
            bAllDone = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The current index of image being checked if it's a duplicate.
        /// </summary>
        public int SearchIndex => mSearchIndex;
        /// <summary>
        /// The total number of images being compared for duplicates.
        /// </summary>
        public int SearchCount => mImageFiles == null ? 0 : mImageFiles.Length;
        /// <summary>
        /// The current percentage of the image files that have been compared 
        /// for duplicates, between <c>0.0</c> and <c>1.0</c>, inclusive.
        /// </summary>
        public double SearchFraction
        {
            get
            {
                double denom;
                if (mImageFiles == null || mImageFiles.Length == 0)
                    denom = 1.0;
                else
                    denom = mImageFiles.Length;
                return mSearchIndex / denom;
            }
        }
        /// <summary>
        /// Average time spent comparing two image files 
        /// to see if one is a duplicate of the other.
        /// </summary>
        public TimeSpan AverageCompareTime => mAvgCompTime;
        /// <summary>
        /// The total number of duplicate images found and sequestered.
        /// </summary>
        public int DuplicateCount => mDuplicateCount;
        /// <summary>
        /// The total size, in bytes, of the duplicate images found and moved.
        /// </summary>
        public long DuplicateBytes => mDuplicateBytes;
        #endregion

        public string AppendDisplayString(StringBuilder sb,
            bool printRunTime, bool printDupCount)
        {
            if (sb == null)
                throw new ArgumentNullException("sb");
            sb.Append(mCurrentDir.Name);
            sb.Append(" : ");
            sb.Append(mImageGroupName);
            sb.Append(" : ");
            int searchCount = mImageFiles == null ? 0 : mImageFiles.Length;
            double percent = searchCount == 0 
                ? 0.0 : (double)mSearchIndex / searchCount;
            sb.Append(percent.ToString("P"));
            sb.Append(" ( ");
            sb.Append(mSearchIndex.ToString("N0"));
            sb.Append(" / ");
            sb.Append(searchCount.ToString("N0"));
            sb.Append(" )");
            if (printDupCount)
            {
                sb.Append(" | Dups: ");
                sb.Append(mDuplicateCount.ToString("N0"));
                sb.Append(" ( ");
                sb.Append(GetSizeString(mDuplicateBytes));
                sb.Append(" ) ( ");
                percent = mSearchIndex == 0 
                    ? 0.0 : (double)mDuplicateCount / mSearchIndex;
                sb.Append(percent.ToString("P"));
                sb.Append(" / ");
                percent = (double)mDuplicateCount / searchCount;
                sb.Append(percent.ToString("P"));
                sb.Append(" ) ");
            }
            if (printRunTime)
            {
                sb.Append(" | Elapsed: ");
                TimeSpan ts = DateTime.Now - mSearchStartTime;
                sb.Append(ts.ToString(ElapsedTimeFormat));
                sb.Append(" | Remaining: ");
                int diff = searchCount - mSearchIndex;
                ts = new TimeSpan(diff * mAvgCompTime.Ticks);
                sb.Append(ts.ToString(ElapsedTimeFormat));
            }
            sb.Append(" | Avg: ");
            sb.Append(mAvgCompTime.ToString(AverageTimeFormat));
            sb.Append(" s / Img");
            return sb.ToString();
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

        private void WriteLogCloser()
        {
            int count = mImageFiles == null ? 0 : mImageFiles.Length;
            double percentTotal = count == 0 ? 1 : count;
            percentTotal = mDuplicateCount / percentTotal;
            double percentSearch = mSearchIndex == 0 ? 1 : mSearchIndex;
            percentSearch = mDuplicateCount / percentSearch;
            if (mImageGroupName == AllImagesGroupName)
            {
                WriteLog("Found {0:N0} Duplicates ( {1} ) " 
                    + "out of {2:N0} / {3:N0} Images ( {4:P} / {5:P} ) in {6}.",
                    mDuplicateCount,
                    GetSizeString(mDuplicateBytes), 
                    mSearchIndex, count, percentSearch, percentTotal, 
                    mCurrentDir.Name);
            }
            else
            {
                WriteLog("Found {0:N0} {1} Duplicates ( {2} ) " 
                    + "out of {3:N0} / {4:N0} {1} Images ( {5:P} / {6:P} ) in {7}.",
                    mDuplicateCount, mImageGroupName,
                    GetSizeString(mDuplicateBytes), 
                    mSearchIndex, count, percentSearch, percentTotal, 
                    mCurrentDir.Name);
            }
        }

        private void Compute()
        {
            mSearchIndex = 0;
            mDuplicateCount = 0;
            mDuplicateBytes = 0L;

            FileAttributes attr;
            DateTime cTime, mTime, aTime;

            int attempts;
            long avgTicks;
            TimeSpan compTime;
            DateTime startTime;
            Bitmap currPic, lastPic = null;
            int fileCount = mImageFiles.Length;
            FileInfo file = mImageFiles[0];
            for (int k = 0; !bAllDone && k < fileCount; k++)
            {
                while (bPaused)
                {
                    Thread.Sleep(1000);
                }
                file = mImageFiles[k];
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
                    // There is a lastPic and currPic successfully opened.
                    // Proceed to compare them.
                    if (ImagesDifferUnsafe(currPic, lastPic))
                    {
                        // currPic and lastPic are different.
                        // currPic is the new image to compare to  
                        // all following images for duplicates.
                        lastPic.Dispose();
                        lastPic = currPic;
                    }
                    else
                    {
                        // currPic is a duplicate of lastPic.
                        // Move currPic to the duplicates folder,
                        // then move on to the next image.
                        currPic.Dispose();
                        mDuplicateCount++;
                        mDuplicateBytes += file.Length;

                        // Preserve file info before moving.
                        attr = file.Attributes;
                        cTime = file.CreationTimeUtc;
                        mTime = file.LastWriteTimeUtc;
                        aTime = file.LastAccessTimeUtc;

                        // Move the duplicate image file.
                        file.MoveTo(Path.Combine(
                            mDuplicateDir.FullName, file.Name));

                        // Apply saved file info after moving.
                        file.Attributes = attr;
                        file.CreationTimeUtc = cTime;
                        file.LastWriteTimeUtc = mTime;
                        file.LastAccessTimeUtc = aTime;
                    }
                }
                compTime = DateTime.Now - startTime;
                avgTicks = mAvgCompTime.Ticks * k + compTime.Ticks;
                mAvgCompTime = new TimeSpan(avgTicks / (k + 1));
                mSearchIndex++;
            }
            if (lastPic != null)
                lastPic.Dispose();
            WriteLogCloser();
            bAllDone = true;
        }

        #region Image Comparison Functions

        private bool ImagesDifferSafe(Bitmap currPic, Bitmap lastPic)
        {
            if (currPic == null || lastPic == null)
                return true;
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

            if (div < 2)
            {
                for (int i = 0; isMatch && i < length; i++)
                {
                    if (lastScan[i] != currScan[i])
                    {
                        isMatch = false;
                    }
                }
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
            if (currPic == null || lastPic == null)
                return true;
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

            if (div < 2)
            {
                for (int i = 0; isMatch && i < length; i++)
                {
                    if (lastScan[i] != currScan[i])
                    {
                        isMatch = false;
                    }
                }
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

            private Thread mThread;

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

        #endregion
    }
}
