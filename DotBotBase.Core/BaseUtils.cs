using DotBotBase.Core.Logging;
using Mono.Cecil;

namespace DotBotBase.Core;

public static class BaseUtils
{
    public static async Task SafeInvoke(this Logger log, string error, Func<Task> func)
    {
        try
        {
            await func();
        }
        catch (Exception ex)
        {
            log.LogError(error, ex);
        }
    }
    
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

    // https://stackoverflow.com/a/15608028
    public static bool IsManagedAssembly(string fileName)
    {
        using (Stream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        using (BinaryReader binaryReader = new BinaryReader(fileStream))
        {
            if (fileStream.Length < 64)
            {
                return false;
            }

            //PE Header starts @ 0x3C (60). Its a 4 byte header.
            fileStream.Position = 0x3C;
            uint peHeaderPointer = binaryReader.ReadUInt32();
            if (peHeaderPointer == 0)
            {
                peHeaderPointer = 0x80;
            }

            // Ensure there is at least enough room for the following structures:
            //     24 byte PE Signature & Header
            //     28 byte Standard Fields         (24 bytes for PE32+)
            //     68 byte NT Fields               (88 bytes for PE32+)
            // >= 128 byte Data Dictionary Table
            if (peHeaderPointer > fileStream.Length - 256)
            {
                return false;
            }

            // Check the PE signature.  Should equal 'PE\0\0'.
            fileStream.Position = peHeaderPointer;
            uint peHeaderSignature = binaryReader.ReadUInt32();
            if (peHeaderSignature != 0x00004550)
            {
                return false;
            }

            // skip over the PEHeader fields
            fileStream.Position += 20;

            const ushort PE32 = 0x10b;
            const ushort PE32Plus = 0x20b;

            // Read PE magic number from Standard Fields to determine format.
            var peFormat = binaryReader.ReadUInt16();
            if (peFormat != PE32 && peFormat != PE32Plus)
            {
                return false;
            }

            // Read the 15th Data Dictionary RVA field which contains the CLI header RVA.
            // When this is non-zero then the file contains CLI data otherwise not.
            ushort dataDictionaryStart = (ushort)(peHeaderPointer + (peFormat == PE32 ? 232 : 248));
            fileStream.Position = dataDictionaryStart;

            uint cliHeaderRva = binaryReader.ReadUInt32();
            if (cliHeaderRva == 0)
            {
                return false;
            }

            return true;
        }
    }
}
