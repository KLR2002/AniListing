# 1. Build Frontend
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-front
WORKDIR /src
COPY AniListingFront/ AniListingFront/
RUN dotnet publish AniListingFront/AniListingFront.csproj -c Release -o /app/front

# 2. Build Backend
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-api
WORKDIR /src
COPY AniListingAPI/ AniListingAPI/
RUN dotnet publish AniListingAPI/AniListingAPI.csproj -c Release -o /app/api

# 3. Final Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build-api /app/api .
# Copy published Blazor WASM files directly into the API's wwwroot
COPY --from=build-front /app/front/wwwroot ./wwwroot

# Expose Render's standard port
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "AniListingAPI.dll"]
