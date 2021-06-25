# Labeltool LITE Backend

This container is responsible for hosting Labeltool LITE Backend in Docker environment. The
container acts as mounted container.

# **Code**
Labeltool LITE Backend provides a WebAPI which is based on ASP.NET Core 3.1. The code files
(dll) for the backend can be found in code folder.

# **Documentation**

## How to configure
Configuration of Labeltool LITE Backend can be done by editing the appsettings.json file
inside the code folder.

## How to build

docker build -t labeltool.lite.backend .

## How to run

Run Labeltool LITE Backend with the following docker command.
Labeltool LITE Backend will run on internal port 80. This port will be forwarded to an
user defined port, e.g. 8070. Labeltool LITE Backend will run at http://localhost:8070

docker run -d --rm -p 8070:80 labeltool.lite.backend:latest

