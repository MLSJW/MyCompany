FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

# Используем файл решения
RUN dotnet restore "MyCompany.sln"
RUN dotnet publish "MyCompany.sln" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .
COPY appsettings.json ./
ENTRYPOINT ["dotnet", "MyCompany.dll"]