using Plugins.Uniject.Interface;
using UnityEngine;

namespace Plugins.Uniject.impl
{
    public class UnityMeshRenderer : IRenderer
    {
        private readonly MeshRenderer _meshRenderer;
        public UnityMeshRenderer(GameObject obj) {
            _meshRenderer = obj.GetComponent<MeshRenderer>();
            if (null == _meshRenderer) {
                _meshRenderer = obj.AddComponent<MeshRenderer>();
            }
        }
        
        public void SetColor(Color color)
        {
            _meshRenderer.material.color = color;
        }
    }
}