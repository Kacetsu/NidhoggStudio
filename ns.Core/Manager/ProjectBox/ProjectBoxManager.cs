using ns.Base.Manager;
using ns.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace ns.Core.Manager.ProjectBox {

    public class ProjectBoxManager : BaseManager {
        public const string EXTENSION_XML = ".xml";
        public const string PROJECTFILE_NAME = "Project";
        public const string PROJECTBOX_NAME = "ProjectBox";

        public string ProjectsDirectory => DocumentsPath + "Projects" + Path.DirectorySeparatorChar;

        public string DefaultProjectDirectory => ProjectsDirectory + "Default" + Path.DirectorySeparatorChar;

        public List<ProjectInfoContainer> ProjectInfos => new List<ProjectInfoContainer>();

        public ProjectBoxConfiguration Configuration { get; private set; } = new ProjectBoxConfiguration();

        public override bool Initialize() {
            bool resultCreateDirectory = false;
            bool resultCreateDefaultProject = true;
            bool resultGenerateInfoList = false;
            string path = ProjectsDirectory;
            resultCreateDirectory = Base.FileInfo.CreateDirectory(path);
            if (Base.FileInfo.IsDirectoryEmpty(path)) {
                if (!Base.FileInfo.CreateDirectory(DefaultProjectDirectory + "Images"))
                    resultCreateDefaultProject = false;
                else {
                    resultCreateDefaultProject = CopyDefaultProject();
                }
            } else if (!File.Exists(DefaultProjectDirectory + PROJECTFILE_NAME + EXTENSION_XML)) {
                resultCreateDefaultProject = CopyDefaultProject();
            }

            if (!File.Exists(ProjectsDirectory + PROJECTBOX_NAME + EXTENSION_XML) && resultCreateDefaultProject) {
                resultCreateDefaultProject = SaveDefaultProjectBoxConfiguration();
            }

            Configuration = Load(ProjectsDirectory + PROJECTBOX_NAME + EXTENSION_XML) as ProjectBoxConfiguration;

            resultGenerateInfoList = GenerateInfoList();

            return resultCreateDirectory && resultCreateDefaultProject && resultGenerateInfoList;
        }

        public override object Load(FileStream stream) {
            try {
                object obj;
                XmlSerializer serializer = new XmlSerializer(typeof(ProjectBoxConfiguration));
                obj = serializer.Deserialize(stream);
                return obj;
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return null;
            }
        }

        public override bool Save(ref MemoryStream stream) {
            try {
                XmlSerializer serializer = new XmlSerializer(Configuration.GetType());
                serializer.Serialize(stream, Configuration);
                stream.Flush();
                return true;
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return false;
            }
        }

        public override bool Save() {
            return Save(ProjectsDirectory + PROJECTBOX_NAME + EXTENSION_XML);
        }

        public bool SaveProject() {
            string path = Configuration.LastUsedProjectPath;
            bool wasDefault = false;
            ProjectManager projectManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;

            if (path.Equals(DefaultProjectDirectory + PROJECTFILE_NAME + EXTENSION_XML)) {
                if (projectManager.Load(path) == null)
                    return false;

                path = ProjectsDirectory + PROJECTFILE_NAME + "_" + DateTime.Now.ToFileTime().ToString() + Path.DirectorySeparatorChar + PROJECTFILE_NAME + EXTENSION_XML;
                wasDefault = true;
            }

            if (projectManager.Save(path)) {
                if (wasDefault) {
                    if (projectManager.Load(path) == null) {
                        return false;
                    } else {
                        Base.FileInfo.CopyDirectory(DefaultProjectDirectory + "Images", Path.GetDirectoryName(path) + Path.DirectorySeparatorChar + "Images");
                    }
                }

                Configuration.LastUsedProjectPath = path;
                Save();
                GenerateInfoList();
                return true;
            }

            return false;
        }

        public bool LoadProject(string path) {
            ProjectManager projectManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;
            if (projectManager.Load(path) != null) {
                SetUsedProject(path);
                return true;
            } else {
                Base.Log.Trace.WriteLine("Could not load project!", TraceEventType.Error);
                return false;
            }
        }

        public bool CreateNewProject() {
            return SaveDefaultProjectBoxConfiguration();
        }

        private bool SaveDefaultProjectBoxConfiguration() {
            Configuration.LastUsedProjectPath = DefaultProjectDirectory + PROJECTFILE_NAME + EXTENSION_XML;
            return SaveProject();
        }

        private bool CopyDefaultProject() {
            string assemblyPath = AssemblyPath + Path.DirectorySeparatorChar;
            string defaultProjectPathSource = assemblyPath + "DefaultProject" + EXTENSION_XML;
            if (Base.FileInfo.CopyFile(defaultProjectPathSource, DefaultProjectDirectory + PROJECTFILE_NAME + EXTENSION_XML)) {
                return SaveDefaultProjectBoxConfiguration();
            }
            return false;
        }

        private void SetUsedProject(string path) {
            foreach (ProjectInfoContainer container in ProjectInfos) {
                if (container.Path.Equals(path))
                    container.IsUsed = true;
                else
                    container.IsUsed = false;
            }
        }

        private bool GenerateInfoList() {
            try {
                ProjectInfos.Clear();
                string[] directories = Base.FileInfo.GetDirectories(ProjectsDirectory);
                ProjectManager dummyManager = new ProjectManager();
                foreach (string directory in directories) {
                    string projectFilePath = directory + Path.DirectorySeparatorChar + PROJECTFILE_NAME + EXTENSION_XML;
                    if (File.Exists(projectFilePath) && !Path.GetDirectoryName(projectFilePath).Equals(Path.GetDirectoryName(DefaultProjectDirectory))) {
                        ProjectManager tmpManager = dummyManager.LoadManager(projectFilePath);
                        string projectName = tmpManager.Configuration.Name.Value as string;
                        ProjectInfoContainer container = new ProjectInfoContainer(projectFilePath, projectName);
                        container.PropertyChanged += Container_PropertyChanged;
                        if (container.Path.Equals(Configuration.LastUsedProjectPath)) container.IsUsed = true;

                        ProjectInfos.Add(container);
                    }
                }
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, TraceEventType.Error);
                return false;
            }
            return true;
        }

        private void Container_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals("Name")) {
                ProjectInfoContainer container = sender as ProjectInfoContainer;
                ProjectManager dummyManager = new ProjectManager();
                ProjectManager tmpManager = dummyManager.LoadManager(container.Path);
                tmpManager.Configuration.Name.Value = container.Name;
                tmpManager.Save(container.Path);
            }
        }
    }
}