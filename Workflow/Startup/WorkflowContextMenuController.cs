﻿using System;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Membership;
using Umbraco.Web;
using Workflow.Helpers;

namespace Workflow.Startup
{
    public class WorkflowContextMenuController : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            Umbraco.Web.Trees.TreeControllerBase.MenuRendering += ContentTreeController_MenuRendering;
        }

        private static void ContentTreeController_MenuRendering(Umbraco.Web.Trees.TreeControllerBase sender, Umbraco.Web.Trees.MenuRenderingEventArgs e)
        {
            // only add context menu to content nodes, exclude the root and recycle bin
            if (sender.TreeAlias != Constants.Trees.Content 
                || Convert.ToInt32(e.NodeId) == Constants.System.Root 
                || Convert.ToInt32(e.NodeId) == Constants.System.RecycleBinContent) return;

            int menuLength = e.Menu.Items.Count;
            string nodeName = Utility.GetNode(int.Parse(e.NodeId)).Name;
            IUser currentUser = UmbracoContext.Current.Security.CurrentUser;
            var items = new Umbraco.Web.Models.Trees.MenuItemList();

            var i = new Umbraco.Web.Models.Trees.MenuItem("workflowHistory", "Workflow history");
            i.LaunchDialogView("/App_Plugins/workflow/Backoffice/dialogs/workflow.history.dialog.html", "Workflow history: " + nodeName);
            i.AdditionalData.Add("width", "800px");
            i.SeperatorBefore = true;
            i.Icon = "directions-alt";

            items.Add(i);

            if (currentUser.IsAdmin())
            {
                i = new Umbraco.Web.Models.Trees.MenuItem("workflowConfig", "Workflow configuration");
                i.LaunchDialogView("/App_Plugins/workflow/Backoffice/dialogs/workflow.config.dialog.html", "Workflow configuration: " + nodeName);
                i.Icon = "path";

                items.Add(i);
            }

            if (menuLength < 5)
            {
                e.Menu.Items.AddRange(items);
            } else
            {
                e.Menu.Items.InsertRange(5, items);
            }
        }
    }    
}
