# RCV Labeltool Backend

This container is responsible for hosting RCV Labeltool Backend in Docker environment. The
container acts as mounted container.

# **Code**
RCV Labeltool Backend provides a WebAPI which is based on ASP.NET Core 2.1. The code files
(dll) for the backend can be found in code folder.

# **Documentation**

## How to configure
Configuration of RCV Labeltool backend can be done by editing the appsettings.json file
inside the code folder.

### Configuration of on premise systems
RCV labeltool backend should be configured for on premise systems. On premise systems will 
store the backend data inside a file container on host system. It's neccessary to set
Mode to OnPremise when working with local file storages. Also OnPremise-Settings has
to be set!

The following example illustrates the appsettings for on premise configuration of RCV
Labeltool backend. Training data (images, labels) will be written to 
trainingdata directory.
The directory paths can be bound outside of the container.
```
"AppSetttings" : {
	"OnPremise" : {
		"TrainingDataDirectoryPath" : "/training-data"
	}
}
```

## How to build

docker build -t rcv.labeltool.backend .

## How to run

Run RCV Labeltool Backend with the following docker command. The description of all
flags can be found in docker reference.
RCV Labeltool Backend will run on internal port 80. This port will be forwarded to an
user defined port, e.g. 8070. RCV Backend will run at http://localhost:8070

docker run -d
	-v /c//rcv/training-data:/training-data  
	--rm -p 8070:80 rcv.labeltool.backend:latest

