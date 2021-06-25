FROM mcr.microsoft.com/dotnet/sdk:3.1 AS builder

LABEL maintainer="john@doe.com"

WORKDIR /src/app
COPY ./code ./

WORKDIR /src/app/ExternalLibraries/RCV-Controller 
RUN dotnet publish Rcv.ControllerBase -c Release -o /src/app/out && \
    dotnet pack /p:Version=1.0.0 -o /src/app/packages

WORKDIR /src/app/ExternalLibraries/RCV-HttpUtils
RUN dotnet publish Rcv.HttpUtils -c Release -o /src/app/out && \
    dotnet pack /p:Version=1.4.0 -o /src/app/packages

WORKDIR /src/app/ExternalLibraries/RCV-FileContainer/RCV-FileContainer
RUN dotnet publish RCV-FileContainer -c Release -o /src/app/out && \
    dotnet pack /p:Version=2.5.2 -o /src/app/packages

WORKDIR /src/app/ExternalLibraries/RCV-FileUtils
RUN dotnet publish Rcv.FileUtils -c Release -o /src/app/out && \
    dotnet pack /p:Version=1.0.0 -o /src/app/packages

WORKDIR /src/app/ExternalLibraries/RCV-Imagine
RUN dotnet publish Robotron.Imagine -f netcoreapp3.1 -c Release -o /src/app/out && \ 
    dotnet pack /p:Version=1.2.2 -o /src/app/packages

WORKDIR /src/app/ExternalLibraries/RCV-SwaggerExt
RUN dotnet publish Rcv.SwaggerExtensions -c Release -o /src/app/out && \
    dotnet pack /p:Version=1.0.0 -o /src/app/packages

WORKDIR /src/app/
RUN dotnet restore Rcv.Labeltool.Backend -s /src/app/packages -s https://api.nuget.org/v3/index.json && \
    dotnet publish Rcv.Labeltool.Backend -c Release -o /src/app/out

FROM mcr.microsoft.com/dotnet/aspnet:3.1

LABEL maintainer="john@doe.com"

WORKDIR /app
EXPOSE 80

COPY --from=builder /src/app/out .
COPY ./config/appsettings.json .

ENTRYPOINT ["dotnet", "Rcv.LabelTool.Backend.dll"]