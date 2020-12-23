using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Wbase59 {
    public class Wbase59Encoder {
        private readonly StreamReader stream;
        public Wbase59Encoder(Stream stream) => this.stream = new StreamReader(stream);

        public IEnumerable<Nabe> Encode() {
            var encoder = new UnicodeEncoding(true, false);
            while (stream.Peek() > 0) {
                foreach (var b in Encoding.UTF8.GetBytes(stream.ReadLine() ?? string.Empty)) {
                    var val = b;
                    yield return new Nabe((BaseNabe) (val % 3));

                    do {
                        yield return Wbase59.Create(val % Max);
                        val /= Max;
                    } while (val >= Max);

                    yield return Wbase59.Create(val);
                }
            }
        }

        private const int Max = 56;
    }
}