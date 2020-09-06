# LogicSIM
Logic simulator written in VB.NET

![LogicSIM full adder](https://xfx.net/stackoverflow/LogicSIM/logicsim04.png)

### Currently supported gates

| Logic gates | I/O gates | Miscellaneous |
|-------------|-----------|---------------|
| OR          | LED       | Wire [autorouting](https://github.com/RedpointGames/AStarPathFinder/blob/master/AStarPathFinder/PathFinder.cs) |
| AND         | SWITCH    | Wire nodes
| NOR         | CLOCK     |
| NAND        |           |
| NOT         |           |
| XOR         |           |
| XNOR        |           |

### Working with the simulator

The program lacks lots of the features expected in an end-user/production application, as it is a work in progress.
At this moment it's more a proof of concept than a usable logic simulator.

* Adding: Select a gate from the left pane and the click on the circuit surface.
* Deleting: Select a gate from the circuit surface and press de Delete key.
* Rotating: Select a gate from the circuit surface and press de Tab key. Hold the Shift key to rotate the gate counter clockwise.
* Connecting: Drag a wire from either an input or an output pin to the desired pin.
* Multiple Connections: Add a Node to the circuit surface to connect up to four gates.
* Saving/Loading: Not implemented. When closing the simulator, the circuit will be saved in an XML file which will be automatically loaded when re-opening the program.
