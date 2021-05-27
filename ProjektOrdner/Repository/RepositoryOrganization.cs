using Newtonsoft.Json;
using ProjektOrdner.App;
using ProjektOrdner.Permission;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.Repository
{
    public class RepositoryOrganization
    {
        // // // // // // // // // // // // // // // // // // // // //
        // Variablen
        // 

        public Guid ID { get; private set; }

        public string ProjektName { get; set; }

        [JsonIgnore]
        public string ProjektPath
        {
            get
            {
                return Path.Combine(RootPath, ProjektName);
            }
        }

        [JsonIgnore]
        public string RootPath { get; set; }

        public DateTime ErstelltAm { get; set; }

        public DateTime ProjektEnde { get; set; }

        [JsonIgnore]
        public RepositoryVersion Version { get; set; }

        [JsonIgnore]
        public List<RepositoryPermission> LegacyPermissions { get; private set; }

        [JsonIgnore]
        public string RequestFilePath { get; set; }


        // // // // // // // // // // // // // // // // // // // // //
        // Constructors
        //

        public RepositoryOrganization()
            : this(string.Empty, string.Empty, DateTime.MinValue, RepositoryVersion.Unknown)
        { }

        public RepositoryOrganization(string projektName, string projektRoot)
            : this(projektName, projektRoot, DateTime.MinValue, RepositoryVersion.Unknown)
        { }

        public RepositoryOrganization(string folderPath)
            : this(new DirectoryInfo(folderPath).Name, new DirectoryInfo(folderPath).Parent.FullName, DateTime.MinValue, RepositoryVersion.Unknown)
        { }

        public RepositoryOrganization(string projektName, string rootPath, DateTime projektEnde, RepositoryVersion version)
        {
            // INIT
            ID = Guid.NewGuid();
            ErstelltAm = DateTime.Now;
            LegacyPermissions = new List<RepositoryPermission>();

            // SET
            ProjektName = projektName;
            RootPath = rootPath;
            ProjektEnde = projektEnde;
            Version = version;
        }



        // // // // // // // // // // // // // // // // // // // // //
        // LOAD
        // 


        /// <summary>
        /// 
        /// Läd eine Organisationsdatei
        /// 
        /// </summary>
        public async Task LoadOrganization()
        {
            Version = GetRepositoryVersion();
            switch (Version)
            {
                case RepositoryVersion.V1:
                {
                    await LoadV1Async(string.Empty);
                    break;
                }
                case RepositoryVersion.V2:
                {
                    await LoadV2Async();
                    break;
                }
            }
        }


        /// <summary>
        /// 
        /// Läd eine Anforderungsdatei
        /// 
        /// </summary>
        public async Task LoadRequest(string filePath)
        {
            Version = RepositoryVersion.V2; // Define to create V2 Projects
            RootPath = new FileInfo(filePath).Directory.Parent.FullName;
            RequestFilePath = filePath;

            if (File.Exists(filePath) == false)
                return;

            await LoadV1Async(filePath);
        }


        /// <summary>
        /// 
        /// Lade die Organisations- oder Antragsdatei Version 1
        /// 
        /// </summary>
        private async Task LoadV1Async(string filePath)
        {
            if(string.IsNullOrWhiteSpace(filePath) == true)
                filePath = GetOrganizationFilePath(ProjektPath, RepositoryVersion.V1);

            if (File.Exists(filePath) == false)
                return;

            string fileText = "";
            using (StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8))
            {
                fileText = await streamReader.ReadToEndAsync();

                if (fileText == "")
                    return;
            }

            string[] fileLines = fileText
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                .ToArray();

            // Process every Line
            foreach (string line in fileLines)
            {
                if (line.Contains("=") == true && line.Contains("#") == false)
                {
                    string key = line.Substring(0, line.IndexOf("=")).ToLower().Trim();
                    string value = line.Substring(line.IndexOf("=") + 1).TrimStart().TrimEnd();

                    if ((string.IsNullOrEmpty(key) == true) || (string.IsNullOrEmpty(value) == true))
                    {
                        continue; // Keine Daten vorhanden!
                    }

                    switch (key)
                    {
                        case "projektname":
                        {
                            ProjektName = value;
                            break;
                        }
                        case "startdatum":
                        {
                            if (DateTime.TryParse(value, out DateTime dateTime))
                            {
                                ErstelltAm = dateTime;
                            }
                            break;
                        }
                        case "enddatum":
                        {
                            if (DateTime.TryParse(value, out DateTime dateTime))
                            {
                                ProjektEnde = dateTime;
                            }
                            break;
                        }
                        case "projektmanager": // Nur für Projektantrag!
                        {
                            CreateLegacyPermission(PermissionRole.Manager, value);
                            break;
                        }
                        case "readwrite": // Nur für Projektantrag!
                        {
                            CreateLegacyPermission(PermissionRole.Member, value);
                            break;
                        }
                        case "read": // Nur für Projektantrag!
                        {
                            CreateLegacyPermission(PermissionRole.Guest, value);
                            break;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// Lade die Organisationsdatei Version 2
        /// 
        /// </summary>
        private async Task<bool> LoadV2Async()
        {
            string filePath = GetOrganizationFilePath(ProjektPath, RepositoryVersion.V2);

            if (File.Exists(filePath) == true)
            {
                using (StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8))
                {
                    string fileText = await streamReader.ReadToEndAsync();
                    RepositoryOrganization repoObjekt = JsonConvert.DeserializeObject<RepositoryOrganization>(fileText, new JsonSerializerSettings() { Formatting = Formatting.Indented });

                    if (null != repoObjekt)
                    {
                        ID = repoObjekt.ID;
                        ProjektName = repoObjekt.ProjektName;
                        ErstelltAm = repoObjekt.ErstelltAm;
                        ProjektEnde = repoObjekt.ProjektEnde;
                    }
                }

                return true;
            }

            return false;
        }



        // // // // // // // // // // // // // // // // // // // // //
        // SAVE
        //

        /// <summary>
        /// 
        /// Erstellt eine Organisationsdatei v2
        /// 
        /// </summary>
        public async Task Save(RepositoryVersion version)
        {
            string filePath = GetOrganizationFilePath(ProjektPath, version);

            if (version == RepositoryVersion.V1)
                await SaveV1Async(filePath);
            else if (version == RepositoryVersion.V2)
                await SaveV2Async(filePath);
        }


        /// <summary>
        /// 
        /// Erstellt eine Organisationsdatei v2
        /// 
        /// </summary>
        private async Task SaveV1Async(string filePath)
        {
            string projektManager = string.Join(",", LegacyPermissions
                .Where(permission => permission.Role == PermissionRole.Manager)
                .Select(permission => permission.User.SamAccountName));

            string readWrite = string.Join(",", LegacyPermissions
                .Where(permission => permission.Role == PermissionRole.Member)
                .Select(permission => permission.User.SamAccountName));

            string read = string.Join(",", LegacyPermissions
                .Where(permission => permission.Role == PermissionRole.Guest)
                .Select(permission => permission.User.SamAccountName));

            string fileContent = $@"#
# ProjektOrdner - InfoFile
#
# Diese Datei wird aus organisatorischen Gründen benötigt ! NICHT LÖSCHEN !
#

##
# <> Name des ProjektOrdners <>
# Leerzeichen sind im Ordnernamen erlaubt, sollten aber nicht direkt nach dem '=' erfolgen.
# Maximal darf der ProjektOrdner 45 Zeichen (a-z A-Z 0-9 und Leerzeichen) beinhalten.
ProjektName={ProjektName}

##
# <> Projekt Organisation <>
StartDatum={ErstelltAm.ToShortDateString()}
EndDatum={ProjektEnde.ToShortDateString()}

##
# <> BERECHTIGUNGEN <>
# Bitte geben Sie die Anmeldenamen nacheinander, durch ein einfaches Komma ',' getrennt, an.
# Der ProjektManager hat automatisch eine 'ReadWrite'-Berechtigung
ProjektManager={projektManager}
ReadWrite={readWrite}
Read={read}";

            using (StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                await streamWriter.WriteAsync(fileContent);
            }
        }


        /// <summary>
        /// 
        /// Speichere die Organisationsdatei Version 2
        /// 
        /// </summary>
        private async Task<bool> SaveV2Async(string filePath)
        {
            try
            {
                string organizationJson = JsonConvert.SerializeObject(this, new JsonSerializerSettings() { Formatting = Formatting.Indented });
                using (StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    await streamWriter.WriteAsync(organizationJson);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }



        // // // // // // // // // // // // // // // // // // // // //
        // Public Functions
        //


        /// <summary>
        /// 
        /// Erweitert das Projekt Ende Datum
        /// 
        /// </summary>
        public async Task SaveExpireDate(string folderPath, DateTime newDate)
        {
            await LoadOrganization();
            ProjektEnde = newDate;
            await Save(Version);
        }


        /// <summary>
        /// 
        /// Erstelle einen neuen Text Projektordner Antrag
        /// 
        /// </summary>
        public static string GetRequestTemplate()
        {
            return @"#
# ProjektOrdner - Beantragungsformular
# 


#> ORGANISATION <#
#
# 'ProjektName' darf bis zu 45 Zeichen (a-z A-Z 0-9 und Leerzeichen) beinhalten. (Eingabe erforderlich!)
# 'EndDatum' geben Sie bitte im Format TT.MM.JJJJ an. (Eingabe erforderlich!)
#
ProjektName=
EndDatum=


#> BERECHTIGUNGEN <#
#
# Sie können den Anmeldenamen, die eMail-Adresse oder die Matrikelnummer des Mitarbeiters/Studenten
# kommagetrennt (,) angeben.
#
# 'ProjektManager'  -> Lesen, Dateien bearbeiten und Andere in´s Projekt einladen (Eingabe erforderlich!)
# 'Mitarbeiter'     -> Lesen u. Dateien bearbeiten
# 'Gast'            -> Nur lesen und nichts verändern
#
ProjektManager=
Mitarbeiter=
Gast=
";
        }


        /// <summary>
        /// 
        /// Speichert einen Projektantrag
        /// 
        /// </summary>
        public static async Task SaveRequestTemplateAsync(string filePath, string requestTemplate)
        {
            using (StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                await streamWriter.WriteAsync(requestTemplate);
            }
        }


        /// <summary>
        /// 
        /// Prüft ein Repository auf gültige Daten
        /// 
        /// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(ProjektName) == true)
                return false;

            if (string.IsNullOrWhiteSpace(RootPath) == true)
                return false;

            if (ProjektEnde == DateTime.MinValue)
                return false;

            if (Version == RepositoryVersion.Unknown)
                return false;

            return true;
        }


        /// <summary>
        /// 
        /// Ermittelt die Verion des ProjektOrdners.
        /// 
        /// </summary>
        public RepositoryVersion GetRepositoryVersion()
        {
            string orgaVersion1FilePath = Path.Combine(ProjektPath, AppConstants.OrganisationFileNameV1);
            string orgaVersion2FilePath = Path.Combine(ProjektPath, Path.Combine(AppConstants.OrganisationFolderName, AppConstants.OrganisationFileNameV2));

            if (File.Exists(orgaVersion2FilePath) == true)
            {
                return RepositoryVersion.V2;
            }
            else
            {
                if (File.Exists(orgaVersion1FilePath) == true)
                {
                    return RepositoryVersion.V1;
                }
                else
                {
                    return RepositoryVersion.Unknown;
                }
            }
        }


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        public static string GetOrganizationFilePath(string projektPath, RepositoryVersion version)
        {
            string filePath = "";
            switch (version)
            {
                case RepositoryVersion.V1:
                {
                    filePath = Path.Combine(projektPath, AppConstants.OrganisationFileNameV1);
                    break;
                }
                case RepositoryVersion.V2:
                {
                    filePath = Path.Combine(
                        projektPath,
                        Path.Combine(
                            AppConstants.OrganisationFolderName,
                            AppConstants.OrganisationFileNameV2
                            )
                        );
                    break;
                }
            }

            return filePath;
        }


        /// <summary>
        /// 
        /// Erstellt eine alte Berechtigung, falls diese in einer Datei vorhanden ist.
        /// 
        /// </summary>
        private void CreateLegacyPermission(PermissionRole accessRole, string line)
        {
            List<string> usernames = line
                .Split(',')
                .Where(username => string.IsNullOrWhiteSpace(username) == false)
                .ToList();

            if (usernames.Count == 0)
                return;

            //foreach(string user in usernames)
            //LegacyPermissions.Add(new RepositoryPermission(new AdUser(user), accessRole, PermissionSource.File, null));

        }



        // // // // // // // // // // // // // // // // // // // // //
        // Validation Functions
        //

        public bool FileExist()
        {
            string filePath = GetOrganizationFilePath(ProjektPath, Version);
            if (File.Exists(filePath) == false)
                return false;

            return true;
        }


    }
}
