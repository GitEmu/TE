using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace uhttpsharp.RequestProviders
{
    public interface IStreamReader
    {

        Task<string> ReadLine();

        Task<byte[]> ReadBytes(int count);


    }
    class StreamReaderAdapter : IStreamReader
    {
        private readonly StreamReader _reader;
        public StreamReaderAdapter(StreamReader reader)
        {
            _reader = reader;
        }

        public async Task<string> ReadLine()
        {
            return await _reader.ReadLineAsync().ConfigureAwait(false);
        }
        public async Task<byte[]> ReadBytes(int count)
        {
            var tempBuffer = new char[count];

            await _reader.ReadBlockAsync(tempBuffer, 0, count).ConfigureAwait(false);

            var retVal = new byte[count];

            for (int i = 0; i < tempBuffer.Length; i++)
            {
                retVal[i] = (byte)tempBuffer[i];
            }

            return retVal;
        }
    }

    class MyStreamReader : IStreamReader
    {
        private const int BufferSize = 8096 / 4;
        private readonly Stream _underlyingStream;

        private readonly byte[] _middleBuffer = new byte[BufferSize];
        private int _index;
        private int _count;

        public MyStreamReader(Stream underlyingStream)
        {
            _underlyingStream = underlyingStream;
        }

        private async Task ReadBuffer()
        {
            do
            {
                _count = await _underlyingStream.ReadAsync(_middleBuffer, 0, BufferSize).ConfigureAwait(false);

                if (_count == 0)
                {
                    // Fix for 100% CPU
                    await Task.Delay(100).ConfigureAwait(false);
                }
            }
            while (_count == 0);

            _index = 0;
        }

        public async Task<string> ReadLine()
        {
            var builder = new StringBuilder(64);

            if (_index == _count)
            {
                await ReadBuffer().ConfigureAwait(false);
            }
            var readByte = _middleBuffer[_index++];

            while (readByte != '\n' && (builder.Length == 0 || builder[builder.Length - 1] != '\r'))
            {
                builder.Append((char)readByte);

                if (_index == _count)
                {
                    await ReadBuffer().ConfigureAwait(false);
                }
                readByte = _middleBuffer[_index++];
            }

            //Debug.WriteLine("Readline : " + sw.ElapsedMilliseconds);

            return builder.ToString(0, builder.Length - 1);
        }

        public async Task<byte[]> ReadBytes(int count)
        {
            var buffer = new byte[count];
            int currentByte = 0;

            // Empty the buffer
            int bytesToRead = Math.Min(_count - _index, count) + _index;
            for (int i = _index; i < bytesToRead; i++)
            {
                buffer[currentByte++] = _middleBuffer[i];
            }

            _index = _count;

            // Read from stream
            while (currentByte < count)
            {
                currentByte += await _underlyingStream.ReadAsync(buffer, currentByte, count - currentByte).ConfigureAwait(false);
            }

            //Debug.WriteLine("ReadBytes(" + count + ") : " + sw.ElapsedMilliseconds);

            return buffer;
        }
    }
}