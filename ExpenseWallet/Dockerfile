#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ExpenseWallet/ExpenseWallet.csproj", "ExpenseWallet/"]
RUN dotnet restore "ExpenseWallet/ExpenseWallet.csproj"
COPY . .
WORKDIR "/src/ExpenseWallet"
RUN dotnet build "ExpenseWallet.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExpenseWallet.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExpenseWallet.dll"]