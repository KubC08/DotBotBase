using DotBotBase.Core.Logging;
using Mono.Cecil;

namespace DotBotBase.Core;

public static class BaseUtils
{
    public static void SafeInvoke(this Logger log, string error, Action func)
    {
        try
        {
            func();
        }
        catch (Exception ex)
        {
            log.LogError(error, ex);
        }
    }

    public static CustomAttribute[] GetCustomAttributes(this TypeDefinition typeDef, Type attribType)
    {
        string? typeName = attribType.FullName;
        if (typeName == null || !typeDef.HasCustomAttributes) return Array.Empty<CustomAttribute>();

        List<CustomAttribute> customAttributes = new List<CustomAttribute>();
        foreach (var customAttribute in typeDef.CustomAttributes)
            if (customAttribute.AttributeType.FullName == typeName)
                customAttributes.Add(customAttribute);
        return customAttributes.ToArray();
    }

    public static bool TryGetAttribute(this TypeDefinition typeDef, Type attribType, out CustomAttribute? result)
    {
        result = null;
        string? typeName = attribType.FullName;
        if (typeName == null || !typeDef.HasCustomAttributes) return false;

        foreach (var customAttribute in typeDef.CustomAttributes)
        {
            if (customAttribute.AttributeType.FullName != typeName) continue;

            result = customAttribute;
            return true;
        }
        return false;
    }
}