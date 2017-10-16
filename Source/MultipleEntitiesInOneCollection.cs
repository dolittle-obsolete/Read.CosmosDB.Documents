/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Linq;
using doLittle.Read.DocumentDB.Entities;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace doLittle.Read.DocumentDB
{
    /// <summary>
    /// Represents an implementation of <see cref="ICollectionStrategy"/> that 
    /// deals with entities sitting in one collection called Entities
    /// </summary>
    public class MultipleEntitiesInOneCollection : ICollectionStrategy
    {
        static string _partitionKey = $"{EntityContextConfiguration.DocumentTypeProperty}";
        
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
            document.SetPropertyValue(EntityContextConfiguration.DocumentTypeProperty, documentType);
        }

        /// <inheritdoc/>
        public void ConfigureCollectionForCreation(DocumentCollection collection)
        {
            collection.PartitionKey.Paths.Add(_partitionKey);
        }

        /// <inheritdoc/>
        public void HandleFeedOptionsFor<T>(FeedOptions feedOptions)
        {
            feedOptions.PartitionKey = new PartitionKey(typeof(T).Name);
        }

        /// <inheritdoc/>
        public void HandleRequestOptionsFor<T>(RequestOptions requestOptions)
        {
            requestOptions.PartitionKey = new PartitionKey(typeof(T).Name);
        }
    }
}
