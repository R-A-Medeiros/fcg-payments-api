# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY nuget.config ./
COPY src/PaymentsAPI/PaymentsAPI.csproj src/PaymentsAPI/
RUN dotnet restore src/PaymentsAPI/PaymentsAPI.csproj

COPY . .
RUN dotnet publish src/PaymentsAPI/PaymentsAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "PaymentsAPI.dll"]
