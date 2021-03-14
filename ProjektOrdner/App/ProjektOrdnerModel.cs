using ProjektOrdner.Permission;
using ProjektOrdner.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektOrdner.App
{
    public class ProjektOrdnerModel
    {
        public RepositoryModel Repository { get; set; }
        public List<PermissionModel> Permissions { get; set; }

        public ProjektOrdnerModel()
        {
            Repository = new RepositoryModel();
            Permissions = new List<PermissionModel>();
        }

        public ProjektOrdnerModel(RepositoryModel repository)
        {
            Repository = repository;
            Permissions = new List<PermissionModel>();
        }

        public ProjektOrdnerModel(RepositoryModel repository, List<PermissionModel> permissions)
        {
            Repository = repository;
            Permissions = permissions;
        }
    }
}
