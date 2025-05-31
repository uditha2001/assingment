# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project file and restore dependencies
COPY ProductService.API.csproj ./
RUN dotnet restore

# Copy all files and publish
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose internal port
EXPOSE 8080

# Run the service
ENTRYPOINT ["dotnet", "ProductService.API.dll"]
