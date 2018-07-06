FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY SituationCenterCore/SituationCenterCore.csproj SituationCenterCore/
COPY SituationCenter.Shared/SituationCenter.Shared.csproj SituationCenter.Shared/
COPY SituationCenterBackServer.Interfaces/SituationCenterBackServer.Interfaces.csproj SituationCenterBackServer.Interfaces/
RUN dotnet restore SituationCenterCore/SituationCenterCore.csproj
COPY . .
WORKDIR /src/SituationCenterCore
RUN dotnet build SituationCenterCore.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish SituationCenterCore.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SituationCenterCore.dll"]
