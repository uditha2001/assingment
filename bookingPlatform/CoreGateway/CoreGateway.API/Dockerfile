# Stage 1: Build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy only the .csproj file and restore dependencies
COPY CoreGateway.API.csproj ./
RUN dotnet restore

# Copy the rest of the code and publish
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Run the application using a lightweight runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose the container port
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "CoreGateway.API.dll"]
