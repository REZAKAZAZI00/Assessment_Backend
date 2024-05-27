#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Assessment_Backend/Assessment_Backend.csproj", "Assessment_Backend/"]
COPY ["Assessment_Backend.Core/Assessment_Backend.Core.csproj", "Assessment_Backend.Core/"]
COPY ["Assessment_Backend.DataLayer/Assessment_Backend.DataLayer.csproj", "Assessment_Backend.DataLayer/"]
RUN dotnet restore "./Assessment_Backend/Assessment_Backend.csproj"
COPY . .
WORKDIR "/src/Assessment_Backend"
RUN dotnet build "./Assessment_Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Assessment_Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Assessment_Backend.dll"]