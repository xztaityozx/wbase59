using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Wbase59 {
    public class Wbase59Decoder {
        private readonly Stream stream;

        public Wbase59Decoder(Stream stream) => this.stream = stream;

        public IEnumerable<byte> Decode() {
            var bitStream = new List<bool>();
            while (stream.Position < stream.Length) {
                var bn = GetFlagBaseNabe();
                var section = GetSectionNabe();

                if (section % 3 != (int) bn) throw new ParityNabeCheckFailedException();

                yield return (byte)section;
            }

        }

        private BaseNabe GetFlagBaseNabe() {
            if (!stream.CanRead) throw new IOException();
            if (stream.Position >= stream.Length - 2) throw InvalidNabeFormatException.NotEnoughByteLength;

            var buff = new byte[2];
            var span = new Span<byte>(buff);

            stream.Read(span);

            return Nabe.Parse(span).Base;
        }

        private int GetSectionNabe() {
            if (!stream.CanRead) throw new IOException();

            var stack = new Stack<int>();

            while (stream.Position < stream.Length) {
                var bn = GetFlagBaseNabe();

                var buf = new byte[4];
                var span = new Span<byte>(buf);
                stream.Read(span);

                if (buf[0] == 0xdb && buf[1] == 0x40 && buf[2] == 0xdd) {
                    stack.Push(bn switch {
                        BaseNabe.辺 => buf[3],
                        BaseNabe.邊 => buf[3] + 3,
                        BaseNabe.邉 => buf[3] + 24,
                        _ => throw new ArgumentException()
                    });
                }
                else if (
                    buf[0] == 0x8F && buf[1] == 0xBA ||
                    buf[0] == 0x90 && buf[1] == 0x89 ||
                    buf[0] == 0x90 && buf[1] == 0x8A
                ) {
                    
                    Console.WriteLine($"extracted: {string.Join(",", stack)}");
                    var cur = 0;
                    while (stack.TryPop(out var top)) {
                        cur = cur * 56 + top;
                    }

                    stream.Position -= 6;
                    return cur;
                }
                else {
                    throw new InvalidNabeFormatException(buf[0], buf[1]);
                }
            }

            if (stack.Any()) {
                var cur = 0;
                while (stack.TryPop(out var top)) {
                    cur = cur * 56 + top;
                }

                return cur;
            }
            else {
                throw new InvalidDataException();
            }
        }
    }

    public class ParityNabeCheckFailedException : Exception {
        public ParityNabeCheckFailedException() : base("パリティナベチェックに失敗しました") {
        }
    }
}