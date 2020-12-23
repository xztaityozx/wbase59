using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Wbase59 {
    public class Wbase59Decoder {
        private readonly StreamReader stream;

        public Wbase59Decoder(Stream stream) => this.stream = new StreamReader(stream);

        public IEnumerable<byte> Decode() {
            var encoder = new UnicodeEncoding(true, false);

            var nextMod = 0;
            var stack = new Stack<int>();

            while (stream.Peek() > 0) {
                for (var item = StringInfo.GetTextElementEnumerator(stream.ReadLine() ?? string.Empty);
                    item.MoveNext();) {
                    var bytes = encoder.GetBytes(item.GetTextElement());

                    var nextNabe = Nabe.Parse(bytes.AsSpan());

                    if (nextNabe.Position is null) {
                        if (stack.Any()) {
                            yield return (byte) Aggregate(stack, nextMod);
                        }

                        nextMod = (int) nextNabe.Base;
                    }
                    else {
                        stack.Push(Wbase59.ToByteValue(nextNabe));
                    }
                }

                if (!stack.Any()) continue;
                yield return (byte) Aggregate(stack, nextMod);
            }
        }

        private static int Aggregate(Stack<int> stack, int mod) {
            var cur = 0;

            while (stack.TryPop(out var val)) {
                cur = cur * 56 + val;
            }

            if (cur % 3 != mod) throw new ParityNabeCheckFailedException();

            return cur;
        }
    }

    public class ParityNabeCheckFailedException : Exception {
        public ParityNabeCheckFailedException() : base("パリティナベチェックに失敗しました") {
        }
    }
}