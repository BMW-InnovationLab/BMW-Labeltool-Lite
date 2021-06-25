# Labeltool LITE

This container is responsible for hosting Labeltool LITE in Docker environment. The container
can be used in Standalone mode.

# **Code**

Labeltool LITE is a webapplication based on nginx image.

# **Data (static)**

## nginx.conf

Configuation of ningx server can be found in nginx.conf file of static data folder. This file
describes the behaviour of the hosted nginx server inside the docker container.

# **Documentation**

## How to configure

Labeltool LITE interacts with Labeltool LITE Backend which is responsible for hosting
labeltool data and operations. Labeltool LITE interacts as webapplication.

The configuration file can be found at code-folder 'assets/config/config.json'.

### Configuration of Labeltool LITE Backend

The Labeltool LITE Backend which should be used by the Labeltool LITE is configured
within the property "apiUrl". The suffix "/api" has to be added.

Example:

```
  "apiUrl": "/api"
```

## How to build

docker build -t labeltool.lite .

## How to run

Run Labeltool LITE with the following docker command.
Labeltool LITE will run on internal port 80. This port will be forwarded to an
user defined port, e.g. 8000.

docker run -d --rm -p 8000:80 labeltool.lite:latest
