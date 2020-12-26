using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colorful;
using ConsoleAppFramework;
using Microsoft.Extensions.Hosting;
using Console = System.Console;

namespace Wbase59 {
    using cc = Colorful.Console;

    public class Program : ConsoleAppBase {
        private static async Task Main(string[] args) {
            await Host.CreateDefaultBuilder()
                .RunConsoleAppFrameworkAsync<Program>(args);
        }
        
        /// <summary>
        /// wbase59 --list-encodingで呼び出される部分。利用可能なエンコーディングを列挙して終了する
        /// </summary>
        [Command("--list-encoding", "show available input/output encodings")]
        public int PrintEncodingList() {
            foreach (var item in new[] {
                StreamEncoding.utf8,
                StreamEncoding.utf16be,
                StreamEncoding.utf16bem,
                StreamEncoding.utf16le,
                StreamEncoding.utf16lem,
                StreamEncoding.system
            }.OrderBy(e => $"{e}")) {
                Console.WriteLine(item);
            }

            return 0;
        }

        /// <summary>
        /// エントリーポイント
        /// </summary>
        /// <param name="path">ファイルへのパス。なし、もしくは`-`の場合はSTDINから読み込む</param>
        /// <param name="decode">trueのときデコード動作をする</param>
        /// <param name="inputEncoding">入力ストリームのエンコーディング</param>
        /// <param name="outputEncoding">出力ストリームのエンコーディング</param>
        /// <returns>ステータスコード</returns>
        public int Run(
            [Option(0, "FILE")] string path = "-",
            [Option("d", "decode")] bool decode = false,
            [Option("f", "input encoding")] StreamEncoding inputEncoding = StreamEncoding.system,
            [Option("t", "output encoding")] StreamEncoding outputEncoding = StreamEncoding.system
        ) {
            var iEncoding = GetEncoding(inputEncoding);
            var oEncoding = GetEncoding(outputEncoding);
            using var oStream = new StreamWriter(Console.OpenStandardOutput(), oEncoding);

            using var stream = GetStream(path);
            if (decode) {
                try {
                    var decoder = new Wbase59Decoder(stream, iEncoding);
                    oStream.Write(oEncoding.GetString(decoder.Decode().ToArray()));
                }
                catch (InvalidNabeFormatException e) {
                    LogError("ナベストリーム解析中にエラーが発生しました");
                    LogError(e.Message);
                    return 1;
                }
                catch (InvalidNabeIvsException e) {
                    LogError("不正なIvsです");
                    LogError(e.Message);
                    return 1;
                }
                catch (ParityNabeCheckFailedException e) {
                    LogError(e.Message);
                    return 1;
                }
                catch (Exception e) {
                    LogError(e);
                    return 1;
                }
            }
            else {
                try {
                    var encoder = new Wbase59Encoder(stream, iEncoding);
                    foreach (var nabe in encoder.Encode()) {
                        oStream.Write(new UnicodeEncoding(true, false).GetString(nabe.GetSpan()));
                    }

                    oStream.Flush();
                }
                catch (InvalidNabeFormatException e) {
                    LogError(e);
                    return 1;
                }
            }

            return 0;
        }

        /// <summary>
        /// エラー出力用メソッド
        /// </summary>
        /// <param name="msg">出力したいオブジェクト</param>
        private static void LogError(object msg) {
            cc.WriteFormatted("[{0}]", Color.DarkRed, new Formatter[] {new("Critical", Color.DarkRed)});
            cc.WriteLine($"[{DateTime.Now.ToLocalTime()}] {msg}");
        }

        /// <summary>
        /// ファイルエンコーディングのリスト
        /// </summary>
        public enum StreamEncoding {
            utf8,
            utf16be,
            utf16le,
            utf16bem,
            utf16lem,
            system
        }

        /// <summary>
        /// StreamEncodingからSystem.Text.Encodingに変える君
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static Encoding GetEncoding(StreamEncoding e) {
            return e switch {
                StreamEncoding.utf8 => Encoding.UTF8,
                StreamEncoding.utf16be => new UnicodeEncoding(true, false),
                StreamEncoding.utf16bem => new UnicodeEncoding(true, true),
                StreamEncoding.utf16le => new UnicodeEncoding(false, false),
                StreamEncoding.utf16lem => new UnicodeEncoding(false, true),
                StreamEncoding.system => Encoding.Default,
                _ => throw new ArgumentOutOfRangeException(nameof(e))
            };
        }

        /// <summary>
        /// 与えられたパスを見て、STDINを開くかファイルを開くか決める君
        /// </summary>
        /// <param name="path">パス。空文字か`-`でも良い</param>
        /// <returns>入力ストリーム</returns>
        /// <exception cref="FileNotFoundException">パスを与えたものの見つからなかったときに投げられる</exception>
        private static Stream GetStream(string path) {
            if (path.Length == 0 || path == "-") {
                return Console.OpenStandardInput();
            }

            if (!File.Exists(path)) throw new FileNotFoundException(path);

            return new StreamReader(path).BaseStream;
        }
    }
}