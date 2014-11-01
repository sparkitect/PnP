﻿using OfficeDevPnP.Core.IdentityModel.WSTrustBindings;
using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Security;

namespace OfficeDevPnP.Core.IdentityModel.TokenProviders.ADFS
{
    /// <summary>
    /// ADFS Active authentication based on username + password. Uses the trust/13/usernamemixed ADFS endpoint.
    /// </summary>
    public class UsernameMixed : BaseProvider
    {
        /// <summary>
        /// Performs active authentication against ADFS using the trust/13/usernamemixed ADFS endpoint.
        /// </summary>
        /// <param name="siteUrl">Url of the SharePoint site that's secured via ADFS</param>
        /// <param name="userName">Name of the user (e.g. domain\administrator) </param>
        /// <param name="password">Password of th user</param>
        /// <param name="userNameMixed">Uri to the ADFS usernamemixed endpoint</param>
        /// <param name="relyingPartyIdentifier">Identifier of the ADFS relying party that we're hitting</param>
        /// <returns>A cookiecontainer holding the FedAuth cookie</returns>
        public CookieContainer GetFedAuthCookie(string siteUrl, string userName, string password, Uri userNameMixed, string relyingPartyIdentifier)
        {
            UsernameMixed adfsTokenProvider = new UsernameMixed();
            var token = adfsTokenProvider.RequestToken(userName, password, userNameMixed, relyingPartyIdentifier);
            string samlToken = TransformSamlTokenToFedAuth(token.TokenXml.OuterXml, siteUrl);

            CookieContainer cc = null;

            if (!string.IsNullOrEmpty(samlToken))
            {
                cc = new CookieContainer();
                Cookie samlAuth = new Cookie("FedAuth", samlToken);
                samlAuth.Expires = DateTime.Now.AddHours(1);
                samlAuth.Path = "/";
                samlAuth.Secure = true;
                samlAuth.HttpOnly = true;
                Uri samlUri = new Uri(siteUrl);
                samlAuth.Domain = samlUri.Host;
                cc.Add(samlAuth);
            }

            return cc;
        }

        private GenericXmlSecurityToken RequestToken(string userName, string passWord, Uri userNameMixed, string relyingPartyIdentifier)
        {
            var factory = new WSTrustChannelFactory(new UserNameWSTrustBinding(SecurityMode.TransportWithMessageCredential),
                                                    new EndpointAddress(userNameMixed));

            factory.TrustVersion = TrustVersion.WSTrust13;
            // Hookup the user and password 
            factory.Credentials.UserName.UserName = userName;
            factory.Credentials.UserName.Password = passWord;

            var requestSecurityToken = new RequestSecurityToken
            {
                RequestType = RequestTypes.Issue,
                AppliesTo = new EndpointReference(relyingPartyIdentifier),
                KeyType = KeyTypes.Bearer
            };

            IWSTrustChannelContract channel = factory.CreateChannel();
            GenericXmlSecurityToken genericToken = channel.Issue(requestSecurityToken) as GenericXmlSecurityToken;

            return genericToken;
        }

    }
}