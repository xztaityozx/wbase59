using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Wbase59;
using Xunit;
using Xunit.Abstractions;

namespace test {
    public class BitStreamTest {
        
        [Fact]
        public void Test_Next() {
            var bytes = Enumerable.Range(byte.MinValue, byte.MaxValue)
                .OrderBy(_ => Guid.NewGuid())
                .Select(b => (byte) b).ToArray();

            using var ms = new MemoryStream(bytes);
            var bitStream = new BitStream(ms);

            List<bool> actual = new(), expect = new();
            
            foreach (var b in bytes) {
                for (int i = 0, val = b; i < 8; i++, val <<= 1) {
                    actual.Add(bitStream.Next() == 1);
                    expect.Add((val & 0x80) > 0);
                }
            }
            
            Assert.Equal(expect, actual);

            Assert.Throws<ArgumentException>(() => bitStream.Next());
        }
    }
}