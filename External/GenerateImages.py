import os, math
from PIL import Image, ImageDraw

image_size = 512
max_power = 6

#create output directories
if not os.path.exists('Output/Trapped'):
	os.makedirs('Output/Trapped')

if not os.path.exists('Output/Passing'):
	os.makedirs('Output/Passing')

#init vertex array
vertex = []
with open('Data/g132543.00700.node') as f:
	for line in f:
		data = line.split()
		vertex.append([float(data[1]),-float(data[2])])

#normalize vertices
minY = min(vertex, key=lambda x:x[1])[1]
YRange = max(vertex, key=lambda x:x[1])[1] - minY
minX = min(vertex, key=lambda x:x[0])[0]
XRange = max(vertex, key=lambda x:x[0])[0] - minX

for v in vertex:
	v[1] = (v[1]-minY) / YRange
	v[1] = v[1]*0.9 + 0.05
	v[0] = (v[0]-minX) / XRange
	v[0] = v[0]*0.9 + 0.05

#init triangle array
triangle = []
with open('Data/g132543.00700.ele') as f:
	for line in f:
		data = line.split()
		triangle.append([int(data[1]),int(data[2]),int(data[3])])

#init bootstrap current array
current = [[]]
index = 0
timestep = 0
total = 0
with open('Data/xgca_mira044_jb_trapped_passing.dat') as f:
	for line in f:
		total += 1
		if line == "\n":
			index = 0
			timestep += 1
			current.append([])
		else:
			[t, p] = line.split();
			current[timestep].append({'t':float(t),'p':float(p)})
		index += 1

#use bootstrap current to set colors
tcolors = []
pcolors = []
for i, timestep in enumerate(current):
	print(i)
	tcolors.append([])
	pcolors.append([])
	for vertex in timestep:
		#trapped particles
		tr = 255 if vertex['t'] <= 0 else 0
		tb = 255 if vertex['t'] >= 0 else 0
		ta = 0 if vertex['t'] == 0 else max(min(int(math.log10(abs(vertex['t']))*255/max_power), 255), 0)
		tcolors[i].append((tr,0,tb,ta))
		#passing particles
		pr = 255 if vertex['p'] <= 0 else 0
		pb = 255 if vertex['p'] >= 0 else 0
		pa = 0 if vertex['p'] == 0 else max(min(int(math.log10(abs(vertex['p']))*255/max_power), 255), 0)
		pcolors[i].append((pr,0,pb,pa))

#init a new image
image = Image.new('RGBA', (image_size, image_size), (0,0,0,0)) 
