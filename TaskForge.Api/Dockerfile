# Use the official .NET 8 ASP.NET Core runtime as a base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# FIX: Aligned with your actual shortened project names and folder structures
COPY ["TaskForge.Api/TaskForge.Api.csproj", "TaskForge.Api/"]
COPY ["TaskForge.Application/Application.csproj", "TaskForge.Application/"]
COPY ["TaskForge.Domain/Domain.csproj", "TaskForge.Domain/"]
COPY ["TaskForge.Infrastructure.Data/Infrastructure.Data.csproj", "TaskForge.Infrastructure.Data/"]
COPY ["TaskForge.Infrastructure.IoC/Infrastructure.IoC.csproj", "TaskForge.Infrastructure.IoC/"]

# Run restore on the API project
RUN dotnet restore "TaskForge.Api/TaskForge.Api.csproj"

# Copy the entire source code and build it
COPY . .
WORKDIR "/src/TaskForge.Api"
RUN dotnet build "TaskForge.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TaskForge.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage: copy published output to the runtime environment
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskForge.Api.dll"]