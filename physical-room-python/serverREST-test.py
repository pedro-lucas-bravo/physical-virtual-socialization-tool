from flask import Flask, json, request
import threading
import time
import numpy as np

api = Flask(__name__)


########################## ENDPOINTS ##########################


@api.route('/position/<rb_id>', methods=['GET'])
def get_physical_person_position(rb_id=0):
    global pos    
    if rb_id == '1':
        return str(pos[0])+","+str(pos[1])+","+str(pos[2])
    if rb_id == '2':
        return str(pos[0] + 1)+","+str(pos[1] + 1)+","+str(pos[2])
    else:
        return 'none'
        
@api.route('/set_virtual_position/<v_id>/<user_name>', methods=['GET'])
def set_virtual_person_position(v_id, user_name):
    global dic_virtual_people    
    p_x = request.headers.get('p_x')
    p_y = request.headers.get('p_y')
    p_z = request.headers.get('p_z')
    dic_virtual_people[v_id] = [p_x + "," + p_y + "," + p_z, user_name, time.time()]
    return 'ok'
    
@api.route('/get_virtual_position/<v_id>', methods=['GET'])
def get_virtual_person_position(v_id):
    global dic_virtual_people    
    if v_id in dic_virtual_people:
        return dic_virtual_people[v_id][0]
    return 'none'
    
@api.route('/get_all_virtual/<v_id>', methods=['GET'])
def get_all_virtual_people(v_id):
    global dic_virtual_people
    ids = list(dic_virtual_people.keys())
    #Remove if it is teh same requesting
    if v_id in dic_virtual_people:
        ids.remove(v_id)
    #Remove if person is not sending for some seconds (2 sec)
    for k, v in dic_virtual_people.items():
        if time.time() - v[2] > 2.0:
            ids.remove(k)
    #Add user 
    for i in range(len(ids)):
        ids[i] += '-' + dic_virtual_people[ids[i]][1]
    return ','.join(ids)


########################## OTHER FUNCTIONS ##########################
def flaskThread():
    api.run(host="192.168.1.101", port=5000)
    #api.run(host="193.157.137.60", port=5000)



########################## MAIN ##########################
pos = np.array([0.0,0.0,0.0])
dic_virtual_people = {}

if __name__ == '__main__':
     
    threading.Thread(target=flaskThread).start()
    counter = 0
    while True:
        pos[0] = -3 + 0.1 * (counter%40)
        pos[1] = -3 + 0.1 * (counter%40)
        counter += 1
        #print(pos)
        time.sleep(1/60)