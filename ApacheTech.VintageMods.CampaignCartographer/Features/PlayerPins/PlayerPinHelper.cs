using System;
using System.Drawing;
using ApacheTech.VintageMods.Core.Abstractions.Features;

// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.PlayerPins
{
    /// <summary>
    ///     A helper class that eases the retrieval and setting of pin values, based on an entity's relationship to the current client player.
     /// </summary>
    public sealed class PlayerPinHelper : WorldSettingsConsumer<PlayerPinsSettings>
    {
        /// <summary>
        ///     Gets or sets the relation.
        /// </summary>
        /// <value>The relation.</value>
        public static PlayerRelation Relation { get; set; } = PlayerRelation.Self;

        public static Color Colour
        {
            get
            {
                return Relation switch
                {
                    PlayerRelation.Self => Settings.SelfColour,
                    PlayerRelation.Friend => Settings.FriendColour,
                    PlayerRelation.Others => Settings.OthersColour,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            set
            {
                switch (Relation)
                {
                    case PlayerRelation.Self:
                        Settings.SelfColour = value;
                        break;
                    case PlayerRelation.Friend:
                        Settings.FriendColour = value;
                        break;
                    case PlayerRelation.Others:
                        Settings.OthersColour = value;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static int Scale
        {
            get
            {
                return Relation switch
                {
                    PlayerRelation.Self => Settings.SelfScale,
                    PlayerRelation.Friend => Settings.FriendScale,
                    PlayerRelation.Others => Settings.OthersScale,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            set
            {
                switch (Relation)
                {
                    case PlayerRelation.Self:
                        Settings.SelfScale = value;
                        break;
                    case PlayerRelation.Friend:
                        Settings.FriendScale = value;
                        break;
                    case PlayerRelation.Others:
                        Settings.OthersScale = value;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}