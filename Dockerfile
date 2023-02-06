#ATTENTION: This dockerFile uses the output of the publish build. In order to build the complete solution
#including plugins, execute the command 'dotnet publish -c Release'. After this, you can do a docker build.
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.16 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80
EXPOSE 443
RUN apk --no-cache add curl

COPY "KingTech.P1Reader/bin/Release/net6.0" .
ENTRYPOINT ["dotnet", "KingTech.P1Reader.dll"]