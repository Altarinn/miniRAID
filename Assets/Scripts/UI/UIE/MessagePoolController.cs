using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using System;
using System.Linq;

using miniRAID;
using System.Text;
using Sirenix.Utilities;

namespace miniRAID.UIElements
{
    public class MessagePoolController
    {
        VisualElement masterElem, messagePoolContainer;
        private Label recentMessage, allMessage;
        private Button toggleLog, toggleSound, downloadLog;

        public string message;

        private bool sound;

        public MessagePoolController(VisualElement elem)
        {
            masterElem = elem;

            messagePoolContainer = elem.Q("MessagePoolContainer");
            recentMessage = elem.Q<Label>("RecentMessage");
            allMessage = elem.Q<Label>("AllMessages");
            toggleLog = elem.Q<Button>("ToggleLog");
            toggleSound = elem.Q<Button>("ToggleBGM");
            downloadLog = elem.Q<Button>("DownloadLog");

            toggleLog.clicked += TogglePool;
            toggleSound.clicked += ToggleSound;
            downloadLog.clicked += DownloadLogOnclicked;

            SetSound(true);

            message = "";
        }

        private void DownloadLogOnclicked()
        {
            Globals.combatTracker.ExportJSON();
        }

        public void TogglePool()
        {
            Debug.Log("TogglePool");
            if (messagePoolContainer.resolvedStyle.visibility == Visibility.Hidden)
            {
                messagePoolContainer.style.visibility = Visibility.Visible;
                messagePoolContainer.pickingMode = PickingMode.Position;
            }
            else
            {
                messagePoolContainer.style.visibility = Visibility.Hidden;
                messagePoolContainer.pickingMode = PickingMode.Ignore;
            }
        }

        void SetSound(bool val)
        {
            sound = val;
            GameObject.FindObjectOfType<AudioListener>().enabled = sound;
            GameObject.FindObjectsOfType<AudioSource>().ForEach(source => source.mute = !sound);
        }

        public void ToggleSound()
        {
            SetSound(!sound);
            toggleSound.style.color = sound ? Color.white : Color.gray;
        }

        public void AddMessage(string message)
        {
            string minSec = string.Format("<color=#92d7e7>回合{0}</color> <color=#d3e173>{1:00}m {2:00}s</color>", miniRAID.Globals.combatMgr.Instance.turn, (int)Time.realtimeSinceStartup / 60, (int)Time.realtimeSinceStartup % 60);
            
            this.message += $"{minSec} {message}\n";
            allMessage.text = this.message;
            recentMessage.text = $"{minSec} {message}";
        }
    }
}