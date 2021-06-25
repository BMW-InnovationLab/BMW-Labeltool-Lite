FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /code
EXPOSE 80

COPY /code .
ENTRYPOINT ["dotnet", "Rcv.LabelTool.Backend.dll"]