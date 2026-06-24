FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /src

COPY DrugstoreSystem.slnx .
COPY src/DrugstoreSystem.Domain/DrugstoreSystem.Domain.csproj src/DrugstoreSystem.Domain/
COPY src/DrugstoreSystem.Application/DrugstoreSystem.Application.csproj src/DrugstoreSystem.Application/
COPY src/DrugstoreSystem.Infrastructure/DrugstoreSystem.Infrastructure.csproj src/DrugstoreSystem.Infrastructure/
COPY src/DrugstoreSystem.Web/DrugstoreSystem.Web.csproj src/DrugstoreSystem.Web/
COPY tests/DrugstoreSystem.UnitTests/DrugstoreSystem.UnitTests.csproj tests/DrugstoreSystem.UnitTests/
RUN dotnet restore

COPY . .
RUN dotnet build src/DrugstoreSystem.Web/DrugstoreSystem.Web.csproj -c Debug --no-restore

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Run from the project directory so the staticwebassets runtime manifest
# (which maps _framework/blazor.web.js to the in-container NuGet cache) is found.
WORKDIR /src/src/DrugstoreSystem.Web
ENTRYPOINT ["dotnet", "bin/Debug/net10.0/DrugstoreSystem.Web.dll"]
