dotnet publish  -c Release
cp -r src/LucasSpider.Agent/bin/Release/netcoreapp3.1/publish/ dockerfile/agent/out
cd dockerfile/agent || exit
docker build -t lucasspider/agent:latest .
