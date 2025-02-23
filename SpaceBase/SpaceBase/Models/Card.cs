using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpaceBase.Models
{
    public enum CardType
    {
        Standard = 0,
        Charge = 1,
        Colony = 2
    }

    public abstract class CardBase : ISerializable
    {
        private protected int _level;
        private protected int _sectorID;
        private protected int _cost;

        [JsonPropertyOrder(0)]
        public int Level { get => _level; }

        [JsonPropertyOrder(2)]
        public int SectorID { get => _sectorID; }

        [JsonPropertyOrder(3)]
        public int Cost { get => _cost; }

        [JsonIgnore]
        public abstract CardType CardType { get; }

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

        public abstract override bool Equals(object? obj);

        public abstract override int GetHashCode();
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
        public Card(int level, int sectorID, int cost, ActionType effectType, int amount, int? secondaryAmount,
            ActionType deployedEffectType, int deployedAmount, int? deployedSecondaryAmount)
        {
            if (level < Constants.MinCardLevel || level > Constants.MaxCardLevel)
                throw new ArgumentOutOfRangeException($"The card level must be between {Constants.MinCardLevel} and {Constants.MaxCardLevel} inclusive.");

            if (level == 1 && (cost < 2 || cost > 5))
                throw new ArgumentOutOfRangeException(nameof(cost), "If the level is 1, then the cost must be between 2 and 5.");
            else if (level == 2 && (cost < 7 || cost > 9))
                throw new ArgumentOutOfRangeException(nameof(cost), "If the level is 2, then the cost must be between 7 and 9.");
            else if (level == 3 && (cost < 12 || cost > 14))
                throw new ArgumentOutOfRangeException(nameof(cost), "If the level is 3, then the cost must be between 12 and 14.");

            if (sectorID < Constants.MinSectorID || sectorID > Constants.MaxSectorID)
                throw new ArgumentOutOfRangeException($"The sector must be between {Constants.MinSectorID} and {Constants.MaxSectorID} inclusive.");

            _level = level;
            _sectorID = sectorID;
            _cost = cost;
            EffectType = effectType;
            _effect = CardActions.GetAction(effectType);
            _amount = amount;
            _secondaryAmount = secondaryAmount ?? 0;
            DeployedEffectType = deployedEffectType;
            _deployedEffect = CardActions.GetAction(deployedEffectType);
            _deployedAmount = deployedAmount;
            _deployedSecondaryAmount = deployedSecondaryAmount ?? 0;
        }

        [JsonPropertyOrder(4), JsonConverter(typeof(JsonStringEnumConverter))]
        public ActionType EffectType { get; }

        [JsonPropertyOrder(5)]
        public int Amount { get => _amount; }

        [JsonPropertyOrder(6)]
        public int? SecondaryAmount { get => _secondaryAmount; }

        [JsonPropertyOrder(7), JsonConverter(typeof(JsonStringEnumConverter))]
        public ActionType DeployedEffectType { get; }

        [JsonPropertyOrder(8)]
        public int DeployedAmount { get => _deployedAmount; }

        [JsonPropertyOrder(9)]
        public int? DeployedSecondaryAmount { get => _deployedSecondaryAmount; }

        [JsonIgnore]
        public Action<Player, int, int> Effect { get => _effect; }

        [JsonIgnore]
        public Action<Player, int, int> DeployedEffect { get => _deployedEffect; }

        [JsonIgnore]
        public override CardType CardType { get => CardType.Standard; }

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

        public override bool Equals(object? obj)
        {
            if (obj is not Card otherCard)
                return false;

            return Level == otherCard.Level &&
                SectorID == otherCard.SectorID &&
                Cost == otherCard.Cost &&
                EffectType == otherCard.EffectType &&
                Amount == otherCard.Amount &&
                SecondaryAmount == otherCard.SecondaryAmount &&
                DeployedEffectType == otherCard.DeployedEffectType &&
                DeployedAmount == otherCard.DeployedAmount &&
                DeployedSecondaryAmount == otherCard.DeployedSecondaryAmount;
        }

        public override int GetHashCode()
        {
            return Level * 17 + SectorID * 17 + Cost * 17 + (int)EffectType * 17 + (int)DeployedEffectType * 17;
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
        public ChargeCard(int level, int sectorID, int cost, ActionType effectType, int amount, int? secondaryAmount,
            ActionType deployedEffectType, int deployedAmount, int? deployedSecondaryAmount,
            ActionType chargeEffectType, int requiredChargeCubes, int chargeCubeLimit, ChargeCardType chargeCardType)
            : base(level, sectorID, cost, effectType, amount, secondaryAmount, deployedEffectType, deployedAmount, deployedSecondaryAmount)
        {
            _numChargeCubes = 0;
            ChargeEffectType = chargeEffectType;
            ChargeEffect = CardActions.GetAction(ChargeEffectType);
            RequiredChargeCubes = requiredChargeCubes;
            ChargeCubeLimit = chargeCubeLimit;
            ChargeCardType = chargeCardType;
        }

        [JsonPropertyOrder(10), JsonConverter(typeof(JsonStringEnumConverter))]
        public ActionType ChargeEffectType { get; }

        [JsonPropertyOrder(11)]
        public int RequiredChargeCubes { get; }

        [JsonPropertyOrder(12)]
        public int ChargeCubeLimit { get; }

        [JsonPropertyOrder(13), JsonConverter(typeof(JsonStringEnumConverter))]
        public ChargeCardType ChargeCardType { get; }

        [JsonIgnore]
        public override CardType CardType { get => CardType.Charge; }

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

        public override bool Equals(object? obj)
        {
            if (obj is not ChargeCard otherCard)
                return false;

            if (!base.Equals(obj))
                return false;

            return ChargeEffectType == otherCard.ChargeEffectType &&
                RequiredChargeCubes == otherCard.RequiredChargeCubes &&
                ChargeCubeLimit == otherCard.ChargeCubeLimit &&
                ChargeCardType == otherCard.ChargeCardType;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + (int)EffectType * 17 + (int)DeployedEffectType * 17 + (int)ChargeEffectType * 17;
        }
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

        [JsonIgnore]
        public override CardType CardType { get => CardType.Colony; }

        #region ISerializable methods

        public override string Serialize() => JsonSerializer.Serialize(this);

        public override object? Deserialize(string str) => JsonSerializer.Deserialize<ColonyCard>(str);

        #endregion ISerializable methods

        public override bool Equals(object? obj)
        {
            if (obj is not Card otherCard)
                return false;

            return Level == otherCard.Level &&
                SectorID == otherCard.SectorID &&
                Cost == otherCard.Cost;
        }

        public override int GetHashCode()
        {
            return Level * 17 + SectorID * 17 + Cost * 17;
        }
    }
}
