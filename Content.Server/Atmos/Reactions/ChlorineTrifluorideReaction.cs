using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Atmos.Reactions
{
    [UsedImplicitly]
    [DataDefinition]
    public sealed partial class ChlorineTrifluorideReaction : IGasReactionEffect
    {
        public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
        {
            var energyReleased = 0f;
            var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
            var temperature = mixture.Temperature;
            var location = holder as TileAtmosphere;
            mixture.ReactionResults[(byte)GasReaction.Fire] = 0;

            var initialCLF3Moles = mixture.GetMoles(Gas.ChlorineTrifluoride);

            // Use reaction temperature (hotspot preferred) and scale decomposition with temperature
            var reactionTemperature = temperature;
            if (location?.Hotspot.Valid == true)
            {
                reactionTemperature = location.Hotspot.Temperature;
            }

            var temperatureScale = 0f;
            if (reactionTemperature > Atmospherics.PlasmaUpperTemperature)
                temperatureScale = 1f;
            else if (reactionTemperature > Atmospherics.PlasmaMinimumBurnTemperature)
                temperatureScale = (reactionTemperature - Atmospherics.PlasmaMinimumBurnTemperature) /
                                   (Atmospherics.PlasmaUpperTemperature - Atmospherics.PlasmaMinimumBurnTemperature);

            var decompositionRate = initialCLF3Moles * 0.25f * temperatureScale; // increased for faster decomposition

            if (decompositionRate > Atmospherics.MinimumHeatCapacity)
            {
                mixture.SetMoles(Gas.ChlorineTrifluoride, initialCLF3Moles - decompositionRate);
                mixture.AdjustMoles(Gas.Chlorine, decompositionRate * 0.5f);
                mixture.AdjustMoles(Gas.Fluorine, decompositionRate * 1.5f);

                // Release ~=319 kJ per mole of ClF3 decomposed (exothermic)
                energyReleased = 319000f * decompositionRate;
                energyReleased /= heatScale;
                mixture.ReactionResults[(byte)GasReaction.Fire] = decompositionRate * 1.5f;
            }

            if (energyReleased != 0f)
            {
                var newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
                if (newHeatCapacity > Atmospherics.MinimumHeatCapacity)
                    mixture.Temperature = (temperature * oldHeatCapacity + energyReleased) / newHeatCapacity;
            }

            if (location != null && decompositionRate > Atmospherics.MinimumHeatCapacity)
            {
                atmosphereSystem.HotspotExpose(location, mixture.Temperature, mixture.Volume);
            }

            return mixture.ReactionResults[(byte)GasReaction.Fire] != 0 ? (ReactionResult.Reacting | ReactionResult.StopReactions) : ReactionResult.NoReaction;
        }
    }
}
