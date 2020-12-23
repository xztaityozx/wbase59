using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Wbase59;
using Xunit;
using Xunit.Abstractions;

namespace test {
    public class Wbase59DecoderTest {
        private readonly ITestOutputHelper testOutputHelper;

        public Wbase59DecoderTest(ITestOutputHelper helper) => testOutputHelper = helper;
        
        [Fact]
        public void Test_Decode() {
            var expect = "あいうえお";
            var encoded = new Wbase59Encoder(new MemoryStream(Encoding.UTF8.GetBytes(expect))).Encode()
                .SelectMany(n => n.GetSpan().ToArray()).ToArray();

            var actual = new Wbase59Decoder(new MemoryStream(
                    Encoding.Convert(new UnicodeEncoding(true, false), Encoding.UTF8, encoded)
                )).Decode();

            Assert.Equal(expect, Encoding.UTF8.GetString(actual.ToArray()));
        }
    }
}