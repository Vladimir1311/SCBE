using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IPResolver.Models.Network
{
    class PartialStream : Stream
    {
        private Stream baseStream;
        private readonly long length;
        private long position;
        public PartialStream(Stream baseStream, long length)
        {

            this.baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
            if (!baseStream.CanRead) throw new ArgumentException("can't read base stream");

            this.length = length;

        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            long remaining = length - position;
            if (remaining <= 0) return 0;
            if (remaining < count) count = remaining > int.MaxValue ? int.MaxValue : (int)remaining;
            int read = baseStream.Read(buffer, offset, count);
            position += read;
            return read;
        }
        private void CheckDisposed()
        {
            if (baseStream == null) throw new ObjectDisposedException(GetType().Name);
        }
        public override long Length
        {
            get { CheckDisposed(); return length; }
        }
        public override bool CanRead
        {
            get { CheckDisposed(); return length - position > 0; }
        }
        public override bool CanWrite
        {
            get { CheckDisposed(); return false; }
        }
        public override bool CanSeek
        {
            get { CheckDisposed(); return false; }
        }
        public override long Position
        {
            get
            {
                CheckDisposed();
                return position;
            }
            set { throw new NotSupportedException(); }
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
        public override void Flush()
        {
            CheckDisposed(); baseStream.Flush();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (baseStream != null)
                {
                    try { baseStream.Dispose(); }
                    catch { }
                    baseStream = null;
                }
            }
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
