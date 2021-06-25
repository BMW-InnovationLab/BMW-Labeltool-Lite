from typing import Any
from pydantic import BaseModel

class ApiResponse:

    def __init__(self, success=True, data=None, error=None):
        """
        Defines the response shape
        :param success: A boolean that returns if the request has succeeded or not
        :param data: The model's response
        :param error: The error in case an exception was raised
        """
        self.data = data
        self.error = error.__str__() if error is not None else ''
        self.success = success

class ApiResponseModel(BaseModel):
    data: Any
    error: str
    success: bool