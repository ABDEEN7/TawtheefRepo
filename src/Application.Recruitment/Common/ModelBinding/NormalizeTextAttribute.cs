using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Application.Recruitment.Common.ModelBinding;

[AttributeUsage(AttributeTargets.Property)]
public sealed class NormalizeTextAttribute() : ModelBinderAttribute(typeof(NormalizedTextModelBinder));

public sealed class NormalizedTextModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

        var normalizedValue = valueProviderResult.FirstValue?
            .Replace("\u200B", string.Empty)
            .Replace("\uFEFF", string.Empty)
            .Replace('\u00A0', ' ')
            .Trim();

        bindingContext.Result = ModelBindingResult.Success(normalizedValue);
        return Task.CompletedTask;
    }
}
