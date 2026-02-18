using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymlinkCreator.core
{
    public class SymlinkAgent
    {
        #region members

        private readonly List<string> _sourceFileOrFolderList;
        private string _destinationPath;
        private readonly bool _shouldUseRelativePath;
        private readonly bool _shouldRetainScriptFile;
        private readonly bool _replicateToAgentFolders;
        private readonly bool _overwriteExisting;

        private static readonly string[] AgentReplicaSubfolders =
        {
            ".agent\\skills",
            ".agents\\skills",
            ".claude\\skills"
        };

        #endregion


        #region constructor

        public SymlinkAgent(IEnumerable<string> sourceFileOrFolderList, string destinationPath,
            bool shouldUseRelativePath = true, bool shouldRetainScriptFile = false,
            bool replicateToAgentFolders = false, bool overwriteExisting = false)
        {
            this._sourceFileOrFolderList = sourceFileOrFolderList.ToList();
            this._destinationPath = destinationPath;
            this._shouldUseRelativePath = shouldUseRelativePath;
            this._shouldRetainScriptFile = shouldRetainScriptFile;
            this._replicateToAgentFolders = replicateToAgentFolders;
            this._overwriteExisting = overwriteExisting;
        }

        #endregion


        #region methods

        public void CreateSymlinks()
        {
            // Check for destination path
            if (!Directory.Exists(_destinationPath))
            {
                throw new FileNotFoundException("Destination path does not exist", _destinationPath);
            }

            // Normalize base destination path (remove trailing '\\' if present)
            if (_destinationPath[_destinationPath.Length - 1] == '\\')
                _destinationPath = _destinationPath.Substring(0, _destinationPath.Length - 1);

            List<string> destinationPaths = BuildDestinationPaths();

            foreach (string path in destinationPaths)
            {
                // Ensure target subfolders exist before scripting
                Directory.CreateDirectory(path);
            }

            string scriptFileName = ApplicationConfiguration.ApplicationFileName + "_" +
                                    DateTime.Now.Ticks.ToString() + ".cmd";

            ScriptExecutor scriptExecutor = PrepareScriptExecutor(scriptFileName, destinationPaths);
            scriptExecutor.ExecuteAsAdmin();

            if (!_shouldRetainScriptFile)
                File.Delete(scriptFileName);

            if (scriptExecutor.ExitCode != 0)
            {
                throw new ApplicationException("Symlink script exited with an error.\n" + scriptExecutor.StandardError);
            }
        }

        #endregion


        #region helper methods

        private List<string> BuildDestinationPaths()
        {
            if (_replicateToAgentFolders)
            {
                List<string> replicaPaths = new List<string>();
                foreach (string subfolder in AgentReplicaSubfolders)
                {
                    replicaPaths.Add(Path.Combine(_destinationPath, subfolder));
                }
                return replicaPaths;
            }

            return new List<string> { _destinationPath };
        }

        private ScriptExecutor PrepareScriptExecutor(string scriptFileName, IEnumerable<string> destinationPaths)
        {
            ScriptExecutor scriptExecutor = new ScriptExecutor(scriptFileName);

            foreach (string destinationPath in destinationPaths)
            {
                string[] splittedDestinationPath = GetSplittedPath(destinationPath);

                // Switch drive and change directory
                scriptExecutor.WriteLine(splittedDestinationPath[0]);
                scriptExecutor.WriteLine("cd \"" + destinationPath + "\"");

                foreach (string sourceFilePath in _sourceFileOrFolderList)
                {
                    string[] splittedSourceFilePath = GetSplittedPath(sourceFilePath);

                    string commandLineTargetPath = sourceFilePath;
                    if (_shouldUseRelativePath)
                    {
                        // Check if both root drives are same
                        if (splittedSourceFilePath.First() == splittedDestinationPath.First())
                        {
                            commandLineTargetPath = GetRelativePath(splittedDestinationPath, splittedSourceFilePath);
                        }
                    }

                    string targetName = splittedSourceFilePath.Last();
                    string fullTargetPath = Path.Combine(destinationPath, targetName);

                    bool targetExists = Directory.Exists(fullTargetPath) || File.Exists(fullTargetPath);
                    if (targetExists && !_overwriteExisting)
                    {
                        continue;
                    }

                    if (_overwriteExisting)
                    {
                        // Best-effort removal of existing file/dir/symlink before recreating
                        scriptExecutor.WriteLine("if exist \"" + targetName + "\\\" rmdir \"" + targetName + "\" /s /q");
                        scriptExecutor.WriteLine("if exist \"" + targetName + "\" del \"" + targetName + "\" /f /q");
                    }

                    scriptExecutor.Write("mklink ");
                    if (Directory.Exists(sourceFilePath))
                        scriptExecutor.Write("/d ");

                    scriptExecutor.WriteLine("\"" + targetName + "\" " +
                                             "\"" + commandLineTargetPath + "\"");
                }
            }

            return scriptExecutor;
        }

        private string[] GetSplittedPath(string path)
        {
            return path.Split('\\');
        }

        private string GetRelativePath(string[] splittedCurrentPath, string[] splittedTargetPath)
        {
            List<string> splittedCurrentPathList = splittedCurrentPath.ToList();
            List<string> splittedTargetPathList = splittedTargetPath.ToList();

            while (splittedCurrentPathList.Any() && splittedTargetPathList.Any())
            {
                if (splittedCurrentPathList.First() == splittedTargetPathList.First())
                {
                    splittedCurrentPathList.RemoveAt(0);
                    splittedTargetPathList.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }

            StringBuilder relativePathStringBuilder = new StringBuilder();

            for (int i = 0; i < splittedCurrentPathList.Count; i++)
            {
                relativePathStringBuilder.Append("..\\");
            }

            foreach (string splittedPath in splittedTargetPathList)
            {
                relativePathStringBuilder.Append(splittedPath);
                relativePathStringBuilder.Append('\\');
            }

            if (relativePathStringBuilder[relativePathStringBuilder.Length - 1] == '\\')
                relativePathStringBuilder.Length--;

            return relativePathStringBuilder.ToString();
        }

        #endregion
    }
}