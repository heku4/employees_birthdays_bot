FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BirthdayBot/BirthdayBot.csproj", "BirthdayBot/"]
RUN dotnet restore "BirthdayBot/BirthdayBot.csproj"
COPY . .
WORKDIR "/src/BirthdayBot"
RUN dotnet build "BirthdayBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BirthdayBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BirthdayBot.dll"]
