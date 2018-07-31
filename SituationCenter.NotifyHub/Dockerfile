FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY SituationCenter.NotifyHub/SituationCenter.NotifyHub.csproj SituationCenter.NotifyHub/
RUN dotnet restore SituationCenter.NotifyHub/SituationCenter.NotifyHub.csproj
COPY . .
WORKDIR /src/SituationCenter.NotifyHub
RUN dotnet build SituationCenter.NotifyHub.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish SituationCenter.NotifyHub.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SituationCenter.NotifyHub.dll"]
