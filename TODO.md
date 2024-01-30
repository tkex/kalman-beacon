# TODO

## Sune 30.1.

1. [x] Python Script nutzt dynamisches deltaT
2. [ ] Python Script nutzt Unity Daten
   1. [ ] Schreibe Logs in Unity
      1. [ ] positionGT = (timestamp, pos.x, pos.y, vel_x, vel_y)
      2. [ ] angleGT = (timestamp, angle_beacon0, angle_beacon1)
      3. [ ] angleNoisy = (timestamp, angle_beacon0, angle_beacon1)
3. [ ] Unity sendet SYNCHRON-Daten über Websocket (erst SYNCHRON, langfristig: asynchron für einzelne Beacons)
   1. [ ] Format: (pos.x)
4. [ ] Python nimmt dynamisch SYNCHRON-Daten über Websocket entgegen und predictet Position
5. [ ] Unity visualisiert Boot mit Python's SYNCHRON-Daten
