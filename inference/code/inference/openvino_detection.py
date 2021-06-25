import os
import jsonschema
import asyncio
import json
from pathlib import Path
import numpy as np
from PIL import Image, ImageDraw, ImageFont
from inference.base_inference_engine import AbstractInferenceEngine
from inference.exceptions import InvalidModelConfiguration, InvalidInputData, ApplicationError

from openvino.inference_engine import IECore
import ngraph as ng
import cv2


class InferenceEngine(AbstractInferenceEngine):

	def __init__(self, model_path):
		self.label_path = ""
		self.NUM_CLASSES = None
		self.classes = None
		self.net = None
		self.exec_net = None
		self.font = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf", 20)
		super().__init__(model_path)

	def load(self):
		with open(os.path.join(self.model_path, 'config.json')) as f:
			data = json.load(f)
		try:
			self.validate_json_configuration(data)
			self.set_model_configuration(data)
		except ApplicationError as e:
			raise e

		with open(os.path.join(self.model_path, 'classes.txt'), 'r') as f:
			self.classes = [line.strip() for line in f.readlines()]

		self.ie = IECore()

		# Load the model
		model_xml = next(Path(self.model_path).glob("*.xml"))
		model_bin = model_xml.parent / (model_xml.stem + ".bin")
		self.net = self.ie.read_network(model=str(model_xml),
										weights=str(model_bin))
		func = ng.function_from_cnn(self.net)
		ops = func.get_ordered_ops()

		assert (len(self.net.input_info.keys()) in {1, 2}), \
			"Network must have one or two inputs"

		self.input_name, self.input_info_name = None, None
		for k, v in self.net.input_info.items():
			if len(v.layout) == 4:
				self.input_name = k
				v.precision = "U8"
			elif len(v.layout) == 2:
				self.input_info_name = k
				v.precision = "FP32"
				assert (v.input_data.shape[1] in {3, 6} and v.input_data.shape[0] ==
						1), "Input info should be 3 or 6 values length"

		self.output_name, output_info = "", self.net.outputs[next(iter(self.net.outputs.keys()))]
		output_ops = {op.friendly_name : op for op in ops \
					if op.friendly_name in self.net.outputs and op.get_type_name() == "DetectionOutput"}
		if len(output_ops) != 0:
			self.output_name, output_info = output_ops.popitem()

		if self.output_name == "":
			print("Can't find a DetectionOutput layer in the topology")
			sys.exit(-1)

		output_info.precision = "FP32"

		self.exec_net = self.ie.load_network(
			network=self.net, device_name="CPU")

	async def infer(self, input_data, draw):
		await asyncio.sleep(0.00001)
		try:
			pillow_image = Image.open(input_data.file).convert('RGB')
			np_image = np.array(pillow_image)
		except Exception as e:
			raise InvalidInputData('corrupted image')
		try:
			with open(self.model_path + '/config.json') as f:
				data = json.load(f)
		except Exception as e:
			raise InvalidModelConfiguration('config.json not found or corrupted')
		json_confidence = data['confidence']
		json_predictions = data['predictions']
		
		n, c, h, w = self.net.input_info[self.input_name].input_data.shape
		images = np.zeros((n, c, h, w))
		# Read image to array
		img = np_image
		# Preprocess image
		ih, iw = img.shape[:-1]
		if (ih, iw) != (h, w):
			img = cv2.resize(img, (w, h))
		# Change data layout from HWC to CHW
		img = img.transpose((2, 0, 1))
		images[0] = img

		input_dict = {self.input_name: images}
		if self.input_info_name is not None:
			input_dict[self.input_info_name] = np.array([[w, h, c]])
		result = self.exec_net.infer(inputs=input_dict)

		output_key = "detection_output"
		output = result[output_key]
		output = np.squeeze(output, (0, 1))

		output_bboxes = []
		for batch_id, class_id, confidence, x1, y1, x2, y2 in output:
			bbox = list(np.array([x1*iw,
						y1*ih,
						x2*iw,
						y2*ih]).astype(float))
			if batch_id == -1:
				break
			if confidence * 100 >= json_confidence:
				output_bboxes.append( {
					"ObjectClassName": self.classes[int(class_id-1)],
					"ObjectClassId": int(class_id),
					"confidence": float(confidence * 100),
					"coordinates": {
							'left': int(bbox[0]),
							'right': int(bbox[2]),
							'top': int(bbox[1]),
							'bottom': int(bbox[3])
						}
					}
				)
		response = dict([('bounding-boxes', output_bboxes)])

		if not draw:
			return response
		else:
			try:
				self.draw_image(pillow_image, response)
			except ApplicationError as e:
				raise e
			except Exception as e:
				raise e

	def draw_image(self, image, response):
		"""
		Draws on image and saves it.
		:param image: image of type pillow image
		:param response: inference response
		:return:
		"""
		draw = ImageDraw.Draw(image)
		for bbox in response['bounding-boxes']:
			draw.rectangle([bbox['coordinates']['left'], bbox['coordinates']['top'], bbox['coordinates']['right'],
							bbox['coordinates']['bottom']], outline="red")
			left = bbox['coordinates']['left']
			top = bbox['coordinates']['top']
			conf = "{0:.2f}".format(bbox['confidence'])
			draw.text((int(left), int(top) - 20), str(conf) + "% " + str(bbox['ObjectClassName']), 'red', self.font)
		image.save('/app/result.jpg', 'PNG')

	def free(self):
		pass

	def validate_configuration(self):
		# check if weights file exists
		if not list(Path(self.model_path).rglob("*.bin")):
			raise InvalidModelConfiguration('(model).bin not found')
		# check if xml file exists
		if not list(Path(self.model_path).rglob("*.xml")):
			raise InvalidModelConfiguration('(model).xml not found')
		return True

	def set_model_configuration(self, data):
		self.configuration['framework'] = data['framework']
		self.configuration['type'] = data['type']
		self.configuration['network'] = data['network']
		self.NUM_CLASSES = data['number_of_classes']

	def validate_json_configuration(self, data):
		with open(os.path.join('inference', 'ConfigurationSchema.json')) as f:
			schema = json.load(f)
		try:
			jsonschema.validate(data, schema)
		except Exception as e:
			raise InvalidModelConfiguration(e)
