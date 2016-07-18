# Study-hall-monitoring
This project provides a service to allow people to monitor study rooms in real time. 
The users connecting to a web page are able to understand how many seats are free examining a map showed by the client. 
Such a map represents the geometry of the room and it reﬂects the real position both of the seats and of the table.

An high-level overview of this software is given by the picture below
![architecture](https://raw.githubusercontent.com/Study-hall-monitoring/images/Architecture Diagram.png)

##Hardware
The hardware chosen is a Spider Fez 2.0 (Microsoft .NET Gadgeteer platform), its task is to take a photo every n seconds and to send it to a serverTCP. 
The module communicates both with the web service and with the serverTCP. 

##Web Service
Web Service is a SOAP server based developed through WCF framework. 
It handles the authentication of the Gadgeteer and instances a new serverTCP every time that a photo have to be received. 
In fact each photo transfer takes place in a new tcp connection like the ftp protocol. 

##Server TCP
ServerTCP is a module that receives the photo, process it and update the database. 

##Web Client
Web client is the front end module. It uses a web socket that allow it to get the updated data in real time 
and refresh the graphics interface without any interaction by the user.
