import os
import json
from inference.exceptions import ModelNotFound, ApplicationError, InvalidModelConfiguration, InferenceEngineNotFound, ModelNotLoaded


class InferenceEngineFactory:

    @staticmethod
    def get_engine(path_to_model, model_name=""):
        """
        Reads the model's inference engine from the model's configuration and calls the right inference engine class.
        :param path_to_model: Model's path
        :return: The model's instance
        """
        if not os.path.exists(path_to_model):
            raise ModelNotFound("Model {} not found".format(model_name))
        try:
            configuration = json.loads(open(os.path.join(path_to_model, 'config.json')).read())
        except Exception:
            raise InvalidModelConfiguration('config.json not found or corrupted')
        try:
            inference_engine_name = configuration['inference_engine_name']
        except Exception:
            raise InvalidModelConfiguration('missing inference engine name in config.json')
        try:
            # import one of the available inference engine class (in this project there's only one), and return a
            # model instance
            return getattr(__import__(inference_engine_name), 'InferenceEngine')(path_to_model)
        except ApplicationError as e:
            print(e)
            raise e
        except Exception as e:
            print(e)
            raise InferenceEngineNotFound(inference_engine_name)
