FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
RUN dotnet dev-certs https
WORKDIR /app

COPY *.sln .
COPY MarkAsPlayed.Foundation/*.csproj MarkAsPlayed.Foundation/
COPY MarkAsPlayed.Setup/*.csproj MarkAsPlayed.Setup/
COPY MarkAsPlayed.Api/*.csproj MarkAsPlayed.Api/
COPY MarkAsPlayed.Api.Tests/*.csproj MarkAsPlayed.Api.Tests/
RUN dotnet restore MarkAsPlayed.sln

COPY . ./
RUN dotnet build MarkAsPlayed.sln -c Release --no-restore