using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wbase59;
using Xunit;
using Xunit.Abstractions;

namespace test {
    public class Wbase59DecoderTest {
        private readonly ITestOutputHelper testOutputHelper;

        public Wbase59DecoderTest(ITestOutputHelper helper) => testOutputHelper = helper;
        
        [Fact]
        public void Test_Decode() {
            var original = Enumerable.Range(0, byte.MaxValue).OrderBy(_ => Guid.NewGuid()).Select(b => (byte) b).ToArray();
            using var ms = new MemoryStream(original);

            var encoded = new Wbase59Encoder(ms).Encode().ToArray();
            using var inStream = new MemoryStream(encoded.SelectMany(n => n.GetSpan().ToArray()).ToArray());
            var actual = new Wbase59Decoder(inStream).Decode().ToArray();

            Assert.Equal(original, actual);
        }
    }
}