# Crane Simulation Tool

### Project Overview
The aim of this project is to develop a simulation tool for a Ship-to-Shore *(STS)* crane using **C#** and **Python.** Communication between the components will be facilitated through the MQTT protocol connected to a HiveMQ server.

### The developers 
A project by **Eye-Concept Industrial Automation B.V.** carried out by the students from the university of Howest and the RAC:

|      Howest      |             RAC
|------------------|----------------------------
|`Teal'c Flederick`|`Maarten de Leeuw van Weenen`       
|`Nathanael Bens`  |`Nizar Haddouch`       
|                  |`Dennis Hsu`
|                  |`Kerem Yildiz`
|                  |`Bünyamin Bölükbas`

## Requirements

Python (version 3.12.0) https://www.python.org/downloads/ (Please note, when installing python, make sure to check the "Add python.exe to PATH") </br>
Git (version 2.44.0) https://git-scm.com/downloads

Follow this guide to clone the repository from github (clone using the HTTPS key):
https://docs.github.com/en/repositories/creating-and-managing-repositories/cloning-a-repository 

## Installation

MQTT installation

1: Create an account on HiveMQ to make your operating system visible. You can sign up on their website: https://www.hivemq.com/ 

2: Create an MQTT script by following the instructions provided in this link from HiveMQ: [Implementing MQTT in Python.](https://www.hivemq.com/blog/implementing-mqtt-in-python/)

3: Develop a script for establishing a connection from one operating system to another.

4: Create a script for sending messages between the operating systems.

Package dependencies

Before running, install the required dependencies and libraries given in the 'Requirements.txt' file.
To install the required packages (Python version 3.8.2 and MQTT), run the following command in your shell / command prompt

```pip install -r requirements.txt```

## Test

To run the program, first navigate to the right directory using the following command:

 ```cd .\Wxi.CraneSimulation\Wxi.CraneSimulator.PyCore/ ```

Then run the file by entering this command:

 ```python crane.py ```
 

Movement controls:</br>
To control the crane, use the following keys, </br>
←     : Move the spreader forward </br>
→     : Move the spreader backward </br>
↑     : Move the spreader up </br>
↓     : Move the spreader down </br>
Enter : Grab/ungrab a container with the spreader </br>
P     : Emergency stop, stops/unstops the simulation </br>

Move spreader(Red) to container(Blue) </br>
Press enter and grab the container. </br>
After grabbing the container move the spreader and container to the shore</br>
Release the container by pressing enter when you are on the ground.</br>
I case of an emergency, you can press on the 'p' button for immidiate stop of the crane. </br>

## Access to the database 

We have installed a NuGet package via Visual Studio called FirebaseDatabase.net and used it to establish a connection to the Firebase Realtime Database set up through https://firebase.google.com/. </br>
Then, we run a WPF project that launches a dashboard, and we have a button on the dashboard that initiates a connection with the database so that we can retrieve our data from the database. The data is fetched via HiveMQTT and sent to our Firebase database.</br> 
When we are controlling the crane, we can see the live movement of the container in the database accompanied by the cordinates of the container. 

## Backend folder structure
Volume serial number is 8277-E550
C:.
│   container.py </br>
│   crane.py </br>
│   craneComponents.py </br>
│   methodServer.py </br>
│   mqttConnect.py </br>
│   Wxi.CraneSimulator.PyCore.pyproj </br>
│</br>
├───PythonImages </br>
│       .gitkeep </br>
│ </br>
└───pycache </br>
        container.cpython-310.pyc </br>
        craneComponents.cpython-310.pyc </br>
        mqttConnect.cpython-310.pyc </br>


## Limitations of the Code

We did not implement:
Sqlite


## Resources

https://docs.github.com/en/repositories/creating-and-managing-repositories/cloning-a-repository </br>
https://git-scm.com/downloads </br>
https://www.python.org/downloads/</br>
https://firebase.google.com/ </br>
https://www.hivemq.com/blog/implementing-mqtt-in-python/</br>
https://www.hivemq.com/ </br>

## Version history

- Previous Version v1.0.
Release date: 15-01-2024
- Current version v1.1.
release date 29-02-2024

## Latest changes

Readme.md has been updated by the group of contributors on 28-02-2024.