# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy only the project file and restore dependencies
COPY AbcMockService.csproj ./
RUN dotnet restore

# Copy the rest of the application and publish
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose the container's internal port
EXPOSE 8080

# Start the service
ENTRYPOINT ["dotnet", "AbcMockService.dll"]
