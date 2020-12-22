using System.Linq;
using Wbase59;
using Xunit;

namespace test {
    public class NabeTest {
        [Fact]
        public void Test_Parse() {
            var data = new[] {
                new byte[] {0x8F, 0xBA},
                new byte[] {0x90, 0x89},
                new byte[] {0x90, 0x8A},
                new byte[] {0x8F, 0xBA, 0xdb, 0x40, 0xdd, 0x00},
                new byte[] {0x90, 0x89, 0xdb, 0x40, 0xdd, 0x00},
                new byte[] {0x90, 0x8A, 0xdb, 0x40, 0xdd, 0x00}
            };

            foreach (var b in data) {
                Assert.NotNull(Nabe.Parse(b));
            }


            Assert.Throws<InvalidNabeFormatException>(() => Nabe.Parse(new byte[] {0x1}));
            Assert.Throws<InvalidNabeFormatException>(() => Nabe.Parse(System.Array.Empty<byte>()));
        }

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
                Assert.Equal(bytes, new Nabe(nabe).GetSpan().ToArray());

                for (byte i = 0; i < max+2; i++) {
                    var n = new Nabe(nabe, i);
                    if (i >= max) {
                        Assert.Throws<InvalidNabeIvsException>(() => n.GetSpan());
                        continue;
                    }

                    var expect = bytes.Concat(new Ivs(i).GetBytes()).ToArray();
                    var actual = n.GetSpan().ToArray();

                    Assert.Equal(expect, actual);
                }
            }
        }
    }
}