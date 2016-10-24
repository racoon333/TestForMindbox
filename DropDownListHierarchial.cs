using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebPages;

public static class HtmlHelpers
{
    public static MvcHtmlString DropDownListHierarchialFor<TModel, TProperty, TType>(this HtmlHelper<TModel> htmlHelper, 
        Expression<Func<TModel, TProperty>> expression, IEnumerable<TType> itemsList, string idProperty, string displayProperty, string subProperty,
        string optionLabel=null, object htmlAttributes=null)
    {
        bool selectionFound = false;
        var fieldName = ExpressionHelper.GetExpressionText(expression);
        var fullBindingName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
        var fieldId = TagBuilder.CreateSanitizedId(fullBindingName);

        var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
        string value = metadata.Model?.ToString();

        if (value == null)
            selectionFound = true;

        var validationAttributes = htmlHelper.GetUnobtrusiveValidationAttributes(fullBindingName, metadata);

        TagBuilder select = new TagBuilder("select");
        select.MergeAttribute("name", fullBindingName);
        select.MergeAttribute("id", fieldId);

        foreach (var key in validationAttributes.Keys)
        {
            select.MergeAttribute(key, validationAttributes[key].ToString());
        }

        if (htmlAttributes != null)
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            select.MergeAttributes(attributes);
        }

        if (optionLabel != null)
        {
            TagBuilder defaultoption = new TagBuilder("option");
            defaultoption.MergeAttribute("value", optionLabel);
            select.InnerHtml += defaultoption.ToString(TagRenderMode.SelfClosing);
        }

        Type type = null;
        if (itemsList.GetType().IsGenericType)
        {
            type = itemsList.GetType().GetGenericArguments()[0];
        }
        else
        {
            foreach (var item in itemsList)
            {
                type = item.GetType();
                break;
            }
        }

        select.InnerHtml += GetOptionsRecursive(itemsList, type, idProperty, displayProperty, subProperty, value, ref selectionFound, -5);
        return new MvcHtmlString(select.ToString(TagRenderMode.Normal));
    }

    private static string GetOptionsRecursive<TType>(IEnumerable<TType> itemsList, Type type, string idProperty, string displayProperty, 
        string subProperty, string value, ref bool selectionFound, int padding)
    {
        string Options="";
        padding += 5;
        foreach (var item in itemsList)
        {
            var displayValue = type.GetProperty(displayProperty).GetValue(item).ToString();
            var idValue = type.GetProperty(idProperty).GetValue(item).ToString();
            var subValue = (IEnumerable<TType>)type.GetProperty(subProperty).GetValue(item);
            TagBuilder option = new TagBuilder("option");
            option.MergeAttribute("value", idValue);
            option.InnerHtml += String.Concat(Enumerable.Repeat("&nbsp;", padding))+displayValue;
            if (!selectionFound)
                if (idValue == value)
                {
                    option.MergeAttribute("selected", null);
                    selectionFound = true;
                }
            Options += option.ToString(TagRenderMode.Normal);
            if (subValue.Count() > 0)
                Options += GetOptionsRecursive(subValue, type, idProperty, displayProperty, subProperty, value, ref selectionFound, padding);
        }
        return Options;
    }
}
