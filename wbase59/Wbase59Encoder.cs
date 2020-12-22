using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;

namespace Wbase59 {
    public class Wbase59Encoder {
        private readonly Stream stream;
        public Wbase59Encoder(Stream stream) => this.stream = stream;

        public IEnumerable<Nabe> Encode() {
            while (stream.Position < stream.Length) {
                var buf = new byte[1];
                var span = new Span<byte>(buf);
                stream.Read(span);

                var val = buf[0];
                yield return new Nabe((BaseNabe) (val % 3));

                do {
                    yield return Wbase59.Create(val % Max);
                    val /= Max;
                } while (val >= Max);

                yield return Wbase59.Create(val);
            }
        }

        private const int Max = 56;
        private const int BitSize = 11;
    }
}