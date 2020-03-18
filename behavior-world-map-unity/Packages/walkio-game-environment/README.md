# Module Overview

This modules is solely responsible for managing environment affair.

How world is represented should be separated from how pathfinding data is represented. For example, suppose the world map is defined by

- width: 20
- height: 30

and

- tileWidth: 10
- tileHeight: 10

This will form a very nice pathfinding tile data.

However, if the world map is

- width: 32
- height: 46

and

- tileWidth: 12
- tileHeight: 9

Then forming the tile data will be a bit tricky. As for the calculation, there will be

- horizontal tile count: 3
- vertical tile count: 6

Which will be ```3 * 6 = 18``` tiles. But there will be tiles that have no mapped world map info, which need to assign out boundary data for it.