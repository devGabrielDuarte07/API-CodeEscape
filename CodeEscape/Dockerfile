FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore "CodeEscape.csproj"

RUN dotnet publish "CodeEscape.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/wwwroot/uploads/avatars
RUN mkdir -p /app/wwwroot/uploads/capas

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "CodeEscape.dll"]