using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace miniRAID.Editor
{
    public class SOWizard : EditorWindow
    {
        private System.Type type;
        private System.Action<System.Type, string> onChosen;

        private List<System.Type> derivedTypes;

        public void Setup(System.Type SOType, System.Action<System.Type, string> onChosen, string defaultName)
        {
            type = SOType;
            this.onChosen = onChosen;
            this.name = defaultName;

            derivedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract)
                .ToList();
        }

        private Vector2 scrollPos;
        private string name;
        private int selectedIndex;

        void OnGUI()
        {
            EditorGUILayout.LabelField($"Please choose an implementation of {type}:");
            EditorGUILayout.LabelField($"<Insert search bar>");

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            Color color_default = GUI.backgroundColor;
            Color color_selected = Color.blue;

            GUIStyle itemStyle = new GUIStyle(GUI.skin.button); //make a new GUIStyle

            itemStyle.alignment = TextAnchor.MiddleLeft; //align text to the left
            itemStyle.active.background = itemStyle.normal.background; //gets rid of button click background style.
            itemStyle.margin =
                new RectOffset(0, 0, 0,
                    0); //removes the space between items (previously there was a small gap between GUI which made it harder to select a desired item)

            for (int i = 0; i < derivedTypes.Count; i++)
            {
                GUI.backgroundColor = (selectedIndex == i) ? color_selected : Color.clear;

                //show a button using the new GUIStyle
                if (GUILayout.Button($"{derivedTypes[i]}", itemStyle))
                {
                    selectedIndex = i;
                    //do something else (e.g ping an object)
                }

                GUI.backgroundColor = color_default; //this is to avoid affecting other GUIs outside of the list
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.LabelField("Please enter name:");
            name = EditorGUILayout.TextField(name);

            if (GUILayout.Button("Confirm"))
            {
                onChosen.Invoke(derivedTypes[selectedIndex], name);
                Close();
            }

            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
        }
    }
}