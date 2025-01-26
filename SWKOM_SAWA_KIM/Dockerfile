# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and dependencies
COPY SWKOM_SAWA_KIM/SWKOM_SAWA_KIM.csproj ./SWKOM_SAWA_KIM/
COPY SWKOM_SAWA_KIM.BLL/SWKOM_SAWA_KIM.BLL.csproj ./SWKOM_SAWA_KIM.BLL/
COPY SWKOM_SAWA_KIM.DAL/SWKOM_SAWA_KIM.DAL.csproj ./SWKOM_SAWA_KIM.DAL/
COPY SWKOM_SAWA_KIM.RabbitMQ/SWKOM_SAWA_KIM.RabbitMQ.csproj ./SWKOM_SAWA_KIM.RabbitMQ/
COPY SWKOM_SAWA_KIM.Minio/SWKOM_SAWA_KIM.Minio.csproj ./SWKOM_SAWA_KIM.Minio/

# Restore dependencies
RUN dotnet restore "./SWKOM_SAWA_KIM/SWKOM_SAWA_KIM.csproj"

# Copy the rest of the source code
COPY . .

# Build the project
RUN dotnet build "./SWKOM_SAWA_KIM/SWKOM_SAWA_KIM.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
RUN dotnet publish "./SWKOM_SAWA_KIM/SWKOM_SAWA_KIM.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8081

# Copy the published app from the build stage
COPY --from=build /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "SWKOM_SAWA_KIM.dll"]
