using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Events;

namespace SampleSource.Cms.Events
{
    public class AssociationEntryHandler
    {
        protected void OnItemSaving(object sender, EventArgs args)
        {
            var newItem = Event.ExtractParameter(args, 0) as Item;

            if (newItem == null)
            {
                return;
            }

            var originalItem = newItem.Database.GetItem(newItem.ID, newItem.Language, newItem.Version);
            var differences = FindDifferences(newItem, originalItem);

            if (!differences.Any()) return;
            AddAssociationEntry(newItem, originalItem, differences);
        }

        private static List<string> FindDifferences(Item newItem, Item originalItem)
        {
            newItem.Fields.ReadAll();
            var fields = new[] { "Contacts", "Services" };
            var fieldNames = newItem.Fields.Select(f => f.Name);

            return fieldNames
              .Where(fieldName => newItem[fieldName] != originalItem[fieldName])
              .Where(fields.Contains)
              .ToList();
        }

        private static void AddAssociationEntry(Item newItem, Item originalItem, IEnumerable<string> differences)
        {
            foreach (var field in differences)
            {
                var newField = newItem[field].Split('|');
                var originalField = originalItem[field].Split('|');

                var deltaAdd = newField.Except(originalField).Where(x => !string.IsNullOrEmpty(x));
                var deltaRem = originalField.Except(newField).Where(x => !string.IsNullOrEmpty(x));
            }
        }
    }
}
