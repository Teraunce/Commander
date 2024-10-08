﻿using System;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Enums;
using Kingmaker.Items;
using Kingmaker.Items.Slots;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;

namespace Commander.Components
{
    [AllowMultipleComponents]
    [TypeId("8e6139b1f51d4768bf50468bc2bf23e3")]
    public class SaintsPresenceComp : UnitFactComponentDelegate, IUnitActiveEquipmentSetHandler, IUnitEquipmentHandler
    {
        private static BlueprintCharacterClass _oracleClass;

        private static BlueprintCharacterClass OracleClass
        {
            get
            {
                return _oracleClass ??=
                    ResourcesLibrary.TryGetBlueprint(new BlueprintGuid(Guid.Parse(Guids.Oracle))) as
                        BlueprintCharacterClass;
            }
        }

        public void HandleEquipmentSlotUpdated(ItemSlot slot, ItemEntity previousItem)
        {
            if (slot.Owner == Owner) { CheckConditions(); }
        }

        public void HandleUnitChangeActiveEquipmentSet(UnitDescriptor unit)
        {
            CheckConditions();
        }

        public override void OnTurnOn()
        {
            base.OnTurnOn();
            CheckConditions();
        }

        public override void OnTurnOff()
        {
            base.OnTurnOff();
            DeactivateModifier();
        }

        private void CheckConditions()
        {
            if (CheckArmor())
            {
                ActivateModifier();
            }
            else
            {
                DeactivateModifier();
            }
        }

        private bool CheckArmor()
        {
            var armor = Owner.Body.Armor.Armor;
            var type = armor.ArmorType();

            return !armor.Blueprint.IsArmor 
                   || type == ArmorProficiencyGroup.Light 
                   || type == ArmorProficiencyGroup.Medium;
        }

        private void ActivateModifier()
        {
            var level = Owner.Descriptor.Progression.GetClassLevel(OracleClass) / 2;

            var value = Math.Min(Owner.Stats.Charisma.Bonus, level);
            Owner.Stats.AC.AddModifierUnique(value, Runtime, ModifierDescriptor.UntypedStackable);
        }

        private void DeactivateModifier()
        {
            Owner.Stats.AC.RemoveModifiersFrom(Runtime);
        }
    }
}
