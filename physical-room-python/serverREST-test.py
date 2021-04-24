from flask import Flask, json
import threading
import time
import numpy as np

api = Flask(__name__)

@api.route('/position/<rb_id>', methods=['GET'])
def get_position(rb_id=0):
    global pos;    
    if rb_id == '1':
        return str(pos[0])+","+str(pos[1])+","+str(pos[2])
    if rb_id == '2':
        return str(pos[0] + 1)+","+str(pos[1] + 1)+","+str(pos[2])
    else:
        return 'none'

def flaskThread():
    api.run(host="192.168.1.101", port=5000)

pos = np.array([0.0,0.0,0.0])

if __name__ == '__main__':
     
    threading.Thread(target=flaskThread).start()
    counter = 0
    while True:
        pos[0] = -3 + 0.1 * (counter%40)
        pos[1] = -3 + 0.1 * (counter%40)
        counter += 1
        print(pos)
        time.sleep(1/60)