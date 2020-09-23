import os
import errno
import argparse
from PIL import Image
import numpy as np
import json
import sys
parser = argparse.ArgumentParser(description='Process some integers.')
parser.add_argument('--dir', type=str, default="NONE")

args = parser.parse_args()
def get_img_shape(path):
    image=Image.open(path)
    return np.array(image.size)

def convert_labels(path, x1, y1, x2, y2):

    def sorting(l1, l2):
        if l1 > l2:
            lmax, lmin = l1, l2
            return lmax, lmin
        else:
            lmax, lmin = l2, l1
            return lmax, lmin
    size = get_img_shape(path)
    xmax, xmin = sorting(x1, x2)
    ymax, ymin = sorting(y1, y2)
    dw = 1./size[0]
    dh = 1./size[1]
    x = (xmin + xmax)/2.0
    y = (ymin + ymax)/2.0
    w = xmax - xmin
    h = ymax - ymin
    x = x*dw
    w = w*dw
    y = y*dh
    h = h*dh
    return (x,y,w,h)

datasetfolder= os.path.join(os.getcwd(),"training-data",args.dir)

imageslist= os.listdir(os.path.join(datasetfolder,"images"))
labelslist= os.listdir(os.path.join(datasetfolder,"labels/json"))

imageslist.sort()
labelslist.sort()
filename=os.path.join(datasetfolder,"labels/yolo")
os.makedirs(filename,exist_ok=True)

j=0
i=0
while i<len(imageslist) and j<len(labelslist):
    
    image_confirmed=False
    while image_confirmed==False:
        
        if os.path.splitext(imageslist[i])[0] != os.path.splitext(labelslist[j])[0]:

            i=i+1
        else:
            image_confirmed=True


    outf= open(os.path.join(datasetfolder,"labels/yolo",labelslist[j].replace("json","txt")),'w')


    path=os.path.join(datasetfolder,"images",imageslist[i])
    label=os.path.join(datasetfolder,"labels/json",labelslist[j])
    with open(label, 'rb') as f:
        obj=json.load(f)

    print(imageslist[i])
    print(labelslist[j])
    
    
    for k in obj:
        x,y,w,h=convert_labels(path,k['Left'],k['Top'],k['Right'],k['Bottom'])
        if not (x>1 or y>1 or w>1 or h>1):
            outf.write(str(k['ObjectClassId'])+" "+str(x)+" "+str(y)+" "+str(w)+" "+str(h)+"\n")
    j=j+1
    i=i+1

    