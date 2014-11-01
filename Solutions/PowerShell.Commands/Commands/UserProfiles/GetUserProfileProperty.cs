﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands.UserProfiles
{
    [Cmdlet(VerbsCommon.Get, "SPOUserProfileProperty")]
    [CmdletHelp(@"Office365 only: Uses the tenant API to retrieve site information.

You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command. 
", Details = "Requires a connection to a SharePoint Tenant Admin site.")]
    [CmdletExample(Code = @"
PS:> Get-SPOUserProfileProperty -Account 'user@domain.com'", Remarks = "Returns the profile properties for the specified user")]
    [CmdletExample(Code = @"
PS:> Get-SPOUserProfileProperty -Account 'user@domain.com','user2@domain.com'", Remarks = "Returns the profile properties for the specified users")]
    public class GetUserProfileProperty : SPOAdminCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "The account of the user, formatted either as a login name, or as a claims identity, e.g. i:0#.f|membership|user@domain.com", Position = 0)]
        public string[] Account;

        protected override void ExecuteCmdlet()
        {
            PeopleManager peopleManager = new PeopleManager(ClientContext);

            foreach (var acc in Account)
            {
                ClientResult<string> result = Tenant.EncodeClaim(acc);
                ClientContext.ExecuteQuery();
                var properties = peopleManager.GetPropertiesFor(result.Value);
                ClientContext.Load(properties);
                ClientContext.ExecuteQuery();
                WriteObject(properties);
            }
        }
    }
}
