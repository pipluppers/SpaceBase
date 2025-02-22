using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpaceBase.Models
{
    public abstract class CardBase : ISerializable
    {
        private protected int _sectorID;
        private protected int _cost;

        [JsonPropertyOrder(1)]
        public int SectorID { get => _sectorID; }

        [JsonPropertyOrder(2)]
        public int Cost { get => _cost; }

        #region ISerializable methods

        /// <summary>
        /// Gets the string representation of the card.
        /// </summary>
        /// <returns>The string representation of the card.</returns>
        public abstract string Serialize();

        /// <summary>
        /// Constructs a CardBase object from a string.
        /// </summary>
        /// <param name="str">The string to create the CardBase object.</param>
        /// <returns>The Cardbase object.</returns>
        public abstract object? Deserialize(string str);

        #endregion ISerializable methods
    }

    public class Card : CardBase
    {
        private readonly Action<Player, int, int> _effect;
        private readonly int _amount;
        private readonly int _secondaryAmount;
        private readonly Action<Player, int, int> _deployedEffect;
        private readonly int _deployedAmount;
        private readonly int _deployedSecondaryAmount;

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
        /// Activates the stationed effect and updates the given player's resources.
        /// </summary>
        /// <param name="player">The player to receive the resources.</param>
        public void ActivateStationedEffect(Player player) => Effect.Invoke(player, Amount, _secondaryAmount);

        /// <summary>
        /// Activates the deployed effect and updates the given player's resources.
        /// </summary>
        /// <param name="player">The player to receive the resources.</param>
        public void ActivateDeployedEffect(Player player) => DeployedEffect.Invoke(player, DeployedAmount, _deployedSecondaryAmount);

        #region ISerializable methods

        public override string Serialize() => JsonSerializer.Serialize(this);

        public override object? Deserialize(string str) => JsonSerializer.Deserialize<Card>(str);

        #endregion ISerializable methods




        /// <summary>
        /// Shouldn't be used. Just to facilitate testing. Maybe remove later.
        /// </summary>
        public Card(int sectorID, int cost)
        {
            if (sectorID < 1 || sectorID > 12)
                throw new ArgumentOutOfRangeException(nameof(sectorID), "The sector must be between 1 and 12 inclusive.");

            _sectorID = sectorID;
            _cost = cost;
            _effect = CardActions.GetAction(ActionType.AddCredits);
            _amount = 1;
            _secondaryAmount = 1;
            _deployedEffect = CardActions.GetAction(ActionType.AddCredits);
            _deployedAmount = 1;
            _deployedSecondaryAmount = 1;
        }

    }

    public enum ChargeCardType
    {
        Turn = 0,
        OpponentTurn = 1,
        Anytime = 2
    }

    public sealed class ChargeCard : Card
    {
        private int _numChargeCubes;

        [JsonConstructor]
        public ChargeCard(int sectorID, int cost, ActionType effectType, int amount, int? secondaryAmount,
            ActionType deployedEffectType, int deployedAmount, int? deployedSecondaryAmount,
            ActionType chargeEffectType, int requiredChargeCubes, int chargeCubeLimit, ChargeCardType chargeCardType)
            : base(sectorID, cost, effectType, amount, secondaryAmount, deployedEffectType, deployedAmount, deployedSecondaryAmount)
        {
            _numChargeCubes = 0;
            ChargeEffectType = chargeEffectType;
            ChargeEffect = CardActions.GetAction(ChargeEffectType);
            RequiredChargeCubes = requiredChargeCubes;
            ChargeCubeLimit = chargeCubeLimit;
            ChargeCardType = chargeCardType;
        }

        [JsonPropertyOrder(9), JsonConverter(typeof(JsonStringEnumConverter))]
        public ActionType ChargeEffectType { get; }

        [JsonPropertyOrder(10)]
        public int RequiredChargeCubes { get; }

        [JsonPropertyOrder(11)]
        public int ChargeCubeLimit { get; }

        [JsonPropertyOrder(12), JsonConverter(typeof(JsonStringEnumConverter))]
        public ChargeCardType ChargeCardType { get; }

        [JsonIgnore]
        public Action<Player, int, int> ChargeEffect { get; }

        /// <summary>
        /// Activates the charge cube effect, if applicable, and updates the given player's resources. Then reduces the amount of charge cubes on this card.
        /// </summary>
        /// <param name="player">The player to receive the resources.</param>
        public void ActivateChargeEffect(Player player)
        {
            if (_numChargeCubes < RequiredChargeCubes)
                return;

            ChargeEffect.Invoke(player, 1, 1);
            _numChargeCubes -= RequiredChargeCubes;
        }

        #region ISerializable methods

        public override string Serialize() => JsonSerializer.Serialize(this);

        public override object? Deserialize(string str) => JsonSerializer.Deserialize<ChargeCard>(str);

        #endregion ISerializable methods
    }

    /// <summary>
    /// A colony card is a simple card with only a sector ID and cost.
    /// </summary>
    public sealed class ColonyCard : CardBase, ISerializable
    {
        public ColonyCard(int sectorID, int cost)
        {
            if (sectorID < Constants.MinSectorID || sectorID > Constants.MaxSectorID)
                throw new NotSupportedException($"The sector must be between {Constants.MinSectorID} and {Constants.MaxSectorID} inclusive.");

            _sectorID = sectorID;
            _cost = cost;
        }

        #region ISerializable methods

        public override string Serialize() => JsonSerializer.Serialize(this);

        public override object? Deserialize(string str) => JsonSerializer.Deserialize<ColonyCard>(str);

        #endregion ISerializable methods
    }
}
