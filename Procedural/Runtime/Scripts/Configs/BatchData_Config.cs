using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PRK.Procedural
{
#if UNITY_EDITOR
    using UnityEditor;
    public partial class BatchData_Config
    {
      
        private static void SetTextureArray(Texture2D[]texures_,out Texture2DArray output_)
        {
            int depth=texures_.Length;
            Texture2D inTex = texures_[0];
            int width = inTex.width;
            int height = inTex.width;
            output_ = new Texture2DArray(width, height, depth, inTex.format, true);
            for (int i = 0; i < depth; i++)
            {
                inTex = texures_[i];
                for (int mip = 0; mip < inTex.mipmapCount; ++mip)
                {
                    int copyWidth = width >> mip;
                    int copyHeight = height >> mip;
                    Graphics.CopyTexture(inTex, 0, mip, 0, 0, copyWidth, copyHeight, output_, i, mip, 0, 0);
                }
            }
        }
    }
#endif
    [CreateAssetMenu]
    public partial class BatchData_Config : ScriptableObject
    {
        [SerializeField] private Texture2D[] testArray;
        [SerializeField]TexArray[] testTexArray;
    }

    [System.Serializable]
    public class TexArray
    {
        public string m_Name;
        public Texture2DArray m_TexArray;

        public TexArray(string name_, Texture2DArray texArray_)
        {
            m_Name = name_;
            m_TexArray = texArray_;
        }
    }
}
