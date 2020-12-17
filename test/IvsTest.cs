using System;
using Xunit;
using Wbase59;

namespace test {
    public class IvsTest {
        [Fact]
        public void Test_IsEmpty() {
            var data = new[] {
                new {ivs = new Ivs(null), expect = true},
                new {ivs = new Ivs(1), expect = false},
                new {ivs = new Ivs(128), expect = false},
                new {ivs = new Ivs(0), expect = false}
            };

            foreach (var item in data) {
                Assert.Equal(item.expect, item.ivs.IsEmpty);
            }
        }

        [Fact]
        public void Test_GetBytes() {
            for (var b = 0; b <= byte.MaxValue; b++) {
                Assert.Equal(new byte[] {0xdb, 0x40, 0xdd, (byte) b}, new Ivs((byte) b).GetBytes());
            }

            Assert.Throws<InvalidOperationException>(() => new Ivs(null).GetBytes());
        }
    }
}
