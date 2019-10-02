# Unity Pedestrian and Crowd Simulation

![](https://i.imgur.com/wTtoX0y.png)

A collection of templates to add lively pedestrians into your environment. The templates make use of existing Unity NavMesh system for natural path finding and object avoiding.

Unity Pedestrian SIM includes:

## 1. Pedestrian path follow
With this template, it's possible to
- Create a pedestrian path for each pedestrian
- Add multiple path nodes, each node can be set with different speed
- Wait for a number of seconds before start moving to next node
- Possible to be destroyed after reaching the end.

Demonstration video

[![](https://i.imgur.com/b2I7MrI.png)](http://www.youtube.com/watch?v=ZEOUTaBslIQ "")

## 2. Crowd Generation
With this template, it's possible to
- Use existing path from a Pedestrian Path and is able to randomly generate new pedestrians. 
- Randomise the paths: create a slight alternation to previous path so pedestrian will walk more naturally
- Boid-like movement: match surrounding speed, avoid other pedestrians.

Demonstration video

[![](https://i.imgur.com/KvITZ9n.png)](http://www.youtube.com/watch?v=1i5-lEEf59o "")

## 3. Social Force Model pedestrian
This template is an implementation of the Social Force Model based on the research by Helbing and Molnár (1995), later improved by Moussaïd et al. (2009). The model provides more natural walking behaviour for medium to large group of pedestrians.

All three templates can be found at *Example* scene.
