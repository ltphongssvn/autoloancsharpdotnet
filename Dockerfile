FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY AutoLoan.Shared/*.csproj AutoLoan.Shared/
COPY AutoLoan.Api/*.csproj AutoLoan.Api/
RUN dotnet restore AutoLoan.Api/AutoLoan.Api.csproj
COPY AutoLoan.Shared/ AutoLoan.Shared/
COPY AutoLoan.Api/ AutoLoan.Api/
RUN dotnet publish AutoLoan.Api/AutoLoan.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
ARG CACHEBUST=20260205151427
RUN apt-get update && apt-get install -y libkrb5-3 libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "AutoLoan.Api.dll"]

