import os

if not os.path.exists('Output'):
	os.makedirs('Output')

vertex = []
with open('Data/g132543.00700.node') as f:
	for line in f:
		data = line.split()
		vertex.append({'x':float(data[1]),'y':-float(data[2])})

minY = min(vertex, key=lambda x:x['y'])['y']
YRange = max(vertex, key=lambda x:x['y'])['y'] - minY
minX = min(vertex, key=lambda x:x['x'])['x']
XRange = max(vertex, key=lambda x:x['x'])['x'] - minX

for v in vertex:
	v['y'] = (v['y']-minY) / YRange
	v['y'] = v['y']*0.9 + 0.05
	v['x'] = (v['x']-minX) / XRange
	v['x'] = v['x']*0.9 + 0.05

current = [[]]
index = 0
timestep = 0
with open('Data/xgca_mira044_jb_trapped_passing.dat') as f:
	for line in f:
		if line == "\n":
			index = 0
			timestep += 1
			current.append([])
		else:
			[t, p] = line.split();
			if index == 0:
				current[timestep].append([])
			current[timestep][index].append({'t':0,'p':0})
		index += 1