# Overview
- **Map store voxelized blocks.**
- Walking mobs can stands on air blocks over solid, standable blocks.
	- Grids (UI) displays on top of those standable blocks.
## Layout
- Store region map directly into scenes
- SceneInfo (visible to backend) is seperated with actual renderings / mesh.
## GameLogic
- 3D "Colliders" (`GridShape`)
	- Bounds for corase matching (overlapping test)
	- Iterate through all grids for overlap test
- Separate Mob / Map (visible to backend) / Rendered Mesh.
	- Map only cares about data.
	- Since map contains non-full blocks, mob's position are no longer integer.
	- Map (`DataBackend`) stores map and mob separately. Upon query, it find mobs overlapping the queried grid and append it into the query.
		- Queries can specify that this data is not needed for speed-up.
		- Queries can only query integer-coorded blocks.
		- Multiple mobs might be retrieved.
	- Mob (esp. `Movement` / `MovementRequester` etc.) handles whether/how will a mob move to another position.
## Movement
Mobs have "Jump" attribute, indicating the maximum height they can jump vertically.
### MovementType: Walk
- Vertical jumps happens at no cost.
- Can jump horizontally for 1 grid.
![[Pasted image 20250804172508.png]]
### MovementType: Flyable
- Can move vertically as regular moving (still counts as distance).
- End turn mid-air will cost extra AP (reduced AP recovery).
## UI / Presentation
Grids only display on top of standable blocks.
Use L/R to go below/above floor level. Double-tap to snap (downwards/upwards) to nearest floor.
UI for floating grids should look like:
![[Pasted image 20250804172058.png]]