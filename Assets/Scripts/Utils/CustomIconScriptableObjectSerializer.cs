using MemoryPack;
using UnityEngine.AddressableAssets;

namespace miniRAID
{
    public class CustomIconScriptableObjectFormatter : MemoryPackFormatter<CustomIconScriptableObject>
    {
        public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CustomIconScriptableObject value)
        {
            if (value == null)
            {
                writer.WriteNullObjectHeader();
                return;
            }
            
            writer.WriteString(value.Guid);
        }

        public override void Deserialize(ref MemoryPackReader reader, ref CustomIconScriptableObject value)
        {
            if (reader.PeekIsNull())
            {
                reader.Advance(1);
                value = null;
                return;
            }

            var Guid = reader.ReadString();
            var key = new AssetReference(Guid);
            value = Addressables.LoadAssetAsync<CustomIconScriptableObject>(key).WaitForCompletion();
        }
    }
}