#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Electricity.Api/Electricity.Api.csproj", "Electricity.Api/"]
COPY ["Electricity.Application/Electricity.Application.csproj", "Electricity.Application/"]
COPY ["Electricity.Core/Electricity.Core.csproj", "Electricity.Core/"]
COPY ["Electricity.Infrastructure/Electricity.Infrastructure.csproj", "Electricity.Infrastructure/"]
COPY ["Electricity.Persistence/Electricity.Persistence.csproj", "Electricity.Persistence/"]
RUN dotnet restore "./Electricity.Api/./Electricity.Api.csproj"
COPY . .
WORKDIR "/src/Electricity.Api"
RUN dotnet build "./Electricity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Electricity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Electricity.Api.dll"]