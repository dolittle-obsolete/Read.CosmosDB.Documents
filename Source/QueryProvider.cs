/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Linq;
using doLittle.Read;
using doLittle.Read.CosmosDB.Documents.Entities;
using Microsoft.Azure.Documents.Linq;

namespace doLittle.Read.CosmosDB.Documents
{
    /// <summary>
    /// Represents an implementation of <see cref="IQueryProviderFor{T}"/>
    /// </summary>
    public class QueryProvider : IQueryProviderFor<IDocumentQuery>
    {
        EntityContextConnection _connection;

        /// <summary>
        /// Initializes a new instance of <see cref="QueryProvider"/>
        /// </summary>
        /// <param name="connection"><see cref="EntityContextConnection"/> to use for getting to the server</param>
        public QueryProvider(EntityContextConnection connection)
        {
            _connection = connection;
        }


        /// <inheritdoc/>
        public QueryProviderResult Execute(IDocumentQuery query, PagingInfo paging)
        {
            var queryable = query as IQueryable;
            var result = new QueryProviderResult();

            var collection = _connection.GetCollectionFor(queryable.ElementType);
            _connection.Client.ReadDocumentFeedAsync(collection.DocumentsLink)
                .ContinueWith(r => result.TotalItems)
                .Wait();

            /*
             * Todo: As of 12th of October 2017 - this is not supported by the DocumentDB Linq Provider or DocumentDB itself!
            if (paging.Enabled)
            {
                var start = paging.Size * paging.Number;
                queryable = queryable.Skip(start).Take(paging.Size);
            }*/

            result.Items = queryable;

            return result;
        }
    }
}
