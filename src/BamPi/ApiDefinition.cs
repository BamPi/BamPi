using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BamPi.Responders;
using Nancy;

namespace BamPi
{
    public class ApiDefinition
    {
        /// <summary>
        /// Creates new instance of the ApiDefinition.
        /// </summary>
        public ApiDefinition()
        {
            Routes = new List<ApiOperation>();
        }

        /// <summary>
        /// Adds a GET route to the ApiDefinition.
        /// </summary>
        public ConventionalRouteBuilder Get
        {
            get { return new ConventionalRouteBuilder("GET", this); }
        }


        /// <summary>
        /// Adds a PUT route to the ApiDefinition.
        /// </summary>
        public ConventionalRouteBuilder Put
        {
            get { return new ConventionalRouteBuilder("PUT", this); }
        }


        /// <summary>
        /// Adds a POST route to the ApiDefinition.
        /// </summary>
        public ConventionalRouteBuilder Post
        {
            get { return new ConventionalRouteBuilder("POST", this); }
        }

        /// <summary>
        /// Adds a DELETE route to the ApiDefinition.
        /// </summary>
        public ConventionalRouteBuilder Delete
        {
            get { return new ConventionalRouteBuilder("DELETE", this); }
        }

        /// <summary>
        /// Adds a OPTIONS route to the ApiDefinition.
        /// </summary>
        public ConventionalRouteBuilder Options
        {
            get { return new ConventionalRouteBuilder("OPTIONS", this); }
        }

        /// <summary>
        /// Gets a collection of all added routes.
        /// </summary>
        public ICollection<ApiOperation> Routes { get; private set; }

        /// <summary>
        /// Gets or sets the DataContext the routes will use.
        /// </summary>
        public IBamPiDataContext DataContext { get; set; }

        public Responder Create<T>(Action<T> updater = null) where T : class
        {
            return new CreateResponder<T>(updater);
        }

        /// <summary>
        /// Creates a responder for querying a entity set.
        /// </summary>
        /// <typeparam name="T">The type of the entity set to query.</typeparam>
        /// <param name="query">A satic query to add to ever coming from the API.</param>
        public Responder Query<T>(Expression<Func<T, bool>> query) where T : class
        {
            return new QueryResponder<T>(query);
        }

        /// <summary>
        /// Creates a responder for querying a entity set.
        /// </summary>
        /// <typeparam name="T">The type of the entity set to query.</typeparam>
        public Responder Query<T>() where T : class
        {
            return new QueryResponder<T>();
        }

        /// <summary>
        /// Creates a resonder which removes an entity from an entity set.
        /// </summary>
        /// <typeparam name="T">The type of the entity set where the entity gets removed.</typeparam>
        public Responder Remove<T>() where T : class
        {
            return new DeleteResponder<T>();
        }

        /// <summary>
        /// Creates a responder which updates an entity from an entity set.
        /// </summary>
        /// <typeparam name="T">The type of the entity set where the entity gets updated.</typeparam>
        public Responder Update<T>() where T : class
        {
            return new UpdateResponder<T>();
        }

        /// <summary>
        /// Creates a responder for querying a child collection.
        /// </summary>
        /// <typeparam name="TParent">The type of the entity containing the child collection.</typeparam>
        /// <typeparam name="TChild">The type the entities of the child colleciton have.</typeparam>
        /// <param name="property">The property of the parent, containing the child property.</param>
        public Responder QueryChild<TParent, TChild>(Expression<Func<TParent, ICollection<TChild>>> property)
            where TParent : class
            where TChild : class
        {
            return new QueryChildResponder<TParent, TChild>(property);
        }

        /// <summary>
        /// Creates a responder remoing an item from a child collection.
        /// </summary>
        /// <typeparam name="TParent">The type of the entity containing the child collection.</typeparam>
        /// <typeparam name="TChild">The type the entities of the child colleciton have.</typeparam>
        /// <param name="property">The property of the parent, containing the child property.</param>
        public Responder RemoveChild<TParent, TChild>(Expression<Func<TParent, ICollection<TChild>>> property)
            where TParent : class
            where TChild : class
        {
            return new RemoveChildResponder<TParent, TChild>(property);
        }

        /// <summary>
        /// Creates a reponder which adds an entity to a child collection.
        /// </summary>
        /// <typeparam name="TParent">The type of the entity containing the child collection.</typeparam>
        /// <typeparam name="TChild">The type the entities of the child colleciton have.</typeparam>
        /// <param name="property">The property of the parent, containing the child property.</param>
        public Responder AddChild<TParent, TChild>(Expression<Func<TParent, ICollection<TChild>>> property)
            where TParent : class
            where TChild : class
        {
            return new AddChildResponder<TParent, TChild>(property);
        }

        /// <summary>
        /// Register the api definition to a nancy module. All registred routes will then be routed through the Nancy module.
        /// </summary>
        /// <param name="module">The nancy module to register to.</param>
        public ApiDefinition RegisterNancyModule(NancyModule module)
        {
            var nancyActions = new Dictionary<string, NancyModule.RouteBuilder>();
            nancyActions["GET"] = module.Get;
            nancyActions["POST"] = module.Post;
            nancyActions["OPTIONS"] = module.Options;
            nancyActions["PUT"] = module.Put;
            nancyActions["DELETE"] = module.Delete;

            foreach (var operation in Routes)
            {
                var op = operation;
                nancyActions.Where(_ => _.Key == operation.Method).ToList().ForEach(
                    action =>
                        action.Value[operation.Route, true] =
                            async (_, ct) =>
                            {
                                op.Responder.Rules.Select(r => r as IBeforeRule).Where(r => r != null).
                                    ToList().ForEach(r => r.Before(module, _, this));
                                var returnValue = await op.Responder.Execute(module, _, this);
                                op.Responder.Rules.Select(r => r as IAfterRule).Where(r => r != null).
                                    ToList().ForEach(r => r.After(module, _, this, returnValue));
                                return returnValue;
                            });
            }
            return this;
        }
    }
}