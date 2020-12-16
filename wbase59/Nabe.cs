using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
        public Span<byte> GetSpan() {
            if(Position is null) throw new InvalidOperationException();
            
            bytes ??= new byte[] {0xdb, 0x40, 0xdd, (byte)Position};
            return new Span<byte>(bytes);
        }
    }

}
