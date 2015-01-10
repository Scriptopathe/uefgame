using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geex.Edit.Common.Extensibility
{
    /// <summary>
    /// Interface for the main entry point of the plug-ins.
    /// It describes the way to initialize and handle your plugin.
    /// 
    /// IMPORTANT :
    /// Compatibility project/plugin :
    ///     If a project and a plugin have the same Kind, 
    ///     and the same Compatibility number (in the Version), they are compatible.
    ///     
    /// When you want to create a new plugin compatible with an existing project kind :
    ///     - Ensure your plugin have the same kind and compatibility number as the project.
    ///     - Create your own PluginName.
    /// 
    /// The constructor of the plugin should do nothing.
    /// </summary>
    public interface IModuleDeclaration
    {
        /// <summary>
        /// Starts the module : initializes its components
        /// </summary>
        void InitModule();
        /// <summary>
        /// Disposes the module.
        /// </summary>
        void DisposeModule();
        /// <summary>
        /// Returns true if the plugin is disposed.
        /// </summary>
        bool IsDisposed();
        /// <summary>
        /// Gets the plugin name displayed to the user.
        /// </summary>
        /// <returns></returns>
        string GetModuleName();
        /// <summary>
        /// Gets the plugin kind.
        /// It corresponds to the kind of project this plugin is compatible with.
        /// </summary>
        /// <returns></returns>
        string GetModuleKind();
        /// <summary>
        /// Gets the plugin version displayed to the user.
        /// </summary>
        /// <returns></returns>
        Version GetModuleVersion();
    }
}
