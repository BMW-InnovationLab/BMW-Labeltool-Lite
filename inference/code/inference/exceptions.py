__metaclass__ = type


class ApplicationError(Exception):
    """Base class for other exceptions"""

    def __init__(self, default_message, additional_message='', status_code=500):
        self.default_message = default_message
        self.additional_message = additional_message
        self.status_code = status_code

    def __str__(self):
        return self.get_message()

    def get_message(self):
        return self.default_message if self.additional_message == '' else "{}: {}".format(self.default_message,
                                                                                          self.additional_message)


class InvalidModelConfiguration(ApplicationError):
    """Raised when the model's configuration is corrupted"""

    def __init__(self, additional_message=''):
        # super('Invalid model configuration', additional_message)
        super().__init__('Invalid model configuration', additional_message, 404)


class ModelNotFound(ApplicationError):
    """Raised when the model is not found"""

    def __init__(self, additional_message=''):
        # super('Model not found', additional_message)
        super().__init__('Model not found', additional_message, 404)


class ModelNotLoaded(ApplicationError):
    """Raised when the model is not loaded"""

    def __init__(self, additional_message=''):
        # super('Error loading model', additional_message)
        super().__init__('Error loading model', additional_message)


class InvalidInputData(ApplicationError):
    """Raised when the input data is corrupted"""

    def __init__(self, additional_message=''):
        # super('Invalid input data', additional_message)
        super().__init__('Invalid input data', additional_message, 400)


class InferenceEngineNotFound(ApplicationError):
    """Raised when the Inference Engine is not found"""

    def __init__(self, additional_message=''):
        # super('Inference engine not found', additional_message)
        super().__init__('Inference engine not found', additional_message, 404)
