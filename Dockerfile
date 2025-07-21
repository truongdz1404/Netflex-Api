# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
# Copy trực tiếp từ build context thay vì từ build stage
COPY netflex-storage-key.json ./netflex-storage-key.json

ENV ASPNETCORE_HTTP_PORTS=8888
EXPOSE 8888

ENTRYPOINT ["dotnet", "Netflex.dll"]