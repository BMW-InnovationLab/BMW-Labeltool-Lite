import os
import logging
from datetime import datetime
from inference.base_error import AbstractError


class Error(AbstractError):

    def __init__(self):
        if 'logs' not in os.listdir():
            os.mkdir('logs')
        self.date = None
        super().__init__()

    def info(self, message):
        self.check_date()
        self.logger.info(message)

    def warning(self, message):
        self.check_date()
        self.logger.warning(message)

    def error(self, message):
        self.check_date()
        self.logger.error(message)

    def check_date(self):
        """
        Divides logging per day. Each logging file corresponds to a specific day.
        It also removes all logging files exceeding one year.
        :return:
        """
        self.date = datetime.now().strftime('%Y-%m-%d')
        file_path = self.date + '.log'
        if file_path not in os.listdir('logs'):
            self.logger.removeHandler(self.handler)
            self.handler = logging.FileHandler('logs/' + file_path)
            self.handler.setLevel(logging.INFO)
            self.handler.setFormatter(logging.Formatter("%(levelname)s;%(asctime)s;%(message)s"))
            self.logger.addHandler(self.handler)
        oldest_log_file = os.listdir('logs')[0]
        oldest_date = oldest_log_file.split('.')[0]
        a = datetime.strptime(datetime.now().strftime('%Y-%m-%d'), '%Y-%m-%d')
        b = datetime.strptime(oldest_date, '%Y-%m-%d')
        delta = a - b
        if delta.days > 365:
            os.remove('logs/' + oldest_log_file)
