﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuickSharp.Language.Mono {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("QuickSharp.Language.Mono.Resources", typeof(Resources).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Microsoft C# Compiler.
        /// </summary>
        internal static string MicrosoftCSLineParser {
            get {
                return ResourceManager.GetString("MicrosoftCSLineParser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mono C# Compiler (dmcs).
        /// </summary>
        internal static string MonoDMCSCompiler {
            get {
                return ResourceManager.GetString("MonoDMCSCompiler", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mono C# Compiler (gmcs).
        /// </summary>
        internal static string MonoGMCSCompiler {
            get {
                return ResourceManager.GetString("MonoGMCSCompiler", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mono C# Compiler (mcs).
        /// </summary>
        internal static string MonoMCSCompiler {
            get {
                return ResourceManager.GetString("MonoMCSCompiler", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mono Runtime (mono.exe).
        /// </summary>
        internal static string MonoRuntime {
            get {
                return ResourceManager.GetString("MonoRuntime", resourceCulture);
            }
        }
    }
}
