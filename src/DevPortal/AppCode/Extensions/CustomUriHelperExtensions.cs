﻿using DevPortal.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode.Extensions
{
    public static class CustomUrlHelperExtensions
    {
        /// <param name="action">
        /// Optional. If not specified, all actions will match.
        /// </param>
        public static bool IsCurrentRoute(this IUrlHelper uri, string controller = "", string action = null)
        {
            var routeData = uri.ActionContext.RouteData;

            var routeControl = (string)routeData.Values["controller"];
            var routeAction = (string)routeData.Values["action"];

            bool isCurrentController =  controller.Equals(routeControl, StringComparison.CurrentCultureIgnoreCase);
            bool isCurrentAction = action == null || action.Equals(routeAction, StringComparison.CurrentCultureIgnoreCase);

            return isCurrentController && isCurrentAction;
        }

        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ConfirmEmail),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ResetPassword),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }
    }
}
