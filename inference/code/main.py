import sys
from typing import List
from models import ApiResponse, ApiResponseModel
from inference.errors import Error
from starlette.responses import FileResponse
from starlette.staticfiles import StaticFiles
from starlette.middleware.cors import CORSMiddleware
from deep_learning_service import DeepLearningService
from fastapi import FastAPI, Form, File, UploadFile, Header, Request
from fastapi.responses import JSONResponse
from inference.exceptions import ModelNotFound, InvalidModelConfiguration, ApplicationError, ModelNotLoaded, \
	InferenceEngineNotFound, InvalidInputData
from __version__ import __version__

sys.path.append('./inference')

dl_service = DeepLearningService()
error_logging = Error()
app = FastAPI(version=__version__, title='BMW InnovationLab OpenVINO Inference Automation',
			  description="<b>API for performing OpenVINO inference</b>")

responses = {
    404: {"model": ApiResponseModel},
	400: {"model": ApiResponseModel},
	500: {"model": ApiResponseModel},
}
@app.exception_handler(ApplicationError)
async def app_exception_handler(request: Request, exc: ApplicationError):
	return JSONResponse(
        status_code=exc.status_code,
        content=ApiResponse(success=False, error=str(exc)).__dict__,
    )

@app.get('/load', responses=responses, tags=["General"])
async def load_custom():
	"""
	Loads all the available models.
	:return: All the available models with their respective hashed values
	"""
	try:
		return dl_service.load_all_models()
	except ApplicationError as e:
		raise e
	except Exception:
		raise ApplicationError('unexpected server error')


@app.post('/detect', responses=responses, tags=["General"])
async def detect_custom(model: str = Form(...), image: UploadFile = File(...)):
	"""
	Performs a prediction for a specified image using one of the available models.
	:param model: Model name or model hash
	:param image: Image file
	:return: Model's Bounding boxes
	"""
	try:
		output = await dl_service.run_model(model, image, draw=False)
		error_logging.info('request successful;' + str(output))
		return output
	except ApplicationError as e:
		error_logging.warning(model + ';' + str(e))
		raise e
	except Exception as e:
		error_logging.error(model + ' ' + str(e))
		raise ApplicationError('unexpected server error')


@app.post('/models/{model_name}/predict_image', responses=responses, tags=["General"])
async def predict_image(model_name: str, input_data: UploadFile = File(...)):
	"""
	Draws bounding box(es) on image and returns it.
	:param model_name: Model name
	:param input_data: Image file
	:return: Image file
	"""
	try:
		output = await dl_service.run_model(model_name, input_data, draw=True)
		error_logging.info('request successful;' + str(output))
		return FileResponse("/app/result.jpg", media_type="image/jpg")
	except ApplicationError as e:
		error_logging.warning(model_name + ';' + str(e))
		raise e
	except Exception as e:
		error_logging.error(model_name + ' ' + str(e))
		raise ApplicationError('unexpected server error')

@app.get('/models', responses=responses, tags=["Anonymization relevant"])
async def list_models(user_agent: str = Header(None)):
	"""
	Lists all available models.
	:param user_agent:
	:return: APIResponse
	"""
	try:
		return ApiResponse(data={'models': dl_service.list_models()})
	except ApplicationError as e:
		raise e
	except Exception:
		raise ApplicationError('unexpected server error')

@app.get('/models/{model_name}/labels', responses=responses, tags=["Anonymization relevant"])
async def list_model_labels(model_name: str):
	"""
	Lists all the model's labels.
	:param model_name: Model name
	:return: List of model's labels
	"""
	try:
		return ApiResponse(data=dl_service.get_labels(model_name))
	except ApplicationError as e:
		raise e
	except Exception:
		raise ApplicationError('unexpected server error')

@app.get('/models/{model_name}/config', responses=responses, tags=["Anonymization relevant"])
async def list_model_config(model_name: str):
	"""
	Lists all the model's configuration.
	:param model_name: Model name
	:return: List of model's configuration
	"""
	try:
		return ApiResponse(data=dl_service.get_config(model_name))
	except ApplicationError as e:
		raise e
	except Exception:
		raise ApplicationError('unexpected server error')

@app.post('/models/{model_name}/predict', responses=responses, tags=["Anonymization relevant"])
async def run_model(model_name: str, input_data: UploadFile = File(...)):
	"""
	Performs a prediction using the given model name and image file.
	:param model_name: Model name
	:param input_data: An image file
	:return: APIResponse containing the prediction's bounding boxes
	"""
	try:
		output = await dl_service.run_model(model_name, input_data, draw=False)
		error_logging.info('request successful;' + str(output))
		return ApiResponse(data=output)
	except ApplicationError as e:
		error_logging.warning(model_name + ';' + str(e))
		raise e
	except Exception as e:
		error_logging.error(model_name + ' ' + str(e))
		raise ApplicationError('unexpected server error')