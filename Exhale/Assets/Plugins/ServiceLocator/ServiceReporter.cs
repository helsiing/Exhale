using System.Collections.Generic;
using UnityEngine;

namespace Exhale.Scripts.External.ServiceLocators
{
	/// <summary>
	/// Registers services to the service locator.
	/// </summary>
	public class ServiceReporter<TService> : MonoBehaviour
	{
		private ServiceLocator serviceLocator;
		protected ServiceLocator ServiceLocator => serviceLocator;
		private bool didCacheServiceLocator;

		private Stack<TService> registeredServices = new();

		protected virtual void OnDestroyCallback()
		{ }

		/// <summary>
		/// Lets you specify if this reporter should register/unregister on its own volition or if it is to be
		/// instructed to do so by something else, like an EntryPoint.
		/// </summary>
		protected virtual bool ShouldAutoRegister => true;

		private bool didRegister;
		private bool didUnregister;

		private void Awake()
		{
			if (ShouldAutoRegister && !didRegister)
				RegisterServices();
		}

		private void OnDestroy()
		{
			OnDestroyCallback();

			if (ShouldAutoRegister)
				UnregisterServices();
		}

		private void CacheServiceLocator()
		{
			if (didCacheServiceLocator)
				return;

			didCacheServiceLocator = true;
			serviceLocator = ServiceLocator.GetInstance();
		}

		public virtual void RegisterServices()
		{
			CacheServiceLocator();
			didRegister = true;
		}

		public void UnregisterServices()
		{
			// Only unregister if we did register correctly. Otherwise an invalid service reporter
			// may unregister the correct reporter's service instances by accident.
			if (!didRegister || didUnregister)
				return;

			didUnregister = true;

			while (registeredServices.Count > 0)
			{
				ServiceLocator.UnregisterServiceInstance(registeredServices.Pop());
			}
		}

		protected TService RegisterServiceInstance(TService instance)
		{
			var service = (TService) ServiceLocator.RegisterServiceInstance(instance);

			registeredServices.Push(service);

			return service;
		}

		protected T RegisterServiceInstance<T>(T instance) where T : TService
		{
			T service = ServiceLocator.RegisterServiceInstance<T>(instance);

			registeredServices.Push(service);

			return service;
		}
	}
}