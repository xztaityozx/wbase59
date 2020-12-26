# wbase59
ワタナベース59。それ以外の説明はないよ

## Usage
```zsh
# エンコード
$ echo abc | wbase59
邉邉󠄑辺󠄁邊邉󠄒辺󠄁辺邉󠄓辺󠄁

# デコード
$ echo 邉邉󠄑辺󠄁邊邉󠄒辺󠄁辺邉󠄓辺󠄁 | wbase59 -d
abc

# 入力/出力のエンコードを指定できる
$ echo abc | wbase59 --outputEncoding utf16le | wbase59 -d --inputEncoding utf16le
abc
```

## Install

### Build from source

#### Requirements
- .NET 5

```zsh
# 1. git cloneします
$ git clone https://github.com/xztaityozx/wbase59

# 2. cdします
$ cd ./wbase59/wbase59

# 3. dotnet publishします
# linuxの場合
$ dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o ./linux-x64
# macOSの場合
$ dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -o ./osx-x64
# winの場合
$ dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./win-x64

# 4. 出力先のディレクトリの`wbase59`が実行ファイルです
$ ./linux-x64/wbase59 --help
$ ./win-x64/wbase59 --help
$ ./osx-x64/wbase59 --help
```

### Download from GitHub Releases
準備中っす
