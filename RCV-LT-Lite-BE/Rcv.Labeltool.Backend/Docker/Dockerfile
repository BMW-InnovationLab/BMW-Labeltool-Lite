FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS builder
WORKDIR /src/app

COPY . ./
RUN dotnet publish Rcv.Labeltool.Backend -c Release -o out


FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
EXPOSE 80

COPY --from=builder /src/app/out .
ENTRYPOINT ["dotnet", "Rcv.LabelTool.Backend.dll"