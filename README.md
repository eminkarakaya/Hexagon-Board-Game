# Hexagon-Board-Game
When it is finished, it will be a strategy game that is a combination of civilization and legends of runeterra.

  Indexes and positions of hexes are assigned in the HexCoordinates class.
  BFS algorithm was used for the path finding algorithm of the characters. (GraphSearch Class)
    Created dictionary named hexsInRange Dictionary<Vector3Int, Vector3Int>  
    ![image](https://github.com/eminkarakaya/Hexagon-Board-Game/assets/71080980/f2239194-9c94-4e68-91a8-c118317e4e87)

     
    The beginning of the arrow indicates value and the end indicates key.
   GneratePathBFS function was created to find the path from the hexagon you clicked to your current location (GraphSearch class)
   The GeneratePathBFS function finds the hexagon we clicked in the hexInRange dictionary and creates the path by continuously going to its key with a while loop.

   Separate movement systems were written for naval units, land units and settler units. (Settler units can pass through other units)
    ![image](https://github.com/eminkarakaya/Hexagon-Board-Game/assets/71080980/b19d8e29-bd8a-4cb5-b377-8feda3047932)
    
- Playfab was used for data storage and user login.
- Mirror network was used for multiplayer.
- State machine will be used for AI (not done yet)

DONE
Legends of Runeterra style deck system






https://www.youtube.com/watch?v=NTXiuueBVoE
