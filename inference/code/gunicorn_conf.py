import json
import multiprocessing
import os

workers_per_core_str = os.getenv("WORKERS_PER_CORE", "1")
web_concurrency_str = os.getenv("WEB_CONCURRENCY", None)
host = os.getenv("HOST", "0.0.0.0")
port = os.getenv("PORT", "80")
bind_env = os.getenv("BIND", None)
use_loglevel = os.getenv("LOG_LEVEL", "info")
if bind_env:
    use_bind = bind_env
else:
    use_bind = f"{host}:{port}"

cores = multiprocessing.cpu_count()
workers_per_core = float(workers_per_core_str)
default_web_concurrency = workers_per_core * cores
if web_concurrency_str:
    web_concurrency = int(web_concurrency_str)
    assert web_concurrency > 0
else:
    web_concurrency = max(int(default_web_concurrency), 2)

# Gunicorn config variables
loglevel = use_loglevel
workers = web_concurrency
bind = use_bind
keepalive = 120
errorlog = "-"
accesslog = "-"
logconfig_dict = {
    "version": 1,
    "disable_existing_loggers": True,
    "formatters": {
        "generic": {
            "format": "%(asctime)s [%(name)s] %(levelname)s %(message)s"
        }
    },
    "handlers": {
        "console": {
            "level": loglevel.upper(),
            "formatter": "generic",
            "class": "logging.StreamHandler",
            "stream": "ext://sys.stdout"
        }
    },
    "loggers": {
        "": {  # root logger
            "level": loglevel.upper(),
            "handlers": ["console"],
        },
        "gunicorn": {
            "level": "INFO",
            "propagate": True
        },
        "uvicorn": {
            "level": "INFO",
            "propagate": True
        },
        "uvicorn.access": {
            "level": "INFO",
            "propagate": True,
            "handlers": ["console"]
        },
        "gunicorn.access": {
            "level": "INFO",
            "propagate": True,
            "handlers": ["console"]
        },
        "fastapi": {
            "level": "INFO",
            "propagate": True
        }
    }
}

# CUSTOM
timeout = 3600

log_data = {
    "loglevel": loglevel,
    "workers": workers,
    "bind": bind,
    "timeout": timeout,
    "workers_per_core": workers_per_core,
    "host": host,
    "port": port,
}
print(json.dumps(log_data))