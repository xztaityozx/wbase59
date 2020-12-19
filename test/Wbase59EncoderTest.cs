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
            var list = new List<Nabe>();

            foreach (var b in bytes) {
                var baseNabe = (BaseNabe) (b % 3);
                var prefix = new Nabe(baseNabe);
                list.Add(prefix);

                const byte max = 56; 
                var value = b;
                while (value >= max) {
                    list.Add(Wbase59.Wbase59.Create((byte)(value % max)));
                    value /= max;
                }

            }

            var actual = Wbase59Encoder.Encode(new MemoryStream(bytes)).ToArray();
            
            Assert.Equal(list,actual);
        }
    }
}