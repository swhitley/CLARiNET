using System.Reflection;
using System.IO;

namespace CLARiNET
{
    public static class ResourceFile
    {
        /// <summary>
        /// Read contents of an embedded resource file
        /// </summary>
        public static string Read(string filename)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            using (var stream = thisAssembly.GetManifestResourceStream(thisAssembly.GetName().Name + ".Resources." + filename))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}