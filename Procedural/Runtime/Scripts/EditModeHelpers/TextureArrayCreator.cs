using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PRK.Procedural
{
#if UNITY_EDITOR
    public partial class TextureArrayCreator
    {
        [SerializeField] private Texture2D originalTexture;
        [SerializeField] private int width;
        [SerializeField] private int height;
        public int rows = 0;
        public int columns = 0;
        public int padding = 1;
        public int totalCount = 0;
        Texture2D ResizeTexture(Texture2D source, int newWidth_, int newHeight_)
        {
            RenderTexture rt = new RenderTexture(newWidth_, newHeight_, 0);
            Graphics.Blit(source, rt);

            Texture2D result = new Texture2D(newWidth_, newHeight_, TextureFormat.ARGB32, false);
            RenderTexture.active = rt;
            result.ReadPixels(new Rect(0, 0, newWidth_, newHeight_), 0, 0);
            result.Apply();

            RenderTexture.active = null;
            rt.Release();
            return result;
        }

        [ContextMenu("PackLightMaps")]
        void PackLightMaps()
        {
            
            int count=LightmapSettings.lightmaps.Length;
            string path = $"{Application.dataPath + "/________ResizedTexture.png"}";
            Texture2D[]textures=new Texture2D[LightmapSettings.lightmaps.Length];

            for (int i = 0; i < count; i++)
            {
                textures[i]=LightmapSettings.lightmaps[i].lightmapColor;
                string assetpath=AssetDatabase.GetAssetPath(textures[i]);
                TextureImporter tx=TextureImporter.GetAtPath(assetpath) as TextureImporter;
                if (!tx.isReadable)
                {
                    tx.isReadable = true;
                    tx.SaveAndReimport();
                }
            }

            CopyToAtlas(textures,rows,padding,out Texture2D atlas);
            byte[] bytes = atlas.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
        }
        [ContextMenu("ResizeAndSave")]
        void ResizeAndSave()
        {
            Texture2D t=ResizeTexture(originalTexture, width, height);
            SaveTextureToFile(t,$"{Application.dataPath + "/ResizedTexture.png"}");
            AssetDatabase.Refresh();
        }
        void SaveTextureToFile(Texture2D texture, string path)
        {
            byte[] bytes = texture.EncodeToJPG(); // Use EncodeToJPG() for JPG format
            System.IO.File.WriteAllBytes(path, bytes);
            
        }
        public static Texture2D CreateAtlas(Texture2D[] textures, int atlasWidth, int atlasHeight)
        {
            var atlas = new Texture2D(atlasWidth, atlasHeight, TextureFormat.RGBA32, false);
            int x = 0; // Start position

            for (int i = 0; i < textures.Length; i++)
            {
                if (textures[i] == null) continue;

                if (x + textures[i].width >= atlasWidth) break; // Prevent overflow

                Color[] pixels = textures[i].GetPixels();
                atlas.SetPixels(x, 0, textures[i].width, textures[i].height, pixels);
            
                x += textures[i].width + 1; // Move forward with 1-pixel padding
            }

            atlas.Apply();
            return atlas;
        }
        public static int NearestPowerOfTwo(int n,bool ceiling_ =true)
        {
            if (n < 1) return 1; // Edge case for non-positive numbers

            int lower = Mathf.FloorToInt(Mathf.Pow(2, Mathf.Floor(Mathf.Log(n, 2))));
            int upper = Mathf.CeilToInt(Mathf.Pow(2, Mathf.Ceil(Mathf.Log(n, 2))));
            return ceiling_?upper:lower;
          //  return (n - lower < upper - n) ? lower : upper;
        }
        
        private static void CopyToAtlas(Texture2D[] sourceArray_,int rows_,int padding_,out Texture2D atlas)
        {
            int count = sourceArray_.Length;
            int cols_=(count/rows_)+(count%rows_);
            int srcW=sourceArray_[0].width;
            int srcH=sourceArray_[0].height;
            int atlasWidth=(srcW*cols_)+(padding_*(cols_-1));
            int atlasHeight=(srcH*rows_)+(padding_*(rows_-1));
            atlas = new Texture2D(atlasWidth, atlasHeight, TextureFormat.RGBA32, false);
            int x = 0, y = 0; // Start from the top-left

            for (int i = 0; i < sourceArray_.Length; i++)
            {
                if (sourceArray_[i] == null) continue;

                if (x + sourceArray_[i].width > atlasWidth)
                {
                    x = 0;
                    y += sourceArray_[i].height + padding_; // Move down with 1-pixel padding
                }

                if (y + sourceArray_[i].height > atlasHeight)
                {
                    Debug.LogError("problem");
                    break;
                }

                Color[] pixels = sourceArray_[i].GetPixels();
                atlas.SetPixels(x, atlasHeight - y - sourceArray_[i].height, sourceArray_[i].width, sourceArray_[i].height, pixels);

                x += sourceArray_[i].width + padding_; // Move right with 1-pixel padding
            }
            atlas.Apply();
        }
       
    }
#endif
    public partial class TextureArrayCreator : MonoBehaviour
    {
       
    }
}
