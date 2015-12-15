# README #

Simple C# implementation of the Despatch Bay Pro API, I'm not a C# programmer so this implementation is not be production safe, be warned.

I did have some issues with SOAP datatypes, in getShipment().  Where the php soap datatypes being returned for a couple of boolean fields could not be cast.

I would recommend checking this out and seeing if it will compile without updating the Web References, the wsdl files are included in this repo.

These examples were built using

* Hardware: Mac / Linux
* IDE: MonoDevelop / Xaramin
* Framework: Mono
* v0.01


### How do I get set up? ###

The solution contains three projects: 

* Tracking Api project
* Addressing Api project 
* Shipping Api project.

Checkout the repo update the configuration files and build all should work, you WILL need a DBP account and an apiuser and an apikey. It won't run with out those.

Each project requires a configuration.xml file, there is a configuration.xml-template in each project.
plug the apiuser, apikey and apiendpoint in each configuration file.

For example this is a sample shipping configuration.xml


```
#!xml#

<configuration>
	<apiuser>MyApiUser</apiuser>
	<apikey>MyApiKey</apikey>
	<apiendpoint>http://api.despatchbaypro.com/soap/v11/shipping.php</apiendpoint>
</configuration>

```

You can use the same apiuser and key for all the projects, but the endpoint will change.
Tracking


```
#!xml#

<apiendpoint>http://api.despatchbaypro.com/soap/v11/tracking.php</apiendpoint>
```


Addressing

```
#!xml#

<apiendpoint>http://api.despatchbaypro.com/soap/v11/addressing.php</apiendpoint>
```



It's VERY basic, there is little to no error handling, the tracking example may not work as is, as the tracking code is probably out of date and you will need a valid from the courier i.e Parcelforce
