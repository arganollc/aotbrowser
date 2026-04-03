using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Dynamics.AX.Metadata.Kernel;
using Microsoft.Dynamics.AX.Metadata.MetaModel;
using Microsoft.Dynamics.AX.Metadata.Storage;
using Microsoft.Dynamics.AX.Metadata.Core.MetaModel;
using Microsoft.Dynamics.AX.Metadata.Extensions;
using Microsoft.Dynamics.AX.Metadata;

namespace Arbela.Dynamics.Ax.Xpp
{
    public static class MetadataSupport
    {
        private static EdtExtensionLoader edtExtensionLoader;
        private static QuerySimpleExtensionLoader queryExtensionLoader;
        private static MenuItemDisplayExtensionLoader menuItemDisplayExtensionLoader;
        private static MenuItemActionExtensionLoader menuItemActionExtensionLoader;
        private static MenuItemOutputExtensionLoader menuItemOutputExtensionLoader;

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
            if (edtExtensionLoader == null)
            {
                edtExtensionLoader = new EdtExtensionLoader(Accessor.MetadataProvider);
            }
            return edtExtensionLoader.GetExtensionsForBaseObject(edt);
        }

        public static IEnumerable<AxQuerySimpleExtension> GetQueryExtensionsForQuery(string query)
        {
            if (queryExtensionLoader == null)
            {
                queryExtensionLoader = new QuerySimpleExtensionLoader(Accessor.MetadataProvider);
            }
            return queryExtensionLoader.GetExtensionsForBaseObject(query);
        }

        public static IEnumerable<AxMenuItemDisplayExtension> GetMenuItemDisplayExtensionsForMenuItemDisplay(string menuItemDisplay)
        {
            if (menuItemDisplayExtensionLoader == null)
            {
                menuItemDisplayExtensionLoader = new MenuItemDisplayExtensionLoader(Accessor.MetadataProvider);
            }
            return menuItemDisplayExtensionLoader.GetExtensionsForBaseObject(menuItemDisplay);
        }

        public static IEnumerable<AxMenuItemActionExtension> GetMenuItemActionExtensionsForMenuItemAction(string menuItemAction)
        {
            if (menuItemActionExtensionLoader == null)
            {
                menuItemActionExtensionLoader = new MenuItemActionExtensionLoader(Accessor.MetadataProvider);
            }
            return menuItemActionExtensionLoader.GetExtensionsForBaseObject(menuItemAction);
        }

        public static IEnumerable<AxMenuItemOutputExtension> GetMenuItemOutputExtensionsForMenuItemOutput(string menuItemOutput)
        {
            if (menuItemOutputExtensionLoader == null)
            {
                menuItemOutputExtensionLoader = new MenuItemOutputExtensionLoader(Accessor.MetadataProvider);
            }
            return menuItemOutputExtensionLoader.GetExtensionsForBaseObject(menuItemOutput);
        }

        public static IEnumerable<AxFormExtension> GetFormExtensionsForForm(string form)
        {
            IList<string> formExtensionNames = Accessor.GetFormExtensionNames(form);
            List<AxFormExtension> formExtensions = new List<AxFormExtension>();
            foreach(var formExtensionName in formExtensionNames)
            {
                var ext = Accessor.GetFormExtension(formExtensionName);
                if (ext != null)
                {
                    formExtensions.Add(ext);
                }
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
                var ext = Accessor.GetMenuExtension(menuExtensionName);
                if (ext != null)
                {
                    menuExtensions.Add(ext);
                }
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

        public static IEnumerator<string> KPINames()
        {
            return Accessor.GetKPINames().GetEnumerator();
        }

        public static IEnumerable<AxWorkflowApprovalExtension> GetWorkflowApprovalExtensionsForWorkflowApproval(string workflowApproval)
        {
            IList<string> extensionNames = Accessor.GetWorkflowApprovalExtensionNames(workflowApproval);
            List<AxWorkflowApprovalExtension> extensions = new List<AxWorkflowApprovalExtension>();
            foreach (var extensionName in extensionNames)
            {
                var ext = Accessor.GetWorkflowApprovalExtension(extensionName);
                if (ext != null)
                {
                    extensions.Add(ext);
                }
            }

            return extensions;
        }

        public static IEnumerable<AxWorkflowTaskExtension> GetWorkflowTaskExtensionsForWorkflowTask(string workflowTask)
        {
            IList<string> extensionNames = Accessor.GetWorkflowTaskExtensionNames(workflowTask);
            List<AxWorkflowTaskExtension> extensions = new List<AxWorkflowTaskExtension>();
            foreach (var extensionName in extensionNames)
            {
                var ext = Accessor.GetWorkflowTaskExtension(extensionName);
                if (ext != null)
                {
                    extensions.Add(ext);
                }
            }

            return extensions;
        }

        public static IEnumerable<AxWorkflowTemplateExtension> GetWorkflowTemplateExtensionsForWorkflowTemplate(string workflowTemplate)
        {
            IList<string> extensionNames = Accessor.GetWorkflowTemplateExtensionNames(workflowTemplate);
            List<AxWorkflowTemplateExtension> extensions = new List<AxWorkflowTemplateExtension>();
            foreach (var extensionName in extensionNames)
            {
                var ext = Accessor.GetWorkflowTemplateExtension(extensionName);
                if (ext != null)
                {
                    extensions.Add(ext);
                }
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

        public static string GetXppSourceText(INamedObject axElement)
        {
            return MetadataSupport.GetXppSourceText(axElement, null, true);
        }

        public static string GetXppSourceText(INamedObject axElement, IElementCodePositionCollector codePositionCollector, bool handleMetadataCorruptedException = true)
        {
            string result = string.Empty;
            try
            {
                if (axElement is AxClass)
                {
                    result = ((AxClass)axElement).GetCompleteSource(codePositionCollector);
                }
                else if (axElement is AxTable)
                {
                    result = ((AxTable)axElement).GetCompleteSource(codePositionCollector);
                }
                else if (axElement is AxView)
                {
                    result = ((AxView)axElement).GetCompleteSource(codePositionCollector);
                }
                else if (axElement is AxMap)
                {
                    result = ((AxMap)axElement).GetCompleteSource(codePositionCollector);
                }
                else if (axElement is AxDataEntityView)
                {
                    result = ((AxDataEntityView)axElement).GetCompleteSource(codePositionCollector);
                }
                else if (axElement is AxAggregateDataEntity)
                {
                    result = ((AxAggregateDataEntity)axElement).GetCompleteSource(codePositionCollector);
                }
                else if (axElement is AxForm)
                {
                    result = ((AxForm)axElement).GetCompleteSource(codePositionCollector);
                }
                else if (axElement is AxQuery)
                {
                    result = ((AxQuery)axElement).GetCompleteSource(codePositionCollector);
                }
                else if (axElement is AxMacroDictionary)
                {
                    result = ((AxMacroDictionary)axElement).GetCompleteSource(null);
                }
                else if (axElement is AxFormDataSourceRoot)
                {
                    result = ((AxFormDataSourceRoot)axElement).GetCompleteSource(true, null);
                }
                else if (axElement is AxFormDataSourceField)
                {
                    result = ((AxFormDataSourceField)axElement).GetCompleteSource(true, null);
                }
                else if (axElement is AxFormControl)
                {
                    result = ((AxFormControl)axElement).GetCompleteSource(true, null);
                }
            }
            catch (MetadataCorruptedException ex) when (handleMetadataCorruptedException)
            {
                //AxLogHandler.DisplayError(ex.Message);
                return string.Empty;
            }
            return result;
        }

        public static bool CanGetSourceText(Object checkObject)
        {
            if (checkObject is AxClass
                || checkObject is AxTable
                || checkObject is AxView
                || checkObject is AxMap
                || checkObject is AxDataEntityView
                || checkObject is AxAggregateDataEntity
                || checkObject is AxForm
                || checkObject is AxQuery
                || checkObject is AxMacroDictionary
                || checkObject is AxFormDataSourceRoot
                || checkObject is AxFormDataSourceField
                || checkObject is AxFormControl)
            {
                return true;
            }

            return false;
        }
    }
}
