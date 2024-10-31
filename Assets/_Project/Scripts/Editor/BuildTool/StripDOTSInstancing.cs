#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.Build;
using UnityEditor.Rendering;

namespace DaftAppleGames.Editor.BuildTool
{
    class StripDOTSInstancing : IPreprocessShaders
    {
        public int callbackOrder { get { return 0; } }
        private ShaderKeyword _KeywordToStrip;

        public StripDOTSInstancing()
        {
            _KeywordToStrip = new ShaderKeyword("DOTS_INSTANCING_ON");
        }

        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {
            if (!shader.name.StartsWith("GPUInstancer"))
                return;
            int count = 0;
            for (int i = 0; i < data.Count; ++i)
            {
                if (data[i].shaderKeywordSet.IsEnabled(_KeywordToStrip))
                {
                    data.RemoveAt(i);
                    --i;
                    count++;
                }
            }
            Debug.Log("Stripped " + count + " variants of shader " + shader + " with keyword DOTS_INSTANCING_ON");
        }
    }
}
#endif