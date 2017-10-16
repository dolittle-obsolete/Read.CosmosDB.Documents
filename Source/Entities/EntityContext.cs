/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using doLittle.Concepts;
using doLittle.Entities;
using doLittle.Collections;
using doLittle.Reflection;
using doLittle.Mapping;
using doLittle.Serialization;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace doLittle.Read.DocumentDB.Entities
{
    /// <summary>
    /// Represents an implementation of <see cref="IEntityContext{T}"/> specifically for DocumentDB
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    public class EntityContext<T> : IEntityContext<T>
    {
        readonly EntityContextConnection _connection;
        readonly DocumentCollection _collection;
        readonly IMapper _mapper;
        readonly ISerializer _serializer;

        /// <summary>
        /// Initializes a new instance of <see cref="EntityContext{T}"/>
        /// </summary>
        /// <param name="connection"><see cref="EntityContextConnection"/> to use</param>
        /// <param name="mapper">Mapper to use for mapping objects to documents</param>

        public EntityContext(EntityContextConnection connection, IMapper mapper, ISerializer serializer)
        {
            _serializer = serializer;
            _connection = connection;
            _collection = connection.GetCollectionFor(typeof(T));
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public IQueryable<T> Entities
        {
            get
            {
                var feedOptions = new FeedOptions();
                _connection.CollectionStrategy.HandleFeedOptionsFor<T>(feedOptions);
                var queryable = _connection.Client.CreateDocumentQuery<T>(_collection.DocumentsLink, feedOptions) as IQueryable<T>;
                queryable = _connection.CollectionStrategy.HandleQueryableFor<T>(queryable);

                return queryable;
            }
        }

        /// <inheritdoc/>
        public void Attach(T entity)
        {
        }

        /// <inheritdoc/>
        public void Insert(T entity)
        {
            var documentType = typeof(T).Name;
            var document = new Document();
            PopulateDocumentFrom(document, entity);
            var result = _connection.Client.CreateDocumentAsync(_collection.DocumentsLink, document, GetRequestOptions()).Result;
        }

        /// <inheritdoc/>
        public void Update(T entity)
        {
            try
            {
                var id = GetIdFrom(entity);

                var documentUri = UriFactory.CreateDocumentUri(_connection.Database.Id, _collection.Id, id.ToString());

                var query = GetQueryForId(id);
                var feedOptions = new FeedOptions { MaxItemCount = 1 };
                _connection.CollectionStrategy.HandleFeedOptionsFor<T>(feedOptions);
                var document = _connection.Client.CreateDocumentQuery<Document>(_collection.DocumentsLink, query, feedOptions)
                    .AsEnumerable()
                    .FirstOrDefault();

                PopulateDocumentFrom(document, entity);
                document.Id = id.ToString();
                var result = _connection.Client.ReplaceDocumentAsync(documentUri, document, GetRequestOptions()).Result;
            }
            catch (Exception ex)
            {
                ex = ex;
            }
        }

        /// <inheritdoc/>
        public void Delete(T entity)
        {
            var id = GetIdFrom(entity);

            var documentUri = UriFactory.CreateDocumentUri(_connection.Database.Id, _collection.Id, id.ToString());

            var requestOptions = new RequestOptions();
            _connection.CollectionStrategy.HandleRequestOptionsFor<T>(requestOptions);

            var result = _connection.Client.DeleteDocumentAsync(documentUri, requestOptions).Result;
        }

        /// <inheritdoc/>
        public void Save(T entity)
        {
            Update(entity);
        }

        /// <inheritdoc/>
        public void Commit()
        {
        }

        /// <inheritdoc/>
        public T GetById<TProperty>(TProperty id)
        {
            try
            {
                var query = GetQueryForId(id);
                var feedOptions = new FeedOptions { MaxItemCount = 1 };
                _connection.CollectionStrategy.HandleFeedOptionsFor<T>(feedOptions);
                var document = _connection.Client.CreateDocumentQuery<T>(_collection.DocumentsLink, query, feedOptions)
                    .AsEnumerable()
                    .FirstOrDefault();
                return document;
            }
            catch (Exception ex)
            {
                ex = ex;
                return default(T);
            }
        }

        /// <inheritdoc/>
        public void DeleteById<TProperty>(TProperty id)
        {
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        string GetQueryForId<TProperty>(TProperty id)
        {
            return $"SELECT * FROM {_collection.Id} WHERE {_collection.Id}.id = '{id}' AND {_collection.Id}.{EntityContextConfiguration.DocumentTypeProperty} = '{typeof(T).Name}'";
        }


        RequestOptions GetRequestOptions()
        {
            var requestOptions = new RequestOptions();
            _connection.CollectionStrategy.HandleRequestOptionsFor<T>(requestOptions);
            return requestOptions;
        }


        object GetIdFrom(T entity)
        {
            var properties = typeof(T).GetTypeInfo().GetProperties();
            var idProperty = properties.Where(a => a.Name.ToLowerInvariant() == "id").AsEnumerable().FirstOrDefault();
            var id = idProperty.GetValue(entity);
            return id;
        }

        void PopulateDocumentOrParentFrom(Document document, Dictionary<string, object> parent, object current)
        {
            var properties = current.GetType().GetTypeInfo().GetProperties();
            properties.ForEach(p =>
            {
                var value = p.GetValue(current);
                var typeInfo = p.PropertyType.GetTypeInfo();

                if (p.PropertyType.IsConcept()) value = value?.GetConceptValue();
                else if (p.PropertyType.Implements(typeof(IEnumerable)))
                {
                    var enumerable = ((IEnumerable)value);
                    var arrayList = new ArrayList();
                    foreach (var item in enumerable)
                    {
                        var itemAsDictionary = new Dictionary<string, object>();
                        PopulateDocumentOrParentFrom(document, itemAsDictionary, item);
                        arrayList.Add(itemAsDictionary);
                    }
                    value = arrayList.ToArray();
                }
                else if (!typeInfo.IsValueType && !typeInfo.IsPrimitive && typeInfo.IsClass)
                {
                    var cur = value;
                    value = new Dictionary<string, object>();
                    PopulateDocumentOrParentFrom(document, value as Dictionary<string, object>, cur);
                }

                if (p.Name.ToLowerInvariant() == "id")
                {
                    if( parent != null) parent["Id"] = value?.ToString();
                    else document.Id = value?.ToString();
                }
                else
                {
                    if (parent != null) parent[p.Name] = value;
                    else document.SetPropertyValue(p.Name, value);
                }
            });

        }


        void PopulateDocumentFrom(Document document, T entity)
        {
            PopulateDocumentOrParentFrom(document, null, entity);
            document.SetPropertyValue(EntityContextConfiguration.DocumentTypeProperty, typeof(T).Name);
        }

    }
}
