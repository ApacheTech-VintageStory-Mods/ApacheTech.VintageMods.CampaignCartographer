using ApacheTech.Common.Extensions.System;
using JetBrains.Annotations;
using Vintagestory.API.Common;

// ReSharper disable StringLiteralTypo

namespace ApacheTech.VintageMods.CampaignCartographer.Features.AutoWaypoints.Extensions
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class BlockExtensions
    {
        /// <summary>
        ///     Determines whether the specified block is a surface deposit of ore.
        /// </summary>
        /// <param name="block">The block to check.</param>
        /// <returns><c>true</c> if the block is a surface deposit of ore; otherwise, <c>false</c>.</returns>
        public static bool IsSurfaceDeposit(this Block block)
        {
            return block.Code.Path.ContainsAny("-looseores-", "-loosestones-");
        }
        /// <summary>
        ///     Determines whether the specified block entity is a surface deposit of ore.
        /// </summary>
        /// <param name="blockEntity">The block entity to check.</param>
        /// <returns><c>true</c> if the block entity is a surface deposit of ore; otherwise, <c>false</c>.</returns>
        public static bool IsSurfaceDeposit(this BlockEntity blockEntity)
        {
            return blockEntity.Block.IsSurfaceDeposit();
        }
    }
}