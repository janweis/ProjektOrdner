using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.App
{
    public class AppSettingsRootProfileProcessor
    {
        private int LastID { get; set; }
        public int DefaultID { get; set; }
        public List<AppSettingsRootProfileModel> RootProfiles { get; set; }


        public AppSettingsRootProfileProcessor()
        {
            LastID = -1;
            DefaultID = -1;
            RootProfiles = new List<AppSettingsRootProfileModel>();
        }

        //
        // FUNCTIONS
        // ________________________________________________________________________________


        /// <summary>
        /// 
        /// Ruft den Standard-RootPath ab.
        /// 
        /// </summary>

        public AppSettingsRootProfileModel GetDefaultRoot()
        {
            if (DefaultID == -1)
                return null;

            return RootProfiles[DefaultID];
        }


        /// <summary>
        /// 
        /// Fügt einen neuen RootPath hinzu.
        /// 
        /// </summary>

        public void Add(string rootPath)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
                throw new ArgumentNullException();

            int newID = LastID + 1;
            LastID = newID;

            RootProfiles.Add(new AppSettingsRootProfileModel
            {
                ID = newID,
                Path = rootPath
            });

            // Set Default ID if none is set
            if(DefaultID == -1)
            {
                DefaultID = newID;
            }
        }


        /// <summary>
        /// 
        /// Entfernt einen nicht benötigten RootPath.
        /// 
        /// </summary>

        public void Remove(string rootPath)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
                throw new ArgumentNullException();


            // Find Item
            AppSettingsRootProfileModel rootFolder = RootProfiles
                .Where(item => item.Path == rootPath)
                .FirstOrDefault();

            if (null != rootFolder)
            {
                if(rootFolder.ID == DefaultID)
                {
                    AppSettingsRootProfileModel nextRecord = RootProfiles
                        .Where(item => item.ID != rootFolder.ID)
                        .FirstOrDefault();

                    if(null != nextRecord)
                    {
                        SetDefault(nextRecord.ID);
                    }
                    else
                    {
                        SetDefault(-1);
                    }
                }

                RootProfiles.Remove(rootFolder);
            }
        }


        public void Reset()
        {
            LastID = -1;
            DefaultID = -1;
            RootProfiles = new List<AppSettingsRootProfileModel>();
        }


        /// <summary>
        /// 
        /// Setz einen anderen RootPath als Standard-RootPath
        /// 
        /// </summary>

        public void SetDefault(int id)
        {
            DefaultID = id;
        }
    }
}
