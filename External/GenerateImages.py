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
	v['y'] = v['y']*0.8 + 0.1
	v['x'] = (v['x']-minX) / XRange
	v['x'] = v['x']*0.8 + 0.1

