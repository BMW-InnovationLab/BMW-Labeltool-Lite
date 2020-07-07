FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS builder
WORKDIR /src/app


COPY . ./
RUN wget 'https://dist.nuget.org/win-x86-commandline/v4.1.0/nuget.exe'

WORKDIR /src/app/ExternalLibraries/RCV-Controller 
RUN dotnet publish Rcv.ControllerBase -c Release -o /src/app/out 
RUN dotnet pack /p:Version=1.0.0 -o /src/app/packages

WORKDIR /src/app/ExternalLibraries/RCV-HttpUtils
RUN dotnet publish Rcv.HttpUtils -c Release -o /src/app/out
RUN dotnet pack /p:Version=1.4.0 -o /src/app/packages

WORKDIR /src/app/ExternalLibraries/RCV-ScriptHost
RUN ls
RUN dotnet restore Rcv.ScriptHost -s /src/app/packages -s https://api.nuget.org/v3/index.json
RUN dotnet publish Rcv.ScriptHost -c Release -o /src/app/out
RUN dotnet pack /p:Version=1.2.7 -o /src/app/packages

WORKDIR /src/app/ExternalLibraries/RCV-FileContainer/RCV-FileContainer
RUN dotnet publish RCV-FileContainer -c Release -o /src/app/out
RUN dotnet pack /p:Version=2.5.2 -o /src/app/packages

WORKDIR /src/app/ExternalLibraries/RCV-Babel
RUN dotnet publish RCV.Babel -c Release -o /src/app/out
RUN dotnet pack /p:Version=1.1.0 -o /src/app/packages

WORKDIR /src/app/ExternalLibraries/RCV-FileUtils
RUN dotnet publish Rcv.FileUtils -c Release -o /src/app/out
RUN dotnet pack /p:Version=1.0.0 -o /src/app/packages

WORKDIR /src/app/ExternalLibraries/RCV-Imagine
RUN dotnet publish Robotron.Imagine -c Release -o /src/app/out 
RUN dotnet pack /p:Version=1.2.2 -o /src/app/packages



WORKDIR /src/app/
RUN dotnet restore Rcv.Labeltool.Backend -s /src/app/packages -s https://api.nuget.org/v3/index.json
RUN dotnet publish Rcv.Labeltool.Backend -c Release -o /src/app/out

FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
EXPOSE 80

COPY --from=builder /src/app/out .
COPY ./Rcv.Labeltool.Backend/appsettings.json .

ENTRYPOINT ["dotnet", "Rcv.LabelTool.Backend.dll"]