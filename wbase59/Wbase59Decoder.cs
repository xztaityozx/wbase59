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

        public Wbase59Decoder(Stream stream, Encoding encoding)
            => this.stream = new StreamReader(stream, encoding);

        /// <summary>
        /// デコード本体
        /// </summary>
        /// <returns></returns>
        public IEnumerable<byte> Decode() {
            var nextMod = 0;
            var stack = new Stack<int>();
            var encoding = new UnicodeEncoding(true, false);

            while (stream.Peek() > 0) {
                for (var item = StringInfo.GetTextElementEnumerator(stream.ReadLine() ?? string.Empty);
                    item.MoveNext();) {
                    var bytes = encoding.GetBytes(item.GetTextElement());

                    var nextNabe = Nabe.Parse(bytes.AsSpan());

                    if (nextNabe.Position is null) {
                        if (stack.Any()) {
                            yield return (byte) Aggregate(stack, nextMod);
                        }

                        nextMod = (int) nextNabe.Base;
                    }
                    else {
                        stack.Push(Wbase59.ToValue(nextNabe));
                    }
                }

                if (!stack.Any()) continue;
                yield return (byte) Aggregate(stack, nextMod);
            }
        }

        /// <summary>
        /// ナベビットから復元した余りと商を使ってもとの値を復元する君
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
        /// <exception cref="ParityNabeCheckFailedException">パリティチェックに失敗したときに投げられる</exception>
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