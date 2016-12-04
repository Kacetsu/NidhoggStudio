using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace ns.Base.Collections {

    public class BlockingStream : Stream {
        private readonly BlockingCollection<byte[]> _blocks;
        private byte[] _currentBlock;
        private int _currentBlockIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockingStream"/> class.
        /// </summary>
        /// <param name="streamWriteCountCache">The stream write count cache.</param>
        public BlockingStream(int streamWriteCountCache) {
            _blocks = new BlockingCollection<byte[]>(streamWriteCountCache);
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead { get; } = true;

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek { get; } = false;

        /// <summary>
        /// Gets a value that determines whether the current stream can time out.
        /// </summary>
        public override bool CanTimeout { get; } = false;

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite { get { return _blocks.IsAddingCompleted; } }

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        public override long Length { get { throw new NotSupportedException(); } }

        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <exception cref="System.NotSupportedException">
        /// </exception>
        public override long Position {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the total bytes written.
        /// </summary>
        /// <value>
        /// The total bytes written.
        /// </value>
        public long TotalBytesWritten { get; private set; }

        /// <summary>
        /// Gets the write count.
        /// </summary>
        /// <value>
        /// The write count.
        /// </value>
        public int WriteCount { get; private set; }

        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream. Instead of calling this method, ensure that the stream is properly disposed.
        /// </summary>
        public override void Close() {
            CompleteWriting();
            base.Close();
        }

        /// <summary>
        /// Completes the writing.
        /// </summary>
        public void CompleteWriting() {
            _blocks.CompleteAdding();
        }

        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush() {
        }

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count) {
            ValidateBufferArgs(buffer, offset, count);

            int bytesRead = 0;
            while (true) {
                if (_currentBlock != null) {
                    int copy = Math.Min(count - bytesRead, _currentBlock.Length - _currentBlockIndex);
                    Buffer.BlockCopy(_currentBlock, _currentBlockIndex, buffer, offset + bytesRead, copy);
                    //Array.Copy(_currentBlock, _currentBlockIndex, buffer, offset + bytesRead, copy);
                    _currentBlockIndex += copy;
                    bytesRead += copy;

                    if (_currentBlock.Length <= _currentBlockIndex) {
                        _currentBlock = null;
                        _currentBlockIndex = 0;
                    }

                    if (bytesRead == count)
                        return bytesRead;
                }

                if (!_blocks.TryTake(out _currentBlock, Timeout.Infinite))
                    return bytesRead;
            }
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position.</param>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public override long Seek(long offset, SeekOrigin origin) {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        public override void SetLength(long value) {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count" /> bytes from <paramref name="buffer" /> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count) {
            ValidateBufferArgs(buffer, offset, count);

            var newBuf = new byte[count];
            Array.Copy(buffer, offset, newBuf, 0, count);
            _blocks.Add(newBuf);
            TotalBytesWritten += count;
            WriteCount++;
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing) {
                _blocks.Dispose();
            }
        }

        private static void ValidateBufferArgs(byte[] buffer, int offset, int count) {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");
            if (buffer.Length - offset < count)
                throw new ArgumentException("buffer.Length - offset < count");
        }
    }
}