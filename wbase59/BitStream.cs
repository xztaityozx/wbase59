using System;
using System.IO;

namespace Wbase59 {
    public class BitStream {
        private readonly Stream stream;

        public BitStream(Stream stream) {
            this.stream = stream;
        }

        private readonly byte[] buf = new byte[1];
        private int index;
        private int length;

        public int Next() {
            if (index >= length) {
                Read();
            }
            
            var rt = buf[0] & 0x80;
            buf[0] <<= 1;
            index++;
            return rt > 0 ? 1 : 0;
        }

        public bool HasNext => stream.Position < stream.Length;

        private void Read() {
            if (!HasNext) throw new ArgumentException();
            
            index = 0;
            var span = new Span<byte>(buf);
            length = stream.Read(span) * 8;
        }
    }
}