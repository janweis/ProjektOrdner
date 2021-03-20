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

        public string ProjektPath
        {
            get
            {
                return Path.Combine(RootPath, ProjektName);
            }
        }

        public string RootPath { get; set; }

        public DateTime ErstelltAm { get; set; }

        public DateTime ProjektEnde { get; set; }

        public RepositoryVersion Version { get; set; }

        public List<PermissionModel> LegacyPermissions { get; private set; }


        // // // // // // // // // // // // // // // // // // // // //
        // Constructors
        //

        public RepositoryOrganization()
        {
            ID = Guid.NewGuid();
            ProjektName = string.Empty;
            RootPath = string.Empty;
            ErstelltAm = DateTime.MinValue;
            ProjektEnde = DateTime.MinValue;
            Version = RepositoryVersion.Unknown;
            LegacyPermissions = new List<PermissionModel>();
        }



        // // // // // // // // // // // // // // // // // // // // //
        // Functions
        // 


        /// <summary>
        /// 
        /// Lade die Organisations- oder Antragsdatei Version 1
        /// 
        /// </summary>
        public async Task LoadV1(string folderPath)
        {
            if (Directory.Exists(folderPath) == false)
                return;

            RootPath = Directory.GetParent(folderPath).FullName;
            string filePath = Path.Combine(folderPath, AppConstants.OrganisationFileNameV1);

            await LoadV1(new FileInfo(filePath));
        }


        /// <summary>
        /// 
        /// Lade die Organisations- oder Antragsdatei Version 1
        /// 
        /// </summary>
        public async Task LoadV1(FileInfo file)
        {
            if (File.Exists(file.FullName) == false)
                return;

            RootPath = Directory.GetParent(file.DirectoryName).FullName;

            using (StreamReader streamReader = new StreamReader(file.FullName, Encoding.UTF8))
            {
                string fileText = await streamReader.ReadToEndAsync();

                if (null == fileText)
                    return;

                string[] fileLines = fileText
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                    .ToArray();

                LoadV1(fileLines);
            }
        }


        /// <summary>
        /// 
        /// Lade die Organisations- oder Antragsdatei Version 1
        /// 
        /// </summary>
        public void LoadV1(string[] fileContent)
        {
            if (null == fileContent)
                return;

            Version = RepositoryVersion.V1;

            // Process every Line
            foreach (string lineItem in fileContent)
            {
                if (lineItem.Contains("=") == true && lineItem.Contains("#") == false)
                {
                    string key = lineItem.Substring(0, lineItem.IndexOf("=")).ToLower().Trim();
                    string value = lineItem.Substring(lineItem.IndexOf("=") + 1).TrimStart().TrimEnd();

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
                            CreateLegacyPermission(PermissionRole.ReadWrite, value);
                            break;
                        }
                        case "read": // Nur für Projektantrag!
                        {
                            CreateLegacyPermission(PermissionRole.ReadOnly, value);
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
        public async Task<bool> LoadV2(string folderPath)
        {
            RootPath = Directory.GetParent(folderPath).FullName;
            string filePath = Path.Combine(folderPath, Path.Combine(AppConstants.OrganisationFolderName, AppConstants.OrganisationFileNameV2));

            if (File.Exists(filePath) == true)
            {
                using (StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8))
                {
                    string fileText = await streamReader.ReadToEndAsync();
                    RepositoryOrganization repositoryOrganization = JsonConvert.DeserializeObject<RepositoryOrganization>(fileText, new JsonSerializerSettings() { Formatting = Formatting.Indented });

                    if (null != repositoryOrganization)
                    {
                        ID = repositoryOrganization.ID;
                        ProjektName = repositoryOrganization.ProjektName;
                        ErstelltAm = repositoryOrganization.ErstelltAm;
                        ProjektEnde = repositoryOrganization.ProjektEnde;
                        //RootPath = repositoryOrganization.RootPath;
                        Version = repositoryOrganization.Version;
                    }
                }

                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// Speichere die Organisationsdatei Version 2
        /// 
        /// </summary>
        public async Task<bool> SaveV2(string folderPath = null)
        {
            string filePath;

            if (null == folderPath)
                folderPath = Path.Combine(RootPath, ProjektName);

            filePath = Path.Combine(folderPath, Path.Combine(AppConstants.OrganisationFolderName, AppConstants.OrganisationFileNameV2));

            if (Directory.Exists(folderPath) == true)
            {
                string organizationJson = JsonConvert.SerializeObject(this, new JsonSerializerSettings() { Formatting = Formatting.Indented });
                using (StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    await streamWriter.WriteAsync(organizationJson);
                }

                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// Erstelle einen neuen Text Projektordner Antrag
        /// 
        /// </summary>
        public static string GetRequestTemplate()
        {
            return @"#
## ProjektOrdner - Beantragungsformular
# 

## PROJEKT BEZEICHNUNG
# Leerzeichen sind im Ordnernamen erlaubt, sollten aber nicht direkt nach dem '=' erfolgen.
# den Unterstrich am Beginn des Dateinamens löschen.
# Maximal darf der ProjektOrdner 45 Zeichen (a-z A-Z 0-9 und Leerzeichen) beinhalten.
Name=

## PROJEKT ORGANISATION
EndeDatum=

## PROJEKT BERECHTIGUNGEN
# Der Projekt Manager kann Projektdateien Lesen, Schreiben und Projektberechtigungen anpassen.
ProjektManager=

# 'Lesen & Schreiben' ist die Berechtigung für normale Projektmitglieder
Mitarbeiter=

# 'Nur Lesen' ist für eingeschränkte Projektmitglieder
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
        public RepositoryVersion GetRepositoryVersion(string folderPath)
        {
            string orgaVersion1FilePath = Path.Combine(folderPath, AppConstants.OrganisationFileNameV1);
            string orgaVersion2FilePath = Path.Combine(folderPath, Path.Combine(AppConstants.OrganisationFolderName, AppConstants.OrganisationFileNameV2));

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

            usernames.ForEach(username =>
            {
                LegacyPermissions.Add(new PermissionModel()
                {
                    AccessRole = accessRole,
                    Source = PermissionSource.File,
                    User = new AdUser(username),
                    ProjektPath = ProjektPath
                });
            });
        }

    }
}
