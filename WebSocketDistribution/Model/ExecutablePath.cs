using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WebSocketDistribution.Model
{
    public static class ExecutablePath
    {
        private static string execFileName;

        private static string execPath;
        /// <summary>
        /// путь к исполняемому файлу без завершающего слэш
        /// </summary>
        public static string ExecPath
        {
            get
            {
                if (string.IsNullOrEmpty(execPath)) SetExecAppProperty();
                return execPath;
            }
        }

        /// <summary>
        /// имя исполняемого файла
        /// </summary>
        public static string ExecFileName
        {
            get
            {
                if (string.IsNullOrEmpty(execFileName)) SetExecAppProperty();
                return execFileName;
            }
        }

        public static string Combine(params object[] pathParts)
        {
            var parts = new List<string> { ExecPath };
            parts.AddRange(pathParts.Select(p => p.ToString()));
            return Path.Combine(parts.ToArray());
        }

        private static void SetExecAppProperty()
        {
            var sm = Assembly.GetEntryAssembly() ??
                             Assembly.GetExecutingAssembly();
            execPath = Path.GetDirectoryName(sm.Location);
            execFileName = Path.GetFileName(sm.Location);
        }

        public static void InitializeFake(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                execPath = directoryName != null ? directoryName.Replace("file:\\", "") : string.Empty;
            }
            else
                execPath = path;
        }

        public static void Unitialize()
        {
            execPath = string.Empty;
        }
    }

}
