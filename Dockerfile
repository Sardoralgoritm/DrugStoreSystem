FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY DrugstoreSystem.slnx .
COPY src/DrugstoreSystem.Domain/DrugstoreSystem.Domain.csproj src/DrugstoreSystem.Domain/
COPY src/DrugstoreSystem.Application/DrugstoreSystem.Application.csproj src/DrugstoreSystem.Application/
COPY src/DrugstoreSystem.Infrastructure/DrugstoreSystem.Infrastructure.csproj src/DrugstoreSystem.Infrastructure/
COPY src/DrugstoreSystem.Web/DrugstoreSystem.Web.csproj src/DrugstoreSystem.Web/
COPY tests/DrugstoreSystem.UnitTests/DrugstoreSystem.UnitTests.csproj tests/DrugstoreSystem.UnitTests/
RUN dotnet restore

COPY . .
RUN dotnet publish src/DrugstoreSystem.Web/DrugstoreSystem.Web.csproj \
    -c Release -o /app/publish --no-restore

# blazor.web.js is not copied by publish in server-only mode — find and copy it manually
RUN mkdir -p /app/publish/wwwroot/_framework && \
    BLAZOR_JS=$(find /root/.nuget/packages -name "blazor.web.js" 2>/dev/null \
        | grep -v "\.br$" | grep -v "\.gz$" | head -1) && \
    if [ -n "$BLAZOR_JS" ]; then \
        cp "$BLAZOR_JS" /app/publish/wwwroot/_framework/blazor.web.js && \
        echo "Copied blazor.web.js from $BLAZOR_JS"; \
    else \
        echo "ERROR: blazor.web.js not found" && exit 1; \
    fi

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "DrugstoreSystem.Web.dll"]
