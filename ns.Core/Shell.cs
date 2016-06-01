using ns.Base.Log;
using ns.Base.Manager;
using ns.Core.Manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;

namespace ns.Core {

    /// <summary>
    /// Shell to execute commands from commandline.
    /// </summary>
    public class Shell {
        private List<string> _commands = new List<string>();

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        public List<string> Command { get { return _commands; } }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns>Success of operation.</returns>
        public bool Initialize() {
            string shellCommandsFile = BaseManager.AssemblyPath + Path.DirectorySeparatorChar + "ShellCommands.xml";

            Type coreSystemType = typeof(CoreSystem);
            Type processorType = typeof(Processor);
            Type shellType = typeof(Shell);

            FillCommandListByType(coreSystemType);
            FillCommandListByType(processorType);
            FillCommandListByType(shellType);

            foreach (BaseManager manager in CoreSystem.Managers) {
                Type managerType = manager.GetType();
                FillCommandListByType(managerType);
            }

#if DEBUG
            using (FileStream stream = new FileStream(BaseManager.AssemblyPath + Path.DirectorySeparatorChar + "ShellCommands.xml", FileMode.Create)) {
                XmlSerializer serializer = new XmlSerializer(_commands.GetType());
                serializer.Serialize(stream, _commands);
            }
#endif

            return true;
        }

        /// <summary>
        /// Helps this instance.
        /// </summary>
        public void Help() {
            string shellHelp = "Shell syntax: METHOD_NAME[TYPE PARAMETER]";
            Base.Log.Trace.WriteLine(shellHelp, TraceEventType.Information);
        }

        /// <summary>
        /// Sleeps the specified milliseconds.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        public void Sleep(int milliseconds) {
            Thread.Sleep(milliseconds);
        }

        /// <summary>
        /// Gets the autocomplete commands.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>List of all matching commands.</returns>
        public List<string> GetAutocompleteCommands(string command) {
            List<string> result = _commands.FindAll(s => s.StartsWith(command));
            if (result == null)
                result = new List<string>();

            return result;
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Success of operation.</returns>
        public bool Execute(string command) {
            try {
                Type type = null;
                object returnObj = null;

                if (command.StartsWith("CoreSystem.")) {
                    type = typeof(CoreSystem);
                } else if (command.Contains("Processor.")) {
                    type = typeof(Processor);
                } else if (command.Contains("ProjectManager.")) {
                    type = typeof(ProjectManager);
                } else if (command.Contains("Shell.")) {
                    type = typeof(Shell);
                } else {
                    return false;
                }

                string methodStr = GetMethodFromCommand(command);
                string[] parameters = GetParametersFromCommand(command);
                Type[] parameterTypes = GetTypesFromCommand(command);

                MethodInfo methodInfo = null;
                if (parameterTypes != null) methodInfo = type.GetMethod(methodStr, parameterTypes);
                else methodInfo = type.GetMethod(methodStr);

                object invokeObj = null;

                if (type == typeof(CoreSystem)) {
                    invokeObj = CoreSystem.Instance;
                } else if (type == typeof(Processor)) {
                    invokeObj = CoreSystem.Processor;
                } else if (type == typeof(Shell)) {
                    invokeObj = CoreSystem.Shell;
                } else if (type.Name.Contains("Manager")) {
                    BaseManager manager = CoreSystem.Managers.Find(m => m.Name.Contains(type.Name)) as BaseManager;
                    if (manager != null) {
                        invokeObj = manager;
                    }
                }

                object[] paramterObjects = null;

                if (parameters != null && parameters.Length > 0) {
                    paramterObjects = new object[parameters.Length];

                    for (int index = 0; index < parameters.Length; index++) {
                        if (parameterTypes[index] == typeof(string))
                            paramterObjects[index] = parameters[index];
                        else if (parameterTypes[index] == typeof(int))
                            paramterObjects[index] = Convert.ToInt32(parameters[index]);
                        else if (parameterTypes[index] == typeof(bool))
                            paramterObjects[index] = Convert.ToBoolean(parameters[index]);
                    }
                }

                returnObj = methodInfo.Invoke(invokeObj, paramterObjects);
                string returnStr = string.Empty;

                if (returnObj == null) {
                    returnStr = "null";
                } else if (returnObj.GetType() == typeof(List<string>)) {
                    foreach (string str in returnObj as List<string>) {
                        returnStr += str + "\n";
                    }
                } else {
                    returnStr = returnObj.ToString();
                }

                Base.Log.Trace.WriteLine("Shell command: " + command + " == " + returnStr, TraceEventType.Information);

                return true;
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return false;
            }
        }

        /// <summary>
        /// Gets the method from command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Method name form command.</returns>
        private string GetMethodFromCommand(string command) {
            string value = string.Empty;
            if (command.Contains("[") || command.Contains("]")) {
                int paramIndex = command.IndexOf('[');
                int index = -1;
                if (paramIndex > -1)
                    index = command.LastIndexOf('.', paramIndex, paramIndex);
                else
                    index = command.LastIndexOf('.');
                value = command.Remove(0, index + 1);
                index = value.IndexOf('[');
                value = value.Remove(index);
            } else {
                int index = command.LastIndexOf('.');
                if (index > -1)
                    value = command.Remove(0, index + 1);
                else
                    value = command;
            }
            return value;
        }

        /// <summary>
        /// Gets the parameters from command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Parameters from command.</returns>
        private string[] GetParametersFromCommand(string command) {
            if (command.Contains("[") && command.Contains("]")) {
                int startIndex = command.IndexOf('[');
                string parameterArea = command.Remove(0, startIndex + 1);
                int endIndex = parameterArea.LastIndexOf(']');
                parameterArea = parameterArea.Remove(endIndex);

                if (parameterArea.StartsWith(@"\"))
                    parameterArea = parameterArea.Remove(0, 1);

                string[] pairs = parameterArea.Split(new char[] { ' ' });
                string[] result = new string[pairs.Length / 2];

                for (int index = 0, count = 0; index < pairs.Length; index++) {
                    if (index % 2 != 0) {
                        string str = pairs[index].Trim();
                        if (str.StartsWith(@"\"))
                            result[count] = str.Remove(0, 1);
                        else
                            result[count] = str;
                        count++;
                    }
                }

                return result;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Gets the types from command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Parameter types from command.</returns>
        private Type[] GetTypesFromCommand(string command) {
            if (command.Contains("[") && command.Contains("]")) {
                int startIndex = command.IndexOf('[');
                string parameterArea = command.Remove(0, startIndex + 1);
                int endIndex = parameterArea.LastIndexOf(']');
                parameterArea = parameterArea.Remove(endIndex);

                string[] pairs = parameterArea.Split(new char[] { ' ' });

                string[] resultStr = new string[pairs.Length / 2];
                List<Type> types = new List<Type>();

                for (int index = 0; index < pairs.Length; index++) {
                    if (index % 2 == 0) {
                        if (pairs[index] == "string" || pairs[index] == "String")
                            types.Add(typeof(string));
                        else if (pairs[index] == "int" || pairs[index] == "int32" || pairs[index] == "int64" || pairs[index] == "Int" || pairs[index] == "Int32" || pairs[index] == "Int64")
                            types.Add(typeof(int));
                        else if (pairs[index] == "bool" || pairs[index] == "Bool" || pairs[index] == "Boolean")
                            types.Add(typeof(bool));
                    }
                }

                return types.ToArray();
            } else {
                return null;
            }
        }

        /// <summary>
        /// Checks for supported parameters.
        /// </summary>
        /// <param name="infos">The infos.</param>
        /// <returns>True if supported.</returns>
        private bool CheckForSupportedParameters(ParameterInfo[] infos) {
            foreach (ParameterInfo info in infos) {
                if (info.ParameterType == typeof(string)
                    || info.ParameterType == typeof(int)
                    || info.ParameterType == typeof(bool)) {
                    continue;
                } else {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Fills the type of the command list by.
        /// </summary>
        /// <param name="type">The type.</param>
        private void FillCommandListByType(Type type) {
            MethodInfo[] mis = type.GetMethods();
            foreach (MethodInfo mi in mis) {
                ParameterInfo[] pis = mi.GetParameters();
                if (!CheckForSupportedParameters(pis)) continue;
                string parameters = "[";
                foreach (ParameterInfo pi in pis)
                    parameters += pi.ParameterType.Name + " " + pi.Name;
                parameters += "]";
                if (pis.Length > 0) {
                    _commands.Add(type.Name.ToString() + "." + mi.Name.ToString() + parameters);
                } else {
                    _commands.Add(type.Name.ToString() + "." + mi.Name.ToString());
                }
            }
        }

        /// <summary>
        /// Fills the type of the command list by.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parent">The parent.</param>
        private void FillCommandListByType(Type type, string parent) {
            MethodInfo[] mis = type.GetMethods();
            foreach (MethodInfo mi in mis) {
                ParameterInfo[] pis = mi.GetParameters();
                if (!CheckForSupportedParameters(pis)) continue;
                string parameters = "[";
                foreach (ParameterInfo pi in pis)
                    parameters += pi.ParameterType.Name + " " + pi.Name;
                parameters += "]";
                if (pis.Length > 0) {
                    _commands.Add(parent + "." + type.Name.ToString() + "." + mi.Name.ToString() + parameters);
                } else {
                    _commands.Add(parent + "." + type.Name.ToString() + "." + mi.Name.ToString());
                }
            }
        }
    }
}