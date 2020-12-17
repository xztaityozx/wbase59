using System;
using System.Collections.Generic;
using System.Linq;


namespace wbase59 {
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
    }

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
    }
}
