# Use the official .NET Core SDK image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy the fsproj and restore any dependencies

# COPY ./bin/Debug/net7.0/ ./

COPY *.fsproj ./
RUN dotnet restore

COPY . ./
RUN dotnet build

# Copy the rest of the application and build it


# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "orderssystem_core.dll"]
