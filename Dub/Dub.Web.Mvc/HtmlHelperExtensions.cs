// -----------------------------------------------------------------------
// <copyright file="HtmlHelperExtensions.cs" company="Andrey Kurdiumov">
// Copyright (c) Andrey Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Dub.Web.Mvc
{
    using System;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Mvc;
    using Dub.Web.Mvc.Properties;

    /// <summary>
    /// Extensions for the HTML helper
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Helper method which displays help button.
        /// </summary>
        /// <param name="htmlHelper">Html helper to use.</param>
        /// <param name="content">Content of the help popup.</param>
        /// <returns>Html string which represents help control.</returns>
        public static HtmlString Help(this HtmlHelper htmlHelper, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return new HtmlString(string.Empty);
            }

            var spanTag = new TagBuilder("span");

            content = content.Trim();

            spanTag.Attributes.Add("class", "help-button");
            spanTag.Attributes.Add("data-rel", "popover");
            spanTag.Attributes.Add("data-trigger", "hover");
            spanTag.Attributes.Add("data-placement", "top");
            spanTag.Attributes.Add("data-content", content);
            spanTag.Attributes.Add("data-original-title", Resources.HelpPopupHeader);
            spanTag.SetInnerText("?");
            return new HtmlString(spanTag.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Helper method which displays help button.
        /// </summary>
        /// <typeparam name="TModel">Type of the model.</typeparam>
        /// <typeparam name="TProperty">Type of the property in model for which should be displayed help.</typeparam>
        /// <param name="htmlHelper">Html helper to use.</param>
        /// <param name="expression">Content of the help popup.</param>
        /// <returns>Html string which represents help control.</returns>
        public static HtmlString HelpFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            if (metadata.AdditionalValues.ContainsKey("Help"))
            {
                var value = metadata.AdditionalValues["Help"];
                return Help(htmlHelper, value == null ? null : value.ToString());
            }

            return new HtmlString(string.Empty);
        }
    }
}
