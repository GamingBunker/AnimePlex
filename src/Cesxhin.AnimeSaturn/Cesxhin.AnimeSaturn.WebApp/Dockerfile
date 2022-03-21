#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Cesxhin.AnimeSaturn.WebApp/Cesxhin.AnimeSaturn.WebApp.csproj", "Cesxhin.AnimeSaturn.WebApp/"]
COPY ["Cesxhin.AnimeSaturn.Domain/Cesxhin.AnimeSaturn.Domain.csproj", "Cesxhin.AnimeSaturn.Domain/"]
COPY ["Cesxhin.AnimeSaturn.Application/Cesxhin.AnimeSaturn.Application.csproj", "Cesxhin.AnimeSaturn.Application/"]
RUN dotnet restore "Cesxhin.AnimeSaturn.WebApp/Cesxhin.AnimeSaturn.WebApp.csproj"
COPY . .
WORKDIR "/src/Cesxhin.AnimeSaturn.WebApp"
RUN dotnet build "Cesxhin.AnimeSaturn.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cesxhin.AnimeSaturn.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cesxhin.AnimeSaturn.WebApp.dll"]