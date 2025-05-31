# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project file and restore
COPY OrderService.API.csproj ./
RUN dotnet restore

# Copy all other files and publish
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose internal container port
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "OrderService.API.dll"]
