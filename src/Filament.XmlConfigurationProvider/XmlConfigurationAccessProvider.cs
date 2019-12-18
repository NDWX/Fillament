using System;
using System.Collections.Generic;
using System.Text;
using IO = System.IO;
using System.Xml;

namespace Fillament.ConfigurationAccessProvider.Xml
{
    public class XmlConfigurationAccessProvider : IConfigurationAccessProvider
    {
        private XmlDocument _document;
        private XmlNode _usersNode;
        private XmlNode _groupsNode;
        
        private readonly IO.FileStream _inStream;

        public XmlConfigurationAccessProvider(string configFilePath)
        {
            if (string.IsNullOrWhiteSpace(configFilePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(configFilePath));
            
            try
            {
                _inStream = IO.File.Open(configFilePath, IO.FileMode.Open,
                    IO.FileAccess.ReadWrite, IO.FileShare.None);

                LoadConfig();
            }
            catch (IO.FileNotFoundException fileNotFound)
            {
                throw new ConfigurationAccessException("Unable to find specified configuration file.", fileNotFound);
            }
            catch (IO.IOException ioException)
            {
                throw new ConfigurationAccessException("Unexpected error occured when accessing configuration file.",
                    ioException);
            }
        }

        private void LoadConfig()
        {
            _document = new XmlDocument();
            _usersNode = _document.GetElementsByTagName("Users")?.First();
            _groupsNode = _document.GetElementsByTagName("Groups")?.First();

            _document.Load(_inStream);
        }

        private XmlNode GetUserGroupXmlNode(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            
            if (_groupsNode == null)
                return null;
            
            XmlNodeList nodeList = _groupsNode.SelectNodes($"/Group[@Name={name}");

            return nodeList?[0];
        }

        private XmlNode GetUserXmlNode(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            
            if (_usersNode == null)
                return null;
            
            XmlNodeList nodeList = _usersNode.SelectNodes($"/User[@Name={name}");

            return nodeList?[0];
        }
        
        public void Commit()
        {
            using (XmlWriter xmlWriter = new XmlTextWriter(_inStream, Encoding.Default))
            {
                _document.WriteTo(xmlWriter);
            }

        }

        public void Rollback()
        {
            LoadConfig();
        }

        public bool UserGroupExists(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            
            return GetUserGroupXmlNode(name) != null;
        }
        
        public virtual ICollection<PrincipalInfo> GetUserGroups(bool? enabled)
        {
            string xPathQuery = $"/Group";

            if(enabled.HasValue)
            {
                string enabledValue = enabled.Value ? "1" : "0";
                xPathQuery = $"/Group[Option[@Name='Enabled'][text() = {enabledValue}]]";
            }
            
            XmlNodeList nodeList = _groupsNode.SelectNodes(xPathQuery);

            List<PrincipalInfo> groupInfos = new List<PrincipalInfo>(nodeList.Count);
            
            PrincipalInfo info = null;
            
            foreach (XmlNode node in nodeList)
            {
                info = new PrincipalInfo();
                
                info.Parse(node);
                
                groupInfos.Add(info);
            }

            return groupInfos;
        }

        public bool Insert(UserGroup userGroup)
        {
            if (userGroup == null) throw new ArgumentNullException(nameof(userGroup));
            
            XmlElement xmlNode = (XmlElement)GetUserGroupXmlNode(userGroup.Info.Name);

            if(xmlNode != null)
                return false;

            xmlNode = _groupsNode.AddElement("Group");
            xmlNode.SetAttribute("Name", userGroup.Info.Name);
            
            userGroup.To(xmlNode);

            return true;
        }

        public virtual UserGroup GetUserGroup(string name)
        {
            XmlNode xmlNode = GetUserGroupXmlNode(name);

            UserGroup userGroup = new UserGroup()
            {
                Info = new PrincipalInfo()
                {
                    Name = xmlNode.Attributes["name"].Value
                },
                HomeDirectory = new Directory(),
                VirtualDirectories = new List<VirtualDirectory>(),
                Options = new ConnectionLimitOptions(),
                Security = new SecurityOptions(),
                SpeedLimits = new SpeedLimits(),
            };
            
            userGroup.Parse(xmlNode);

            return userGroup;
        }

        public bool Update(UserGroup userGroup)
        {
            if (userGroup == null) throw new ArgumentNullException(nameof(userGroup));
            
            XmlElement xmlNode = (XmlElement)GetUserGroupXmlNode(userGroup.Info.Name);

            if(xmlNode == null)
                return false;

            xmlNode.InnerXml = string.Empty;
            
            userGroup.To(xmlNode);

            return true;
        }

        public virtual void DeleteUserGroup(string name)
        {
            XmlNode xmlNode = GetUserGroupXmlNode(name);

            if (xmlNode != null)
                _groupsNode.RemoveChild(xmlNode);
        }
        
        public virtual ICollection<UserPrincipalInfo> GetUsers(bool? enabled)
        {
            string xPathQuery = $"/User";

            if(enabled.HasValue)
            {
                string enabledValue = enabled.Value ? "1" : "0";
                xPathQuery = $"/User[Option[@Name='Enabled'][text() = {enabledValue}]]";
            }
            
            XmlNodeList nodeList = _usersNode.SelectNodes(xPathQuery);

            List<UserPrincipalInfo> userPrincipalInfos = new List<UserPrincipalInfo>(nodeList.Count);
            
            UserPrincipalInfo info = null;
            
            foreach (XmlNode node in nodeList)
            {
                info = new UserPrincipalInfo();
                
                info.Parse(node);
                
                userPrincipalInfos.Add(info);
            }

            return userPrincipalInfos;
        }

        public bool UserExists(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            
            return GetUserXmlNode(name) != null;
        }

        public bool Insert(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            XmlElement xmlNode = (XmlElement)GetUserXmlNode(user.Info.Name);

            if(xmlNode != null)
                return false;

            xmlNode = _usersNode.AddElement("User");
            xmlNode.SetAttribute("Name", user.Info.Name);
            
            user.To(xmlNode);

            return true;
        }

        public virtual User GetUser(string name)
        {
            XmlNode xmlNode = GetUserXmlNode(name);

            User user = new User()
            {
                Info = new UserPrincipalInfo()
                {
                    Name = xmlNode.Attributes["name"].Value
                },
                HomeDirectory = new Directory(),
                VirtualDirectories = new List<VirtualDirectory>(),
                Options = new ConnectionLimitOptions(),
                Security = new UserSecurityOptions(),
                SpeedLimits = new SpeedLimits(),
            };

            user.Parse(xmlNode);

            return user;
        }

        public bool Update(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            XmlElement xmlNode = (XmlElement)GetUserXmlNode(user.Info.Name);

            if(xmlNode == null)
                return false;

            xmlNode.InnerXml = string.Empty;
            
            user.To(xmlNode);

            return true;
        }

        public virtual void DeleteUser(string name)
        {
            XmlNode xmlNode = GetUserXmlNode(name);

            if (xmlNode != null)
                _usersNode.RemoveChild(xmlNode);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _inStream?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}