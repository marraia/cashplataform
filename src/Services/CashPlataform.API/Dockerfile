#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update
RUN apt-get install -y locales locales-all
ENV LC_ALL pt_BR.UTF-8
ENV LANG pt_BR.UTF-8
ENV LANGUAGE pt_BR.UTF-8

RUN apt-get update \
    && apt-get install -y --no-install-recommends libgdiplus libc6-dev \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Services/CashPlataform.API/CashPlataform.API.csproj", "src/Services/CashPlataform.API/"]
COPY ["src/Modules/CashPlataform.Infrastructure.IoC/CashPlataform.Infrastructure.IoC.csproj", "src/Modules/CashPlataform.Infrastructure.IoC/"]
COPY ["src/Modules/CashPlataform.Application/CashPlataform.Application.csproj", "src/Modules/CashPlataform.Application/"]
COPY ["src/Adapters/Excel.Adapter/Excel.Adapter.csproj", "src/Adapters/Excel.Adapter/"]
COPY ["src/Modules/CashPlataform.Domain/CashPlataform.Domain.csproj", "src/Modules/CashPlataform.Domain/"]
COPY ["src/Modules/CashPlataform.Infrastructure.Repositories/CashPlataform.Infrastructure.Repositories.csproj", "src/Modules/CashPlataform.Infrastructure.Repositories/"]
RUN dotnet restore "src/Services/CashPlataform.API/CashPlataform.API.csproj"
COPY . .
WORKDIR "/src/src/Services/CashPlataform.API"
RUN dotnet build "CashPlataform.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CashPlataform.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CashPlataform.API.dll"]