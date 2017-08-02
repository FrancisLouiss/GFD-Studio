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
    public static class ModuleExportUtillities
    {
        /// <summary>
        /// Tries to create a stream containing exported data for a given resource.
        /// </summary>
        /// <param name="resource">Resource to export.</param>
        /// <param name="stream">Stream containing exported data if successful. If not successful, then this will be null.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public static bool TryCreateStream( object resource, out Stream stream )
        {
            var type = resource.GetType();
            if ( !ModuleRegistry.ModuleByType.TryGetValue( type, out var module ) )
            {
                stream = null;
                return false;
            }

            // write to memory stream
            stream = new MemoryStream();
            module.Export( resource, stream );

            return true;
        }

        /// <summary>
        /// Creates a stream containing exported data for a given resource.
        /// </summary>
        /// <param name="resource">Resource to export.</param>
        /// <returns>Stream containing the resource's exported data.</returns>
        public static Stream CreateStream( object resource )
        {
            if ( !TryCreateStream( resource, out var stream ))
            {
                throw new ArgumentException( "Failed to export resource to stream" );
            }

            return stream;
        }
    }
}
