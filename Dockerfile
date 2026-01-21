# ===== BUILD STAGE =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj & restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy everything else & publish
COPY . ./
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ===== RUNTIME STAGE =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Railway / cloud platforms expect port 8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TokoApi.dll"]
