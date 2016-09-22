# A-Star-Path-Finding-Visual-Implementation
This is the implementation of A-Star(A*) Path Finding algorithm in windows forms using C# that I have done as a project for Artifical Intelligence Course At University.

## Get This thing run!
Follow the instructions to make this application run.

### Map File:
  An image file that consists of only two colors.White pixels are representative of walkable areas of a map.and Black pixels stand for obstacles.You can use the sample map file as an example.

### Introduce Map File to System
  You only need to set the "imageFilePath" private property in the Form1.cs file in any way you want.(I hardcoded for the ease
of implementation at the time of writing this application)

### Find the Path!
  Now,you've set that map variable you are now able to run the application.After running,you have 3 keys available.
Follow these steps:

1. Push "Generate Graph" button. and wait for the map graph to be generated based on the map file.
2. Click on two different arbitrary points of the map.One by one.
**Note:** first click sets source point,and second click sets destination.
3. Now push the button labeld as "Draw Path".A dialog box appears to inform you of the time spent to find the path in milliseconds.
4. In case you want to clear the screen of the map and repeat the path finding steps,you may click on "Clear" button.

I would be glad to recieve your feedback on this project.

**Note:** One thing to note is that,a data struture called PriorityQueue is used in implementation of A-Star algorithm,I faked this data structure at the time,in order to focus on the A-Star itself.
