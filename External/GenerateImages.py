import os, math
from PIL import Image, ImageDraw

image_size = 512
max_power = 7

def MeshToImage(xyPair):
	return xyPair[0]*image_size, xyPair[1]*image_size

#create output directories
if not os.path.exists('Output/Trapped'):
	os.makedirs('Output/Trapped')

if not os.path.exists('Output/Passing'):
	os.makedirs('Output/Passing')

print('Loading Vertex Data')

#init vertex array
vertices = []
with open('Data/g132543.00700.node') as f:
	for line in f:
		data = line.split()
		vertices.append([float(data[1]),-float(data[2])])

print('...Complete')

print('Calculating Vertex Positions')

#normalize vertices
minY = min(vertices, key=lambda x:x[1])[1]
YRange = max(vertices, key=lambda x:x[1])[1] - minY
minX = min(vertices, key=lambda x:x[0])[0]
XRange = max(vertices, key=lambda x:x[0])[0] - minX

for v in vertices:
	v[1] = (v[1]-minY) / YRange
	v[1] = v[1]*0.9 + 0.05
	v[0] = (v[0]-minX) / XRange
	v[0] = v[0]*0.9 + 0.05

print('...Complete')

print('Loading Triangle Data')

#init triangle array
triangles = []
with open('Data/g132543.00700.ele') as f:
	for line in f:
		data = line.split()
		triangles.append([int(data[1])-1,int(data[2])-1,int(data[3])-1])

print('...Complete')

print('Loading Current Data')

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

print('...Complete')

print('Calculating Vertex Colors')

#use bootstrap current to set colors
tcolors = []
pcolors = []
for i, timestep in enumerate(current):
	tcolors.append([])
	pcolors.append([])
	for vertex in timestep:
		#trapped particles
		tr = 255 if vertex['t'] <= 0 else 0
		tb = 255 if vertex['t'] >= 0 else 0
		#ta = 0 if vertex['t'] == 0 else max(min(int(math.log10(abs(vertex['t']))*255/max_power), 255), 0)
		ta = 0 if vertex['t'] == 0 else max(min(int(abs(vertex['t'])*0.0001), 255), 0)
		tcolors[i].append((tr,0,tb,ta))
		#passing particles
		pr = 255 if vertex['p'] <= 0 else 0
		pb = 255 if vertex['p'] >= 0 else 0
		#pa = 0 if vertex['p'] == 0 else max(min(int(math.log10(abs(vertex['p']))*255/max_power), 255), 0)
		pa = 0 if vertex['p'] == 0 else max(min(int(abs(vertex['p'])*0.0001), 255), 0)
		pcolors[i].append((pr,0,pb,pa))

print('...Complete')

print('Calculating Pixel Colors')

#Setting Image values
for ts in range(0, len(current)-1):
	#create image
	iTrapped = Image.new('RGBA', (image_size, image_size), (0,0,0,0))
	iPassing = Image.new('RGBA', (image_size, image_size), (0,0,0,0))
	#create pixel map
	pTrapped = iTrapped.load()
	pPassing = iPassing.load()

	#iterate through triangles
	for t in triangles:
		#convert to image coordinates
		[x0,y0] = MeshToImage(vertices[t[0]])
		[x1,y1] = MeshToImage(vertices[t[1]])
		[x2,y2] = MeshToImage(vertices[t[2]])
		#detect bounding box for triangle
		minX = int(round(min([x0,x1,x2])))
		maxX = int(round(max([x0,x1,x2])))
		minY = int(round(min([y0,y1,y2])))
		maxY = int(round(max([y0,y1,y2])))

		#for each pixel in bounding box
		for x in range(minX, maxX+1):
			for y in range(minY, maxY+1):

				#calculate barycentric coordinate of pixel
				d = (y1 - y2) * (x0 - x2) + (x2 - x1) * (y0 - y2)
				a = ((y1 - y2) * (x - x2) + (x2 - x1) * (y - y2)) / d
				b = ((y2 - y0) * (x - x2) + (x0 - x2) * (y - y2)) / d
				c = 1 - a - b

				#only continue if pixel is inside triangle
				if (0 <= a and a <= 1 and 0 <= b and b <= 1 and 0 <= c and c <= 1):

					#get color from list
					tcolor = [tcolors[ts][t[0]], tcolors[ts][t[1]], tcolors[ts][t[2]]]
					pcolor = [pcolors[ts][t[0]], pcolors[ts][t[1]], pcolors[ts][t[2]]]

					#interpolate color using barycentric coordinates
					pTrapped[x,y] = tuple([int(tcolor[0][i] * a + tcolor[1][i] * b + tcolor[2][i] * c) for i in range(0,4)])
					pPassing[x,y] = tuple([int(pcolor[0][i] * a + pcolor[1][i] * b + pcolor[2][i] * c) for i in range(0,4)])

	#save images
	iTrapped.save('Output/Trapped/trapped_' + str(ts).zfill(3) + '.png', 'PNG')
	iPassing.save('Output/Passing/passing_' + str(ts).zfill(3) + '.png', 'PNG')

print('...Complete')
print('\n***Image Generation Completed Successfully!***\n')