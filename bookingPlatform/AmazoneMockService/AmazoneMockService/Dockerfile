# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the app and publish it
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose container port
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "CdeMockService.dll"]
