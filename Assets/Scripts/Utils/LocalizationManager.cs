using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace miniRAID
{
    public class LocalizationManager
    {
        static private LocalizationManager instance;
        static public LocalizationManager GetSingleton()
        {
            if (instance == null)
            {
                instance = new LocalizationManager();
            }
            return instance;
        }

        public IEnumerator Initialization()
        {
            yield return LocalizationSettings.InitializationOperation;
            Debug.Log("Initialization Completed");
        }
        
        public string L(LocalizedString key)
        {
            if (key == null) return null;
            if (key.TableReference == null) return null;
            if (key.IsEmpty) return null;
            
            return LocalizationSettings.StringDatabase.GetLocalizedString(key.TableReference, key.TableEntryReference);
        }
    }
}