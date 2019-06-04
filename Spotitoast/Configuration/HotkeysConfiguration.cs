using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Spotitoast.Logic.Business.Action;

namespace Spotitoast.Configuration
{
    public class HotkeysConfiguration : BaseConfiguration
    {
        [JsonIgnore]
        public IDictionary<HotKeys.Model.HotKeys, ActionFactory.PlayerAction> HotKeys { get; set; } =
            new Dictionary<HotKeys.Model.HotKeys, ActionFactory.PlayerAction>
            {
                {
                    new HotKeys.Model.HotKeys(Spotitoast.HotKeys.Model.HotKeys.Keys.Home, Spotitoast.HotKeys.Model.HotKeys.ModifierKeys.Control),
                    ActionFactory.PlayerAction.TogglePlayback
                },
                {
                    new HotKeys.Model.HotKeys(Spotitoast.HotKeys.Model.HotKeys.Keys.PageUp, Spotitoast.HotKeys.Model.HotKeys.ModifierKeys.Control),
                    ActionFactory.PlayerAction.Like
                },
                {
                    new HotKeys.Model.HotKeys(Spotitoast.HotKeys.Model.HotKeys.Keys.PageDown, Spotitoast.HotKeys.Model.HotKeys.ModifierKeys.Control),
                    ActionFactory.PlayerAction.Dislike
                },
                {
                    new HotKeys.Model.HotKeys(Spotitoast.HotKeys.Model.HotKeys.Keys.End, Spotitoast.HotKeys.Model.HotKeys.ModifierKeys.Control),
                    ActionFactory.PlayerAction.CurrentlyPlaying
                },
            };

        [UsedImplicitly]
        public IList<KeyValuePair<HotKeys.Model.HotKeys, ActionFactory.PlayerAction>> SavedHotKeys
        {
            get => HotKeys.ToList();
            set => HotKeys = value.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        /// <summary>
        /// Get the action for the given hotkey
        /// </summary>
        /// <param name="hotKeys"></param>
        /// <returns></returns>
        public ActionFactory.PlayerAction GetAction(HotKeys.Model.HotKeys hotKeys)
        {
            HotKeys.TryGetValue(hotKeys, out var action);
            return action;
        }

        public override void Migrate()
        {
        }
    }
}