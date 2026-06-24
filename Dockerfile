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

RUN echo "=== Searching for blazor.web.js ===" && \
    find / -name "blazor.web*" 2>/dev/null | head -30 || true

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "DrugstoreSystem.Web.dll"]
