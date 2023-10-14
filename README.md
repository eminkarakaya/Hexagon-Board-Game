# Hexagon-Board-Game
When it is finished, it will be a strategy game that is a combination of civilization and legends of runeterra.
# Movement

  Indexes and positions of hexes are assigned in the HexCoordinates class.
  BFS algorithm was used for the path finding algorithm of the characters. (GraphSearch Class)
    Created dictionary named hexsInRange Dictionary<Vector3Int, Vector3Int>  
    ![image](https://github.com/eminkarakaya/Hexagon-Board-Game/assets/71080980/f2239194-9c94-4e68-91a8-c118317e4e87)

     
    The beginning of the arrow indicates value and the end indicates key.
   GneratePathBFS function was created to find the path from the hexagon you clicked to your current location (GraphSearch class)
   The GeneratePathBFS function finds the hexagon we clicked in the hexInRange dictionary and creates the path by continuously going to its key with a while loop.

   Imoveable interface was created. It contains classes such as movementResult.

   Separate movement systems were written for naval units, land units and settler units. (Settler units can pass through other units)
    ![image](https://github.com/eminkarakaya/Hexagon-Board-Game/assets/71080980/b19d8e29-bd8a-4cb5-b377-8feda3047932)


   The settlers class created. Builds a city in the hexagon where it is activated.
   ![Movie_001](https://github.com/eminkarakaya/Hexagon-Board-Game/assets/71080980/07746368-c907-4627-91e6-1736adae6cfa)

   Unit class was created. Includes interfaces ISelectable, IMovable, IAttackable, IVisionable,IDamagable, ISideable,ITaskable.
# ATTACK
   You can hit a unit containing Iattackable with a unit containing Idamagable.
   ![1012](https://github.com/eminkarakaya/Hexagon-Board-Game/assets/71080980/4fecc32c-0bd2-437a-9a23-f0ee24103eed)

   You can change the attack type from the inspector
   
   ![image](https://github.com/eminkarakaya/Hexagon-Board-Game/assets/71080980/1461b256-129e-45ba-aa67-eba90035eaf8)

   Default - 
    ![Movie_013](https://github.com/eminkarakaya/Hexagon-Board-Game/assets/71080980/503126f5-3318-4af1-84b8-4f2c3d5cad2e)


   Take Hostage- 
   ![Movie_015](https://github.com/eminkarakaya/Hexagon-Board-Game/assets/71080980/822ac3a2-1ebd-4433-9138-cb083cf8f86d)


   I made 2 for now, can be increased
  # VISION

  If objects with IVisionable are not within vision range, they switch to a different camera layer and are not visible (Ivisionable and vision classes).
  In the vision class, you can adjust how far the hexagon can see with the vision range. (range 2 in this video)
  
  ![1012(1)](https://github.com/eminkarakaya/Hexagon-Board-Game/assets/71080980/798a59d1-abd3-43d1-91ea-a33914e69312)

  # CAPTURABLE OBJECTS
    Objects with ISideable can be captured (change sides)

  ![1014](https://github.com/eminkarakaya/Hexagon-Board-Game/assets/71080980/3925233f-9d5c-4e2b-a38b-a8a1f11a9abb)

  
    
  # BUILDINGS
  ### Base
     you can produce units from the base.
  ![image](https://github.com/eminkarakaya/Hexagon-Board-Game/assets/71080980/b59be47b-7ee8-461e-bc00-be8959f41ae7)

  ### MINE
  You can mine the resources.
  Mines earn money per turn.
  Mines can be plundered and seized.
  Mines can be built by workers.
  ![Movie_022](https://github.com/eminkarakaya/Hexagon-Board-Game/assets/71080980/981d1452-ef30-491b-82b0-716a00884826)

  ![1014(1)](https://github.com/eminkarakaya/Hexagon-Board-Game/assets/71080980/23e54eb6-1116-4ee3-aabe-8bf80c0578ee)

  # ROUND MANAGER
    Everyone must press the next turn button to complete the round.

    
https://github.com/eminkarakaya/Hexagon-Board-Game/assets/71080980/6175f7a6-bfc7-4eb2-b10a-b7a38652f74f


  
  # MULTIPLAYER

  # DEALS
    You can make agreements with other civilizations (for now there is only war and peace)
  https://github.com/eminkarakaya/Hexagon-Board-Game/assets/71080980/e7120f89-55e0-4bda-97ca-3a1fec3c2e2e



  # LOBBY
  ##coming soon
  # DECK SYSTEM
  Legends of Runeterra style deck creation system has been created. It has not been integrated into the game yet.
  
  # PLAYFAB
  Registration and login system was created. 
    Data was kept in the database.
  
https://github.com/eminkarakaya/Hexagon-Board-Game/assets/71080980/e510db8d-4cc8-4be7-b6a4-075bcebbf169


    
  
   
- Playfab was used for data storage and user login.
- Mirror network was used for multiplayer.
- State machine will be used for AI (not done yet)

DONE
Legends of Runeterra style deck system

## CONTACT

  meminkkaya1@gmail.com
  
  https://www.linkedin.com/in/muhammet-emin-karakaya/
  
  [@eminkarakaya](https://github.com/eminkarakaya)

  project link : https://github.com/eminkarakaya/Hexagon-Board-Game/

  Gameplay = https://www.youtube.com/watch?v=NTXiuueBVoE





