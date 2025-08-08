using UnityEngine;
using System.IO;
using System.IO.Compression;
using Sirenix.Serialization;

namespace Backend.Map
{
    // 32x32x32 area as a chunk
    [System.Serializable]
    public class MapChunk
    {
        public const int SIZE = 32;
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
        [OdinSerialize] public bool[] IsSolid = new bool[FULL_SIZE];
        
        // Standable block can be stood on top. It is not necessarily solid.
        [OdinSerialize] public bool[] IsStandable = new bool[FULL_SIZE];
        
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
        [OdinSerialize] public bool[] IsPassable = new bool[FULL_SIZE];

        // Which direction does the floor supports. Typically, it is YPos.
        [OdinSerialize] public FloorDirections[] FloorDirection = new FloorDirections[FULL_SIZE];
        
        // Amount of intrude the block has in FloorDirection. 0~7.
        [OdinSerialize] public byte[] BlockIntrude = new byte[FULL_SIZE];

        // Terrain types merged from GridData
        [OdinSerialize] public miniRAID.GridData.TerrainType[] TerrainTypes = new miniRAID.GridData.TerrainType[FULL_SIZE];

        // Chunk world coordinates
        public Vector3Int ChunkCoordinate { get; set; }

        public MapChunk()
        {
            // Initialize arrays to default values
            for (int i = 0; i < FULL_SIZE; i++)
            {
                IsSolid[i] = false;
                IsStandable[i] = false;
                IsPassable[i] = true; // Default to passable
                FloorDirection[i] = FloorDirections.YPos;
                BlockIntrude[i] = 0;
                TerrainTypes[i] = miniRAID.GridData.TerrainType.Normal;
            }
        }

        public MapChunk(Vector3Int chunkCoordinate) : this()
        {
            ChunkCoordinate = chunkCoordinate;
        }

        // Convert 3D coordinates to flat array index
        // Y is least significant for better gzip compression along Y axis
        public static int CoordinateToIndex(int x, int y, int z)
        {
            return y + x * SIZE + z * SIZE * SIZE;
        }

        // Convert flat array index to 3D coordinates
        public static Vector3Int IndexToCoordinate(int index)
        {
            int y = index % SIZE;
            int x = (index / SIZE) % SIZE;
            int z = index / (SIZE * SIZE);
            return new Vector3Int(x, y, z);
        }

        // Get block data at local chunk coordinates
        public bool GetIsSolid(int x, int y, int z)
        {
            if (x < 0 || x >= SIZE || y < 0 || y >= SIZE || z < 0 || z >= SIZE) return false;
            return IsSolid[CoordinateToIndex(x, y, z)];
        }

        public bool GetIsPassable(int x, int y, int z)
        {
            if (x < 0 || x >= SIZE || y < 0 || y >= SIZE || z < 0 || z >= SIZE) return true;
            return IsPassable[CoordinateToIndex(x, y, z)];
        }

        public bool GetIsStandable(int x, int y, int z)
        {
            if (x < 0 || x >= SIZE || y < 0 || y >= SIZE || z < 0 || z >= SIZE) return false;
            return IsStandable[CoordinateToIndex(x, y, z)];
        }

        public miniRAID.GridData.TerrainType GetTerrainType(int x, int y, int z)
        {
            if (x < 0 || x >= SIZE || y < 0 || y >= SIZE || z < 0 || z >= SIZE) 
                return miniRAID.GridData.TerrainType.Normal;
            return TerrainTypes[CoordinateToIndex(x, y, z)];
        }

        // Set block data at local chunk coordinates
        public void SetIsSolid(int x, int y, int z, bool value)
        {
            if (x < 0 || x >= SIZE || y < 0 || y >= SIZE || z < 0 || z >= SIZE) return;
            IsSolid[CoordinateToIndex(x, y, z)] = value;
        }

        public void SetIsPassable(int x, int y, int z, bool value)
        {
            if (x < 0 || x >= SIZE || y < 0 || y >= SIZE || z < 0 || z >= SIZE) return;
            IsPassable[CoordinateToIndex(x, y, z)] = value;
        }

        public void SetIsStandable(int x, int y, int z, bool value)
        {
            if (x < 0 || x >= SIZE || y < 0 || y >= SIZE || z < 0 || z >= SIZE) return;
            IsStandable[CoordinateToIndex(x, y, z)] = value;
        }

        public void SetTerrainType(int x, int y, int z, miniRAID.GridData.TerrainType value)
        {
            if (x < 0 || x >= SIZE || y < 0 || y >= SIZE || z < 0 || z >= SIZE) return;
            TerrainTypes[CoordinateToIndex(x, y, z)] = value;
        }

        // Serialization methods with gzip compression along Y axis
        public byte[] SerializeToBytes()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    var data = SerializationUtility.SerializeValue(this, DataFormat.Binary);
                    gzipStream.Write(data, 0, data.Length);
                }
                return memoryStream.ToArray();
            }
        }

        public static MapChunk DeserializeFromBytes(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    using (var decompressedStream = new MemoryStream())
                    {
                        gzipStream.CopyTo(decompressedStream);
                        var decompressedData = decompressedStream.ToArray();
                        return SerializationUtility.DeserializeValue<MapChunk>(decompressedData, DataFormat.Binary);
                    }
                }
            }
        }
    }
}