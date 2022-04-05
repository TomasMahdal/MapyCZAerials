# OMSI and LOTUS Map Aerials
Tool, that enables you to use [Mapy.cz](http://mapy.cz/) maps in simulators like OMSI (2) and LOTUS.
Why [Mapy.cz](http://mapy.cz/)? Because they have done amazing work on mapping Czech and Slovak. Some map types for example touristic maps are just amazing.

![image](https://user-images.githubusercontent.com/47004276/161613768-979b57b7-ad6e-4a76-bc78-8be5a7f0099e.png)

## Requirements
 - .NET Framework 4.7.2 and newer

## How does it work?
This tool creates a WWW server on your local computer.
It generates a URL, that you copy into the simulator ([How to use it with simulators?](HOW_TO.MD)).
Then you download the map aerials in a simulator like normal and use it for creating your map.

### Yes, you can change map type at runtime!
This feature is added by this tool. Simulators are using only one URL, so it is getting only one type of map. If you use this tool, you can change the map type in the tool and redownload tilemaps in the simulator and you’ve got another type of map!
