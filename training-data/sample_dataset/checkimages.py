from PIL import Image 
import os 

images_list=os.listdir("./new_dataset/images")
print(len(images_list))
one=Image.open("./new_dataset/images/000000.png")

print(one)

