using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Resources;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Recipes;

namespace Restaurant.Menus;

[EventHandler("23dddecf-3897-4806-8aae-9e750ecb7bab")]
public class RestaurantMenu
{
    readonly IResources _resources;

    public RestaurantMenu(IResources resources)
    {
        _resources = resources;
    }

    public Task Handle(DishAddedToMenu @event, EventContext ctx)
        => UpdateCollection(menus =>
            menus.UpdateOneAsync(
                Builders<Menu>.Filter.Eq(_ => _.Restaurant, ctx.EventSourceId.ToString()),
                Builders<Menu>.Update.Push(_ => _.Dishes, new Menu.Dish {Name = @event.Dish}),
                new UpdateOptions {IsUpsert = true}));
    
    public Task Handle(DishRemovedFromMenu @event, EventContext ctx)
        => UpdateCollection(menus =>
            menus.UpdateOneAsync(
                Builders<Menu>.Filter.Eq(_ => _.Restaurant, ctx.EventSourceId.ToString()),
                Builders<Menu>.Update.PullFilter(_ => _.Dishes, Builders<Menu.Dish>.Filter.Eq(_ => _.Name, @event.Dish)),
                new UpdateOptions {IsUpsert = true}));

    public Task Handle(RecipeDescripionAdded @event, EventContext ctx)
        => UpdateCollection(menus =>
            menus.UpdateManyAsync(
                Builders<Menu>.Filter.ElemMatch(_ => _.Dishes, Builders<Menu.Dish>.Filter.Eq(_ => _.Name, ctx.EventSourceId.ToString())),
                Builders<Menu>.Update.Set(_ => _.Dishes[-1].Description, @event.Description)));

    public Task Handle(RecipeDescriptionChanged @event, EventContext ctx)
        => UpdateCollection(menus =>
            menus.UpdateManyAsync(
                Builders<Menu>.Filter.ElemMatch(_ => _.Dishes, Builders<Menu.Dish>.Filter.Eq(_ => _.Name, ctx.EventSourceId.ToString())),
                Builders<Menu>.Update.Set(_ => _.Dishes[-1].Description, @event.Description)));

    async Task UpdateCollection(Func<IMongoCollection<Menu>, Task> action)
    {
        var database = await _resources.MongoDB.GetDatabase().ConfigureAwait(false);
        await action(database.GetCollection<Menu>("menus")).ConfigureAwait(false);
    }
    
    
    


    public class Menu
    {
        [BsonId]
        public string Restaurant { get; set; }

        public List<Dish> Dishes { get; set; } = new();

        public class Dish 
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}