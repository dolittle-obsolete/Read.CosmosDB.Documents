/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Linq;
using Microsoft.Azure.Documents;

namespace doLittle.Read.CosmosDB.Documents
{
    /// <summary>
    /// Represents an implementation of <see cref="ICollectionStrategy"/> that 
    /// deals with entities sitting in one collection called Entities
    /// </summary>
    public class MultipleEntitiesInOneCollection : ICollectionStrategy
    {
        const string _collectionName = "Entities";

        /// <inheritdoc/>
        public string CollectionNameFor<T>()
        {
            return _collectionName;
        }

        /// <inheritdoc/>
        public string CollectionNameFor(Type type)
        {
            return _collectionName;
        }

        /// <inheritdoc/>
        public IQueryable<T> HandleQueryableFor<T>(IQueryable<T> queryable)
        {
            var documentType = typeof(T).Name;
            return queryable.DocumentType(documentType);
        }

        /// <inheritdoc/>
        public void HandleDocumentFor<T>(Document document)
        {
            var documentType = typeof(T).Name;
            document.SetPropertyValue("_DOCUMENT_TYPE", documentType);
        }
    }
}
