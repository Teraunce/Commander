﻿using System.Collections.Generic;
using JetBrains.Annotations;
using Kingmaker.Blueprints;

namespace Commander
{
    internal static class Resources 
    {
        public static readonly Dictionary<BlueprintGuid, SimpleBlueprint> ModBlueprints = new Dictionary<BlueprintGuid, SimpleBlueprint>();

        public static T GetBlueprint<T>(string id) where T : SimpleBlueprint 
        {
            var assetId = new BlueprintGuid(System.Guid.Parse(id));
            return GetBlueprint<T>(assetId);
        }

        public static T GetBlueprint<T>(BlueprintGuid id) where T : SimpleBlueprint 
        {
            var asset = ResourcesLibrary.TryGetBlueprint(id);
            var value = asset as T;
            if (value == null) { Main.Log($"COULD NOT LOAD: {id} - {typeof(T)}"); }
            return value;
        }

        public static void AddBlueprint([NotNull] SimpleBlueprint blueprint) 
        {
            AddBlueprint(blueprint, blueprint.AssetGuid);
        }

        public static void AddBlueprint([NotNull] SimpleBlueprint blueprint, string assetId) 
        {
            var id = BlueprintGuid.Parse(assetId);
            AddBlueprint(blueprint, id);
        }

        public static void AddBlueprint([NotNull] SimpleBlueprint blueprint, BlueprintGuid assetId) 
        {
            var loadedBlueprint = ResourcesLibrary.TryGetBlueprint(assetId);

            if (loadedBlueprint == null) 
            {
                ModBlueprints[assetId] = blueprint;
                ResourcesLibrary.BlueprintsCache.AddCachedBlueprint(assetId, blueprint);
                blueprint.OnEnable();
                Main.LogPatch("Added", blueprint);
            }
            else 
            {
                Main.Log($"Failed to Add: {blueprint.name}");
                Main.Log($"Asset ID: {assetId} already in use by: {loadedBlueprint.name}");
            }
        }
    }
}
