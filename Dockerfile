FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем всё из папки с проектом (там есть .csproj)
COPY . .

# Восстанавливаем зависимости (.csproj уже здесь)
RUN dotnet restore

# Публикуем
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Копируем результат
COPY --from=build /app/publish .

# Запускаем
ENTRYPOINT ["dotnet", "MyCompany.dll"]