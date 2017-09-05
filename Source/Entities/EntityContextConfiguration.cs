/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using doLittle.Configuration;
using doLittle.Entities;

namespace doLittle.Read.CosmosDB.Documents.Entities
{
    /// <summary>
    /// Implements the <see cref="IEntityContextConfiguration"/> specific for the DocumentDB support
    /// </summary>
    public class EntityContextConfiguration : IEntityContextConfiguration
    {
        /// <summary>
        /// Gets or sets the url endpoint for the database server
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the database id 
        /// </summary>
        public string DatabaseId { get; set; }

        /// <summary>
        /// Gets or sets the authorization key
        /// </summary>
        public string AuthorizationKey { get; set; }


        /// <inheritdoc/>
        public Type EntityContextType { get { return typeof(EntityContext<>); } }

        /// <inheritdoc/>
        public IEntityContextConnection Connection { get; set; }
    }
}
