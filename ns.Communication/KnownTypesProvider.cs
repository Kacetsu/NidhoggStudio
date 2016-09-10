using ns.Base;
using ns.Core;
using ns.Core.Manager;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ns.Communication {

    internal class KnownTypesProvider {

        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider) {
            List<Type> result = new List<Type>();
            PluginManager pluginManager = CoreSystem.FindManager<PluginManager>();
            if (pluginManager?.KnownTypes.Count > 0) {
                result = new List<Type>(pluginManager?.KnownTypes);
            }

            result.Add(typeof(Rectangle));

            return result;
        }
    }
}