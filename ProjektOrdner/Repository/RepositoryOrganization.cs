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
        //
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

        public DateTime ProjektEnde { get; set; }

        public RepositoryVersion Version { get; set; }

        public PermissionModel[] LegacyPermissions { get; private set; }


        //
        // Constructors
        //

        public RepositoryOrganization()
        {
            ID = Guid.NewGuid();
            Name = string.Empty;
            RootPath = string.Empty;
            ErstelltAm = DateTime.MinValue;
            ProjektEnde = DateTime.MinValue;
            Version = RepositoryVersion.Unknown;
        }


        //
        // Functions
        // 

        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        public async Task<bool> LoadV2(string folderPath)
        {
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
                        ProjektEnde = repositoryOrganization.ProjektEnde;
                        RootPath = repositoryOrganization.RootPath;
                        Version = repositoryOrganization.Version;

                        return true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        public async Task<bool> LoadV1(string folderPath)
        {
            string filePath = Path.Combine(folderPath, AppConstants.OrganisationFileNameV1);

            if (File.Exists(filePath) == true)
            {
                using (StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8))
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

                                    if (null == ProjektPath)
                                    {
                                        // Vermutlich wurde nur ein ProjektAntrag ausgelesen. Der ProjektPath
                                        // muss noch gefüllt werden.

                                        ProjektPath = Path.Combine(RootFolder, projektOrganisation.ProjektName);
                                    }

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
                                    AddPermissionEntry(PermissionAccessRole.Manager, value);
                                    break;
                                }
                                case "mitarbeiter": // Nur für Projektantrag!
                                {
                                    AddPermissionEntry(PermissionAccessRole.ReadWrite, value);
                                    break;
                                }
                                case "gast": // Nur für Projektantrag!
                                {
                                    AddPermissionEntry(PermissionAccessRole.ReadOnly, value);
                                    break;
                                }
                            }
                        }
                    }







                    if (null != repositoryOrganization)
                    {
                        ID = repositoryOrganization.ID;
                        Name = repositoryOrganization.Name;
                        ErstelltAm = repositoryOrganization.ErstelltAm;
                        ProjektEnde = repositoryOrganization.ProjektEnde;
                        RootPath = repositoryOrganization.RootPath;
                        Version = repositoryOrganization.Version;

                        return true;
                    }
                }
            }

            return false;


        }


        /// <summary>
        /// 
        /// 
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
        /// 
        /// 
        /// </summary>
        public static string GetOrganisationFilePath(string projektPath, RepositoryVersion version)
        {
            string filePath = string.Empty;
            switch (version)
            {
                case RepositoryVersion.V1:
                {
                    filePath = Path.Combine(
                        projektPath,
                        AppConstants.OrganisationFileNameV1);

                    break;
                }
                case RepositoryVersion.V2:
                {
                    filePath = Path.Combine(
                        projektPath,
                        Path.Combine(
                            AppConstants.OrganisationFolderName,
                            AppConstants.OrganisationFileNameV2));

                    break;
                }
                case RepositoryVersion.Unknown:
                {
                    break;
                }
            }

            return filePath;
        }




        /// <summary>
        /// 
        /// Falls in einem Projektantrag Berechtigungen vorhanden sind, können diese abgerufen werden.
        /// Erst GetInformationAsync abrufen!
        /// 
        /// </summary>
        public PermissionModel[] GetPermissionEntries()
        {
            ProjektPermissionsFromOrga.ForEach(permissionItem =>
            {
                UserProcessor userProcessor = new UserProcessor(permissionItem.User);
                userProcessor.UpdateUserData();
            });

            return ProjektPermissionsFromOrga.ToArray();
        }


        /// <summary>
        /// 
        /// Schreibt die JSON-Organisationsdatei Version 2
        /// 
        /// </summary>
        public async Task Save(RepositoryOrgaModel projektOrganisation)
        {
            if (null == projektOrganisation)
                throw new ArgumentNullException();

            JsonSerializerSettings jsonSerializerOptions = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string jsonKonfiguration = JsonConvert.SerializeObject(projektOrganisation, jsonSerializerOptions);
            using (StreamWriter streamWriter = new StreamWriter(OrganisationFilePathv2, false, Encoding.UTF8))
            {
                await streamWriter.WriteAsync(jsonKonfiguration);
            }
        }


        /// <summary>
        /// 
        /// Erstelle einen neuen Text Projektordner Antrag
        /// 
        /// </summary>
        public static string[] AntragFileTemplate()
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
StartDatum=
EndDatum=

## PROJEKT BERECHTIGUNGEN
# Der Projekt Manager kann Projektdateien Lesen, Schreiben und Projektberechtigungen anpassen.
ProjektManager=

# 'Lesen & Schreiben' ist die Berechtigung für normale Projektmitglieder
LesenSchreiben=

# 'Nur Lesen' ist für eingeschränkte Projektmitglieder
NurLesen=
" };
        }


        /// <summary>
        /// 
        /// Erstelle einen neuen CSV Projektordner Antrag
        /// 
        /// </summary>

        public static string[] AntragCsvTemplate()
        {
            return new[] { "ProjektName;ProjektEnde;Manager;Mitarbeiter;NurLesen" };
        }


        /// <summary>
        /// 
        /// Prüft ein Repository auf gültige Daten
        /// 
        /// </summary>
        public bool HasValidRepositoryData(RepositoryOrgaModel repository)
        {
            if (null == repository)
                return false;

            if (repository.ProjektName == string.Empty)
                return false;

            if (repository.RootPath == string.Empty)
                return false;

            if (repository.ProjektEnde == DateTime.MinValue)
                return false;

            if (repository.Version == RepositoryVersion.Unknown)
                return false;

            return true;
        }


        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        private void AddPermissionEntry(PermissionAccessRole accessRole, string line)
        {
            var usernames = line.Split(',');

            if (null == usernames)
                return;

            foreach (string username in usernames)
            {
                ProjektPermissionsFromOrga.Add(
                    new PermissionModel()
                    {
                        AccessRole = accessRole,
                        Source = PermissionSource.File,
                        User = new UserModel(username),
                        ProjektPath = ProjektPath
                    });
            }
        }

    }

}
