using System;
using System.Collections.Generic;

namespace Fillament
{
    public interface IConfigurationAccessProvider : IDisposable
    {
        void Commit();

        void Rollback();

        ICollection<PrincipalInfo> GetUserGroups(bool? enabled);
        
        bool UserGroupExists(string name);

        bool Insert(UserGroup userGroup);

        UserGroup GetUserGroup(string name);

        bool Update(UserGroup userGroup);
        
        void DeleteUserGroup(string name);

        bool UserExists(string name);
        
        ICollection<UserPrincipalInfo> GetUsers(bool? enabled);

        bool Insert(User user);
        
        User GetUser(string name);

        bool Update(User user);

        void DeleteUser(string name);

    }
}