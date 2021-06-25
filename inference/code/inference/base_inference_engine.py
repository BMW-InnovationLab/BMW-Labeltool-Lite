from abc import ABC, abstractmethod
from inference.exceptions import InvalidModelConfiguration, ModelNotLoaded, ApplicationError


class AbstractInferenceEngine(ABC):

	def __init__(self, model_path):
		"""
		Takes a model path and calls the load function.
		:param model_path: The model's path
		:return:
		"""
		self.labels = []
		self.configuration = {}
		self.model_path = model_path
		try:
			self.validate_configuration()
		except ApplicationError as e:
			raise e
		try:
			self.load()
		except ApplicationError as e:
			raise e
		except Exception as e:
			print(e)
			raise ModelNotLoaded()

	@abstractmethod
	def load(self):
		"""
		Loads the model based on the underlying implementation.
		"""
		pass

	@abstractmethod
	def free(self):
		"""
		Performs any manual memory implementation required to when unloading a model.
		Will be called when the class's destructor is called.
		"""
		pass

	@abstractmethod
	async def infer(self, input_data, draw):
		"""
		Performs the required inference based on the underlying implementation of this class.
		Could be used to return classification predictions, object detection coordinates...
		:param predict_batch: Boolean
		:param input_data: A single image
		:param draw: Used to draw bounding boxes on image instead of returning them
		:return: A bounding-box
		"""
		pass

	@abstractmethod
	def validate_configuration(self):
		"""
		Validates that the model and its files are valid based on the underlying implementation's requirements.
		Can check for configuration values, folder structure...
		"""
		pass

	@abstractmethod
	def set_model_configuration(self, data):
		"""
		Takes the configuration from the config.json file
		:param data: Json data
		:return:
		"""
		pass

	@abstractmethod
	def validate_json_configuration(self, data):
		"""
		Validates the configuration of the config.json file.
		:param data: Json data
		:return:
		"""
		pass

	def __del__(self):
		self.free()
