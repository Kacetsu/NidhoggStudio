using ns.Base.Manager;
using ns.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ns.Core.Manager.ProjectBox {

    public class ProjectBoxManager : GenericConfigurationManager<ProjectBoxConfiguration> {
        public const string EXTENSION_XML = ".xml";
        public const string PROJECTBOX_NAME = "ProjectBox";
        public const string PROJECTFILE_NAME = "Project";

        /// <summary>
        /// Gets the default project directory.
        /// </summary>
        /// <value>
        /// The default project directory.
        /// </value>
        public string DefaultProjectDirectory => ProjectsDirectory + "Default" + Path.DirectorySeparatorChar;

        /// <summary>
        /// Gets the project infos.
        /// </summary>
        /// <value>
        /// The project infos.
        /// </value>
        public List<ProjectInfoContainer> ProjectInfos { get; private set; } = new List<ProjectInfoContainer>();

        /// <summary>
        /// Gets the projects directory.
        /// </summary>
        /// <value>
        /// The projects directory.
        /// </value>
        public string ProjectsDirectory => DocumentsPath + "Projects" + Path.DirectorySeparatorChar;

        /// <summary>
        /// Creates the new project.
        /// </summary>
        /// <returns></returns>
        public bool CreateNewProject() {
            return SaveDefaultProjectBoxConfiguration();
        }

        /// <summary>
        /// Initialize the instance of the manager.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.IO.FileLoadException"></exception>
        public override bool Initialize() {
            Configuration = new ProjectBoxConfiguration();
            bool resultCreateDirectory = false;
            bool resultCreateDefaultProject = true;
            bool resultGenerateInfoList = false;
            string path = ProjectsDirectory;
            resultCreateDirectory = Base.FileInfo.CreateDirectory(path);
            if (Base.FileInfo.IsDirectoryEmpty(path)) {
                if (!Base.FileInfo.CreateDirectory(DefaultProjectDirectory + "Images"))
                    resultCreateDefaultProject = false;
                else {
                    resultCreateDefaultProject = SaveDefaultProject();
                }
            } else if (!File.Exists(DefaultProjectDirectory + PROJECTFILE_NAME + EXTENSION_XML)) {
                resultCreateDefaultProject = SaveDefaultProject();
            }

            if (!File.Exists(ProjectsDirectory + PROJECTBOX_NAME + EXTENSION_XML) && resultCreateDefaultProject) {
                resultCreateDefaultProject = SaveDefaultProjectBoxConfiguration();
            }

            if (Load(ProjectsDirectory + PROJECTBOX_NAME + EXTENSION_XML) == false) {
                throw new FileLoadException(nameof(ProjectBoxConfiguration));
            }

            resultGenerateInfoList = GenerateInfoList();

            return resultCreateDirectory && resultCreateDefaultProject && resultGenerateInfoList && LoadProject(Configuration.LastUsedProjectPath);
        }

        /// <summary>
        /// Loads the project.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public bool LoadProject(string path) {
            ProjectManager projectManager = CoreSystem.FindManager<ProjectManager>();
            if (projectManager.Load(path)) {
                SetUsedProject(path);
                return true;
            } else {
                Base.Log.Trace.WriteLine("Could not load project!", TraceEventType.Error);
                return false;
            }
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Save() {
            return Save(ProjectsDirectory + PROJECTBOX_NAME + EXTENSION_XML);
        }

        /// <summary>
        /// Saves the project.
        /// </summary>
        /// <returns></returns>
        public bool SaveProject() {
            string path = Configuration.LastUsedProjectPath;
            bool wasDefault = false;
            ProjectManager projectManager = CoreSystem.FindManager<ProjectManager>();

            if (path.Equals(DefaultProjectDirectory + PROJECTFILE_NAME + EXTENSION_XML)) {
                path = ProjectsDirectory + PROJECTFILE_NAME + "_" + DateTime.Now.ToFileTime().ToString() + Path.DirectorySeparatorChar + PROJECTFILE_NAME + EXTENSION_XML;
                wasDefault = true;
            }

            if (projectManager.Save(path)) {
                if (wasDefault) {
                    if (!projectManager.Load(path)) {
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

        private bool GenerateInfoList() {
            try {
                ProjectInfos.Clear();
                string[] directories = Base.FileInfo.GetDirectories(ProjectsDirectory);
                ProjectManager dummyManager = new ProjectManager();
                foreach (string directory in directories) {
                    string projectFilePath = directory + Path.DirectorySeparatorChar + PROJECTFILE_NAME + EXTENSION_XML;
                    if (File.Exists(projectFilePath) && !Path.GetDirectoryName(projectFilePath).Equals(Path.GetDirectoryName(DefaultProjectDirectory))) {
                        dummyManager.Load(projectFilePath);
                        ProjectConfiguration tmpConfiguration = dummyManager.Configuration;
                        string projectName = tmpConfiguration.FileName.Value;
                        ProjectInfoContainer container = new ProjectInfoContainer(projectFilePath, projectName);
                        container.PropertyChanged += Container_PropertyChanged;
                        if (container.Path.Equals(Configuration.LastUsedProjectPath)) {
                            container.IsUsed = true;
                        }

                        ProjectInfos.Add(container);
                    }
                }
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, TraceEventType.Error);
                return false;
            }
            return true;
        }

        private bool SaveDefaultProject() {
            ProjectManager projectManager = CoreSystem.FindManager<ProjectManager>();
            projectManager.CreateDefaultProject();
            return projectManager.Save(DefaultProjectDirectory + PROJECTFILE_NAME + EXTENSION_XML);
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