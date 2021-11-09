# Baba-Is-You

## 1.	Software Patterns Used  
  ### Model View Presenter Pattern  
  Actor and its derived class– Model  
  MainWindow class – View and Presenter  

  ### Memento Pattern – Handling Undo  
  Memento class – Memento  
  Originator class – Originator  
  CareTaker class -- CareTaker  


## 2.	class design and reuse
### Actor class:
Actor class is a base class that is created to handle objects in the game. It has two protected int x and y, representing the coordinates of an object. Two protected methods setX and setY are for the updates of the object positions within Actor and its derived class. The coordinates can only be read using getters X and Y properties outside Actor and its derived class. Four methods (isLeftCollision, isRightCollision, isTopCollision, isBottomCollision) are implemented to check if there is any collision with the object itself given another object of class Actor.

The derived classes from Actor class are: Wall, Player, Baggage, GOOP, Area class. These classes represent the walls, the player that can be moved directly using keyboard, baggage objects that can be pushed around by player, goops that will sink/kill the player when the player steps on any of them, areas that is the reach goal of the player.

Classes of the word blocks in the game are further derived from Baggage class as they can be pushed around just like baggage objects. These classes are: BABA, ROCK, IS, YOU, FLAG, WIN, PUSH, WALL, STOP, SINK, and Goop.

Each derived class has its own initialization. When an instances of these classes is created, it creates an object at a particular position (x,y), and initializes its own display (color, shape, text) in the canvas (window).

### State class:
The state class is designed to store an instant state of the ongoing game. It stores all the objects in the game as read-only lists. The object will be restored using the undo method in MainWindow class.

### Memento, Originator and CareTaker classes:
These three classes are designed following the memento pattern for undo element of the game. The state in these classes is a self-defined object created from the State mentioned above.

### MainWindow class:
MainWindow class stores the game objects, parameters, and game levels (there are two levels in total) and create game objects from Actor and its derived class accordingly using initWorld method. Game parameters like OFFSET is the offset of the top left corner of the display to the display area (e.g. canvas for WPF GUI mode). SPACE is the distance for a step move of the objects and should equal the width and height of their corresponding canvas objects in WPF GUI mode. Integer w and h stores the boundaries of the objects, with w (width) being the far most x coordinate and h (height) being the far most y coordinate*. Game levels are stored in string form, with each character representing a certain object type and “\n” being an increase of y coordinate. The initWorld method then scan through the game level string layer by layer and create object instances base on the x y positions transformed from the level string. The boundaries (h and w) are thus obtained in this method.

The buildworld method scans through every object in the object lists and displays the game level objects onto the canvas (window) using the draw method defined in object classes. Update method updates the canvas same as buildWorld after each user event and show messages whenever necessary. The checkWallCollision method takes in an object and returns true if the object has collided with any of the walls in game. This method divides the cases into four direction of collisions (left, right, top, bottom). When scanning through each walls of the game, it makes use of the isDirectionCollision method as defined in the object classes. Similarly, the checkBagCollision method return true if a user is stuck because of the arrangement of the baggage objects (e.g. when user bumps into a baggage object that collides with the wall). Note that if no collision is detected, all the baggage objects concerned are moved along with the user.

The isCompleted method checks if the user has completed the level by placing player into the desired position. The condition varies when given combination of the word blocks. The isDead method check if the user has stepped into any of the deadly areas (e.g. goops). The rockGoopCombo method checks if the user has pushed rocks into any of the goop areas and removes both the sunk rocks and goops if any. The storeWords method extracts word blocks from the list of movable objects, i.e. baggs, and store them into respectively word block class. This method is designed to streamline the process of checking game rules. Methods FlagIsU, BABAIsU, FlagIsWin, RockIsPush check if certain word phrases are aligned.

The restartLevel method is implemented when user wants to restart the level. It clears all the objects and return to the original state of the game. The nextLevel method is invoked when user won a certain level and would like to move on to the next level. The saveState method saves the current game state into the memento. It is invoked whenever the game state is changed. The undo method restores the previous states into the current game with the help of careTaker and originator objects.

The Window_KeyDown method detects a user input and move the objects in the game given the game rules. It ignores the user control inputs when the game has ended, or user has died already. Otherwise, it checks the user key input: R for restart of the current level, Z for undo, arrow keys for moving the player. When user inputs arrow keys, it will check collisions using checkWallCollision and checkBagCollision and move the player if there are no collisions. Whenever the player is moved, it then stores the new game state using saveState method. After each step, it checks the game situation using methods isCompleted, isDead and rockGoopCombo. Finally, update the display using update method.

## 3.	Challenges
Moving multiple baggage objects at a time:
In a Sokoban game, player can at most move one baggage object. However, in BABA IS YOU, player can move multiple objects. Moving multiple baggage objects can be quite a challenge if we want to extend the method for checking baggage collisions in the Sokoban game, which returns true when only two baggage objects are in the way of the player. To solve this, checkbaggCollision method is implement in a recursive fashion: it recursively finds the next collided baggage object and returns true when the next collided baggage object collides with a wall (a terminating condition). If all baggage objects are movable (not touching any of the walls), move all the object recursively and return false.

## 4.	Accommodating the Additional Requirement

As the display of objects in textbox mode are in text format, the initializations of objects are changed to text strings instead of canvas objects. In particular, each object is initialized using four-character spaces, with two in the upper line and two in the lower line. For instance, the display of an object of class ROCK (a word block class) should be “RO” and “CK”, with “RO” in the upper line and “CK” in the lower line with the same indentation as “RO”.

To achieve that, the protected strings firstLine and secondLine are defined in the base class Actor. The corresponding getters are FirstLine and SecondLine for read-only use outside Actor and its derived classes.

For the display method implementation, all the objects are stored in a list of base class Actor, namely, actors. A new string levelText is the string that stores all text display objects and will be written to the textbox afterwards. Two additional methods existActor and getActor are designed to be used in the display methods. existActor takes in a coordinate (x,y) and scan through actors too see if there is an object in position (x,y). It returns true if there is indeed an object in position (x,y). getActor takes in a coordinate (x,y) and returns the first encountered object in actors with coordinate (x,y).

In the buildWorld method, it scans through the xy-plane (of height h and width w*) with y being the outer for loop. For each y value, we concatenate two strings separated by “\n” to levelText as each object will take up two lines in the textbox display. For the first string, it scans through all the x’s and concatenate the firstLine of the object obtained from getActor if existActor returns true. We do the same for the second string except we concatenate the secondLine of the object to it. If existActor returns false (no object in that position), concatenate white spaces to the strings. The update method follows the same design as the buildWorld method with game-situation-checking methods same as before. After scanning the entire xy-plane, levelText is assigned to textbox.Text for the textbox display. 

The other parts of the code remain unchanged.
