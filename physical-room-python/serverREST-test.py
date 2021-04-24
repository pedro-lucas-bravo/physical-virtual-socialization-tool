from flask import Flask, json
import threading
import time

api = Flask(__name__)

@api.route('/position/<rb_id>', methods=['GET'])
def get_position(rb_id=0):
    global pos;    
    if rb_id == '1':
        return str(pos)
    else:
        return "X"

def flaskThread():
    api.run(host="192.168.1.101", port=5000)

pos = 1

if __name__ == '__main__':
     
    threading.Thread(target=flaskThread).start()
    while True:
        pos += 1
        print(str(pos))
        time.sleep(1)