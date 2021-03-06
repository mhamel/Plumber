### Installing

If you're reading this, you've successfully installed the latest version of Plumber, the workflow solution for Umbraco.

The installation process created a new backoffice section (you're in it), and added a suite of new features to your Umbraco website.

The installer grants access to the new section for all users in the Administrators group. You may wish to modify this according to best suit your site and requirements.

Check in on the Workflow section dashboard to stay up-to-date with the latest release - when the installed version is superceded, Plumber will prompt you to upgrade.

### Overview

Plumber extends Umbraco's standard publishing model to allow creation of multi-stage approval workflows.

A workflow process can comprise multiple steps (no limit aside from practicality), with multiple users assigned to the group responsible for providing approval at each step.

A user can be a member of multiple groups in the same workflow, although again, for practicality's sake, this probably doesn't make sense.

To initiate an approval workflow, a user updates content, saves their changes, then selects 'Request approval' from the editor button drawer.

After entering a comment describing the nature off the changes, and submitting the request, members of the approving group are notified via email, and have a task pushed into their workflow dashboard.

Tasks can be approved (or cancelled or rejected) from the dashboard or from the content node button drawer.

The workflow dashboard updates to reflect the state of each task, providing an easy overview of a user's submissions and tasks.

Rejecting a task returns the workflow to the original author, who can update the content and resubmit. The resubmitted content does not restart the workflow, but returns to the stage at which it was rejected.

Once content has been submitted into a workflow, either as an initial request or resubmit, the content can only be edited by the original change author - members of approving groups at subsequent steps can not modify content in an active workflow.

### Settings

Plumber comes pre-wired with sensible defaults, but these should be modified to best suit your site.

- **Default approval group:** this is the fallback group for cases when no workflow is configured. You'll need to configure your groups before setting this value.
- **Flow type:** how should the approval flow progress? These options manage how the change author is included in the workflow:
    - **Explicit:** all steps of the workflow must be completed, and all users will be notified of tasks (including the change author)
    - **Implicit:** all steps where the original change author is NOT a member of the group must be completed. Steps where the original change author is a member of the approving group will be completed automatically, and noted in the workflow history as not required.
- **Send notifications:** if your users are active in the backoffice, email notifications might not be required. Turn them off here.
- **Workflow email:** Set a sender address for notification emails. This defaults to the system email as defined in umbracoSettings.config
- **Site URL:** the URL for the public website (including schema - http[s])
- **Edit site URL:** the URL for the editing environment (including schema - http[s])
- **Exclude nodes:** nodes selected here are excluded from the workflow engine and will be published per the configured Umbraco user permissions
- **Document-type approvals:** configure workflows to be applied to all content of the selected document type. Refer to xx for details on approval flow types

### Upgrades

Plumber will display a prompt on the Workflow section dashboard when a new version is available.

Displaying the prompt is determined by comparing the assembly version of the current installed version with the latest released version tagged in the GitHub repository.

If the remote version is newer than the installed package, the upgrade prompt displays the release notes and a link to the package.

### Approval groups

Plumber uses a separate groups model from the rest of your Umbraco website. It's different, but looks familiar.

The view displays the current assigned responsibilities for the group, to help keep track of who is approving which pages.

Add users to approval groups to determine which users will be responsible for approving content changes.

- **Group email:** sometimes it's more appropriate to send workflow notifications to a generic inbox rather than the individual group members. Add a value here to do exactly that.
- **Description:** it isn't used anywhere other than the group view. It's a note to remind you why the group exists.

### Approval flow types

Approval flows come in three flavours: explicit, inherited and document-type.

A given content node may have all three approval flow types applied, but only one will be applied per the following order of priority:

- **Explicit:** set directly on a content node via the context menu. This type will take priority over all others.
- **Document-type:** set in the settings section. This approval flow will apply to all content of the selected document type, unless the node has an explicit flow set.
- **Inherited:** if a node has no explicit approval flow, nor a flow applied to its document-type, Plumber will traverse the content tree until it finds a node with an explicit flow, and will use this flow for the current change. For this reason, an approval flow should always be set on the homepage node as a default.

Current responsibilites for groups can be reviewed on the user group view, for explicit and document-type approval flows only.

### Dashboards

Plumber adds a set of dashboards to the Umbraco install:

- **User dashboard:** added in the content section, this view displays all submissions and pending tasks for the current user. If the current user is also a workflow administrator (ie they have access to the Workflow section), they are also shown a paginated list of all current workflow tasks.
- **Admin dashboard:** the default view in the Workflow section, the admin dashboard renders two tabs:
    - the overview tab renders a chart displaying workflow activity over the selected time period (defaults to 28 days). Tasks are grouped by status: approved, cancelled, rejected and pending. The dashboard also displays a notification when the installed Plumber version needs updating.
    - the documentation tab is the one you're reading.
    - the log viewer tab provides an interface to the internal logs generated by workflow actions. Hopefully there's no ERROR items in here. Errors happen, this dashboard helps with addressing them.
    - the deployment tab is a potentially dangerous place. Refer to the [Deployment section](#deployment-tools) of the documentation for more detail.

### Context menu

Plumber adds two new options to the content node context menu:

- **Workflow history:** displays an overview of the workflow tasks completed for the current node
- **Workflow configuration:** provides an interface to set the explicit approval flow for the current node, and displays any document-type or inherited approval flows (these can not be modified from the context menu)

Both options are only displayed for workflow administrator users (ie, those users with access to the Workflow section).

### Button drawer

Plumber replaces the default Umbraco button set in the editor drawer. Depending on user permissions, content state and workflow state, the button set will display one of the following:

- Save
- Request publish
- Approve changes

The button set dropdown will include additional options:

- Reject changes
- Cancel workflow
- Request unpublish

Plumber overrides Umbraco's user/group publishing permissions. Provided the user has permission to update the node, they will be able to intiate a workflow process on that node. Plumber essentially shifts Umbraco from a centrally administered publishing model (ie controlled by a site administrator) to a distributed model, where editors publish content based on their responsibilities assigned through inclusion in workflows.

In cases where the content is already in a workflow, a notification is displayed next to the button set. Plumber also ensures modified content is saved before submitting for publish approval by watching for changes on the content form, then updating the visible button as appropriate.

For nodes where the workflow has been disabled, the default Umbraco options are displayed.

### Deployment tools<a name="deployment-tools"></a>

This feature is experimental. Use it at your own risk.

The Deployment tab in the Workflow section provides an interface for exporting and importing Plumber configuration, for deployment across different environments. 

The endpoints used for both actions can be access programmatically as part of a deployment script.

For manual exports, the Export Plumber config does exactly that - generates a JSON representation of all data in the Plumber database tables (settings, usergroups, user2usergroup and usergrouppermissions).

This JSON model can then be re-imported into other environments.

Warning - the import is destructive and can not be reversed. If the uploaded configuration model is valid, all data in the relevant tables will be deleted and overwritten.