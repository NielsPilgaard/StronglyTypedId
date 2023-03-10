using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace StronglyTypedIds
{
    internal static class EmbeddedSources
    {
        private static readonly Assembly ThisAssembly = typeof(EmbeddedSources).Assembly;
        internal static readonly string StronglyTypedIdAttributeSource = LoadTemplateForEmitting("StronglyTypedIdAttribute");
        internal static readonly string StronglyTypedIdDefaultsAttributeSource = LoadTemplateForEmitting("StronglyTypedIdDefaultsAttribute");

        internal static readonly string AutoGeneratedHeader = LoadEmbeddedResource("StronglyTypedIds.Templates.AutoGeneratedHeader.cs");

        internal static string LoadEmbeddedResource(string resourceName)
        {
            var resourceStream = ThisAssembly.GetManifestResourceStream(resourceName);
            if (resourceStream is null)
            {
                var existingResources = ThisAssembly.GetManifestResourceNames();
                throw new ArgumentException($"Could not find embedded resource {resourceName}. Available names: {string.Join(", ", existingResources)}");
            }

            using var reader = new StreamReader(resourceStream, Encoding.UTF8);

            return reader.ReadToEnd();
        }

        internal static string LoadTemplateForEmitting(string resourceName)
        {
            var resource = LoadEmbeddedResource($"StronglyTypedIds.Templates.Sources.{resourceName}.cs");
            return AutoGeneratedHeader + @"#if STRONGLY_TYPED_ID_EMBED_ATTRIBUTES

" + resource
                       .Replace("public sealed", "internal sealed")
                       .Replace("public enum", "internal enum")
                   + @"
#endif";
        }
    }
}