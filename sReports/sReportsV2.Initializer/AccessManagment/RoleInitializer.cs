using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.AccessManagment;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;

namespace sReportsV2.Initializer.AccessManagment
{
    public class RoleInitializer
    {
        private readonly IRoleDAL roleDAL;
        private readonly IGlobalThesaurusRoleDAL globalThesaurusRoleDAL;
        private readonly IModuleDAL moduleDAL;
        private readonly IPermissionDAL permissionDAL;
        private readonly IPermissionModuleDAL permissionModuleDAL;

        public RoleInitializer()
        {
            this.roleDAL = DependencyResolver.Current.GetService<IRoleDAL>();
            this.globalThesaurusRoleDAL = DependencyResolver.Current.GetService<IGlobalThesaurusRoleDAL>();
            this.moduleDAL = DependencyResolver.Current.GetService<IModuleDAL>();
            this.permissionDAL = DependencyResolver.Current.GetService<IPermissionDAL>();
            this.permissionModuleDAL = DependencyResolver.Current.GetService<IPermissionModuleDAL>();
        }
        public void SetInitialData()
        {
            SetInitialModules();
            SetInitialPermissions();
            SetInitialRoles();
        }
        private void SetInitialRoles()
        {
            if(roleDAL.Count() == 0)
            {
                Role roleSuperAdministrator = new Role
                {
                    Name = PredifinedRole.SuperAdministrator.ToString(),
                    Description = "A super administrator is a person who has full access to user and organizational modules, and other non-patient-related resources of the system. Super administrator can create users, add organizations and change general system properties."
                };
                Role roleAdministrator = new Role
                {
                    Name = PredifinedRole.Administrator.ToString(),
                    Description = "A administrator is a person who has can manage users within an organization he belongs to and perform other non-patient related system tasks defined by the Super Administrator"
                };
                Role roleDoctor = new Role
                {
                    Name = PredifinedRole.Doctor.ToString(),
                    Description = "A doctor is a person who has can edit documents of patient's clinical trials within organization he is employed and do other activities defined by administrators."
                };

                SetInitialPermissionsForRole(roleDoctor, permissionModuleDAL.GetAllByModule(ModuleNames.Patients));
                SetInitialPermissionsForRole(roleSuperAdministrator, permissionModuleDAL.GetAllByModule(ModuleNames.Administration));

                roleDAL.InsertOrUpdate(roleSuperAdministrator);
                roleDAL.InsertOrUpdate(roleAdministrator);
                roleDAL.InsertOrUpdate(roleDoctor);
            }

            if(globalThesaurusRoleDAL.Count() == 0)
            {
                GlobalThesaurusRole roleSuperAdministrator = new GlobalThesaurusRole
                {
                    Name = PredifinedGlobalUserRole.SuperAdministrator.ToString(),
                    Description = "A super administrator is a person who has full access to user and any other modules. Super administrator can manage user's roles."
                };
                GlobalThesaurusRole roleViewer = new GlobalThesaurusRole
                {
                    Name = PredifinedGlobalUserRole.Viewer.ToString(),
                    Description = "A viewer can access to the smart oncology content and view it."
                };
                GlobalThesaurusRole roleEditor = new GlobalThesaurusRole
                {
                    Name = PredifinedGlobalUserRole.Editor.ToString(),
                    Description = "A editor can edit terminology terms."
                };
                GlobalThesaurusRole roleCurator = new GlobalThesaurusRole
                {
                    Name = PredifinedGlobalUserRole.Curator.ToString(),
                    Description = "A curator can edit and cure terms."
                };

                globalThesaurusRoleDAL.InsertOrUpdate(roleSuperAdministrator);
                globalThesaurusRoleDAL.InsertOrUpdate(roleViewer);
                globalThesaurusRoleDAL.InsertOrUpdate(roleEditor);
                globalThesaurusRoleDAL.InsertOrUpdate(roleCurator);
            }
        }

        private void SetInitialPermissionsForRole(Role role, List<PermissionModule> permissionModules)
        {
            List<PermissionRole> permissions = permissionModules.Select(p => new PermissionRole() { ModuleId = p.ModuleId, PermissionId = p.PermissionId }).ToList();
            role.Permissions.AddRange(permissions);
        }

        private void SetInitialModules()
        {
            if (moduleDAL.Count() == 0)
            {
                Module administrationModule = new Module
                {
                    Name = ModuleNames.Administration,
                    Description = "Within this module administrator can edit user roles, global, user and document related properties and release or block functionality of the system."
                };
                Module designerModule = new Module
                {
                    Name = ModuleNames.Designer,
                    Description = "Designer module has functionalities that are related to defining medical forms and documents (creating and editing, reviewing with comments and consensus finding process)"
                };
                Module engineModule = new Module
                {
                    Name = ModuleNames.Engine,
                    Description = "Engine module has functionalities that are related to creating documents, viewing its' content, exporting in appropriate format and downloading."
                };
                Module thesaurusModule = new Module
                {
                    Name = ModuleNames.Thesaurus,
                    Description = "Thesaurus module has functionalities that are related to creating thesaurus with defining its own codes or integrate with UMLS."
                };
                Module patientModule = new Module
                {
                    Name = ModuleNames.Patients,
                    Description = "Patients module has functionalities that are related to editing patient's data and adding new episode of cares and encounters."
                };
                Module simulatorModule = new Module
                {
                    Name = ModuleNames.Simulator,
                    Description = "Simulator module has functionalities that are related to parametarize probabilities of some values and establish relations between them."
                };

                List<Module> modules = new List<Module>
                {
                    designerModule,
                    engineModule,
                    thesaurusModule,
                    patientModule,
                    simulatorModule,
                    administrationModule
                };
                moduleDAL.InsertMany(modules);
            }
        }

        private void SetInitialPermissions()
        {
            if (permissionDAL.Count() == 0)
            {
                Permission createUpdate = new Permission
                {
                    Name = PermissionNames.CreateUpdate,
                    Description = "Create or update entity"
                };
                Permission delete = new Permission
                {
                    Name = PermissionNames.Delete,
                    Description = "Delete entity"
                };
                Permission view = new Permission
                {
                    Name = PermissionNames.View,
                    Description = "View entity"
                };
                List<Permission> generalPermissions = new List<Permission>()
                {
                    createUpdate,
                    view,
                    delete
                };

                List<Permission> administrationPermissions = new List<Permission>();
                List<Permission> simulatorPermissions = new List<Permission>();

                Permission showJson = new Permission
                {
                    Name = PermissionNames.ShowJson,
                    Description = "Show Json"
                };
                Permission changeState = new Permission
                {
                    Name = PermissionNames.ChangeState,
                    Description = "Change State"
                };
                Permission findConsensus = new Permission
                {
                    Name = PermissionNames.FindConsensus,
                    Description = "Find Consensus"
                };
                Permission viewAdministrativeData = new Permission
                {
                    Name = PermissionNames.ViewAdministrativeData,
                    Description = "View Administrative Data"
                };
                Permission viewComments = new Permission
                {
                    Name = PermissionNames.ViewComments,
                    Description = "View Comments"
                };
                Permission addComment = new Permission
                {
                    Name = PermissionNames.AddComment,
                    Description = "Add Comment"
                };
                Permission changeCommentState = new Permission
                {
                    Name = PermissionNames.ChangeCommentStatus,
                    Description = "Change Comment Status"
                };
                List<Permission> designerPermisssions = new List<Permission>()
                {
                    showJson,
                    changeState,
                    findConsensus,
                    viewAdministrativeData,
                    viewComments,
                    addComment,
                    changeCommentState
                };

                Permission downloadDocument = new Permission
                {
                    Name = PermissionNames.Download,
                    Description = "Download document"
                };
                List<Permission> enginePermissions = new List<Permission>()
                {
                    downloadDocument
                };

                Permission createCode = new Permission
                {
                    Name = PermissionNames.CreateCode,
                    Description = "Create Code"
                };
                Permission umls = new Permission
                {
                    Name = PermissionNames.UMLS,
                    Description = "UMLS"
                };
                List<Permission> thesaurusPermisssions = new List<Permission>()
                {
                    createCode,
                    umls
                };

                Permission addEpisodeOfCare = new Permission
                {
                    Name = PermissionNames.AddEpisodeOfCare,
                    Description = "Add Episode Of Care"
                };
                Permission addEncounter = new Permission
                {
                    Name = PermissionNames.AddEncounter,
                    Description = "Add Encounter"
                };
                List<Permission> patientsPermisssions = new List<Permission>()
                {
                    addEpisodeOfCare,
                    addEncounter
                };

                List<Permission> generalPermissionsDB = permissionDAL.InsertMany(generalPermissions);

                administrationPermissions.AddRange(generalPermissionsDB);
                designerPermisssions.AddRange(generalPermissionsDB);
                enginePermissions.AddRange(generalPermissionsDB);
                thesaurusPermisssions.AddRange(generalPermissionsDB);
                patientsPermisssions.AddRange(generalPermissionsDB);
                simulatorPermissions.AddRange(generalPermissionsDB);

                SetInitialPermissionsForModule(ModuleNames.Administration, administrationPermissions);
                SetInitialPermissionsForModule(ModuleNames.Designer, designerPermisssions);
                SetInitialPermissionsForModule(ModuleNames.Engine, enginePermissions);
                SetInitialPermissionsForModule(ModuleNames.Thesaurus, thesaurusPermisssions);
                SetInitialPermissionsForModule(ModuleNames.Patients, patientsPermisssions);
                SetInitialPermissionsForModule(ModuleNames.Simulator, simulatorPermissions);
            }
            SetAdditionalPermissions(ModuleNames.Engine, GetEngineAdditionaPermissions());
        }

        private List<Permission> GetEngineAdditionaPermissions()
        {
            return new List<Permission>()
            {
                new Permission
                {
                    Name = PermissionNames.SignFormInstance,
                    Description = "Sign Form Instance"
                }, 
                new Permission
                {
                    Name = PermissionNames.ChangeFormInstanceState,
                    Description = "Change Form Instance State"
                }
            };
        }

        private void SetAdditionalPermissions(string moduleName, List<Permission> additionalEnginePermissions)
        {
            SetInitialPermissionsForModule(moduleName, PrepareNewPermissions(additionalEnginePermissions));
        }

        private List<Permission> PrepareNewPermissions(List<Permission> permissionsToAdd)
        {
            List<Permission> newPermissionsToAdd = new List<Permission>();
            foreach(Permission permission in permissionsToAdd)
            {
                if (!permissionDAL.HasPermission(permission.Name))
                {
                    newPermissionsToAdd.Add(permission);
                }
            }
            return newPermissionsToAdd;
        }

        private void SetInitialPermissionsForModule(string moduleName, List<Permission> permissions)
        {
            List<Permission> generalPermissions = permissions.Where(x => x.PermissionId > 0).ToList();
            List<Permission> modulePersmissions = permissions.Where(x => x.PermissionId == 0).ToList();
            
            List<int> permissionsIds = permissionDAL.InsertMany(modulePersmissions).Select(x => x.PermissionId).ToList();
            permissionsIds.AddRange(generalPermissions.Select(x => x.PermissionId));
            
            Module module = moduleDAL.GetByName(moduleName);
            List<PermissionModule> permissionsPerModule = permissionsIds.Select(pId => new PermissionModule { ModuleId = module.ModuleId, PermissionId = pId }).ToList();
            permissionModuleDAL.InsertMany(permissionsPerModule);
        }
    }
}
