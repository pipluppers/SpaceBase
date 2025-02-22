using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpaceBase.Models
{
    public class Card : ISerializable
    {
        private readonly int _sectorID;
        private readonly int _cost;

        private readonly Action<Player, int, int> _effect;
        private readonly int _amount;
        private readonly int _secondaryAmount;
        private readonly Action<Player, int, int> _deployedEffect;
        private readonly int _deployedAmount;
        private readonly int _deployedSecondaryAmount;

        /// <summary>
        /// Shouldn't be used. Just to facilitate testing. Maybe remove later.
        /// </summary>
        public Card(int sectorID, int cost)
        {
            if (sectorID < 1 || sectorID > 12)
                throw new ArgumentOutOfRangeException(nameof(sectorID), "The sector must be between 1 and 12 inclusive.");

            _sectorID = sectorID;
            _cost = cost;
            _effect = CardActions.GetAction(ActionType.AddGold);
            _amount = 1;
            _secondaryAmount = 1;
            _deployedEffect = CardActions.GetAction(ActionType.AddGold);
            _deployedAmount = 1;
            _deployedSecondaryAmount = 1;
        }

        [JsonConstructor]
        public Card(int sectorID, int cost, ActionType effectType, int amount, int? secondaryAmount,
            ActionType deployedEffectType, int deployedAmount, int? deployedSecondaryAmount)
        {
            if (sectorID < Constants.MinSectorID || sectorID > Constants.MaxSectorID)
                throw new NotSupportedException($"The sector must be between {Constants.MinSectorID} and {Constants.MaxSectorID} inclusive.");

            _sectorID = sectorID;
            _cost = cost;
            EffectType = effectType;
            _effect = CardActions.GetAction(effectType);
            _amount = amount;
            _secondaryAmount = secondaryAmount ?? 0;
            DeployedEffectType = deployedEffectType;
            _deployedEffect = CardActions.GetAction(deployedEffectType);
            _deployedAmount = deployedAmount;
            _secondaryAmount = deployedSecondaryAmount ?? 0;
        }

        [JsonPropertyOrder(1)]
        public int SectorID { get => _sectorID; }

        [JsonPropertyOrder(2)]
        public int Cost { get => _cost; }

        // For serialization
        [JsonPropertyOrder(3), JsonConverter(typeof(JsonStringEnumConverter))]
        public ActionType EffectType { get; }

        [JsonPropertyOrder(4)]
        public int Amount { get => _amount; }

        [JsonPropertyOrder(5)]
        public int? SecondaryAmount { get => _secondaryAmount; }

        [JsonPropertyOrder(6), JsonConverter(typeof(JsonStringEnumConverter))]
        public ActionType DeployedEffectType { get; }

        [JsonPropertyOrder(7)]
        public int DeployedAmount { get => _deployedAmount; }

        [JsonPropertyOrder(8)]
        public int? DeployedSecondaryAmount { get => _deployedSecondaryAmount; }

        [JsonIgnore]
        public Action<Player, int, int> Effect { get => _effect; }

        [JsonIgnore]
        public Action<Player, int, int> DeployedEffect { get => _deployedEffect; }

        /// <summary>
        /// Activates the active effect and updates the given player's resources.
        /// </summary>
        /// <param name="player">The player whose resource to update.</param>
        public void ActivateActiveEffect(Player player)
        {
            Effect.Invoke(player, Amount, _secondaryAmount);
        }

        /// <summary>
        /// Activates the deployed effect and updates the given player's resources.
        /// </summary>
        /// <param name="player">The player whose resource to update.</param>
        public void ActivateDeployedEffect(Player player)
        {
            DeployedEffect.Invoke(player, DeployedAmount, _deployedSecondaryAmount);
        }

        public object? Deserialize(string str)
        {
            return JsonSerializer.Deserialize<Card>(str);
        }

        public static Card? DeserializeCard(string s)
        {
            return JsonSerializer.Deserialize<Card>(s);
        }

        public string Serialize()
        {
            string s = JsonSerializer.Serialize(this);
            return s;
        }
    }
}
