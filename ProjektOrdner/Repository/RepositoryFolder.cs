using MimeKit;
using ProjektOrdner.App;
using ProjektOrdner.Mail;
using ProjektOrdner.Permission;
using ProjektOrdner.Utils;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjektOrdner.Repository
{
    public class RepositoryFolder
    {
        // // // // // // // // // // // // // // // // // // // // //
        // Variables
        // 

        public enum RepositoryStatus { Ok, Corrupted, NotChecked }

        public RepositoryOrganization Organization { get; set; }
        public RepositorySettings Settings { get; set; }
        public RepositoryVersion Version { get; set; }
        public RepositoryStatus Status { get; set; }

        private AppSettings AppSettings { get; set; }
        private ActiveDirectoryUtil AdUtil { get; set; }


        // // // // // // // // // // // // // // // // // // // // //
        // Constructors
        // 

        public RepositoryFolder(AppSettings appSettings)
        {
            AppSettings = appSettings;
            Version = RepositoryVersion.Unknown;
            Status = RepositoryStatus.NotChecked;
        }

        public RepositoryFolder(RepositoryOrganization organization, RepositorySettings settings, RepositoryVersion version, AppSettings appSettings)
        {
            AppSettings = appSettings;
            Organization = organization;
            Settings = settings;
            Version = version;
            Status = RepositoryStatus.NotChecked;
        }


        // // // // // // // // // // // // // // // // // // // // //
        // Public Functions
        // 


        /// <summary>
        /// 
        /// Ruft einen ProjektOrdner ab und läd ihn in dieses Objekt
        /// 
        /// </summary>
        public async Task<bool> Load(string folderPath)
        {
            try
            {
                // Read RepositoryInfoFile
                Organization = new RepositoryOrganization(folderPath);
                await Organization.LoadOrganization();

                // Read ProjektSettings
                Settings = new RepositorySettings();
                await Settings.Load(folderPath);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// 
        /// Benennt den Projektordner um.
        /// 
        /// </summary>
        public async Task RenameAsync(RepositoryFolder repository, string newName, IProgress<string> progressMessage)
        {
            progressMessage.Report("Project sucessfully renamed!");
            await Task.Delay(1);

            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// Entfernt einen ProjektOrdner
        /// 
        /// </summary>
        public void Remove(IProgress<string> progressMessage)
        {
            // Remove all Files / Folders
            progressMessage.Report("Deleting projekt data... (1/3)");
            DeleteProjektData();

            // Remove RepositoryInfoFile
            progressMessage.Report("Deleting repository data... (2/3)");
            DeleteRepositoryData();

            // Remove Ad Groups
            progressMessage.Report("Deleting Active Directory Groups... (3/3)");
            DeleteAdGroups();

            progressMessage.Report("Project sucessfully deleted!");
        }


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        public void Verify() { throw new NotImplementedException(); }


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        public void Repair() { throw new NotImplementedException(); }


        // // // // // // // // // // // // // // // // // // // // //
        // Private Functions
        // 


        private void DeleteRepositoryData()
        {
            DirectoryInfo projekt = new DirectoryInfo(Organization.ProjektPath);

            foreach (FileInfo file in projekt.GetFiles("*", SearchOption.AllDirectories))
            {
                file.Delete();
            }

            foreach (DirectoryInfo directory in projekt.GetDirectories("*", SearchOption.AllDirectories))
            {
                directory.Delete(true);
            }

            projekt.Delete();
        }

        private void DeleteProjektData()
        {
            DirectoryInfo projekt = new DirectoryInfo(Organization.ProjektPath);

            foreach (FileInfo file in projekt.GetFiles("*", SearchOption.AllDirectories))
            {
                if (file.Directory.Name == AppConstants.OrganisationFolderName)
                    continue;

                file.Delete();
            }

            foreach (DirectoryInfo directory in projekt.GetDirectories("*", SearchOption.AllDirectories))
            {
                if (directory.Name == AppConstants.OrganisationFolderName)
                    continue;

                directory.Delete(true);
            }
        }

        private void DeleteAdGroups()
        {
            foreach (GroupPrincipal group in AdUtil.GetAdGroups(Organization.ProjektName))
            {
                if (null != group)
                {
                    group.Delete();
                }
            }
        }


        public override string ToString()
        {
            return $@"\n
- Name: '{Organization.ProjektName}'
- Ende: {Organization.ProjektEnde.ToShortDateString()}
- Version: {Version.ToString()}
";
        }

    }
}
