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
RUN dotnet build src/DrugstoreSystem.Web/DrugstoreSystem.Web.csproj --no-restore

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
WORKDIR /src/src/DrugstoreSystem.Web
ENTRYPOINT ["dotnet", "run", "--no-restore", "--no-build", "--no-launch-profile"]
