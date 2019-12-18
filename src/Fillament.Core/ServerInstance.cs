using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Fillament
{
	[SuppressMessage("ReSharper", "UnusedType.Global")]
	public class ServerInstance : IServerInstance
	{
		private readonly IConfigurationAccessFactory _configurationAccessFactory;

		public ServerInstance(IConfigurationAccessFactory configurationAccessFactory)
		{
			_configurationAccessFactory = configurationAccessFactory;
		    
		    OpenConfigFileFor((stream) => { /* test access to file */ });
	    }

		TResult OpenConfigFileFor<TResult>(Func<IConfigurationAccessProvider, TResult> function)
		{
			using (IConfigurationAccessProvider accessProvider = _configurationAccessFactory.GetInstance())
			{
				return function(accessProvider);
			}
		}
		
		void OpenConfigFileFor(Action<IConfigurationAccessProvider> action)
		{
			using (IConfigurationAccessProvider accessProvider = _configurationAccessFactory.GetInstance())
			{
				action(accessProvider);
			}
		}

		public ICollection<PrincipalInfo> GetUserGroups(bool? enabled)
	    {
		    return OpenConfigFileFor(
			    config => config.GetUserGroups(enabled)
			);
	    }

	    public bool UserGroupExists(string name)
	    {
		    if (string.IsNullOrWhiteSpace(name))
			    throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
		    
		    return OpenConfigFileFor(config => config.UserGroupExists(name));
	    }

		public void Add(UserGroup principal)
		{
			if(principal == null) throw new ArgumentNullException(nameof(principal));

			OpenConfigFileFor(config =>
					{
						if(!config.Insert(principal))
							throw new DuplicatePrincipal();
						
						config.Commit();
					}
				);
		}

		public UserGroup GetUserGroup(string name)
	    {
		    if (string.IsNullOrWhiteSpace(name))
			    throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
		    
		    return OpenConfigFileFor(config => config.GetUserGroup(name));
	    }

	    public void Update(UserGroup principal)
	    {
		    if (principal == null) throw new ArgumentNullException(nameof(principal));
		    
		    OpenConfigFileFor(config =>
			{
				if(!config.Update(principal) )
					throw new UnknownUserGroup();
				
				config.Commit();
			});
	    }

	    public void DeleteUserGroup(string name)
	    {
		    if (string.IsNullOrWhiteSpace(name))
			    throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
		    
		    OpenConfigFileFor(
			    config =>
			    {
				    config.DeleteUserGroup(name);
				    config.Commit();
			    }
		    );
	    }

	    public ICollection<UserPrincipalInfo> GetUsers(bool? enabled)
		{
			return OpenConfigFileFor(
					config => config.GetUsers(enabled)
				);
		}

        public bool UserExists(string name)
        {
	        if (string.IsNullOrWhiteSpace(name))
		        throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
	        
	        return OpenConfigFileFor(config => config.UserExists(name));
        }

		public void Add(User user)
		{
			if(user == null) throw new ArgumentNullException(nameof(user));

			OpenConfigFileFor(config =>
					{
						if(!config.Insert(user))
							throw new DuplicatePrincipal();

						config.Commit();
					}
				);
		}

		public User GetUser(string name)
        {
	        if (string.IsNullOrWhiteSpace(name))
		        throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
	        
	        return OpenConfigFileFor(config => config.GetUser(name));
        }

        public void DeleteUser(string name)
        {
	        if (string.IsNullOrWhiteSpace(name))
		        throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
	        
	        OpenConfigFileFor(
		        config =>
		        {
			        config.DeleteUser(name);
			        config.Commit();
		        }
	        );
        }

		public void Update(User user)
		{
			if(user == null) throw new ArgumentNullException(nameof(user));

			OpenConfigFileFor(
					config =>
					{
						if(!config.Update(user))
							throw new UnknownUser();
						
						config.Commit();
					}
				);
		}

		public void Reload()
		{
			
		}
	}
}