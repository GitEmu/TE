using System;
using System.IO;
using System.Text;

namespace uhttpsharp
{
    class LoggingStream : Stream
    {
        private readonly Stream _child;

        private readonly string _tempFileName = Path.GetTempFileName();
        public LoggingStream(Stream child)
        {
            _child = child;

            Console.WriteLine("Logging to " + _tempFileName);
        }

        public override void Flush()
        {
            _child.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _child.Seek(offset, origin);
        }
        public override void SetLength(long value)
        {
            _child.SetLength(value);
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            var retVal = _child.Read(buffer, offset, count);

            using (var stream = File.Open(_tempFileName, FileMode.Append))
            {
                stream.Seek(0, SeekOrigin.End);
                stream.Write(buffer, offset, retVal);
            }

            return retVal;
        }

        public override int ReadByte()
        {
            var retVal = _child.ReadByte();

            using (var stream = File.Open(_tempFileName, FileMode.Append))
            {
                stream.Seek(0, SeekOrigin.End);
                stream.WriteByte((byte)retVal);
            }

            return retVal;
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            _child.Write(buffer, offset, count);

        }
        public override void WriteByte(byte value)
        {
            _child.WriteByte(value);

        }
        public override bool CanRead
        {
            get { return _child.CanRead; }
        }
        public override bool CanSeek
        {
            get { return _child.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _child.CanWrite; }
        }
        public override long Length
        {
            get { return _child.Length; }
        }
        public override long Position
        {
            get { return _child.Position; }
            set { _child.Position = value; }
        }
        public override int ReadTimeout
        {
            get { return _child.ReadTimeout; }
            set { _child.ReadTimeout = value; }
        }
        public override int WriteTimeout
        {
            get { return _child.WriteTimeout; }
            set { _child.WriteTimeout = value; }
        }
    }

    class LimitedStream : Stream
    {
        private const string _exceptionMessageFormat = "The Stream has exceeded the {0} limit specified.";
        private readonly Stream _child;
        private long _readLimit;
        private long _writeLimit;

        public LimitedStream(Stream child, long readLimit = -1, long writeLimit = -1)
        {
            _child = child;
            _readLimit = readLimit;
            _writeLimit = writeLimit;
        }
        public override void Flush()
        {
            _child.Flush();
        }
        
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _child.Seek(offset, origin);
        }
        public override void SetLength(long value)
        {
            _child.SetLength(value);
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            var retVal = _child.Read(buffer, offset, count);

            AssertReadLimit(retVal);

            return retVal;
        }
        private void AssertReadLimit(int coefficient)
        {
            if (_readLimit == -1)
            {
                return;
            }

            _readLimit -= coefficient;

            if (_readLimit < 0)
            {
                throw new IOException(string.Format(_exceptionMessageFormat, "read"));
            }
        }

        private void AssertWriteLimit(int coefficient)
        {
            if (_writeLimit == -1)
            {
                return;
            }

            _writeLimit -= coefficient;

            if (_writeLimit < 0)
            {
                throw new IOException(string.Format(_exceptionMessageFormat, "write"));
            }
        }

        public override int ReadByte()
        {
            var retVal = _child.ReadByte();

            AssertReadLimit(1);

            return retVal;
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            _child.Write(buffer, offset, count);

            AssertWriteLimit(count);
        }
        public override void WriteByte(byte value)
        {
            _child.WriteByte(value);

            AssertWriteLimit(1);
        }
        public override bool CanRead
        {
            get { return _child.CanRead; }
        }
        public override bool CanSeek
        {
            get { return _child.CanSeek; }
        }
        
        public override bool CanWrite
        {
            get { return _child.CanWrite; }
        }
        public override long Length
        {
            get { return _child.Length; }
        }
        public override long Position
        {
            get { return _child.Position; }
            set { _child.Position = value; }
        }
        public override int ReadTimeout
        {
            get { return _child.ReadTimeout; }
            set { _child.ReadTimeout = value; }
        }
        public override int WriteTimeout
        {
            get { return _child.WriteTimeout; }
            set { _child.WriteTimeout = value; }
        }
    }
}
