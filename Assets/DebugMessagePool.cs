using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class DebugMessagePool : MonoBehaviour
{
    public TMPro.TextMeshProUGUI messagePrefab;
    public Button toggleButton;
    public Color activateColor, deactivateColor;

    public void Message(string message)
    {
        var msg = Instantiate(messagePrefab.gameObject, transform).GetComponent<TMPro.TextMeshProUGUI>();
        string minSec = string.Format("<color=#92d7e7>回合{0}</color> <color=#d3e173>{1:00}m {2:00}s</color>", miniRAID.Globals.combatMgr.Instance.turn, (int)Time.realtimeSinceStartup / 60, (int)Time.realtimeSinceStartup % 60);
        msg.text = $"{minSec} {message}";
    }

    public CanvasGroup parentGroup;
    bool _activated = false;

    public void Toggle()
    {
        if(_activated)
        {
            parentGroup.interactable = false;
            parentGroup.blocksRaycasts = false;

            parentGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 64);

            var cb = toggleButton.colors;
            cb.normalColor = deactivateColor;
            cb.selectedColor = deactivateColor;
            toggleButton.colors = cb;

            _activated = false;
        }
        else
        {
            parentGroup.interactable = true;
            parentGroup.blocksRaycasts = true;

            parentGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 200);

            var cb = toggleButton.colors;
            cb.normalColor = activateColor;
            cb.selectedColor = activateColor;
            toggleButton.colors = cb;

            _activated = true;
        }
    }
}
