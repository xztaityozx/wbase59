using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wbase59;
using Xunit;

namespace test {
    public class Wbase59EncoderTest {
        [Fact]
        public void Test_Encode() {
            var bytes = Enumerable.Range(0, byte.MaxValue).Select(b => (byte) b).ToArray();
            var expect = new List<Nabe>();

            for (var i = 0; i < bytes.Length; i += 2) {
                var val = (uint) ((bytes[i] << 8));
                if (i + 1 < bytes.Length) val |= bytes[i + 1];

                expect.Add(new Nabe((BaseNabe) (val % 3)));

                do {
                    expect.Add(Wbase59.Wbase59.Create((int) (val % 56)));
                    val /= 56;
                } while (val >= 56);
            }

            using var ms = new MemoryStream(bytes) {Position = 0};
            var actual = Wbase59Encoder.Encode(ms).ToArray();

            Assert.Equal(expect, actual);
        }
    }
}