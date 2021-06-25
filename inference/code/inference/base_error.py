import logging
from datetime import datetime
from abc import ABC, abstractmethod


class AbstractError(ABC):

    def __init__(self):
        """
        Sets the logger file, level, and format.
        The logging file will contain the logging level, request date, request status, and model response.
        """
        self.logger = logging.getLogger('logger')
        date = datetime.now().strftime('%Y-%m-%d')
        file_path = 'logs/' + date + '.log'
        self.handler = logging.FileHandler(file_path)
        self.handler.setLevel(logging.INFO)
        self.handler.setFormatter(logging.Formatter("%(levelname)s;%(asctime)s;%(message)s"))
        self.logger.addHandler(self.handler)

    @abstractmethod
    def info(self, message):
        """
        Logs an info message to the logging file.
        :param message: Containing the request status and the model response
        :return:
        """
        pass

    @abstractmethod
    def warning(self, message):
        """
        Logs a warning message to the logging file.
        :param message: Containing the request status and the model response
        :return:
        """
        pass

    @abstractmethod
    def error(self, message):
        """
        Logs an Error message to the logging file.
        :param message: Containing the request status and the model response
        :return:
        """
        pass
