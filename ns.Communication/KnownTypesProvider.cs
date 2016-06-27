﻿using ns.Core;
using ns.Core.Manager;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ns.Communication {

    internal class KnownTypesProvider {

        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider) {
            PluginManager pluginManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(PluginManager))) as PluginManager;
            if (pluginManager?.KnownTypes.Count > 0) {
                return pluginManager?.KnownTypes;
            } else {
                return new List<Type>();
            }
        }
    }
}