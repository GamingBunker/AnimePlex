#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ["src/Cesxhin.AnimeSaturn/Cesxhin.AnimeSaturn.UpdateService/", "./Cesxhin.AnimeSaturn.UpdateService/"]
COPY ["src/Cesxhin.AnimeSaturn/Cesxhin.AnimeSaturn.Domain/", "./Cesxhin.AnimeSaturn.Domain/"]
COPY ["src/Cesxhin.AnimeSaturn/Cesxhin.AnimeSaturn.Application/", "./Cesxhin.AnimeSaturn.Application/"]

RUN dotnet restore "./Cesxhin.AnimeSaturn.UpdateService/Cesxhin.AnimeSaturn.UpdateService.csproj"

COPY . .
WORKDIR "./Cesxhin.AnimeSaturn.UpdateService"

RUN dotnet build "Cesxhin.AnimeSaturn.UpdateService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cesxhin.AnimeSaturn.UpdateService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cesxhin.AnimeSaturn.UpdateService.dll"]