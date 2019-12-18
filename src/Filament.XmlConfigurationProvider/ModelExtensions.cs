using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using Pug;

namespace Fillament.ConfigurationAccessProvider.Xml
{
    public static class ModelExtensions
    {
        public static bool ConvertToBoolean(this string text, bool defaultValue = false)
        {
            if (text == null)
                return defaultValue;

            bool result = defaultValue;

            if (!Boolean.TryParse(text, out result))
            {
                result = defaultValue;

                if (text == "1")
                    result = true;
                else if (text == "0")
                    result = false;
            }

            return result;
        }

        public static int ConvertToInteger(this string text, int defaultValue = 0)
        {
            int result = defaultValue;

            if (string.IsNullOrEmpty(text))
                return result;

            int.TryParse(text, out result);

            return result;
        }
        
        public static void Parse(this PrincipalInfo info, XmlNode xnlNode)
        {
            info.Name = xnlNode.Attributes["name"].Value;

            bool commentParsed = false, statusParsed = false;

            foreach (XmlNode node in xnlNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "Option":
                        switch (node.Attributes["Name"].Value)
                        {
                            case "Comments":
                                info.Description = node.InnerText ?? string.Empty;
                                commentParsed = true;
                                break;

                            case "Enabled":
                                info.Enabled = ConvertToBoolean(node.InnerText);
                                statusParsed = true;
                                break;
                        }

                        break;
                }

                if (commentParsed && statusParsed)
                    break;
            }
        }
    
        public static void Parse(this ConnectionLimitOptions options, XmlNode xmlNode)
        {
        }


        private static List<SpeedLimitRule> ParseSpeedLimitRules(XmlNode node)
        {
            List<SpeedLimitRule> downloadLimitRules = new List<SpeedLimitRule>(node.ChildNodes.Count);

            foreach(XmlNode ruleNode in node.ChildNodes)
            {
                if(ruleNode.Name == "Rule")
                {
                    SpeedLimitRule rule = new SpeedLimitRule();
                    rule.Parse(ruleNode);

                    downloadLimitRules.Add(rule);
                }
            }

            return downloadLimitRules;
        }

        private static void Parse(this SpeedLimitRule rule, XmlNode ruleNode)
        {
            rule.SpeedLimit = int.Parse(ruleNode.Attributes["Speed"].Value);
            TimeSpan periodStart = TimeSpan.Zero, periodEnd = TimeSpan.Zero;

            foreach(XmlNode element in ruleNode.ChildNodes)
            {
                switch(element.Name)
                {
                    case "Days":
                        rule.DaysOfWeek = (DaysOfWeek) int.Parse(element.Value);
                        break;

                    case "Date":
                        rule.Date = new DateTime(int.Parse(element.Attributes["Year"].Value),
                                                 int.Parse(element.Attributes["Month"].Value),
                                                 int.Parse(element.Attributes["Day"].Value));
                        break;

                    case "From":
                        periodStart = new TimeSpan(int.Parse(element.Attributes["Hour"].Value),
                                                   int.Parse(element.Attributes["Minute"].Value),
                                                   int.Parse(element.Attributes["Second"].Value));
                        break;

                    case "To":
                        periodEnd = new TimeSpan(int.Parse(element.Attributes["Hour"].Value),
                                                 int.Parse(element.Attributes["Minute"].Value),
                                                 int.Parse(element.Attributes["Second"].Value));
                        break;
                }
            }

            rule.TimePeriod = new Range<TimeSpan>(periodStart, periodEnd);
        }
        
        public static void Parse(this SpeedLimits speedLimits, XmlNode xmlNode)
        {
            speedLimits.DownloadLimit = new SpeedLimit();
            speedLimits.UploadLimit = new SpeedLimit();

            foreach(XmlAttribute attribute in xmlNode.Attributes)
            {
                switch( attribute.Name)
                {
                    case "DlType":
                        speedLimits.DownloadLimit.Type = (SpeedLimitType) int.Parse(attribute.Value);
                        break;
                    
                    case "DlLimit":
                        speedLimits.DownloadLimit.ConstantLimit = int.Parse(attribute.Value);
                        break;
                    
                    case "ServerDlLimitBypass":
                        speedLimits.DownloadLimit.OverrideServerLimit = ConvertToBoolean(attribute.Value);
                        break;
                    
                    case "UlType":
                        speedLimits.UploadLimit.Type = (SpeedLimitType) int.Parse(attribute.Value);
                        break;
                    
                    case "UlLimit":
                        speedLimits.UploadLimit.ConstantLimit = int.Parse(attribute.Value);
                        break;
                    
                    case "ServerUlLimitBypass":
                        speedLimits.UploadLimit.OverrideServerLimit = ConvertToBoolean(attribute.Value);
                        break;
                }
            }
            
            foreach(XmlNode node in xmlNode)
            {
                switch(node.Name)
                {
                    case "Download":
                        speedLimits.DownloadLimit.Rules = ParseSpeedLimitRules(node);
                        break;
                    
                    case "Upload":
                        speedLimits.UploadLimit.Rules = ParseSpeedLimitRules(node);
                        break;
                }
            }
        }

        public static bool Parse(this VirtualDirectory virtualDirectory, XmlNode xmlNode)
        {
            bool isHome = false;
            
            virtualDirectory.Path = xmlNode.Attributes["Dir"].Value;
            virtualDirectory.FolderPermissions = new FolderPermissions();
            virtualDirectory.FilePermissions = new FilePermissions();
            
            foreach(XmlNode node in xmlNode)
            {
                switch(node.Name)
                {
                    case "Aliases":
                        virtualDirectory.Alias = node.ChildNodes.First()?.Value;
                        break;
                    
                    case "Option":

                        foreach(XmlNode optionNode in node.ChildNodes)
                        {
                            switch(optionNode.Name)
                            {
                                case "IsHome":
                                    isHome = bool.Parse(optionNode.Value);
                                    break;
                                
                                case "AutoCreate":
                                    virtualDirectory.AutoCreate = bool.Parse(optionNode.Value);
                                    break;
                                
                                case "FileRead":
                                    virtualDirectory.FilePermissions.AllowRead = bool.Parse(optionNode.Value);
                                    break;
                                
                                case "FileWrite":
                                    virtualDirectory.FilePermissions.AllowWrite = bool.Parse(optionNode.Value);
                                    break;
                                
                                case "FileDelete":
                                    virtualDirectory.FilePermissions.AllowDelete = bool.Parse(optionNode.Value);
                                    break;
                                
                                case "FileAppend":
                                    virtualDirectory.FilePermissions.AllowAppend = bool.Parse(optionNode.Value);
                                    break;
                                
                                case "DirCreate":
                                    virtualDirectory.FolderPermissions.AllowCreate = bool.Parse(optionNode.Value);
                                    break;
                                
                                case "DirDelete":
                                    virtualDirectory.FolderPermissions.AllowDelete = bool.Parse(optionNode.Value);
                                    break;
                                
                                case "DirRead":
                                    virtualDirectory.FolderPermissions.AllowList = bool.Parse(optionNode.Value);
                                    break;
                                
                                case "DirSubdirs":
                                    virtualDirectory.FolderPermissions.AllowSubdirectories = bool.Parse(optionNode.Value);
                                    break;
                            }
                        }
                        
                        break;
                }
            }

            return isHome;
        }

        static ICollection<IPAddress>  ParseIpAddressList(XmlNode node)
        {
            IPAddress[] addresses = new IPAddress[node.ChildNodes.Count];

            int idx = 0;

            foreach(XmlNode ipNode in node.ChildNodes)
            {
                addresses[idx] = IPAddress.Parse(ipNode.InnerText);

                idx++;
            }

            return addresses;
        }
        
        public static void Parse(this SecurityOptions securityOptions, XmlNode xmlNode)
        {
            foreach(XmlNode node in xmlNode.ChildNodes)
            {
                switch(node.Name)
                {
                    case "Disallowed":
                        securityOptions.IpBlacklist = ParseIpAddressList(node);
                        break;
                    
                    case "Allowed":
                        securityOptions.IpWhitelist = ParseIpAddressList(node);
                        break;
                }
            }
        }

        public static void CopyFrom(this FilePermissions permissions, FilePermissions other)
        {
            permissions.AllowAppend = other.AllowAppend;
            permissions.AllowDelete = other.AllowDelete;
            permissions.AllowRead = other.AllowRead;
            permissions.AllowWrite = other.AllowWrite;
        }

        public static void CopyFrom(this FolderPermissions permissions, FolderPermissions other)
        {
            permissions.AllowCreate = other.AllowCreate;
            permissions.AllowDelete = other.AllowDelete;
            permissions.AllowList = other.AllowList;
            permissions.AllowList = other.AllowList;
        }

        public static void CopyFrom(this Directory directory, Directory other)
        {
            directory.Path = other.Path;
            directory.FilePermissions.CopyFrom(other.FilePermissions);
            directory.FolderPermissions.CopyFrom(other.FolderPermissions);
        }
        
        public static void Parse(this IGenericPrincipal<PrincipalInfo, SecurityOptions> principal, XmlNode xmlNode)
        {
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "Option":
                        switch (node.Attributes["Name"].Value)
                        {
                            case "Bypass server userlimit":
                                principal.Options.OverrideServerLimits = ConvertToBoolean(node.InnerText);
                                break;

                            case "User Limit":
                                principal.Options.MaximumConnections = node.InnerText.ConvertToInteger();
                                break;

                            case "IP Limit":
                                principal.Options.MaximumConnectionsPerIp = node.InnerText.ConvertToInteger();
                                break;

                            case "Enabled":
                                principal.Info.Enabled = ConvertToBoolean(node.InnerText);
                                break;

                            case "Comments":
                                principal.Info.Description = node.InnerText ?? string.Empty;
                                break;

                            case "ForceSsl":
                                principal.Security.SslRequired = ConvertToBoolean(node.InnerText);
                                break;
                        }

                        break;

                    case "IpFilter":
                        principal.Security.Parse(node);
                        break;

                    case "Permissions":

                        foreach (XmlNode directoryNode in node.ChildNodes)
                        {
                            VirtualDirectory directory = new VirtualDirectory();

                            if (directory.Parse(directoryNode))
                            {
                                principal.HomeDirectory.CopyFrom(directory);
                            }
                            else
                            {
                                principal.VirtualDirectories.Add(directory);
                            }
                        }

                        break;

                    case "SpeedLimits":
                        principal.SpeedLimits.Parse(node);
                        break;
                }
            }
        }

        public static XmlElement AddElement(this XmlNode xmlNode, string name)
        {
            XmlElement node = xmlNode.OwnerDocument.CreateElement("Option");
            
            xmlNode.AppendChild(node);
            
            return node;
        }
        
        public static XmlElement AddElement(this XmlNode xmlNode, string name, string value)
        {
            XmlElement node = xmlNode.AddElement("Option");
            node.Value = value;
            
            return node;
        }

        public static XmlElement InsertOption(this XmlNode xmlNode, string name, string value)
        {
            XmlElement node = xmlNode.AddElement("Option", value);

            node.SetAttribute("Name", name);

            return node;
        }

        public static string ToBinaryString(this bool boolean)
        {
            return boolean ? "1" : "0";
        }

        public static void To(this VirtualDirectory directory, XmlElement xmlNode, bool isHome = false)
        {
            xmlNode.SetAttribute("Dir", directory.Path);
            xmlNode.InsertOption("FileRead", directory.FilePermissions.AllowRead.ToBinaryString());
            xmlNode.InsertOption("FileWrite", directory.FilePermissions.AllowWrite.ToBinaryString());
            xmlNode.InsertOption("FileDelete", directory.FilePermissions.AllowDelete.ToBinaryString());
            xmlNode.InsertOption("FileAppend", directory.FilePermissions.AllowAppend.ToBinaryString());
            xmlNode.InsertOption("DirCreate", directory.FolderPermissions.AllowCreate.ToBinaryString());
            xmlNode.InsertOption("DirDelete", directory.FolderPermissions.AllowDelete.ToBinaryString());
            xmlNode.InsertOption("DirList", directory.FolderPermissions.AllowList.ToBinaryString());
            xmlNode.InsertOption("DirSubDirs", directory.FolderPermissions.AllowSubdirectories.ToBinaryString());
            xmlNode.InsertOption("IsHome", isHome.ToBinaryString());
            xmlNode.InsertOption("AutoCreate", directory.AutoCreate.ToBinaryString());

            if(!isHome)
                 xmlNode.AddElement("Aliases").AddElement("Alias", directory.Alias);
        }

        public static void To(this Directory directory, XmlElement xmlNode)
        {
            ((VirtualDirectory) directory).To(xmlNode, true);
        }

        public static void To(this ConnectionLimitOptions options, XmlElement xmlNode, string limitPrefix)
        {
        }

        public static void To(this SpeedLimits limits, XmlElement xmlNode)
        {
            xmlNode.SetAttribute($"DlType", ((int)limits.DownloadLimit.Type).ToString());
            xmlNode.SetAttribute($"ServerDlLimitBypass", limits.DownloadLimit.OverrideServerLimit.ToString());
            xmlNode.SetAttribute($"DlLimit", limits.DownloadLimit.ConstantLimit.ToString());
            
            xmlNode.SetAttribute($"UlType", ((int)limits.UploadLimit.Type).ToString());
            xmlNode.SetAttribute($"ServerULimitBypass", limits.UploadLimit.OverrideServerLimit.ToString());
            xmlNode.SetAttribute($"ULimit", limits.UploadLimit.ConstantLimit.ToString());

            limits.DownloadLimit.To(xmlNode.AddElement("Download"));
            limits.DownloadLimit.To(xmlNode.AddElement("Upload"));
        }

        public static void To(this SpeedLimit limit, XmlElement xmlNode)
        {
            foreach(SpeedLimitRule rule in limit.Rules)
                rule.To(xmlNode.AddElement("Rule"));
        }

        public static void To(this SpeedLimitRule rule, XmlElement xmlNode)
        {
            xmlNode.SetAttribute("Speed", rule.SpeedLimit.ToString());
            xmlNode.AddElement("Days", ((int) rule.DaysOfWeek).ToString());
            rule.Date.To(xmlNode.AddElement("Date"));
        }

        public static void To(this DateTime date, XmlElement xmlNode)
        {
            xmlNode.SetAttribute("Year", date.Year.ToString());
            xmlNode.SetAttribute("Month", date.Month.ToString());
            xmlNode.SetAttribute("Day", date.Day.ToString());
        }

        public static void To(this Range<TimeSpan> range, XmlElement xmlNode)
        {
            range.Start.To(xmlNode.AddElement("From"));
            range.End.To(xmlNode.AddElement("To"));
        }
        
        public static void To(this TimeSpan time, XmlElement xmlNode)
        {
            xmlNode.SetAttribute("Hour", time.Hours.ToString());
            xmlNode.SetAttribute("Minute", time.Minutes.ToString());
            xmlNode.SetAttribute("Second", time.Seconds.ToString());
        }
        
        public static void To(this IGenericPrincipal<PrincipalInfo, SecurityOptions> principal, XmlElement xmlNode)
        {
            xmlNode.InsertOption("Bypass server userlimit", principal.Options.OverrideServerLimits.ToBinaryString());
            xmlNode.InsertOption("User Limit", principal.Options.MaximumConnections.ToString());
            xmlNode.InsertOption("Ip Limit", principal.Options.MaximumConnectionsPerIp.ToString());
            xmlNode.InsertOption("Enabled", principal.Info.Enabled.ToBinaryString());
            xmlNode.InsertOption("Comments", principal.Info.Description);
            xmlNode.InsertOption("ForceSsl", principal.Security.SslRequired.ToBinaryString());
            
            principal.Security.To(xmlNode.AddElement("IpFilter"));
            
            XmlElement directoriesElement = xmlNode.AddElement("Permissions");
            
            principal.HomeDirectory.To(directoriesElement.AddElement("Permission"));
            
            foreach(VirtualDirectory directory in principal.VirtualDirectories)
                directory.To(directoriesElement.AddElement("Permission"));
        }
        
        public static void To(this IGenericPrincipal<UserPrincipalInfo, UserSecurityOptions> principal, XmlElement xmlNode)
        {
            xmlNode.InsertOption("Pass", principal.Security.PasswordHash);
            xmlNode.InsertOption("Salt", principal.Security.PasswordSalt);
            
            ((IGenericPrincipal<PrincipalInfo, SecurityOptions>) principal).To(xmlNode);
        }

        public static void To(this SecurityOptions options, XmlElement xmlNode)
        {
            XmlElement element = xmlNode.AddElement("Disallowed");

            foreach(IPAddress address in options.IpBlacklist)
                element.AddElement("IP", address.ToString());

            element = xmlNode.AddElement("Allowed");
            
            foreach(IPAddress address in options.IpWhitelist)
                element.AddElement("IP", address.ToString());
        }
    }
}