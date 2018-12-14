using System;
using UnityEngine;

namespace Uniject.Impl
{
	public class UnityCamera : ICamera
	{
		readonly Camera _camera;
		public Camera Camera { get { return _camera; } }

		public UnityCamera(GameObject obj) {
			_camera = obj.GetComponent<Camera>();
			if (_camera == null) {
				_camera = obj.AddComponent<Camera>();
			}
		}

		public bool Enabled {
			get { return  _camera.enabled; } 
			set { _camera.enabled = value; }
		}
	}


}

