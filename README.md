# Physical-Virtual Socialization Prototype

This is part of the final project for the course [MCT4031](https://www.uio.no/studier/emner/hf/imv/MCT4031/) from 
the [MCT master](https://mct-master.github.io/) programme at UiO and NTNU.

## Description

![Hybrid Concept](https://github.com/pedro-lucas-bravo/physical-virtual-socialization-tool/blob/master/images/hybridConcept.png)

In the picture above we have a physical and a virtual space. In the case of the physical space, the user is tracked through an “Indoor Positioning System (IPS)” which collects position data that is preprocessed to filter the relevant information that is sent later to a central server. At the same time, people that are in a virtual space can move with an avatar through a desktop or web application that maps the physical space, and send also their information to the same server. On both sides they should be able to see other people (physical or virtual) in the same virtual space. Virtual users would navigate mostly through a computer, while physical users would use their mobile device (smartphone, tablet, etc). Both types of participants will interact with each other or with “interest points”, which in case of a conference could be a poster or a social place. 

Regarding these interest points, people in the physical place could see in their device through a mobile application their own position, as well as additional information when they approach these points inside a trigger area. Something similar happens for virtual people since they can approach their avatar to the same mapped points in the virtual world. Each interest point could be customized according to its nature, for instance, a poster point could trigger a set of options to go to a video, a live Zoom room with the presenter, or more information on the website regarding that poster. 

A simple socialization approach based on this solution could be approaching other participants in the virtual world and communicating to them through third-party services (text, voice or video chats). Also it could use similar models like in Gather Town or Mozilla hubs to enrich the social interaction but considering the physical attendants. 
Based on this solution, we implemented a working prototype by mapping the conceptual elements to technological components:

* **Physical Space:** MCT Portal.
* **Virtual Space:** Custom 2D application build for Android, Windows, and HTML5. This application was implemented in the Unity3D game engine and the graphical interface is shown in the picture below. It maps the MCT Portal in a very simplistic way.
* **Indoor Positioning System (IPS):** Optitrack Motion Capture System (MoCap) taking position date from “rigid bodies” (a set of detectable markers that compound one object with position and rotation) in the format (x, y, z).
* **Data processing and dispatch:** Python module that takes the position data from the MoCap  and connects to a remote server in the internet to send it.
* **Internet:**  A remote server deployed in an instance (Centos 8, ip: 158.39.201.178) created in the cloud service NREC provided by UiO. This server is created in Python by using the Flask library and provides a REST API that is consumed by the Python module used for the MoCap (Physical Space) and the UnityWebRequest API in the Unity application (Virtual Space).
* **Interest Points:** These points are fixed points in the room. In the physical space they are imaginary places in the MCT Portal, and in the virtual space they are denoted by black squares like the ones shown in the “Virtual Environment” below. When a user approaches one of theses points, it pops up a simple image of what it represents (Poster, social place, etc).
* **Physical User:** There are two physical users walking in the room, each wearing a “rigid body” whose position is detected by the MoCap system. Every user can hold an Android mobile device to see in which part of the virtual world they are positioned, as well as other users and interest points. In the virtual world they are named as P1 (blue circle) and P2 (red circle).
* **Virtual User:** These are remote users behind a computer that can enter to the virtual room through a Windows or a web client application. The web application is hosted here (http://pedrolucas.tech/virtualroomweb/ ). The avatar that a client can move (using arrow keys) is the green square, while the purple triangles are other remote virtual users moving around.

![Virtual Space](https://github.com/pedro-lucas-bravo/physical-virtual-socialization-tool/blob/master/images/virtualSpace.png)

In the picture below we show a testing session for this prototype in the MCT Portal. As you can see, both people are mapped (as P1 and P2 through the MoCap system) to the virtual world. A short video about this testing can be found [here](https://www.youtube.com/watch?v=ZIbpIhHn1g4) (Video without sound).

![Physical Space](https://github.com/pedro-lucas-bravo/physical-virtual-socialization-tool/blob/master/images/physicalSpace.png)

As a fast prototype, it does not consider some technical optimizations, especially in terms of data communication since sometimes the objects moving in the room (those who take their position from the network) are not updated smoothly (using a REST API for real time iis slow). A suitable solution is using technologies related to multiplayer video games and integrating third-party services for technological support.  The goal of this implementation is to inspire a future development of a hybrid solution that could work in the context of a conference like the one we are targeting in this project.
