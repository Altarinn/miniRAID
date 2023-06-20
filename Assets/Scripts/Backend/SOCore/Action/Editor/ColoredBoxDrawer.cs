using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace miniRAID.Editor
{
    [DrawerPriority(20)]
    public class ColoredBoxDrawer : OdinAttributeDrawer<ColoredBoxAttribute>
    {
        public static Vector3 LerpHSV(Color a, Color b, float x)
        {
            Vector3 ah;
            Color.RGBToHSV(a, out ah.x, out ah.y, out ah.z);
            Vector3 bh;
            Color.RGBToHSV(b, out bh.x, out bh.y, out bh.z);

            return new Vector3(
                Mathf.LerpAngle(ah.x, bh.x, x),
                Mathf.Lerp(ah.y, bh.y, x),
                Mathf.Lerp(ah.z, bh.z, x)
            );
        }

        private Color LerpColorHSV(Color a, Color b, float t)
        {
            var newHSV = LerpHSV(a, b, t);
            return Color.HSVToRGB(newHSV.x, newHSV.y, newHSV.z);
        }
        
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Color oldColor = GUI.backgroundColor;
            Color baseColor = Attribute.color;
            GUI.backgroundColor = baseColor;

            // SirenixEditorGUI.Title(Property.NiceName, "", TextAlignment.Left, true, true);

            SirenixEditorGUI.BeginBox();

            if(Property.ValueEntry.WeakSmartValue == null)
            {
                CallNextDrawer(label);
            }
            else
            {
                var labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.normal.textColor = baseColor;
                labelStyle.hover.textColor = Color.Lerp(Color.white, baseColor, 0.25f);
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.fontStyle = FontStyle.Bold;
                
                var subtitleStyle = new GUIStyle(GUI.skin.label);
                subtitleStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
                subtitleStyle.alignment = TextAnchor.MiddleCenter;
                subtitleStyle.fontSize = 10;
            
                EditorGUILayout.LabelField($"↯ {Property.NiceName}", labelStyle, GUILayout.Height(17));
                EditorGUILayout.LabelField($"{Property.ValueEntry.TypeOfValue}", subtitleStyle, GUILayout.Height(14));

                Property.GetActiveDrawerChain().MoveNext();
                Property.GetActiveDrawerChain().MoveNext();

                CallNextDrawer(null);
            }

            SirenixEditorGUI.EndBox();

            GUI.backgroundColor = oldColor;
        }
    }
}