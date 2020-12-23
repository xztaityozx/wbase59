using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppFramework;
using Microsoft.Extensions.Hosting;

namespace Wbase59 {
    public class Program : ConsoleAppBase {
        private static async Task Main(string[] args) {
            await Host.CreateDefaultBuilder().RunConsoleAppFrameworkAsync<Program>(args);
        }

        public void Run(
            [Option(0)] string path = "",
            [Option("d", "decode")] bool decode = false
        ) {
            using var stream = GetStream(path);
            if (decode) {
                using var oStream = Console.OpenStandardOutput();
                var decoder = new Wbase59Decoder(stream);
                foreach (var b in decoder.Decode()) {
                    oStream.WriteByte(b);
                }
                // oStream.Flush();
            }
            else {
                var encoder = new Wbase59Encoder(stream);
                foreach (var nabe in encoder.Encode()) {
                    Console.Write(new UnicodeEncoding(true, false).GetString(nabe.GetSpan()));
                }
            }
        }

        private static Stream GetStream(string path) {
            if (path.Length == 0 || path == "-") {
                return Console.OpenStandardInput();
            }

            if (!File.Exists(path)) throw new FileNotFoundException(path);

            return new StreamReader(path).BaseStream;
        }
    }
}