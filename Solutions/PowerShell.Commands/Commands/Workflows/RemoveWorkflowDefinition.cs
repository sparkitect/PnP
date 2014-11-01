﻿using System.Management.Automation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WorkflowServices;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System.Collections.Generic;
using System;

namespace OfficeDevPnP.PowerShell.Commands.Workflows
{
    [Cmdlet(VerbsCommon.Remove, "SPOWorkflowDefinition")]
    public class RemoveWorkflowDefinition : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The subscription to remove", Position = 0)]
        public WorkflowDefinitionPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (Identity.Definition != null)
            {
                Identity.Definition.Delete();
            }
            else if (Identity.Id != Guid.Empty)
            {
                var definition = this.SelectedWeb.GetWorkflowDefinition(Identity.Id);
                if (definition != null)
                    definition.Delete();
            }
            else if (!string.IsNullOrEmpty(Identity.Name))
            {
                var definition = this.SelectedWeb.GetWorkflowDefinition(Identity.Name);
                if (definition != null)
                    definition.Delete();
            }
        }
    }

}
