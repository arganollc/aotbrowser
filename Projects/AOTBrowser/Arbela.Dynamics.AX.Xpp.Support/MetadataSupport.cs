using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Dynamics.AX.Metadata.Kernel;
using Microsoft.Dynamics.AX.Metadata.MetaModel;
using System.Collections.Specialized;
using Microsoft.Dynamics.AX.Metadata.Storage;

namespace Arbela.Dynamics.Ax.Xpp
{
    public static class MetadataSupport
    {
        public static IEnumerable<AxViewExtension> GetViewExtensionsForView(string viewName)
        {
            return Accessor.GetViewExtensionsForView(viewName);
        }

        public static IEnumerable<AxEnumExtension> GetEnumExtensionsForEnum(string enumName)
        {
            return Accessor.GetEnumExtensionForEnum(enumName);
        }

        public static IEnumerable<AxDataEntityViewExtension> GetDataEntityViewExtensionsForDataEntityView(string dataEntityView)
        {
            return Accessor.GetDataEntityViewExtensionsForDataEntityView(dataEntityView);
        }

        public static IEnumerable<AxEdtExtension> GetEdtExtensionsForEdt(string edt)
        {
            var extensionLoader = new EdtExtensionLoader(Accessor.MetadataProvider);
            return extensionLoader.GetExtensionsForBaseObject(edt);
        }

        public static IEnumerable<AxQuerySimpleExtension> GetQueryExtensionsForQuery(string query)
        {
            var extensionLoader = new QuerySimpleExtensionLoader(Accessor.MetadataProvider);
            return extensionLoader.GetExtensionsForBaseObject(query);
        }

        public static IEnumerable<AxMenuItemDisplayExtension> GetMenuItemDisplayExtensionsForMenuItemDisplay(string menuItemDisplay)
        {
            var extensionLoader = new MenuItemDisplayExtensionLoader(Accessor.MetadataProvider);
            return extensionLoader.GetExtensionsForBaseObject(menuItemDisplay);
        }

        public static IEnumerable<AxMenuItemActionExtension> GetMenuItemActionExtensionsForMenuItemAction(string menuItemAction)
        {
            var extensionLoader = new MenuItemActionExtensionLoader(Accessor.MetadataProvider);
            return extensionLoader.GetExtensionsForBaseObject(menuItemAction);
        }

        public static IEnumerable<AxMenuItemOutputExtension> GetMenuItemOutputExtensionsForMenuItemOutput(string menuItemOutput)
        {
            var extensionLoader = new MenuItemOutputExtensionLoader(Accessor.MetadataProvider);
            return extensionLoader.GetExtensionsForBaseObject(menuItemOutput);
        }

        public static IEnumerable<AxFormExtension> GetFormExtensionsForForm(string form)
        {
            IList<string> formExtensionNames = Accessor.GetFormExtensionNames(form);
            List<AxFormExtension> formExtensions = new List<AxFormExtension>();
            foreach(var formExtensionName in formExtensionNames)
            {
                formExtensions.Add(Accessor.GetFormExtension(formExtensionName));
            }

            return formExtensions;
        }

        public static IEnumerator<string> MenuNames()
        {
            return Accessor.GetMenuNames().GetEnumerator();
        }

        public static IEnumerable<AxMenuExtension> GetMenuExtensionsForMenu(string menu)
        {
            IList<string> menuExtensionNames = Accessor.GetMenuExtensionNames(menu);
            List<AxMenuExtension> menuExtensions = new List<AxMenuExtension>();
            foreach (var menuExtensionName in menuExtensionNames)
            {
                menuExtensions.Add(Accessor.GetMenuExtension(menuExtensionName));
            }

            return menuExtensions;
        }

        public static IEnumerator<string> TileNames()
        {
            return Accessor.GetTileNames().GetEnumerator();
        }

        public static IEnumerator<string> TableCollectionNames()
        {
            return Accessor.GetTableCollectionNames().GetEnumerator();
        }

        public static AxTableCollection GetTableCollection(string tableCollection)
        {
            return Accessor.GetTableCollection(tableCollection);
        }

        public static IEnumerator<string> CompositeDataEntityNames()
        {
            return Accessor.GetCompositeEntityViewNames().GetEnumerator();
        }

        public static AxCompositeDataEntityView GetCompositeDataEntity(string compositeDataEntity)
        {
            return Accessor.GetCompositeDataEntityView(compositeDataEntity);
        }

        public static IEnumerator<string> SecurityDutyNames()
        {
            return Accessor.GetSecurityDutyNames().GetEnumerator();
        }

        public static IEnumerator<string> SecurityPrivilegeNames()
        {
            return Accessor.GetSecurityPrivilegeNames().GetEnumerator();
        }

        public static AxSecurityDuty GetSecurityDuty(string securityDuty)
        {
            return Accessor.GetSecurityDuty(securityDuty);
        }

        public static AxSecurityPrivilege GetSecurityPrivilege(string securityPrivilege)
        {
            return Accessor.GetSecurityPrivilege(securityPrivilege);
        }

        public static IEnumerable<AxSecurityDutyExtension> GetSecurityDutyExtensionsForSecurityDuty(string securityDuty)
        {
            return Accessor.GetSecurityDutyExtensionForSecurityDuty(securityDuty);
        }

        public static IEnumerable<AxSecurityRoleExtension> GetSecurityRoleExtensionsForSecurityRole(string securityrole)
        {
            return Accessor.GetSecurityRoleExtensionForSecurityRole(securityrole);
        }

        public static IEnumerator<string> AggregateDataEntityNames()
        {
            return Accessor.GetAggregateDataEntityNames().GetEnumerator();
        }

        // Current support for this starts in PU29
        //public static IEnumerable<AxWorkflowApprovalExtension> GetWorkflowApprovalExtensionsForWorkflowApproval(string workflowApproval)
        //{
        //    IList<string> extensionNames = Accessor.GetWorkflowApprovalExtensionNames(workflowApproval);
        //    List<AxWorkflowApprovalExtension> extensions = new List<AxWorkflowApprovalExtension>();
        //    foreach (var extensionName in extensionNames)
        //    {
        //        extensions.Add(Accessor.GetWorkflowApprovalExtension(extensionName));
        //    }

        //    return extensions;
        //}

        // Current support for this starts in PU29
        //public static IEnumerable<AxWorkflowTaskExtension> GetWorkflowTaskExtensionsForWorkflowTask(string workflowTask)
        //{
        //    IList<string> extensionNames = Accessor.GetWorkflowTaskExtensionNames(workflowTask);
        //    List<AxWorkflowTaskExtension> extensions = new List<AxWorkflowTaskExtension>();
        //    foreach (var extensionName in extensionNames)
        //    {
        //        extensions.Add(Accessor.GetWorkflowTaskExtension(extensionName));
        //    }

        //    return extensions;
        //}

        public static IEnumerable<AxWorkflowTemplateExtension> GetWorkflowTemplateExtensionsForWorkflowTemplate(string workflowTemplate)
        {
            IList<string> extensionNames = Accessor.GetWorkflowTemplateExtensionNames(workflowTemplate);
            List<AxWorkflowTemplateExtension> extensions = new List<AxWorkflowTemplateExtension>();
            foreach (var extensionName in extensionNames)
            {
                extensions.Add(Accessor.GetWorkflowTemplateExtension(extensionName));
            }

            return extensions;
        }

        public static AxService GetService(string service)
        {
            return Accessor.GetService(service);
        }

        public static AxServiceGroup GetServiceGroup(string serviceGroup)
        {
            return Accessor.GetServiceGroup(serviceGroup);
        }

        public static IEnumerator<string> ServiceGroupNames()
        {
            return Accessor.GetServiceGroupNames().GetEnumerator();
        }
    }
}
