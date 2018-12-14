using System;
using UnityClient;

namespace Uniject
{
	public interface IAnimatorEventListener
	{
		void Configure(IAnimatorEventHandler handler);
	}
}

