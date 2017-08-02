﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdEditor.Modules
{
    /// <summary>
    /// Utilities for importing files using the module system.
    /// </summary>
    public static class ModuleImportUtillities
    {
        /// <summary>
        /// Gets a list of modules with the given extension.
        /// </summary>
        /// <param name="extension">The extension to match.</param>
        /// <returns>A list of modules with the given extension.</returns>
        public static List<IModule> GetModulesWithExtension( string extension )
        {
            extension = extension.TrimStart( '.' );
            return ModuleRegistry.Modules.Where( x => x.Extensions.Contains( extension, StringComparer.InvariantCultureIgnoreCase ) ).ToList();
        }

        /// <summary>
        /// Attemps to get the appropriate module for importing the specified file.
        /// </summary>
        /// <param name="filepath">The file to import.</param>
        /// <param name="module">The out parameter containing the found module, if none are found then it will be null.</param>
        /// <returns>Whether or not a module was found.</returns>
        public static bool TryGetModuleForImport( string filepath, out IModule module )
        {
            using ( var stream = File.OpenRead( filepath ) )
            {
                return TryGetModuleForImport( stream, out module, Path.GetFileName( filepath ) );
            }
        }

        /// <summary>
        /// Attemps to get the appropriate module for importing the specified data.
        /// </summary>
        /// <param name="data">The data to import.</param>
        /// <param name="module">The out parameter containing the found module, if none are found then it will be null.</param>
        /// <param name="filename">Optional filename parameter. Might be required by some modules.</param>
        /// <returns>Whether or not a module was found.</returns>
        public static bool TryGetModuleForImport( byte[] data, out IModule module, string filename = null )
        {
            using ( var stream = new MemoryStream( data ) )
            {
                return TryGetModuleForImport( stream, out module, filename );
            }
        }

        /// <summary>
        /// Attemps to get the appropriate module for importing the specified stream data.
        /// </summary>
        /// <param name="stream">The stream containing data to import.</param>
        /// <param name="module">The out parameter containing the found module, if none are found then it will be null.</param>
        /// <param name="filename">Optional filename parameter. Might be required by some modules.</param>
        /// <returns>Whether or not a module was found.</returns>
        public static bool TryGetModuleForImport( Stream stream, out IModule module, string filename = null )
        {
            // try to find a module that can import this file
            module = ModuleRegistry.Modules.SingleOrDefault( x => x.CanImport( stream, filename ) );

            // simplicity is nice sometimes c:
            return module != null;
        }
    }
}
