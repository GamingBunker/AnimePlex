#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Cesxhin.AnimeSaturn.Api/Cesxhin.AnimeSaturn.Api.csproj", "Cesxhin.AnimeSaturn.Api/"]
COPY ["Cesxhin.AnimeSaturn.Domain/Cesxhin.AnimeSaturn.Domain.csproj", "Cesxhin.AnimeSaturn.Domain/"]
COPY ["Cesxhin.AnimeSaturn.Persistence/Cesxhin.AnimeSaturn.Persistence.csproj", "Cesxhin.AnimeSaturn.Persistence/"]
COPY ["Cesxhin.AnimeSaturn.Application/Cesxhin.AnimeSaturn.Application.csproj", "Cesxhin.AnimeSaturn.Application/"]
RUN dotnet restore "Cesxhin.AnimeSaturn.Api/Cesxhin.AnimeSaturn.Api.csproj"
COPY . .
WORKDIR "/src/Cesxhin.AnimeSaturn.Api"
RUN dotnet build "Cesxhin.AnimeSaturn.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cesxhin.AnimeSaturn.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cesxhin.AnimeSaturn.Api.dll"]