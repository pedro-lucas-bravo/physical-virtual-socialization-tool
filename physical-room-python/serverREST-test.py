from flask import Flask, json, request, make_response
from flask_cors import CORS, cross_origin
import threading
import time
import numpy as np

api = Flask(__name__)


########################## ENDPOINTS ##########################


@api.route('/position/<rb_id>', methods=['GET'])
def get_physical_person_position(rb_id=0):
    global pos_rb1
    global pos_rb2
    if rb_id == '1':
        resp = str(pos_rb1[0])+","+str(pos_rb1[1])+","+str(pos_rb1[2])
    elif rb_id == '2':
        resp = str(pos_rb2[0])+","+str(pos_rb2[1])+","+str(pos_rb2[2])
    else:
        resp = 'none_rb'
    response = make_response(resp)
    response.headers['Access-Control-Allow-Origin'] = '*'
    return response
        
@api.route('/set_physical_position/<rb_id>', methods=['GET'])
def set_physical_person_position(rb_id=0):
    global pos_rb1    
    global pos_rb2
    if rb_id == '1':
        pos = pos_rb1
    else:
        pos = pos_rb2    
    pos[0] = float(request.headers.get('p_x'))
    pos[1] = float(request.headers.get('p_y'))
    pos[2] = float(request.headers.get('p_z'))  
    
    response = make_response('ok')
    response.headers['Access-Control-Allow-Origin'] = '*'
    return response
        
@api.route('/set_virtual_position/<v_id>/<user_name>', methods=['GET'])
@cross_origin()
def set_virtual_person_position(v_id, user_name):
    global dic_virtual_people    
    p_x = request.headers.get('p_x')
    p_y = request.headers.get('p_y')
    p_z = request.headers.get('p_z')
    dic_virtual_people[v_id] = [p_x + "," + p_y + "," + p_z, user_name, time.time()]
    
    response = make_response('ok')
    return response
    
@api.route('/get_virtual_position/<v_id>', methods=['GET'])
def get_virtual_person_position(v_id):
    global dic_virtual_people    
    if v_id in dic_virtual_people:
        resp = dic_virtual_people[v_id][0]
    else:
        resp = 'none'
    
    response = make_response(resp)
    response.headers['Access-Control-Allow-Origin'] = '*'
    return response
    
@api.route('/get_all_virtual/<v_id>', methods=['GET'])
def get_all_virtual_people(v_id):
    global dic_virtual_people
    ids = list(dic_virtual_people.keys())
    #Remove if it is teh same requesting
    if v_id in dic_virtual_people:
        ids.remove(v_id)
    #Remove if person is not sending for some seconds (2 sec)
    for k, v in dic_virtual_people.items():
        if time.time() - v[2] > 2.0 and k in ids:
            ids.remove(k)
    #Add user 
    for i in range(len(ids)):
        ids[i] += '-' + dic_virtual_people[ids[i]][1]
    resp = ','.join(ids)
    
    response = make_response(resp)
    response.headers['Access-Control-Allow-Origin'] = '*'
    return response
    
@api.route('/set_poke/<id>', methods=['GET'])
@cross_origin()
def set_poke(id):
    global dic_poked_people   
    remote_id   = request.headers.get('remote_id')
    dic_poked_people[remote_id] = id
    
    response = make_response('ok')
    return response
    
@api.route('/get_poke/<id>', methods=['GET'])
def get_poke(id):
    global dic_poked_people
    resp = ''
    if id in dic_poked_people:
        resp = dic_poked_people[id]
    dic_poked_people[id] = ''
    
    response = make_response(resp)
    response.headers['Access-Control-Allow-Origin'] = '*'
    return response


########################## OTHER FUNCTIONS ##########################
def flaskThread():
    api.run(host="192.168.1.101", port=5000)
    #api.run(host="193.157.137.60", port=5000)



########################## MAIN ##########################
pos_rb1 = np.array([0.0,0.0,0.0])
pos_rb2 = np.array([0.0,0.0,0.0])
dic_virtual_people = {}
dic_poked_people = {}

if __name__ == '__main__':
     
    threading.Thread(target=flaskThread).start()
    # counter = 0
    # while True:
        # pos[0] = -3 + 0.1 * (counter%40)
        # pos[1] = -3 + 0.1 * (counter%40)
        # counter += 1
        # #print(pos)
        # time.sleep(1/60)