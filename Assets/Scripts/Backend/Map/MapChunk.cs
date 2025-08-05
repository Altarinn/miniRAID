namespace Backend.Map
{
    // 32x32x32 area as a chunk
    public class MapChunk
    {
        const int SIZE = 32;
        private const int FULL_SIZE = SIZE * SIZE * SIZE;

        public enum FloorDirections : byte
        {
            YPos,
            YNeg,
            XPos,
            XNeg,
            ZPos,
            ZNeg
        }

        // Solid blocks can block line of sight, projectile paths and so on.
        public bool[] IsSolid = new bool[FULL_SIZE];
        
        // Standable block can be stood on top. It is not necessarily solid.
        public bool[] IsStandable = new bool[FULL_SIZE];
        
        // Passable block can contain mobs.
        // Mob can stand on floor only in a passable block with standable block at the same position or underneath
        // (+ correct FloorDirection).
        // Possible?    Standable   Passable
        // YES          [F]         [T]
        //              [T]         [F]
        // YES          [T]         [T]
        // NO           [F]         [T]
        //              [F]         [F]
        // NO           [T]         [F]
        //              [T]         [F]
        public bool[] IsPassable = new bool[FULL_SIZE];

        // Which direction does the floor supports. Typically, it is YPos.
        public FloorDirections[] FloorDirection = new FloorDirections[FULL_SIZE];
        
        // Amount of intrude the block has in FloorDirection. 0~7.
        public byte[] BlockIntrude = new byte[FULL_SIZE];
    }
}