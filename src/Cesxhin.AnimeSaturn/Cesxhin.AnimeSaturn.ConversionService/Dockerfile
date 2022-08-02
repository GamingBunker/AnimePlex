#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Cesxhin.AnimeSaturn/Cesxhin.AnimeSaturn.ConversionService/", "./Cesxhin.AnimeSaturn.ConversionService/"]
COPY ["src/Cesxhin.AnimeSaturn/Cesxhin.AnimeSaturn.Application/", "./Cesxhin.AnimeSaturn.Application/"]
COPY ["src/Cesxhin.AnimeSaturn/Cesxhin.AnimeSaturn.Domain/", "./Cesxhin.AnimeSaturn.Domain/"]

RUN dotnet restore "./Cesxhin.AnimeSaturn.ConversionService/Cesxhin.AnimeSaturn.ConversionService.csproj"
COPY . .
WORKDIR "./Cesxhin.AnimeSaturn.ConversionService"

RUN dotnet build "Cesxhin.AnimeSaturn.ConversionService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cesxhin.AnimeSaturn.ConversionService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["apt", "update"]
ENTRYPOINT ["apt", "install", "-y", "ffmpeg"]
ENTRYPOINT ["dotnet", "Cesxhin.AnimeSaturn.ConversionService.dll"]