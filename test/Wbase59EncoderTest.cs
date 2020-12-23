using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Wbase59;
using Xunit;

namespace test {
    public class Wbase59EncoderTest {
        [Fact]
        public void Test_Encode() {
            var bytes = Encoding.UTF8.GetBytes("あいうえお");

            using var ms = new MemoryStream(bytes);
            var expect = new List<Nabe>();

            foreach (var b in bytes) {
                var val = b;
                expect.Add(new Nabe((BaseNabe) (val % 3)));
                do {
                    expect.Add(Wbase59.Wbase59.Create(val % 56));
                    val /= 56;
                } while (val >= 56);

                expect.Add(Wbase59.Wbase59.Create(val));
            }

            var actual = new Wbase59Encoder(ms).Encode();
            
            Assert.Equal(expect, actual);
        }
    }
}