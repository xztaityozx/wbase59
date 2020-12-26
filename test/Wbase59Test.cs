using System;
using Wbase59;
using Xunit;

namespace test {
    public class Wbase59Test {
        [Fact]
        public void Test_Create_ThrowsException() {
            Assert.Throws<ArgumentOutOfRangeException>(() => Wbase59.Wbase59.ToNabe(56));
            Assert.Throws<ArgumentOutOfRangeException>(() => Wbase59.Wbase59.ToNabe(-1));
        }

        [Fact]
        public void Test_Create() {
            var data = new (int val, BaseNabe expect) [] {
                new (0, BaseNabe.辺),
                new (2, BaseNabe.辺),
                new (3, BaseNabe.邊),
                new (23, BaseNabe.邊),
                new (24, BaseNabe.邉),
                new (55, BaseNabe.邉),
            };

            foreach (var (val, bn) in data) {
                var actual = Wbase59.Wbase59.ToNabe(val);
                var expect = new Nabe(bn, (byte) (val - bn switch {
                    BaseNabe.辺 => 0,
                    BaseNabe.邉 => 24,
                    BaseNabe.邊 => 3,
                    _ => throw new ArgumentException()
                }));

                Assert.Equal(expect, actual);
            }
        }
    }
}