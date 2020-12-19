using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;

namespace Wbase59 {
    public static class Wbase59Encoder {
        public static IEnumerable<Nabe> Encode(Stream stream) {
            const int max = 56;

            while (stream.Position < stream.Length) {
                var buff = new byte[2];
                var span = new Span<byte>(buff);

                stream.Read(span);

                var val = (uint) ((span[0] << 8) + span[1]);
                var bn = (BaseNabe) (val % 3);

                yield return new Nabe(bn);

                do {
                    yield return Wbase59.Create((int) (val % max));
                    val /= max;
                } while (val >= max);
            }
        }
    }
}