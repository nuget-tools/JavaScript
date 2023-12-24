#! /usr/bin/env bash
set -vx
set -e
cwd=`pwd`
if [ "$LANG" = "" ]; then
  rm -rf bin obj
  dotnet build -f net462 -c Release -r win-x64 JavaScript.Script.csproj
  vbpack.exe -i bin/x64/Release/net462/win-x64/JavaScript.Script.exe -o ./bin/JavaScript.Script.exe
  ls -lh ./bin/JavaScript.Script.exe
else
  dotnet publish -f net6.0 -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true JavaScript.Script.csproj
fi
