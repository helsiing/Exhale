using System;
using UnityEngine;

namespace Exhale.Scripts.External.ServiceLocators
{
	/// <summary>
	/// Service reference wrapper object. Automatically gets an instance of the service from the service locator when
	/// asked for one and caches it for you. Also correctly clears the cache even if domain reloading is turned off in
	/// the project settings. Also takes into consideration application shutdown flow.
	///
	/// When unsubscribing from events in OnDestroy or OnDisable, use HasCachedReference to check if a service has
	/// been cached and should be unsubscribed from, then use CachedReference to do the unsubscribing. This ensures
	/// that you don't unnecessarily ask for a new service instance. If you never asked for one to begin with then you
	/// never subscribed and also shouldn't unsubscribe.
	/// </summary>
	public class ServiceReference<T> : IServiceObservable where T : class
	{
		[NonSerialized] private T instance;

		private ServiceLocator serviceLocator;
		public ServiceLocator ServiceLocator
		{
			get
			{
				if (serviceLocator == null)
					serviceLocator = ServiceLocator.GetInstance();
				return serviceLocator;
			}
		}

		/// <summary>
		/// Gets the cached reference to the service. Caches it first if it wasn't cached yet. Do not
		/// use in OnDestroy or OnDisable.
		/// </summary>
		public T Reference
		{
			get
			{
				// Don't ask for new service instances during shutdown.
				if (ServiceLocator.IsApplicationQuitting)
					return CachedReference;
				
				// Apparently objects can be destroyed but not be null.
				// https://answers.unity.com/questions/586144/destroyed-monobehaviour-not-comparing-to-null.html
				if (instance == null || instance.Equals(null))
				{
					instance = ServiceLocator.GetService<T>();
					
					ServiceLocator.UnsubscribeToServiceChanges<T>(this);
					ServiceLocator.SubscribeToServiceChanges<T>(this);
				}
				
				return instance;
			}
		}
		
		public bool HasCachedReference
		{
			get
			{
				// If the application is shutting down, don't use service references. These checks are to unsubscribe
				// correctly, allow garbage collection to happen and prevent memory leaks. During application shutdown
				// memory leaks are no longer an issue and we'd rather simplify the flow and prevent exceptions, because
				// exceptions can still be a no-no depending on the target platform
				if (ServiceLocator.IsApplicationQuitting)
					return false;
				
				// If the service has been unregistered, it should not be used any more. Regardless of whether we have
				// a cached instance that pinky promises to not be null. Spoiler alert: it may be null
				if (!IsServiceRegistered)
					return false;
				
				return CachedReference != null && !instance.Equals(null);
			}
		}

		public bool HasReference
		{
			get
			{
				// If the application is shutting down, don't use service references. These checks are to unsubscribe
				// correctly, allow garbage collection to happen and prevent memory leaks. During application shutdown
				// memory leaks are no longer an issue and we'd rather simplify the flow and prevent exceptions, because
				// exceptions can still be a no-no depending on the target platform.
				if (ServiceLocator.IsApplicationQuitting)
					return false;

				// If the service has been unregistered, it should not be used any more. Regardless of whether we have
				// a cached instance that pinky promises to not be null. Spoiler alert: it may be null
				if (!IsServiceRegistered)
					return false;
				
				return Reference != null && !instance.Equals(null);
			}
		}

		/// <summary>
		/// Please keep in mind that the service reporter's execution order precedes that of regular scene objects.
		/// That means that the service reporter's OnDestroy is called *before* that of scene objects, and therefore
		/// services will be unregistered *before* they will show up as null. So this is an excellent check to see
		/// if your cache is still valid and it's safe to use.
		/// </summary>
		private bool IsServiceRegistered => ServiceLocator.HasService<T>();

		/// <summary>
		/// Gets the cached reference to the service. Use in OnDestroy or OnDisable.
		/// </summary>
		public T CachedReference => instance;
		
		private void ClearCache()
		{
			instance = null;
		}
		
		void IServiceObservable.OnServiceRegistered(Type targetType)
		{
			if (!ServiceLocator.TryGetService(out T newInstance))
				return;
            
			if (Equals(newInstance, instance))
				return;
        
			instance = newInstance;
		}
        
		void IServiceObservable.OnServiceUnregistered(Type targetType)
		{
			ClearCache();
		}

		public void WaitForService(ServiceLocator.ServiceRegistrationHandler handler)
		{
			ServiceLocator.WaitForService<T>(handler);
		}

		public static implicit operator T(ServiceReference<T> serviceReference)
		{
			return serviceReference.Reference;
		}
	}
}
