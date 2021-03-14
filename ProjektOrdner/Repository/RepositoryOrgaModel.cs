using ProjektOrdner.App;
using System;
using System.IO;

namespace ProjektOrdner.Repository
{
    public class RepositoryOrgaModel
    {
        public Guid ID { get; }

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


        public RepositoryOrgaModel()
        {
            ID = Guid.NewGuid();
            ProjektName = string.Empty;
            RootPath = string.Empty;
            ErstelltAm = DateTime.MinValue;
            ProjektEnde = DateTime.MinValue;
            Version = RepositoryVersion.Unknown;
        }

    }
}