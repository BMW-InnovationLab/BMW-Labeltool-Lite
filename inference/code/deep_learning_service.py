import os
import re
import json
import uuid
from inference.inference_engines_factory import InferenceEngineFactory
from inference.exceptions import ModelNotFound, InvalidModelConfiguration, ModelNotLoaded, InferenceEngineNotFound, \
    InvalidInputData, ApplicationError


class DeepLearningService:

    def __init__(self):
        """
        Sets the models base directory, and initializes some dictionaries.
        Saves the loaded model's hashes to a json file, so the values are saved even though the API went down.
        """
        # dictionary to hold the model instances (model_name: string -> model_instance: AbstractInferenceEngine)
        self.models_dict = {}
        # read from json file and append to dict
        file_name = '/models_hash/model_hash.json'
        file_exists = os.path.exists(file_name)
        if file_exists:
            try:
                with open(file_name) as json_file:
                    self.models_hash_dict = json.load(json_file)
            except:
                self.models_hash_dict = {}
        else:
            with open('/models_hash/model_hash.json', 'w'):
                self.models_hash_dict = {}
        self.labels_hash_dict = {}
        self.base_models_dir = '/models'

    def load_model(self, model_name, force_reload=False):
        """
        Loads a model by passing the model path to the factory.
        The factory will return a loaded model instance that will be stored in a dictionary.
        :param model_name: Model name
        :param force_reload: Boolean to specify if we need to reload a model on each call
        :return: Boolean
        """
        if not force_reload and self.model_loaded(model_name):
            return True
        model_path = os.path.join(self.base_models_dir, model_name)
        try:
            self.models_dict[model_name] = InferenceEngineFactory.get_engine(model_path,model_name)
            return True
        except ApplicationError as e:
            raise e

    def load_all_models(self):
        """
        Loads all the available models.
        :return: Returns a List of all models and their respective hashed values
        """
        self.load_models(self.list_models())
        models = self.list_models()
        for model in models:
            if model not in self.models_hash_dict:
                self.models_hash_dict[model] = str(uuid.uuid4())
        for key in list(self.models_hash_dict):
            if key not in models:
                del self.models_hash_dict[key]
        # append to json file
        with open('/models_hash/model_hash.json', "w") as fp:
            json.dump(self.models_hash_dict, fp)
        return self.models_hash_dict

    def load_models(self, model_names):
        """
        Loads a set of available models.
        :param model_names: List of available models
        :return:
        """
        for model in model_names:
            self.load_model(model)

    async def run_model(self, model_name, input_data, draw):
        """
        Loads the model in case it was never loaded and calls the inference engine class to get a prediction.
        :param model_name: Model name
        :param input_data: Batch of images or a single image
        :param draw: Boolean to specify if we need to draw the response on the input image
        :param predict_batch: Boolean to specify if there is a batch of images in a request or not
        :return: Model response in case draw was set to False, else an actual image
        """
        if re.match(r'[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}', model_name,
                    flags=0):
            for key, value in self.models_hash_dict.items():
                if value == model_name:
                    model_name = key
        if self.model_loaded(model_name):
            try:
                if not draw:
                    return await self.models_dict[model_name].infer(input_data, draw)
                else:
                    await self.models_dict[model_name].infer(input_data, draw)
            except ApplicationError as e:
                raise e
        else:
            try:
                self.load_model(model_name)
                return await self.run_model(model_name, input_data, draw)
            except ApplicationError as e:
                raise e

    def list_models(self):
        """
        Lists all the available models.
        :return: List of models
        """
        return [folder for folder in os.listdir(self.base_models_dir) if
                os.path.isdir(os.path.join(self.base_models_dir, folder))]

    def model_loaded(self, model_name):
        """
        Returns the model in case it was loaded.
        :param model_name: Model name
        :return: Model name
        """
        return model_name in self.models_dict.keys()

    def get_labels(self, model_name):
        """
        Loads the model in case it's not loaded.
        Returns the model's labels.
        :param model_name: Model name
        :return: List of model labels
        """
        if not self.model_loaded(model_name):
            self.load_model(model_name)
        return self.models_dict[model_name].classes

    def get_config(self, model_name):
        """
        Returns the model's configuration.
        :param model_name: Model name
        :return: List of model's configuration
        """
        if not self.model_loaded(model_name):
            self.load_model(model_name)
        return self.models_dict[model_name].configuration
