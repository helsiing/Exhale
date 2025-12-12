namespace Exhale.Scripts.External.ServiceLocators
{
	/// <summary>
	/// Signifies that it wants a callback when it's registered as a service.
	/// </summary>
	public interface IServiceRegistered
	{
		void RegisteredAsService(ServiceLocator serviceLocator);
	}
}
