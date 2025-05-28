# Base image with ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image with .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy and restore dependencies
COPY ["WebApps/AspNetRunBasic/AspNetRunBasic.csproj", "WebApps/AspNetRunBasic/"]
RUN dotnet restore "WebApps/AspNetRunBasic/AspNetRunBasic.csproj"

# Copy full source and build
COPY . .
WORKDIR "/src/WebApps/AspNetRunBasic"
RUN dotnet build "AspNetRunBasic.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the app
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AspNetRunBasic.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AspNetRunBasic.dll"]
