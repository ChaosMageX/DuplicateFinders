using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FindDuplicateScreenshots
{
    public class StringAndFileWriter : StreamWriter
    {
        private StringBuilder mBuilder;
        private int mVersion;

        public StringAndFileWriter(StringBuilder sb, string path)
            : this(sb, path, false)
        {

        }

        public StringAndFileWriter(StringBuilder sb, string path, bool append)
            : base(path, append)
        {
            mBuilder = sb ?? throw new ArgumentNullException("sb");
            mVersion = 0;
        }

        public override void Write(char value)
        {
            base.Write(value);
            mBuilder.Append(value);
            mVersion++;
        }

        public override void Write(char[] buffer, int index, int count)
        {
            base.Write(buffer, index, count);
            mBuilder.Append(buffer, index, count);
            mVersion++;
        }

        public override void Write(string value)
        {
            base.Write(value);
            mBuilder.Append(value);
            mVersion++;
        }

        public int GetVersion() { return mVersion; }
    }
}
