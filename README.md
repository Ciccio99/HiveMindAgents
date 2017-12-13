# HiveMindAgents
Project that allows the control of autonomous crowds and flocks

### Class CSCI - 712

### Team
- Alberto Scicali
- Yeshwanth Raja

# Final Project Report

### What we set out to do
The initial goal was to create a no programming solution to allow the control of flocks within Unity. The user would be able to adjust parameters that modulate how the flocks interact with each other and their environment. Furthermore, the user is able to attach any models so they can create flocks of various creatures. Lastly, the user will be able to give the boids a leader to follow and swarm around - a pathing tool will also be available so that specific paths can be created for the flocks to follow.

## Solution Design

![alt text](/architecturepic.png "Basic Boid Architecture")
![alt text](/boidmanagerpic.png "BoidManager Component Editor View")

### Implementation Details
- In this boid system, all BoidSubordinates are, essentially, independent. Every BoidSubordinate references its BoidManager to gain access to the properties that determine the weights and values for a boid’s movement. A single Leader can be set for the boids to move towards; alternatively a BoidPather component can be placed in the BoidManager, which will determine a path for the boids to follow.

- The boids are based on Craig Reynold’s classic model of boid simulations. We have added extra features to to their various move functions in order to enhance the simulation. Each of the original movement functions: Separation, Alignment and Cohesion have a linear “magnetic” calculation added to them. For example, in the Separation Function as the a boid moves closer to another neighboring boid, the force that directs the boid away from its neighbor grows stronger, while the further away it is from its neighbor, the weaker the force is. The same is true for the Cohesion, Avoidance and LeaderFollow functions. 

- The pathing tool is a simple tool that allows the user to place points in space. The points will be visibly connected together to show the user the relative path in which the boids will take. The user also has to the option to make the path looping, therefore allowing the boids to continuously cycle through the path points.

![alt text](/pathingtoolpic.png "Boid Pathing Tool")

### Results (Video links)
- https://youtu.be/-w_157MDxg8
- https://youtu.be/WWVhrH45amw
- https://youtu.be/kduzdqEIeNI
- https://youtu.be/eK1gYoRP0NE
- https://youtu.be/qSQRMgICxa4

### Future work
Polishing and applying finishing touches to publish the application in the Unity Asset store
Applying machine learning to the boids to make them more “intelligent” in the sense, they know the flock they belong to
Using algorithms for path finding instead of defining set paths
Advanced path creation


# Original Propsal

### Summary
- Create a Unity3D tools that allows the control of autonomous agents
- Intelligent crowd coordination
- Crowds can communicate information to each other agent within user specified parameters
- Flocking agents can navigate through complex environment given a specific end goal

### Goals
- Demonstrate knowledge of classic Reynold's Boids Algorithm
- Demonstrate knowldege of simulated crowd control
- Allow user to control agents without any programming necessary.
- Allow user to maintain extensive control over the interactions of crowds and flocks via open parameters
- Implement latest and relevant research involved in flock and crowd simulations

#### Stretch Goals
- Integrate crowd & flock control systems into ARkit 

### System & Software
- MacOS & Windows 10
- Unity 3D 2017.1
- ARkit
- C#

### Papers/Research
- http://dl.acm.org/citation.cfm?id=1921604&CFID=806143792&CFTOKEN=51762423
- http://dl.acm.org/citation.cfm?id=2034534&CFID=806143792&CFTOKEN=51762423
- http://dl.acm.org/citation.cfm?id=1272705&CFID=806143792&CFTOKEN=51762423
- http://dl.acm.org/citation.cfm?id=2038840&CFID=806143792&CFTOKEN=51762423
- http://dl.acm.org/citation.cfm?id=2503487&CFID=806143792&CFTOKEN=51762423
