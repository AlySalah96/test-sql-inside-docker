# ===== مرحلة البناء (SDK) =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# انسخ ملف المشروع واعمل restore
COPY *.csproj ./
RUN dotnet restore

# انسخ باقي الملفات وابني المشروع
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ===== مرحلة التشغيل (Runtime فقط) =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "testSQLInsideDocker.dll"]
