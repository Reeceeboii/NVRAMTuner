﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NVRAMTuner.Client.Services.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ServiceResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ServiceResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NVRAMTuner.Client.Services.Resources.ServiceResources", typeof(ServiceResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {&quot;nvramver&quot;: {&quot;default&quot;: &quot;RTCONFIG_NVRAM_VER&quot;, &quot;description&quot;: &quot;&quot;}, &quot;restore_defaults&quot;: {&quot;default&quot;: &quot;0&quot;, &quot;description&quot;: &quot;Set to 0 to not restore defaults on boot&quot;}, &quot;sw_mode&quot;: {&quot;default&quot;: &quot;1&quot;, &quot;description&quot;: &quot;big switch for different mode&quot;}, &quot;ASUS_EULA&quot;: {&quot;default&quot;: &quot;0&quot;, &quot;description&quot;: &quot;&quot;}, &quot;preferred_lang&quot;: {&quot;default&quot;: &quot;EN&quot;, &quot;description&quot;: &quot;&quot;}, &quot;wan_ifnames&quot;: {&quot;default&quot;: &quot;0&quot;, &quot;description&quot;: &quot;&quot;}, &quot;lan_ifnames&quot;: {&quot;default&quot;: &quot;vlan0 eth1 eth2 eth3&quot;, &quot;description&quot;: &quot;&quot;}, &quot;lan1_ifnames&quot;: {&quot;default&quot;: &quot; }&quot;, &quot;descrip [rest of string was truncated]&quot;;.
        /// </summary>
        public static string firmware_variable_defaults {
            get {
                return ResourceManager.GetString("firmware_variable_defaults", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to hostname.
        /// </summary>
        public static string HostName_Command {
            get {
                return ResourceManager.GetString("HostName Command", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to nvram show.
        /// </summary>
        public static string NVRAM_Show_Command {
            get {
                return ResourceManager.GetString("NVRAM Show Command", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to uname -o.
        /// </summary>
        public static string Uname_Os_Command {
            get {
                return ResourceManager.GetString("Uname Os Command", resourceCulture);
            }
        }
    }
}
