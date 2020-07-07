# RCV Labeltool LITE

This container is responsible for hosting RCV Labeltool LITE in Docker environment. The container
can be used in Standalone mode.

# **Code**

RCV Labeltool LITE is a webapplication based on nginx image. The code for the dashboard can be
found in code folder.

# **Data (static)**

## nginx.conf

Configuation of ningx server can be found in nginx.conf file of static data folder. This file
describes the behaviour of the hosted nginx server inside the docker container.

# **Documentation**

## How to configure

RCV Labeltool LITE interacts with RCV Labeltool LITE Backend which is responsible for hosting
labeltool data and operations. RCV Labeltool LITE interacts as webapplication.

The configuration file can be found at code-folder 'assets/config/config.json'.

### Configuration of RCV Labeltool LITE-Backend

The RCV Labeltool LITE Backend which should be used by the RCV Labeltool LITE is configured
within the property "apiUrl". The suffix "/api" has to be added.

Example:

```
  "apiUrl": "http://labeltool-lite-backend.de/api"
```

## How to build

docker build -t rcv.labeltool.lite .

## How to run

Run RCV Labeltool LITE with the following docker command. The description of all
flags can be found in docker reference.
RCV Labeltool LITE will run on internal port 80. This port will be forwarded to an
user defined port, e.g. 9000. RCV Labeltool LITE will run at http://localhost:9000

docker run -d --rm -p 9000:80 rcv.labeltool.lite:latest
