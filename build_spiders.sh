dotnet publish  -c Release
cp -r src/LucasSpider.Spiders/bin/Release/netcoreapp3.1/publish/ dockerfile/spiders/out
cd dockerfile/spiders || exit
docker build -t lucasspider/spiders:latest .