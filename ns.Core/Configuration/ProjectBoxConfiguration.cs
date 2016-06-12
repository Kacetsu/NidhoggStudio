using ns.Base.Configuration;

namespace ns.Core.Configuration {

    public class ProjectBoxConfiguration : BaseConfiguration {
        private string _lastUsedProjectPath = string.Empty;

        public string LastUsedProjectPath {
            get { return _lastUsedProjectPath; }
            set { _lastUsedProjectPath = value; }
        }
    }
}