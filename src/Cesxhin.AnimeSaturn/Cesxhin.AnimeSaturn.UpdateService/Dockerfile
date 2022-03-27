#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Cesxhin.AnimeSaturn.UpdateService/Cesxhin.AnimeSaturn.UpdateService.csproj", "Cesxhin.AnimeSaturn.UpdateService/"]
COPY ["Cesxhin.AnimeSaturn.Domain/Cesxhin.AnimeSaturn.Domain.csproj", "Cesxhin.AnimeSaturn.Domain/"]
COPY ["Cesxhin.AnimeSaturn.Application/Cesxhin.AnimeSaturn.Application.csproj", "Cesxhin.AnimeSaturn.Application/"]
RUN dotnet restore "Cesxhin.AnimeSaturn.UpdateService/Cesxhin.AnimeSaturn.UpdateService.csproj"
COPY . .
WORKDIR "/src/Cesxhin.AnimeSaturn.UpdateService"
RUN dotnet build "Cesxhin.AnimeSaturn.UpdateService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cesxhin.AnimeSaturn.UpdateService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cesxhin.AnimeSaturn.UpdateService.dll"]