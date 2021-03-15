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

        public string Name { get; set; }

        public string ProjektPath
        {
            get
            {
                return Path.Combine(RootPath, Name);
            }
        }

        public string RootPath { get; set; }

        public DateTime ErstelltAm { get; set; }

        public DateTime EndeDatum { get; set; }

        public RepositoryVersion Version { get; set; }

        public List<PermissionModel> LegacyPermissions { get; private set; }


        // // // // // // // // // // // // // // // // // // // // //
        // Constructors
        //

        public RepositoryOrganization()
        {
            ID = Guid.NewGuid();
            Name = string.Empty;
            RootPath = string.Empty;
            ErstelltAm = DateTime.MinValue;
            EndeDatum = DateTime.MinValue;
            Version = RepositoryVersion.Unknown;
            LegacyPermissions = new List<PermissionModel>();
        }


        // // // // // // // // // // // // // // // // // // // // //
        // Functions
        // 


        /// <summary>
        /// 
        /// Lade die Organisationsdatei Version 2
        /// 
        /// </summary>
        public async Task<bool> LoadV2(string folderPath, string rootPath)
        {
            RootPath = rootPath;
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
                        Name = repositoryOrganization.Name;
                        ErstelltAm = repositoryOrganization.ErstelltAm;
                        EndeDatum = repositoryOrganization.EndeDatum;
                        RootPath = repositoryOrganization.RootPath;
                        Version = repositoryOrganization.Version;
                    }
                }

                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// Lade die Organisations- oder Antragsdatei Version 1
        /// 
        /// </summary>
        public async Task<bool> LoadV1(string fileOrFolder, string rootPath)
        {
            RootPath = rootPath;

            if (Path.HasExtension(fileOrFolder) == false)
                fileOrFolder = Path.Combine(fileOrFolder, AppConstants.OrganisationFileNameV1);

            if (File.Exists(fileOrFolder) == true)
            {
                using (StreamReader streamReader = new StreamReader(fileOrFolder, Encoding.UTF8))
                {
                    string fileText = await streamReader.ReadToEndAsync();

                    if (null == fileText)
                        return false;

                    // Split to Array
                    string[] resultContent = fileText
                            .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                            .Where(x => x.Contains("#") == false)
                            .ToArray();

                    // Process every Line
                    foreach (string lineItem in resultContent)
                    {
                        if (lineItem.Contains("="))
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
                                    Name = value;
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
                                case "endedatum":
                                {
                                    if (DateTime.TryParse(value, out DateTime dateTime))
                                    {
                                        EndeDatum = dateTime;
                                    }
                                    break;
                                }
                                case "projektmanager": // Nur für Projektantrag!
                                {
                                    CreateLegacyPermission(PermissionAccessRole.Manager, value);
                                    break;
                                }
                                case "mitarbeiter": // Nur für Projektantrag!
                                {
                                    CreateLegacyPermission(PermissionAccessRole.ReadWrite, value);
                                    break;
                                }
                                case "gast": // Nur für Projektantrag!
                                {
                                    CreateLegacyPermission(PermissionAccessRole.ReadOnly, value);
                                    break;
                                }
                            }
                        }
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
        public async Task<bool> SaveV2(string folderPath)
        {
            string filePath = Path.Combine(folderPath, Path.Combine(AppConstants.OrganisationFolderName, AppConstants.OrganisationFileNameV2));
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
        public static string[] GetTemplate()
        {
            return new[] { @"#
## ProjektOrdner - Beantragungsformular
# 

## PROJEKT BEZEICHNUNG
# Leerzeichen sind im Ordnernamen erlaubt, sollten aber nicht direkt nach dem '=' erfolgen.
# den Unterstrich am Beginn des Dateinamens löschen.
# Maximal darf der ProjektOrdner 45 Zeichen (a-z A-Z 0-9 und Leerzeichen) beinhalten.
ProjektName=

## PROJEKT ORGANISATION
EndeDatum=

## PROJEKT BERECHTIGUNGEN
# Der Projekt Manager kann Projektdateien Lesen, Schreiben und Projektberechtigungen anpassen.
ProjektManager=

# 'Lesen & Schreiben' ist die Berechtigung für normale Projektmitglieder
Mitarbeiter=

# 'Nur Lesen' ist für eingeschränkte Projektmitglieder
Gast=
" };
        }


        /// <summary>
        /// 
        /// Prüft ein Repository auf gültige Daten
        /// 
        /// </summary>
        public bool IsValid()
        {
            if (Name == string.Empty)
                return false;

            if (RootPath == string.Empty)
                return false;

            if (EndeDatum == DateTime.MinValue)
                return false;

            if (Version == RepositoryVersion.Unknown)
                return false;

            return true;
        }


        /// <summary>
        /// 
        /// Erstellt eine alte Berechtigung, falls diese in einer Datei vorhanden ist.
        /// 
        /// </summary>
        private void CreateLegacyPermission(PermissionAccessRole accessRole, string line)
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
                    User = new UserModel(username),
                    ProjektPath = ProjektPath
                });
            });
        }
    }
}
