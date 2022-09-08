using JetBrains.Annotations;
using ProtoBuf;
using Vintagestory.API.MathTools;

namespace ApacheTech.VintageMods.CampaignCartographer.Features.CentreMap.Packets
{
    /// <summary>
    ///     Represents a DTO object to pass player spawn information between the client and server.
    /// </summary>
    [ProtoContract]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class PlayerSpawnPositionDto
    {
        /// <summary>
        ///     Gets or sets the spawn position of the player.
        /// </summary>
        /// <value>The spawn position of the player.</value>
        [ProtoMember(1)]
        public BlockPos SpawnPosition { get; set; }


        /// <summary>
        /// 	Initialises a new instance of the <see cref="PlayerSpawnPositionDto"/> class.
        /// </summary>
        public PlayerSpawnPositionDto()
        {
            SpawnPosition = new BlockPos();
        }

        /// <summary>
        /// 	Initialises a new instance of the <see cref="PlayerSpawnPositionDto"/> class.
        /// </summary>
        /// <param name="spawnPosition">The spawn position.</param>
        public PlayerSpawnPositionDto(BlockPos spawnPosition)
        {
            SpawnPosition = spawnPosition;
        }
    }
}