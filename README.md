# Hexagon-Board-Game
When it is finished, it will be a strategy game that is a combination of civilization and legends of runeterra.

  Indexes and positions of hexes are assigned in the HexCoordinates class.
  BFS algorithm was used for the path finding algorithm of the characters. (GraphSearch Class)
    Created dictionary named hexsInRange Dictionary<Vector3Int, Vector3Int>  
    ![image](https://github.com/eminkarakaya/Hexagon-Board-Game/assets/71080980/b854ff8b-3a85-442d-ae8c-4facd5c210d9)
     
    The beginning of the arrow indicates value and the end indicates key.
   GneratePathBFS function was created to find the path from the hexagon you clicked to your current location (GraphSearch class)
   The GeneratePathBFS function finds the hexagon we clicked in the hexInRange dictionary and creates the path by continuously going to its key with a while loop.

    
    
- Playfab was used for data storage and user login.
- Mirror network was used for multiplayer.
- State machine will be used for AI (not done yet)

DONE
Legends of Runeterra style deck system






https://www.youtube.com/watch?v=NTXiuueBVoE
