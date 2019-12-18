using System.Collections.Generic;

namespace Fillament
{
	public interface IServerInstance
	{
		ICollection<PrincipalInfo> GetUserGroups(bool? enabled);
		bool UserGroupExists(string name);
		void Add(UserGroup principal);
		UserGroup GetUserGroup(string name);
		void Update(UserGroup principal);
		void DeleteUserGroup(string name);
		ICollection<UserPrincipalInfo> GetUsers(bool? enabled);
		bool UserExists(string name);
		void Add(User user);
		User GetUser(string name);
		void DeleteUser(string name);
		void Update(User user);
		void Reload();
	}
}