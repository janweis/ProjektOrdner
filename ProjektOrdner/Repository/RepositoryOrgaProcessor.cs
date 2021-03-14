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
    public class RepositoryOrgaProcessor
    {
        private string OrganisationFilePathv1 { get; set; }
        private string OrganisationFilePathv2 { get; set; }
        private string ProjektPath { get; set; }
        private string RootFolder { get; set; }
        private List<PermissionModel> ProjektPermissionsFromOrga { get; set; }


        public RepositoryOrgaProcessor(string projektPath)
        {
            if (string.IsNullOrWhiteSpace(projektPath))
                throw new ArgumentNullException();

            if (Directory.Exists(projektPath) == false)
                throw new DirectoryNotFoundException();

            ProjektPath = projektPath;
            RootFolder = Directory.GetParent(projektPath).FullName;
            ProjektPermissionsFromOrga = new List<PermissionModel>();

            OrganisationFilePathv1 = GetOrganisationFilePath(projektPath, RepositoryVersion.V1);
            OrganisationFilePathv2 = GetOrganisationFilePath(projektPath, RepositoryVersion.V2);
        }

        public RepositoryOrgaProcessor(FileInfo file, string rootPath)
        {
            OrganisationFilePathv1 = file.FullName;
            RootFolder = rootPath;
            ProjektPermissionsFromOrga = new List<PermissionModel>();
        }


        //
        // ORGANISATION MANAGE FUNCTIONS
        // ------------------------------------------------------------------------------------------------------

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
        /// Ließt die Informationen aus den Projekt-Header-Dateien aus
        /// 
        /// </summary>
        public async Task<RepositoryOrgaModel> GetInformationAsync()
        {
            RepositoryOrgaModel organisation;

            if (File.Exists(OrganisationFilePathv2) == true)
            {
                // Version 2
                organisation = await ReadOrganisationFileV2Async();
            }
            else
            {
                if (File.Exists(OrganisationFilePathv1) == true)
                {
                    // Version 1
                    organisation = await ReadOrganisationFileV1Async();
                }
                else
                {
                    return null;
                }
            }

            return organisation;
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
        public async Task WriteOrganisationAsync(RepositoryOrgaModel projektOrganisation)
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


        //
        // HELPER FUNCTIONS
        // ------------------------------------------------------------------------------------------------------


        /// <summary>
        /// 
        /// Ließt die JSON-Organisationsdatei Version 2
        /// 
        /// </summary>
        private async Task<RepositoryOrgaModel> ReadOrganisationFileV2Async()
        {
            JsonSerializerSettings jsonSerializerOptions = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string jsonRawContent = await ReadFileContentAsync(OrganisationFilePathv2);
            RepositoryOrgaModel repositoryOrga = JsonConvert.DeserializeObject<RepositoryOrgaModel>(jsonRawContent, jsonSerializerOptions);

            if (repositoryOrga != null)
            {
                repositoryOrga.RootPath = RootFolder;
                repositoryOrga.Version = RepositoryVersion.V2;
            }

            return repositoryOrga;
        }


        /// <summary>
        /// 
        /// Interpretiere den Dateiinhalt Text-Organisationdatei Version 1
        /// 
        /// </summary>
        private async Task<RepositoryOrgaModel> ReadOrganisationFileV1Async()
        {
            string fileContent = await ReadFileContentAsync(OrganisationFilePathv1);

            if (null == fileContent)
                return null; // Die Datei konnte nicht gelesen werden!

            RepositoryOrgaModel projektOrganisation = new RepositoryOrgaModel()
            {
                RootPath = RootFolder,
                Version = RepositoryVersion.V1
            };

            // Split to Array
            string[] resultContent = fileContent
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
                            projektOrganisation.ProjektName = value;

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
                                projektOrganisation.ErstelltAm = dateTime;
                            }

                            break;
                        }
                        case "enddatum":
                        {
                            if (DateTime.TryParse(value, out DateTime dateTime))
                            {
                                projektOrganisation.ProjektEnde = dateTime;
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

            if(ProjektPath == string.Empty)
            {
                // Vermutlich wurde nur ein ProjektAntrag ausgelesen. Der ProjektPath
                // muss noch gefüllt werden.

                ProjektPath = Path.Combine(RootFolder, projektOrganisation.ProjektName);
            }


            return projektOrganisation;
        }

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


        /// <summary>
        /// 
        /// Ließt die Text-Organisationdatei Version 1
        /// 
        /// </summary>
        private async Task<string> ReadFileContentAsync(string filePath)
        {
            if (null == filePath)
                throw new ArgumentNullException();

            string fileText;
            using (StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8))
            {
                fileText = await streamReader.ReadToEndAsync();
            }

            return fileText;
        }


    }
}
