using ns.Base;
using ns.Base.Log;
using ns.Base.Manager;
using ns.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ns.Core.Manager.ProjectBox
{
    public class ProjectBoxManager : BaseManager {
        public const string EXTENSION_XML = ".xml";
        public const string PROJECTFILE_NAME = "Project";
        public const string PROJECTBOX_NAME = "ProjectBox";

        private List<ProjectInfoContainer> _projectInfos = new List<ProjectInfoContainer>();
        private ProjectBoxConfiguration _configuration = new ProjectBoxConfiguration();

        public string ProjectsDirectory {
            get { return DocumentsPath + "Projects" + Path.DirectorySeparatorChar; }
        }

        public string DefaultProjectDirectory {
            get { return ProjectsDirectory + "Default" + Path.DirectorySeparatorChar; }
        }

        public List<ProjectInfoContainer> ProjectInfos {
            get { return _projectInfos; }
        }

        public ProjectBoxConfiguration Configuration {
            get { return _configuration; }
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

            if(!File.Exists(ProjectsDirectory + PROJECTBOX_NAME + EXTENSION_XML) && resultCreateDefaultProject) {
                resultCreateDefaultProject = SaveDefaultProjectBoxConfiguration();
            }

            _configuration = Load(ProjectsDirectory + PROJECTBOX_NAME + EXTENSION_XML) as ProjectBoxConfiguration;

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
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                return null;
            }
        }

        public override bool Save(ref MemoryStream stream) {
            try {
                XmlSerializer serializer = new XmlSerializer(_configuration.GetType());
                serializer.Serialize(stream, _configuration);
                stream.Flush();
                return true;
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                return false;
            }
        }

        public override bool Save() {
            return Save(ProjectsDirectory + PROJECTBOX_NAME + EXTENSION_XML);
        }

        public bool SaveProject() {
            string path = _configuration.LastUsedProjectPath;
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

                _configuration.LastUsedProjectPath = path;
                Save();
                GenerateInfoList();
                return true;
            }

            return false;
        }

        private bool SaveDefaultProjectBoxConfiguration() {
            _configuration.LastUsedProjectPath = DefaultProjectDirectory + PROJECTFILE_NAME + EXTENSION_XML;
            return Save() && SaveProject();
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
                _projectInfos.Clear();
                string[] directories = Base.FileInfo.GetDirectories(ProjectsDirectory);
                ProjectManager dummyManager = new ProjectManager();
                foreach (string directory in directories) {
                    string projectFilePath = directory + Path.DirectorySeparatorChar + PROJECTFILE_NAME + EXTENSION_XML;
                    if (File.Exists(projectFilePath) && !Path.GetDirectoryName(projectFilePath).Equals(Path.GetDirectoryName(DefaultProjectDirectory))) {
                        ProjectManager tmpManager = dummyManager.LoadManager(projectFilePath);
                        string projectName = tmpManager.Configuration.Name.Value as string;
                        ProjectInfoContainer container = new ProjectInfoContainer(projectFilePath, projectName);
                        container.PropertyChanged += Container_PropertyChanged;
                        if (container.Path.Equals(_configuration.LastUsedProjectPath))
                            container.IsUsed = true;
                        _projectInfos.Add(container);
                    }
                }
            } catch(Exception ex) {
                Trace.WriteLine(ex.Message, LogCategory.Error);
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
