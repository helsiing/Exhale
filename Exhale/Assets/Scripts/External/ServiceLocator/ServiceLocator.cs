using System;
using System.Collections.Generic;
using UnityEngine;

namespace Exhale.Scripts.External.ServiceLocators
{
	/// <summary>
	/// Manages access to all the services.
	/// Create a prefab with a ServiceLocator component at the path 'Resources/ServiceLocator', then add a service
	/// reporter to that in order to register core services that should always be available. Scene-specific services
	/// can be added via a scene-specific service reporter. See: GameplayServiceReporter.cs
	///
	/// References to a service are managed by a ServiceReference<T>
	/// </summary>
	public sealed class ServiceLocator : MonoBehaviour
	{
		private const string ServiceLocatorPrefabPath = "Services/ServiceLocator";

		[NonSerialized] private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
		
		[NonSerialized] private readonly Dictionary<Type, List<IServiceObservable>> serviceTypeToObservables
			= new Dictionary<Type, List<IServiceObservable>>();

		[NonSerialized] private static bool cachedIsApplicationQuitting;
		public static bool IsApplicationQuitting => cachedIsApplicationQuitting && Application.isPlaying;

		private static ServiceLocator instance;
		
		[NonSerialized] private readonly List<IServiceUpdate> servicesWithUpdateCallbacks = new List<IServiceUpdate>();
		
		public delegate void ServiceRegistrationHandler(ServiceLocator serviceLocator, object service);
		private readonly Dictionary<Type, ServiceRegistrationHandler> serviceTypeToRegistrationCallbacks 
			= new Dictionary<Type, ServiceRegistrationHandler>(); 
		
		public delegate void ServiceUnregistrationHandler(ServiceLocator serviceLocator, object service);
		private readonly Dictionary<Type, ServiceUnregistrationHandler> serviceTypeToUnregistrationCallbacks 
			= new Dictionary<Type, ServiceUnregistrationHandler>();

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init()
		{
			cachedIsApplicationQuitting = false;
			instance = null;
		}

		private void Awake()
		{
			// Cache the instance as soon as possible. This in turn sets up all default game services, as soon as
			// possible. This may be necessary for things like registering a Controls Service that needs to register
			// Input System processors *before* the Input System UI Input Module is initialized... 
			CacheInstance(this);
		}

		private void OnDestroy()
		{
			if (instance == this)
				instance = null;
			
			serviceTypeToRegistrationCallbacks.Clear();
		}

		private void OnApplicationQuit()
		{
			cachedIsApplicationQuitting = true;
		}

		private void Update()
		{
			for (int i = 0; i < servicesWithUpdateCallbacks.Count; i++)
			{
				servicesWithUpdateCallbacks[i].Update();
			}
		}

		/// <summary>
		/// Gets at the default service locator.
		/// </summary>
		/// <returns>An instance of the service locator ready for use.</returns>
		public static ServiceLocator GetInstance()
		{
			CacheInstance();

			return instance;
		}
		
		private static void CacheInstance(ServiceLocator serviceLocator)
		{
			if (instance != null)
				return;
			
			instance = serviceLocator;

			DontDestroyOnLoad(serviceLocator.gameObject);
		}

		private static void CacheInstance()
		{
			if (instance != null || IsApplicationQuitting)
				return;

			if (!Application.isPlaying)
			{
				Debug.LogError($"Tried to instantiate a Service Locator at editor time. This is not intended.");
				return;
			}
			
			GameObject prefab = LoadServiceLocatorPrefab();

			GameObject gameObjectInstance = Instantiate(prefab);
			instance = gameObjectInstance.GetComponent<ServiceLocator>();

			CacheInstance(instance);
		}

		private static GameObject LoadServiceLocatorPrefab()
		{
			return Resources.Load(ServiceLocatorPrefabPath, typeof(GameObject)) as GameObject;
		}

		public bool TryGetService(Type type, out object instance, bool logIfMissing = true)
		{
			// Make sure we have an instance of that type.
			if (!CheckThatWeHaveInstance(type, logIfMissing))
			{
				instance = null;
				return false;
			}

			// Return that instance.
			return services.TryGetValue(type, out instance);
		}

		public object GetService(Type type)
		{
			if (!TryGetService(type, out object service))
				return null;
			
			return service;
		}

		public T GetService<T>() where T : class
		{
			return GetService(typeof(T)) as T;
		}
		
		public bool TryGetService<T>(out T instance) where T : class
		{
			if (TryGetService(typeof(T), out object service))
			{
				instance = service as T;
				return true;
			}
			
			instance = null;
			return false;
		}

		public bool HasService(Type type)
		{
			return services.ContainsKey(type);
		}

		public bool HasService<T>()
		{
			return HasService(typeof(T));
		}

		/// <summary>
		/// Registers the specified service instance if one hasn't been created yet.
		/// </summary>
		/// <param name="instance">The instance to register.</param>
		public object RegisterServiceInstance(object instance, Type type)
		{
			if (HasService(type))
			{
				Debug.LogWarningFormat("Tried to register service {0} of type {1}, which already "
					+ "existed. Unregistering the original.", instance, type);

				UnregisterServiceType(type);
			}

			services.Add(type, instance);
			
			// Provide a registration callback if requested.
			if (instance is IServiceRegistered registered)
				registered.RegisteredAsService(this);
			
			// Register it for Update callbacks if requested.
			if (instance is IServiceUpdate updateable)
				servicesWithUpdateCallbacks.Add(updateable);

			// If registration callbacks were specified, fire them now and forget about them.
			if (serviceTypeToRegistrationCallbacks.TryGetValue(type, out ServiceRegistrationHandler handlers))
			{
				handlers(this, instance);
				serviceTypeToRegistrationCallbacks.Remove(type);
			}

			return instance;
		}

		/// <summary>
		/// Registers the specified service instance if one hasn't been created yet. 
		/// Registers it as its own type.
		/// </summary>
		/// <param name="instance">The instance to register.</param>
		public object RegisterServiceInstance(object instance)
		{
			return RegisterServiceInstance(instance, instance.GetType());
		}
		
		public T RegisterServiceInstance<T>(object instance)
		{
			return (T)RegisterServiceInstance(instance, typeof(T));
		}

		/// <summary>
		/// Unregisters the specified service instance.
		/// </summary>
		/// <param name="instance">The instance to unregister.</param>
		public void UnregisterServiceInstance(object instance)
		{
			if (instance == null)
				return;

			UnregisterServiceType(instance.GetType());
		}
		
		public void UnregisterServiceType<T>()
		{
			UnregisterServiceType(typeof(T));
		}

		/// <summary>
		/// Unregisters the specified service type.
		/// </summary>
		/// <param name="type">The type to unregister.</param>
		public void UnregisterServiceType(Type type)
		{
			if (!CheckThatWeHaveInstance(type, false))
				return;

			DispatchOnUnregisteredService(type);

			bool hadServiceOfType = services.TryGetValue(type, out object instance);
			if (!hadServiceOfType)
				return;
			
			services.Remove(type);
				
			// Unregister it for Update callbacks if requested.
			if (instance is IServiceUpdate updateable)
				servicesWithUpdateCallbacks.Remove(updateable);
			
			// If unregistration callbacks were specified, fire them now and forget about them.
			if (serviceTypeToUnregistrationCallbacks.TryGetValue(type, out ServiceUnregistrationHandler handlers))
			{
				handlers(this, instance);
				serviceTypeToUnregistrationCallbacks.Remove(type);
			}

			// Finally, dispose of the service
			if(instance is IDisposable disposable)
				disposable.Dispose();
		}
		
		private void DispatchOnUnregisteredService(Type targetType)
		{
			if (serviceTypeToObservables.TryGetValue(targetType, out List<IServiceObservable> observables))
			{
				for (int i = 0; i < observables.Count; i++)
					observables[i].OnServiceUnregistered(targetType);
			}
		}

		public void SubscribeToServiceChanges<T>(IServiceObservable observable)
		{
			Type type = typeof(T);
			if (!serviceTypeToObservables.ContainsKey(type))
				serviceTypeToObservables.Add(type, new List<IServiceObservable>());

			if (!serviceTypeToObservables[type].Contains(observable))
				serviceTypeToObservables[type].Add(observable);
		}
        
		public void UnsubscribeToServiceChanges<T>(IServiceObservable observable)
		{
			Type type = typeof(T);
			if (!serviceTypeToObservables.TryGetValue(type, out List<IServiceObservable> observables))
				return;

			observables.Remove(observable);
		}

		private bool CheckThatWeHaveInstance(Type type, bool logIfMissing = true)
		{
			if (!HasService(type))
			{
				if (logIfMissing)
					Debug.LogError("Tried to access service '" + type + " which didn't exist.");
				return false;
			}

			return true;
		}

		private bool CheckThatWeHaveInstance<T>(bool logIfMissing = true) where T : class
		{
			return CheckThatWeHaveInstance(typeof(T), logIfMissing);
		}
		
		public void SubscribeToServiceRegistration(Type type, ServiceRegistrationHandler handler)
		{
			bool didCallbackExist = serviceTypeToRegistrationCallbacks.TryGetValue(
				type, out ServiceRegistrationHandler existingHandlers);
			if (didCallbackExist)
				serviceTypeToRegistrationCallbacks[type] += handler;
			else
				serviceTypeToRegistrationCallbacks.Add(type, handler);
		}

		public void SubscribeToServiceRegistration<T>(ServiceRegistrationHandler handler)
		{
			SubscribeToServiceRegistration(typeof(T), handler);
		}
		
		public void SubscribeToServiceUnregistration(Type type, ServiceUnregistrationHandler handler)
		{
			bool didCallbackExist = serviceTypeToUnregistrationCallbacks.TryGetValue(
				type, out ServiceUnregistrationHandler existingHandlers);
			if (didCallbackExist)
				serviceTypeToUnregistrationCallbacks[type] += handler;
			else
				serviceTypeToUnregistrationCallbacks.Add(type, handler);
		}

		public void SubscribeToServiceUnregistration<T>(ServiceUnregistrationHandler handler)
		{
			SubscribeToServiceUnregistration(typeof(T), handler);
		}
		
		/// <summary>
		/// Calls the handler immediately if the service already exists, otherwise it waits for the service to be
		/// registered and fires the specified callback ONCE and then forgets about it.
		/// </summary>
		public void WaitForService(Type type, ServiceRegistrationHandler handler)
		{
			// If the service already existed, fire the callback immediately.
			if (TryGetService(type, out object existingService, false))
			{
				handler(this, existingService);
				return;
			}

			// Otherwise, wait for the service to be registered.
			SubscribeToServiceRegistration(type, handler);
		}

		/// <summary>
		/// Calls the handler immediately if the service already exists, otherwise it waits for the service to be
		/// registered and fires the specified callback ONCE and then forgets about it.
		/// </summary>
		public void WaitForService<T>(ServiceRegistrationHandler handler) where T : class
		{
			WaitForService(typeof(T), handler);
		}
	}
}
