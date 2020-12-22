using System;
using System.Collections.Generic;
using System.Linq;


namespace Wbase59 {
    public enum BaseNabe {
        辺,邉,邊
    }

    /// <summary>
    /// IVSを表すレコード
    /// </summary>
    public record Ivs(byte? Position) {
        /// <summary>
        /// IVSがあるかどうか
        /// </summary>
        public bool IsEmpty => Position is null;
        private byte[] bytes;

        /// <summary>
        /// IVSのバイト列をSpanで返す
        /// </summary>
        /// <exception cref="InvalidOperationException">IVSがないときはIVSのバイト列を生成出来ない</exception>
        /// <returns>IVSのバイト列</returns>
        public byte[] GetBytes() {
            if(Position is null) throw new InvalidOperationException();
            
            bytes ??= new byte[] {0xdb, 0x40, 0xdd, (byte)Position};
            return bytes;
        }

        public static Ivs Parse(Span<byte> span) {
            var bytes = span.ToArray();
            if (bytes.Length != 4) throw new InvalidNabeIvsException(bytes.Length);

            var rt = new Ivs(bytes.Last());
            if (rt.GetBytes().Zip(span.ToArray()).All(tuple => tuple.First == tuple.Second)) {
                return rt;
            }

            throw new InvalidNabeIvsException(bytes);
        }
    }

    /// <summary>
    /// 一つのナベを表すレコード
    /// </summary>
    public record Nabe(BaseNabe Base, byte? Position = null) {
        private static readonly Dictionary<BaseNabe, byte> MaxOfIvs = new() {
            [BaseNabe.辺] = 3,
            [BaseNabe.邉] = 32,
            [BaseNabe.邊] = 21
        };

        private static readonly Dictionary<BaseNabe, byte[]> BytesOfNabe = new() {
            [BaseNabe.辺] = new byte[] { 0x8F, 0xBA },
            [BaseNabe.邉] = new byte[] { 0x90, 0x89 },
            [BaseNabe.邊] = new byte[] { 0x90, 0x8A },
        };

        public static Nabe Parse(Span<byte> span) {
            var bytes = span.ToArray();
            if (bytes.Length != 2 && bytes.Length != 6) throw new InvalidNabeFormatException(bytes.Length);

            BaseNabe? baseNabe = null;
            foreach (var bn in new[]{BaseNabe.辺, BaseNabe.邉, BaseNabe.邊}) {
                if (BytesOfNabe[bn].Zip(bytes.Take(2)).All(tuple => tuple.First == tuple.Second)) baseNabe = bn;
            }

            if(baseNabe is null) throw new InvalidNabeFormatException(bytes[0], bytes[1]);

            if (bytes.Length == 2) {
                return new Nabe((BaseNabe) baseNabe);
            }

            _ = Ivs.Parse(bytes.Skip(2).ToArray());
            return new Nabe((BaseNabe) baseNabe, bytes.Last());
        }

        public byte Max => MaxOfIvs[Base];

        private Ivs ivs;

        /// <summary>
        /// このナベのバイト列を返す
        /// </summary>
        /// <exception cref="InvalidNabeIvsException">基底ナベとして取りえないIvsの値が指定されたときに投げられる</exception>
        /// <returns></returns>
        public Span<byte> GetSpan() {
            if (Position is not null && Position >= Max) throw new InvalidNabeIvsException(Base, Max, (byte) Position);
            
            ivs ??= new Ivs(Position);
            var bytes = ivs.IsEmpty ? BytesOfNabe[Base] : BytesOfNabe[Base].Concat(ivs.GetBytes()).ToArray();

            return new Span<byte>(bytes);
        }
    }

    public class InvalidNabeIvsException : Exception {
        public InvalidNabeIvsException(BaseNabe baseNabe,byte max, byte got) : base($"基底ナベ: {baseNabe}の異体字セレクタは {max} が最大値ですが、{got} が与えられました") {
        }
        
        public InvalidNabeIvsException(int length):base($"Ivsは4バイトでなければなりませんが、実際は{length}バイトでした") {}

        public InvalidNabeIvsException(IEnumerable<byte> bytes) :
            base($"バイト列 [{string.Join(",", bytes.Select(b => $"{b:X2}"))}] はIvsではありません") {}
    }
    
    public class InvalidNabeFormatException : Exception {
        public InvalidNabeFormatException(byte upper, byte lower) : base($"{upper:X2} {lower:X2} は基底ナベではありません") { }

        public InvalidNabeFormatException(int length) : base($"長さが{length}なバイト列を基底ナベにしようとしました"){}
        
        private InvalidNabeFormatException(string msg):base(msg){}

        public static InvalidNabeFormatException IsNotElementNabe => new("基底ナベをフラグ以外にできません");

        public static InvalidNabeFormatException NotEnoughByteLength => new("ナベストリーム解析中にバイト列が不足しました");
    }
}
