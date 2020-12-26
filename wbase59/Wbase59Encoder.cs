using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Wbase59 {
    public class Wbase59Encoder {
        private readonly StreamReader stream;
        public Wbase59Encoder(Stream stream, Encoding encoding) => this.stream = new StreamReader(stream, encoding);

        public IEnumerable<Nabe> Encode() {
            while (stream.Peek() > 0) {
                foreach (var b in Encoding.UTF8.GetBytes(stream.ReadLine() + Environment.NewLine)) {
                    var val = b;
                    yield return new Nabe((BaseNabe) (val % 3));

                    do {
                        yield return Wbase59.ToNabe(val % Max);
                        val /= Max;
                    } while (val >= Max);

                    yield return Wbase59.ToNabe(val);
                }
            }
        }

        private const int Max = 56;
    }
}