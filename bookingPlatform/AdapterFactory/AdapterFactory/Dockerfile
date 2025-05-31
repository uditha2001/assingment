# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy and restore only the .csproj file first (for caching)
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the files and build the application
COPY . ./
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose the port used by the app inside the container
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "AdapterFactory.dll"]
