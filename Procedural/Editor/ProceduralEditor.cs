using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace PRK.Procedural.Editor
{
    public class ProceduralEditor : ShaderGUI
    {
        private MaterialProperty _mainTex;
        private MaterialProperty _mainColor;
        private MaterialProperty _detailTex;
        private MaterialProperty _detailColor;
        private MaterialProperty _detailMap;
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            
        }
    }
}
