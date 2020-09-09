namespace JoyBrick.Walkio.Build.CreatureDesign.EditorPart
{
    using System.Collections.Generic;
    using System.Linq;
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    using UnityEditor;
    using UnityEngine;

    public partial class CreatureTable
    {
        [TableList(IsReadOnly = true, AlwaysExpanded = true), ShowInInspector]
        private readonly List<CreatureWrapper> _allCreatures;

        public Creature this[int index] => _allCreatures[index].Creature;

        public CreatureTable(IEnumerable<Creature> levels)
        {
            if (levels != null)
            {
                _allCreatures =
                    levels
                        .Select(x =>
                        {
                            if (x == null) return null;
                            return new CreatureWrapper(x);
                        })
                        .Where(x => x != null)
                        .ToList();
            }
            else
            {
                _allCreatures = new List<CreatureWrapper>();
            }
        }
    }

}
