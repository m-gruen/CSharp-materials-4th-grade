FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine-aot AS build
WORKDIR /src

COPY Alpaka.Api.csproj .
RUN dotnet restore -r linux-musl-x64

COPY . .
RUN dotnet publish -c Release -r linux-musl-x64 --self-contained true /p:PublishAot=true /p:InvariantGlobalization=true -o /app/publish

FROM alpine:3.23.3 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8081
EXPOSE 8081

COPY --from=build /app/publish/ ./
ENTRYPOINT ["./Alpaka.Api"]
