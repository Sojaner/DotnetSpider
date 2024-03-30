cd src/LucasSpider.Portal && yarn install
dotnet publish  -c Release
cd ../.. || exit
cp -r src/LucasSpider.Portal/bin/Release/netcoreapp3.1/publish/ dockerfile/portal/out
cd dockerfile/portal || exit
docker build -t lucasspider/portal:latest .