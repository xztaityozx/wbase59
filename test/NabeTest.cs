using System.Linq;
using wbase59;
using Xunit;

namespace test {
    public class NabeTest {
        [Fact]
        public void Test_Max() {
            var data = new[] {
                new {nabe = new Nabe(BaseNabe.辺), expect = 3},
                new {nabe = new Nabe(BaseNabe.邊), expect = 21},
                new {nabe = new Nabe(BaseNabe.邉), expect = 32}
            };

            foreach (var item in data) {
                Assert.Equal(item.expect, item.nabe.Max);
            }
        }

        [Fact]
        public void Test_GetSpan() {
            var data = new (BaseNabe nabe, byte max, byte[] bytes)[] {
                new(BaseNabe.辺, 3, new byte[] {0x8F, 0xBA}),
                new(BaseNabe.邊, 21, new byte[] {0x90, 0x8A}),
                new(BaseNabe.邉, 32, new byte[] {0x90, 0x89}),
            };

            foreach (var (nabe,max,bytes) in data) {
                for (byte i = 0; i < max; i++) {
                    var n = new Nabe(nabe, i);
                    var ivs = new Ivs(i);

                    var expect = bytes.Concat(ivs.GetBytes()).ToArray();
                    var actual = n.GetSpan().ToArray();

                    Assert.Equal(expect, actual);
                }
            }
        }
    }
}