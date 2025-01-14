using System;

namespace Exhale.Scripts.External.ServiceLocators
{
	/// <summary>
	/// Specifies that something can receive callbacks for when a service is registered or unregistered.
	/// </summary>
	public interface IServiceObservable
	{
		void OnServiceRegistered(Type targetType);
		void OnServiceUnregistered(Type targetType);
	}
}