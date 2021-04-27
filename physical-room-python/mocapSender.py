import requests

########## SET PHYSICAL POSITION FROM MOCAP

#test
try:
    r = requests.get('http://192.168.1.101:5000/set_physical_position/2', headers = {'p_x':'0', 'p_y':'-1', 'p_z':'0'})
except Exception as ex:
    print('Connection failed!!!')