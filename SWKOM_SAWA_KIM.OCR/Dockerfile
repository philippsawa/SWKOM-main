# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
# USER $APP_UID
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --allow-unauthenticated \
        libleptonica-dev \
        libtesseract-dev \
        ghostscript \
    && rm -rf /var/lib/apt/lists/*
RUN ln -s /usr/lib/x86_64-linux-gnu/libdl.so.2 /usr/lib/x86_64-linux-gnu/libdl.so
WORKDIR /app/x64
RUN ln -s /usr/lib/x86_64-linux-gnu/liblept.so.5 /app/x64/libleptonica-1.82.0.so
RUN ln -s /usr/lib/x86_64-linux-gnu/libtesseract.so.5 /app/x64/libtesseract50.so
WORKDIR /app

# copy appsettings.json to the container
COPY SWKOM_SAWA_KIM.OCR/appsettings.json /app/appsettings.json

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SWKOM_SAWA_KIM.OCR/SWKOM_SAWA_KIM.OCR.csproj", "SWKOM_SAWA_KIM.OCR/"]
RUN dotnet restore "./SWKOM_SAWA_KIM.OCR/SWKOM_SAWA_KIM.OCR.csproj"
COPY . .
WORKDIR "/src/SWKOM_SAWA_KIM.OCR"
RUN dotnet build "./SWKOM_SAWA_KIM.OCR.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SWKOM_SAWA_KIM.OCR.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy the tessdata folder into the container
COPY ./SWKOM_SAWA_KIM.OCR/tessdata /app/tessdata
COPY ./SWKOM_SAWA_KIM.OCR/docs /app/docs

ENTRYPOINT ["dotnet", "SWKOM_SAWA_KIM.OCR.dll"]