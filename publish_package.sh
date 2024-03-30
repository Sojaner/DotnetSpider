#!/usr/bin/env bash
echo $NUGET_SERVER
rm -rf src/LucasSpider/bin/Release
rm -rf src/LucasSpider.HBase/bin/Release
rm -rf src/LucasSpider.Mongo/bin/Release
rm -rf src/LucasSpider.MySql/bin/Release
rm -rf src/LucasSpider.PostgreSql/bin/Release
rm -rf src/LucasSpider.RabbitMQ/bin/Release
dotnet build -c Release
dotnet pack -c Release
dotnet nuget push src/LucasSpider/bin/Release/*.nupkg -s $NUGET_SERVER -k $NUGET_KEY --skip-duplicate
dotnet nuget push src/LucasSpider.HBase/bin/Release/*.nupkg -s $NUGET_SERVER -k $NUGET_KEY --skip-duplicate
dotnet nuget push src/LucasSpider.Mongo/bin/Release/*.nupkg -s $NUGET_SERVER -k $NUGET_KEY --skip-duplicate
dotnet nuget push src/LucasSpider.MySql/bin/Release/*.nupkg -s $NUGET_SERVER -k $NUGET_KEY --skip-duplicate
dotnet nuget push src/LucasSpider.PostgreSql/bin/Release/*.nupkg -s $NUGET_SERVER -k $NUGET_KEY --skip-duplicate
dotnet nuget push src/LucasSpider.RabbitMQ/bin/Release/*.nupkg -s $NUGET_SERVER -k $NUGET_KEY --skip-duplicate