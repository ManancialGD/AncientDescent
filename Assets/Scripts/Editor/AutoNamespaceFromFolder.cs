#if UNITY_EDITOR
namespace GameName.Editor
{
    using UnityEditor;
    using UnityEngine;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class AutoNamespaceFromFolder
    {
        private const string GameName = "GameName";
        private const string ScriptsRoot = "Assets/Scripts";

        [MenuItem("Tools/Namespaces/Generate Namespace For Selected Scripts")]
        private static void GenerateForSelection()
        {
            foreach (Object obj in Selection.objects)
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);

                if (!assetPath.EndsWith(".cs"))
                    continue;

                if (!assetPath.StartsWith(ScriptsRoot))
                {
                    Debug.LogWarning($"Skipped (not under {ScriptsRoot}): {assetPath}");
                    continue;
                }

                ProcessFile(assetPath);
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Namespaces/Generate Namespaces For All Scripts")]
        private static void GenerateForAll()
        {
            string[] csFiles = Directory.GetFiles(
                ScriptsRoot,
                "*.cs",
                SearchOption.AllDirectories
            );

            int processed = 0;
            int skipped = 0;

            foreach (string fullPath in csFiles)
            {
                string assetPath = NormalizePath(fullPath);

                if (ProcessFile(assetPath))
                    processed++;
                else
                    skipped++;
            }

            AssetDatabase.Refresh();
            Debug.Log($"Namespace generation complete. Processed: {processed}, Skipped: {skipped}");
        }

        private static bool ProcessFile(string assetPath)
        {
            string fullPath = Path.GetFullPath(assetPath);
            string content = File.ReadAllText(fullPath);

            // Skip files that already define a namespace
            if (HasNamespace(content))
                return false;

            string namespaceName = BuildNamespace(assetPath);
            string updatedContent = InsertNamespace(content, namespaceName);

            File.WriteAllText(fullPath, updatedContent, Encoding.UTF8);
            Debug.Log($"Namespace added: {namespaceName}");

            return true;
        }

        private static string InsertNamespace(string content, string namespaceName)
        {
            string[] lines = content.Split('\n');
            int insertIndex = FindUsingBlockEnd(lines);

            StringBuilder builder = new StringBuilder();

            // Write header exactly as-is
            for (int i = 0; i < insertIndex; i++)
                builder.Append(lines[i].TrimEnd('\r')).Append('\n');

            builder.AppendLine();

            builder.AppendLine($"namespace {namespaceName}");
            builder.Append("{");

            // Indent remainder
            for (int i = insertIndex; i < lines.Length; i++)
            {
                if (lines[i].Length > 0)
                    builder.Append("    ");

                builder.AppendLine(lines[i].TrimEnd('\r'));
            }

            builder.AppendLine("}");

            return builder.ToString();
        }

        private static int FindUsingBlockEnd(string[] lines)
        {
            int index = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                if (Regex.IsMatch(lines[i], @"^\s*using\s+.+;"))
                {
                    index = i + 1;
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(lines[i]))
                    break;
            }

            return index;
        }

        private static string BuildNamespace(string assetPath)
        {
            string relativePath = assetPath
                .Substring(ScriptsRoot.Length)
                .TrimStart('/');

            string folderPath = Path.GetDirectoryName(relativePath);

            if (string.IsNullOrEmpty(folderPath))
                return GameName;

            StringBuilder ns = new StringBuilder(GameName);

            foreach (string folder in folderPath.Split(Path.DirectorySeparatorChar, '/'))
            {
                if (string.IsNullOrWhiteSpace(folder))
                    continue;

                ns.Append('.');
                ns.Append(SanitizeIdentifier(folder));
            }

            return ns.ToString();
        }
        
        private static bool HasNamespace(string content)
        {
            return Regex.IsMatch(
                content,
                @"^\s*namespace\s+",
                RegexOptions.Multiline
            );
        }

        private static string SanitizeIdentifier(string value)
        {
            value = Regex.Replace(value, @"[^a-zA-Z0-9_]", "");

            if (!string.IsNullOrEmpty(value) && char.IsDigit(value[0]))
                value = "_" + value;

            return value;
        }

        private static string NormalizePath(string path)
        {
            return path.Replace("\\", "/");
        }
    }
}
#endif
