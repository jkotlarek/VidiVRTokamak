import os

if not os.path.exists('Output'):
	os.makedirs('Output')

vertex = []
with open('Data/g132543.00700.node') as f:
	for line in f:
		data = line.split()
		vertex.append([float(data[1]),-float(data[2])])

minY = min(vertex, key=lambda x:x[1])[1]
YRange = max(vertex, key=lambda x:x[1])[1] - minY
minX = min(vertex, key=lambda x:x[0])[0]
XRange = max(vertex, key=lambda x:x[0])[0] - minX

for v in vertex:
	v[1] = (v[1]-minY) / YRange
	v[1] = v[1]*0.9 + 0.05
	v[0] = (v[0]-minX) / XRange
	v[0] = v[0]*0.9 + 0.05

triangle = []
with open('Data/g132543.00700.ele') as f:
	for line in f:
		data = line.split()
		triangle.append([int(data[1]),int(data[2]),int(data[3])])


current = [[]]
index = 0
timestep = 0
with open('Data/xgca_mira044_jb_trapped_passing.dat') as f:
	for line in f:
		if line == "\n":
			index = 0
			timestep += 1
			current.append([])
			print(timestep)
		else:
			[t, p] = line.split();
			current[timestep].append({'t':float(t),'p':float(p)})
		index += 1
