#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using System.Configuration;

namespace Playground
{
    partial class Program
    {
        //http://www.a2zmenu.com/Blogs/CSharp/How-to-encrypt-configuration-file.aspx
        //http://msdn.microsoft.com/en-us/library/system.configuration.protectedconfigurationprovider.aspx

        private static void ProtectConfiguration()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var provider = "RsaProtectedConfigurationProvider";
            var appSettings = config.AppSettings;

            if (appSettings != null
                && !appSettings.SectionInformation.IsProtected
                && !appSettings.ElementInformation.IsLocked)
            {
                appSettings.SectionInformation.ProtectSection(provider);
                appSettings.SectionInformation.ForceSave = true;
                config.Save();
            }
        }

        private static void UnProtectConfiguration()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var appSettings = config.AppSettings;

            if (appSettings != null
                && appSettings.SectionInformation.IsProtected
                && !appSettings.ElementInformation.IsLocked)
            {
                appSettings.SectionInformation.UnprotectSection();
                appSettings.SectionInformation.ForceSave = true;
                config.Save(ConfigurationSaveMode.Full);
            }
        }
    }
}