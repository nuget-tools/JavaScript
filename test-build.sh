#! /usr/bin/env bash
set -uvx
set -e
cwd=`pwd`
ts=`date "+%Y.%m%d.%H%M.%S"`
version="${ts}"
cd $cwd/JavaScript
#sed -i -e "s/<Version>.*<\/Version>/<Version>${version}<\/Version>/g" JavaScript.csproj
rm -rf obj bin
java -jar ./antlr-4.12.0-complete.jar JSON5.g4 -Dlanguage=CSharp -package JavaScript.Parser.Json5 -o Parser/Json5
dotnet build -c Rlease
cd $cwd/JavaScript.Json
dotnet run -c Release -f net462
dotnet run -c Release -f net6.0
cd $cwd/JavaScript.Main
dotnet run -c Release -f net462
dotnet run -c Release -f net6.0
cd $cwd/JavaScript.Script
dotnet run -c Release -f net462
dotnet run -c Release -f net6.0
cd $cwd
echo "# JavaScript" > README.md
echo "" >> README.md
echo "\`\`\`" >> README.md
iconv -f cp932 -t utf-8 JavaScript.Main/Program.cs >> README.md
echo "\`\`\`" >> README.md
echo "" >> README.md
echo "# GScript (JavaScript) Example" >> README.md
echo "" >> README.md
echo "\`\`\`" >> README.md
iconv -f cp932 -t utf-8 JavaScript.Script/Program.cs >> README.md
echo "\`\`\`" >> README.md
echo "" >> README.md
echo "# Dynamic Data Example" >> README.md
echo "" >> README.md
echo "\`\`\`" >> README.md
iconv -f cp932 -t utf-8 JavaScript.Json/Program.cs >> README.md
echo "\`\`\`" >> README.md

exit 0

cd $cwd/JavaScript
sed -i -e "s/<Version>.*<\/Version>/<Version>${version}<\/Version>/g" JavaScript.csproj
rm -rf obj bin
java -jar ./antlr-4.12.0-complete.jar JSON5.g4 -Dlanguage=CSharp -package JavaScript.Parser.Json5 -o Parser/Json5
rm -rf *.nupkg
dotnet pack -c Rlease -o .
cd $cwd
git add .
git commit -m"JavaScript v$version"
git tag -a v$ts -mv$version
git push origin v$version
git push
git remote -v
