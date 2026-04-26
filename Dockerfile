FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем ВСЁ (включая подпапки)
COPY . .

# Восстанавливаем зависимости (найдёт .csproj автоматически)
RUN dotnet restore

# Публикуем
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MyCompany.dll"]