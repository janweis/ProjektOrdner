using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.App
{
    public class SettingTeam
    {
        public string TeamName { get; set; }
        public string AdGroupSamAccountName { get; set; }

        public SettingTeam(string teamName, string adGroupSamAccountName)
        {
            TeamName = teamName;
            AdGroupSamAccountName = adGroupSamAccountName;
        }
    }
}
