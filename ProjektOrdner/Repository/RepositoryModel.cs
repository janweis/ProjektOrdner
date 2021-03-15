using ProjektOrdner.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.Repository
{
    public class RepositoryModel
    {
        public enum RepositoryStatus { Ok, Corrupted, NotChecked }

        public RepositoryOrganization RepositoryOrga { get; set; }
        public RepositorySettings Settings { get; set; }
        public RepositoryVersion Version { get; set; }
        public RepositoryStatus Status { get; set; }


        public RepositoryModel()
        {
            Version = RepositoryVersion.Unknown;
            Status = RepositoryStatus.NotChecked;
        }

        public RepositoryModel(RepositoryOrganization projektOrganisation, RepositorySettings settings, RepositoryVersion version)
        {
            RepositoryOrga = projektOrganisation;
            Settings = settings;
            Version = version;
        }

        public override string ToString()
        {
            return $@"\n
- Name: '{RepositoryOrga.Name}'
- Ende: {RepositoryOrga.EndeDatum.ToShortDateString()}
- Version: {Version.ToString()}
";
        }
    }
}
