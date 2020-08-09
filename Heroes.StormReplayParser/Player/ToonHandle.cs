using System;
using System.Text;

namespace Heroes.StormReplayParser.Player
{
    /// <summary>
    /// Contains the properties for a player's toon handle.
    /// </summary>
    public class ToonHandle
    {
        /// <summary>
        /// Gets or sets the region value.
        /// </summary>
        public int Region { get; set; }

        /// <summary>
        /// Gets or sets the program id. This id is the identifier for the game.
        /// </summary>
        public long ProgramId { get; set; }

        /// <summary>
        /// Gets or sets the realm value.
        /// </summary>
        public int Realm { get; set; }

        /// <summary>
        /// Gets or sets the id unique associated to the player's account in this region.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// <para>
        /// Gets or sets the the shortcut id. Usually in the format of T:XXXXXXXX@XXX. May not always start with T:.
        /// </para>
        /// <para>
        /// This id is used in the user's document folder (typically in the format of T_XXXXXXXX_XXX@REGION).
        /// </para>
        /// <para>
        /// This id is associated to the player's account in this region.
        /// </para>
        /// </summary>
        public string ShortcutId { get; set; } = string.Empty;

        /// <summary>
        /// Gets the region.
        /// </summary>
        public StormRegion StormRegion
        {
            get
            {
                if (Region == 1)
                    return StormRegion.US;
                else if (Region == 2)
                    return StormRegion.EU;
                else if (Region == 3)
                    return StormRegion.KR;
                else if (Region == 5)
                    return StormRegion.CN;
                else if (Region >= 90)
                    return StormRegion.XX;
                else
                    return StormRegion.Unknown;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            Span<char> buffer = stackalloc char[8];
            Encoding.UTF8.GetChars(BitConverter.GetBytes(ProgramId), buffer);

            return $"{Region}-{buffer.Trim('\0').ToString()}-{Realm}-{Id}";
        }
    }
}
