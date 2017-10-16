﻿/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace doLittle.Read.DocumentDB
{
    /// <summary>
    /// Defines a strategy to be used for dealing with entities and their relation to collections
    /// </summary>
    public interface ICollectionStrategy
    {
        /// <summary>
        /// Gets the name of the collection based on the type
        /// </summary>
        /// <typeparam name="T">Type to get collection name for</typeparam>
        /// <returns>Name of the collection</returns>
        string CollectionNameFor<T>();

        /// <summary>
        /// Gets the name of the collection based on the type
        /// </summary>
        /// <param name="type">Type to get collection name for</param>
        /// <returns>Name of the collection</returns>
        string CollectionNameFor(Type type);

        /// <summary>
        /// Handle the <see cref="FeedOptions"/> typically used when querying
        /// </summary>
        /// <typeparam name="T">Type to handle queryable for</typeparam>
        /// <param name="feedOptions"></param>
        void HandleFeedOptionsFor<T>(FeedOptions feedOptions);

        /// <summary>
        /// Handle the <see cref="RequestOptions"/> typically used when querying
        /// </summary>
        /// <typeparam name="T">Type to handle queryable for</typeparam>
        /// <param name="requestOptions"></param>
        void HandleRequestOptionsFor<T>(RequestOptions requestOptions);

        /// <summary>
        /// Handle queryable for a specific type
        /// </summary>
        /// <typeparam name="T">Type to handle queryable for</typeparam>
        /// <param name="queryable">Queryable to handle</param>
        /// <returns>Handled queryable</returns>
        IQueryable<T> HandleQueryableFor<T>(IQueryable<T> queryable);

        /// <summary>
        /// Handle document for a specific type
        /// </summary>
        /// <typeparam name="T">Type to handle for</typeparam>
        /// <param name="document">Document to handle</param>
        void HandleDocumentFor<T>(Document document);

        /// <summary>
        /// Method for configuring details for the collection strategy at the creation of the collection
        /// </summary>
        /// <param name="collection"><see cref="DocumentCollection"/> to configure</param>
        void ConfigureCollectionForCreation(DocumentCollection collection);
    }
}
