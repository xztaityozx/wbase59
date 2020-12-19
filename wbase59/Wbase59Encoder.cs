using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;

namespace Wbase59 {
    public class Wbase59Encoder{
        public static IEnumerable<Nabe> Encode(Stream stream) {
            const byte max = 56;

            using (stream) {
                while (stream.CanRead) {
                    var b = stream.ReadByte();
                    var baseNabe = (BaseNabe) (b % 3);
                    var flag = new Nabe(baseNabe);
                    
                    yield return flag;

                    do {
                        yield return Wbase59.Create((b % max));
                        b /= max;
                    } while (b >= max);
                }
            }
        }
    }
}