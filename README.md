# DoHomeClient

This simple library allows you to communicate with your DoHome Smart Bulbs.  
The library is targeting devices with the old W600 chip and firmware 1.1.0  
It should work with newer version as well.  

## Getting started

var client = new DoHomeClient();  
client.StartListener();  
var color = new DoHomeColor(4000, 2000, 1000, 0, 0);  
client.ChangeColor(color, false, client.Devices);  
...   
client.Off(client.Devices);  
  

## Communication

The library can communicate with the bulbs over TCP or over UDP.  
To communicate over TCP, use the methods directly defined on the DoHomeDevice.  
To communicate over UDP, use the methods defined on the DoHomeClient.  

The sample client provided shows both.

Since the devices can only handle 1 concurrent TCP connection, you might not be able to use the mobile app at the same time as you use this library.
