using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class LightmapSystemTagEditor : MonoBehaviour
{
    public int systemTag = 1; // Change this per object

    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            SerializedObject so = new SerializedObject(GetComponent<Renderer>());
            SerializedProperty sp = so.FindProperty("m_SystemTag");
            if (sp != null)
            {
                sp.intValue = systemTag;
                so.ApplyModifiedProperties();
                Debug.Log($"Set System Tag {systemTag} for {gameObject.name}");
            }
        }
    }
}