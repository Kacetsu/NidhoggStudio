using ns.Base.Manager;
using ns.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace ns.Core.Manager.ProjectBox {

    public class ProjectBoxManager : GenericConfigurationManager<ProjectBoxConfiguration> {
        public const string EXTENSION_XML = ".xml";
        public const string PROJECTBOX_NAME = "ProjectBox";
        public const string PROJECTFILE_NAME = "Project";
        public string DefaultProjectDirectory => ProjectsDirectory + "Default" + Path.DirectorySeparatorChar;
        public List<ProjectInfoContainer> ProjectInfos => new List<ProjectInfoContainer>();
        public string ProjectsDirectory => DocumentsPath + "Projects" + Path.DirectorySeparatorChar;

        public bool CreateNewProject() {
            return SaveDefaultProjectBoxConfiguration();
        }

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

            if (Load(ProjectsDirectory + PROJECTBOX_NAME + EXTENSION_XML) == false) {
                throw new FileLoadException(nameof(ProjectBoxConfiguration));
            }

            resultGenerateInfoList = GenerateInfoList();

            return resultCreateDirectory && resultCreateDefaultProject && resultGenerateInfoList;
        }

        public override ProjectBoxConfiguration Load(Stream stream) {
            try {
                ProjectBoxConfiguration obj;
                XmlSerializer serializer = new XmlSerializer(typeof(ProjectBoxConfiguration));
                obj = serializer.Deserialize(stream) as ProjectBoxConfiguration;
                return obj;
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return null;
            }
        }

        public bool LoadProject(string path) {
            ProjectManager projectManager = CoreSystem.FindManager<ProjectManager>();
            if (Load(path)) {
                SetUsedProject(path);
                return true;
            } else {
                Base.Log.Trace.WriteLine("Could not load project!", TraceEventType.Error);
                return false;
            }
        }

        public override bool Save() {
            return Save(ProjectsDirectory + PROJECTBOX_NAME + EXTENSION_XML);
        }

        public bool SaveProject() {
            string path = Configuration.LastUsedProjectPath;
            bool wasDefault = false;
            ProjectManager projectManager = CoreSystem.FindManager<ProjectManager>();

            if (path.Equals(DefaultProjectDirectory + PROJECTFILE_NAME + EXTENSION_XML)) {
                if (!Load(path))
                    return false;

                path = ProjectsDirectory + PROJECTFILE_NAME + "_" + DateTime.Now.ToFileTime().ToString() + Path.DirectorySeparatorChar + PROJECTFILE_NAME + EXTENSION_XML;
                wasDefault = true;
            }

            if (projectManager.Save(path)) {
                if (wasDefault) {
                    if (!Load(path)) {
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

        private void Container_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals("Name")) {
                ProjectInfoContainer container = sender as ProjectInfoContainer;
                ProjectManager dummyManager = new ProjectManager();
                // ToDo: Refactor!ProjectConfiguration
                //ProjectManager tmpManager = dummyManager.LoadManager(container.Path);
                //tmpManager.Configuration.ProjectName.Value = container.Name;
                //tmpManager.Save(container.Path);
            }
        }

        private bool CopyDefaultProject() {
            string assemblyPath = AssemblyPath + Path.DirectorySeparatorChar;
            string defaultProjectPathSource = assemblyPath + "DefaultProject" + EXTENSION_XML;
            if (Base.FileInfo.CopyFile(defaultProjectPathSource, DefaultProjectDirectory + PROJECTFILE_NAME + EXTENSION_XML)) {
                return SaveDefaultProjectBoxConfiguration();
            }
            return false;
        }

        private bool GenerateInfoList() {
            try {
                ProjectInfos.Clear();
                string[] directories = Base.FileInfo.GetDirectories(ProjectsDirectory);
                ProjectManager dummyManager = new ProjectManager();
                foreach (string directory in directories) {
                    string projectFilePath = directory + Path.DirectorySeparatorChar + PROJECTFILE_NAME + EXTENSION_XML;
                    if (File.Exists(projectFilePath) && !Path.GetDirectoryName(projectFilePath).Equals(Path.GetDirectoryName(DefaultProjectDirectory))) {
                        // ToDo: Refactor!ProjectConfiguration
                        //ProjectManager tmpManager = dummyManager.LoadManager(projectFilePath);
                        //string projectName = tmpManager.Configuration.ProjectName.Value as string;
                        //ProjectInfoContainer container = new ProjectInfoContainer(projectFilePath, projectName);
                        //container.PropertyChanged += Container_PropertyChanged;
                        //if (container.Path.Equals(Configuration.LastUsedProjectPath)) container.IsUsed = true;

                        //ProjectInfos.Add(container);
                    }
                }
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, TraceEventType.Error);
                return false;
            }
            return true;
        }

        private bool SaveDefaultProjectBoxConfiguration() {
            Configuration.LastUsedProjectPath = DefaultProjectDirectory + PROJECTFILE_NAME + EXTENSION_XML;
            return SaveProject();
        }

        private void SetUsedProject(string path) {
            foreach (ProjectInfoContainer container in ProjectInfos) {
                if (container.Path.Equals(path))
                    container.IsUsed = true;
                else
                    container.IsUsed = false;
            }
        }
    }
}