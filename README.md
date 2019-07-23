# Unity Pedestrian and Crowd Simulation

A collection of templates to add lively pedestrians into your environment. The templates make use of existing Unity NavMesh system for natural path finding and object avoiding.

Unity-PedSIM includes:

## 1. Pedestrian path follow
With this template, it's possible to
- Create a pedestrian path for each pedestrian
- Add multiple path nodes, each node can be set with different speed
- Wait for a number of seconds before start moving to next node
- Possible to be destroyed after reaching the end.

Demonstration video

[![](http://img.youtube.com/vi/ZEOUTaBslIQ/0.jpg)](http://www.youtube.com/watch?v=ZEOUTaBslIQ "")

## 2. Crowd Generation
With this template, it's possible to
- Use existing path from a Pedestrian Path and is able to randomly generate new pedestrians. 
- Randomise the paths: create a slight alternation to previous path so pedestrian will walk more naturally
- Boid-like movement: match surrounding speed, avoid other pedestrians.

Demonstration video

[![](http://img.youtube.com/vi/1i5-lEEf59o/0.jpg)](http://www.youtube.com/watch?v=1i5-lEEf59o "")

## 3. Social Force Model pedestrian
This template is an implementation of the Social Force Model based on existing C++ codes from https://github.com/fawwazbmn/SocialForceModel. The model provides more natural walking behaviour for medium to large group of pedestrians.

All three templates can be found at *Example* scene.
