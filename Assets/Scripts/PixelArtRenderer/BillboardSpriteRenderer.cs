using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BillboardSpriteRenderer : MonoBehaviour
{
    [Header("Sprite Settings")]
    [SerializeField] private Sprite sprite;
    [SerializeField] private Color color = Color.white;
    [SerializeField] private float size = 1f;
    [SerializeField] private bool flipX = false;
    [SerializeField] private bool flipY = false;
    
    [Header("Rendering")]
    [SerializeField] private Material materialOverride;
    [SerializeField] private int sortingOrder = 0;
    [SerializeField] private string sortingLayerName = "Default";
    
    // Components
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh quadMesh;
    private MaterialPropertyBlock propertyBlock;
    
    // Property IDs for shader
    private static readonly int MainTexProp = Shader.PropertyToID("_MainTex");
    private static readonly int ColorProp = Shader.PropertyToID("_Color");
    private static readonly int ScaleProp = Shader.PropertyToID("_Scale");
    private static readonly int PixelSizeProp = Shader.PropertyToID("_PixelSize");
    
    // Default billboard shader
    private const string DefaultShaderName = "PixelArtURP/3DSprites";
    
    public Sprite Sprite
    {
        get => sprite;
        set
        {
            sprite = value;
            UpdateSprite();
        }
    }
    
    public Color Color
    {
        get => color;
        set
        {
            color = value;
            UpdateColor();
        }
    }
    
    public float Size
    {
        get => size;
        set
        {
            size = value;
            UpdateSize();
        }
    }

    public Vector3 Pivot;
    
    void Awake()
    {
        Initialize();
    }
    
    void OnEnable()
    {
        Initialize();
        UpdateSprite();
    }
    
    void OnValidate()
    {
        if (!Application.isPlaying || meshRenderer == null)
        {
            Initialize();
        }
        UpdateSprite();
    }
    
    void Initialize()
    {
        // Get or add components
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        
        // Create property block if needed
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
        
        // Create or get quad mesh
        if (quadMesh == null)
            CreateQuadMesh();
        
        // Setup mesh filter
        meshFilter.sharedMesh = quadMesh;
        
        // Setup material if not set
        if (materialOverride == null && meshRenderer.sharedMaterial == null)
        {
            Shader billboardShader = Shader.Find(DefaultShaderName);
            if (billboardShader != null)
            {
                meshRenderer.sharedMaterial = new Material(billboardShader);
            }
        }
        else if (materialOverride != null)
        {
            meshRenderer.sharedMaterial = materialOverride;
        }
        
        // Disable static batching for this renderer
        // meshRenderer.staticBatchingEnabled = false;
        // meshRenderer.dynamicBatchingEnabled = false;
        
        // Set sorting layer and order
        meshRenderer.sortingLayerName = sortingLayerName;
        meshRenderer.sortingOrder = sortingOrder;
        
        // Ensure we receive shadows but don't cast them (typical for sprites)
        meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        meshRenderer.receiveShadows = true;
    }
    
    void CreateQuadMesh()
    {
        quadMesh = new Mesh();
        quadMesh.name = "BillboardQuad";
        
        // Create a unit quad centered at origin
        Vector3[] vertices = {
            new Vector3(-0.5f, -0.5f, 0f) - Pivot,
            new Vector3(0.5f, -0.5f, 0f) - Pivot,
            new Vector3(-0.5f, 0.5f, 0f) - Pivot,
            new Vector3(0.5f, 0.5f, 0f) - Pivot
        };
        
        Vector2[] uvs = {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f)
        };
        
        int[] triangles = {
            0, 2, 1,
            1, 2, 3
        };
        
        Color[] colors = {
            Color.white,
            Color.white,
            Color.white,
            Color.white
        };
        
        quadMesh.vertices = vertices;
        quadMesh.uv = uvs;
        quadMesh.triangles = triangles;
        quadMesh.colors = colors;
        quadMesh.RecalculateNormals();
        quadMesh.RecalculateBounds();
    }
    
    void UpdateSprite()
    {
        if (meshRenderer == null || propertyBlock == null)
            return;
        
        // meshRenderer.GetPropertyBlock(propertyBlock);
        
        if (sprite != null)
        {
            // Set the sprite texture
            propertyBlock.SetTexture(MainTexProp, sprite.texture);
            
            // Update UVs based on sprite rect if it's from an atlas
            if (sprite.packed || sprite.textureRect != new Rect(0, 0, sprite.texture.width, sprite.texture.height))
            {
                UpdateUVsFromSprite();
            }
            
            // Update aspect ratio based on sprite
            UpdateAspectRatio();
        }
        else
        {
            // Clear texture if no sprite
            propertyBlock.Clear();
        }
        
        // Update other properties
        UpdateColor();
        UpdateSize();
        
        meshRenderer.SetPropertyBlock(propertyBlock);
    }
    
    void UpdateUVsFromSprite()
    {
        if (sprite == null || quadMesh == null)
            return;
        
        // Get UV coordinates from sprite
        Vector2[] spriteUVs = sprite.uv;
        
        // Map sprite UVs to quad, considering flip settings
        Vector2[] uvs = new Vector2[4];
        
        if (!flipX && !flipY)
        {
            uvs[0] = spriteUVs[0]; // Bottom-left
            uvs[1] = spriteUVs[1]; // Bottom-right  
            uvs[2] = spriteUVs[2]; // Top-right
            uvs[3] = spriteUVs[3]; // Top-left
        }
        else if (flipX && !flipY)
        {
            uvs[0] = spriteUVs[1];
            uvs[1] = spriteUVs[0];
            uvs[2] = spriteUVs[3];
            uvs[3] = spriteUVs[2];
        }
        else if (!flipX && flipY)
        {
            uvs[0] = spriteUVs[3];
            uvs[1] = spriteUVs[2];
            uvs[2] = spriteUVs[1];
            uvs[3] = spriteUVs[0];
        }
        else // Both flipped
        {
            uvs[0] = spriteUVs[2];
            uvs[1] = spriteUVs[3];
            uvs[2] = spriteUVs[0];
            uvs[3] = spriteUVs[1];
        }
        
        quadMesh.uv = uvs;
    }
    
    void UpdateAspectRatio()
    {
        if (sprite == null || quadMesh == null)
            return;
        
        float aspectRatio = sprite.rect.width / sprite.rect.height;
        
        Vector3[] vertices = new Vector3[]
        {
            new Vector3((-0.5f - Pivot.x) * aspectRatio, -0.5f - Pivot.y, 0f),
            new Vector3((0.5f - Pivot.x) * aspectRatio, -0.5f - Pivot.y, 0f),
            new Vector3((-0.5f - Pivot.x) * aspectRatio, 0.5f - Pivot.y, 0f),
            new Vector3((0.5f - Pivot.x) * aspectRatio, 0.5f - Pivot.y, 0f)
        };
        
        quadMesh.vertices = vertices;
        quadMesh.RecalculateBounds();
    }
    
    void UpdateColor()
    {
        if (meshRenderer == null || propertyBlock == null)
            return;
        
        // meshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(ColorProp, color);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }
    
    void UpdateSize()
    {
        if (meshRenderer == null || propertyBlock == null)
            return;
        
        // meshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetInt(PixelSizeProp, Mathf.FloorToInt(sprite.rect.height));
        meshRenderer.SetPropertyBlock(propertyBlock);
    }
    
    // Helper method to create a GameObject with BillboardSpriteRenderer
    [MenuItem("GameObject/3D Object/Billboard Sprite", false, 10)]
    public static void CreateBillboardSprite()
    {
        GameObject go = new GameObject("Billboard Sprite");
        go.AddComponent<BillboardSpriteRenderer>();
        
        // Place it at scene view camera position
        if (SceneView.lastActiveSceneView != null)
        {
            go.transform.position = SceneView.lastActiveSceneView.camera.transform.position + 
                                   SceneView.lastActiveSceneView.camera.transform.forward * 5f;
        }
        
        Selection.activeGameObject = go;
    }
    
    // Gizmo to show sprite bounds in Scene view
    void OnDrawGizmosSelected()
    {
        if (sprite == null)
            return;
        
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        
        float aspectRatio = sprite.rect.width / sprite.rect.height;
        Vector3 size3D = new Vector3(aspectRatio * size, size, 0.1f);
        
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size3D);
    }
}

// Custom property drawer for better inspector experience
#if UNITY_EDITOR
[CustomEditor(typeof(BillboardSpriteRenderer))]
public class BillboardSpriteRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BillboardSpriteRenderer renderer = (BillboardSpriteRenderer)target;
        
        EditorGUI.BeginChangeCheck();
        
        // Sprite field with preview
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Sprite", EditorStyles.boldLabel);
        
        Sprite newSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", renderer.Sprite, typeof(Sprite), false);
        if (newSprite != renderer.Sprite)
        {
            Undo.RecordObject(renderer, "Change Sprite");
            renderer.Sprite = newSprite;
        }
        
        // Show sprite preview
        if (renderer.Sprite != null)
        {
            Texture2D preview = AssetPreview.GetAssetPreview(renderer.Sprite);
            if (preview != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(preview, GUILayout.Width(64), GUILayout.Height(64));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
        
        EditorGUILayout.Space();
        
        // Draw the rest of the default inspector
        DrawDefaultInspector();
        
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(renderer);
        }
    }
}
#endif