FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else, including appsettings.json files, and build
COPY . ./
RUN dotnet publish -c Release -o out ./HomeAssistant.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
# Copy the build output
COPY --from=build-env /app/out . 

# Ensure configuration files are copied
COPY appsettings.json .
COPY appsettings.docker.json .

ENTRYPOINT ["dotnet", "HomeAssistant.dll"]
