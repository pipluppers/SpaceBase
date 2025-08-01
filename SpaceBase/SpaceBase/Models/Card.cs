﻿namespace SpaceBase.Models
{
    public interface ICard
    {
        public int SectorID { get; }
        public int Cost { get; }
        public CardType CardType { get; }
        public int Amount { get; }
    }

    public interface IStandardCard : ICard
    {
        public int Level { get; }
        public ICardCommand StationedCommand { get; }
        public ICardCommand DeployedCommand { get; }
    }

    public interface IChargeCard : IStandardCard
    {
        public ICardCommand ChargeCommand { get; }
    }

    public interface IColonyCard : ICard
    {
    }

    public abstract class CardBase : ICard, ISerializable
    {
        private readonly int _sectorID;
        private readonly int _cost;
        private readonly int _amount;

        protected CardBase(int sectorID, int cost, int amount)
        {
            if (sectorID < Constants.MinSectorID || sectorID > Constants.MaxSectorID)
                throw new ArgumentOutOfRangeException(nameof(sectorID), $"The sector must be between {Constants.MinSectorID} and {Constants.MaxSectorID} inclusive.");

            _sectorID = sectorID;
            _cost = cost;
            _amount = amount;
        }

        [JsonPropertyOrder(2)]
        public int SectorID { get => _sectorID; }

        [JsonPropertyOrder(3)]
        public int Cost { get => _cost; }

        [JsonPropertyOrder(5)]
        public int Amount { get => _amount; }

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

        #region Equatable methods

        public override bool Equals(object? obj)
        {
            if (obj is not CardBase other)
                return false;

            return SectorID == other.SectorID && Cost == other.Cost && CardType == other.CardType;
        }

        public override int GetHashCode() => SectorID * 17 + Cost * 17 + (int)CardType * 17;

        #endregion Equatable methods
    }

    /// <summary>
    /// A card that has stationed and deployed effects.
    /// </summary>
    public class Card : CardBase, IStandardCard
    {
        private protected int _level;
        private readonly ActionType _effectType;
        private readonly int _secondaryAmount;
        private readonly int _deployedAmount;
        private readonly int _deployedSecondaryAmount;

        private readonly ICardCommand _stationedCommand;
        private readonly ICardCommand _deployedCommand;

        [JsonConstructor]
        internal Card(int level, int sectorID, int cost, ActionType effectType, int amount, int? secondaryAmount,
            ActionType deployedEffectType, int deployedAmount, int? deployedSecondaryAmount) : base(sectorID, cost, amount)
        {
            _level = level;
            EffectType = effectType;
            _effectType = effectType;
            _secondaryAmount = secondaryAmount ?? 0;
            DeployedEffectType = deployedEffectType;
            _deployedAmount = deployedAmount;
            _deployedSecondaryAmount = deployedSecondaryAmount ?? 0;

            _stationedCommand = GetCommand(effectType, amount, secondaryAmount ?? 0);
            _deployedCommand = GetCommand(deployedEffectType, deployedAmount, deployedSecondaryAmount ?? 0);
        }

        [JsonPropertyOrder(1)]
        public int Level { get => _level; }

        [JsonPropertyOrder(4), JsonConverter(typeof(JsonStringEnumConverter))]
        public ActionType EffectType { get; }

        [JsonPropertyOrder(6)]
        public int? SecondaryAmount { get => _secondaryAmount; }

        [JsonPropertyOrder(7), JsonConverter(typeof(JsonStringEnumConverter))]
        public ActionType DeployedEffectType { get; }

        [JsonPropertyOrder(8)]
        public int DeployedAmount { get => _deployedAmount; }

        [JsonPropertyOrder(9)]
        public int? DeployedSecondaryAmount { get => _deployedSecondaryAmount; }

        [JsonIgnore]
        public override CardType CardType { get => CardType.Standard; }

        [JsonIgnore]
        public ICardCommand StationedCommand { get => _stationedCommand; }

        [JsonIgnore]
        public ICardCommand DeployedCommand { get => _deployedCommand; }

        private ICardCommand GetCommand(ActionType actionType, int amount, int secondaryAmount)
        {
            switch (actionType)
            {
                case ActionType.AddCredits:
                    return new AddCreditsCommand(amount);
                case ActionType.AddIncome:
                    return new AddIncomeCommand(amount);
                case ActionType.AddVictoryPoints:
                    return new AddVictoryPointsCommand(amount);
                case ActionType.AddCreditsIncome:
                    return new AddCreditsIncomeCommand(amount, secondaryAmount);
                case ActionType.AddCreditsVictoryPoints:
                    return new AddCreditsVictoryPointsCommand(amount, secondaryAmount);
                case ActionType.DoubleArrow:
                    return new DoubleArrowCommand();
                case ActionType.AddCreditsArrow:
                    return new AddCreditsArrowCommand(amount);
                case ActionType.AddVictoryPointsArrow:
                    return new AddVictoryPointsArrowCommand(amount);
                case ActionType.ClaimCardsAtLevel:
                    return new ClaimCardsCommand(1, 1); // TODO
                case ActionType.AddChargeCube:
                    {
                        ChargeCard chargeCard = (ChargeCard)this;
                        return new AddChargeCubeCommand(chargeCard);
                    }
                default:
                    throw new NotSupportedException($"Invalid action type: {actionType}");
            }
        }

        #region ISerializable methods

        public override string Serialize() => JsonSerializer.Serialize(this);

        public override object? Deserialize(string str) => JsonSerializer.Deserialize<Card>(str);

        #endregion ISerializable methods

        #region Equatable methods

        public override bool Equals(object? obj)
        {
            if (obj is not Card otherCard)
                return false;

            if (!base.Equals(obj)) return false;

            return _effectType == otherCard._effectType &&
                Amount == otherCard.Amount &&
                SecondaryAmount == otherCard.SecondaryAmount &&
                DeployedEffectType == otherCard.DeployedEffectType &&
                DeployedAmount == otherCard.DeployedAmount &&
                DeployedSecondaryAmount == otherCard.DeployedSecondaryAmount;
        }

        public override int GetHashCode() => base.GetHashCode() + (int)_effectType * 17 + (int)DeployedEffectType * 17;

        #endregion Equatable methods
    }

    /// <summary>
    /// A card that can use charge cubes.
    /// </summary>
    public sealed class ChargeCard : Card, IChargeCard
    {
        private readonly ICardCommand _chargeCommand;

        [JsonConstructor]
        public ChargeCard(int level, int sectorID, int cost, ActionType effectType, int amount, int? secondaryAmount,
            ActionType deployedEffectType, int deployedAmount, int? deployedSecondaryAmount,
            ChargeActionType chargeEffectType, int requiredChargeCubes, int chargeCubeLimit, ChargeCardType chargeCardType,
            ChargeActionType deployedChargeEffectType, int deployedRequiredChargeCubes, int deployedChargeCubeLimit, ChargeCardType deployedChargeCardType)
            : base(level, sectorID, cost, effectType, amount, secondaryAmount, deployedEffectType, deployedAmount, deployedSecondaryAmount)
        {
            // Note: A charge card doesn't have to have charge effects for both stationed and deployed effects.

            if (requiredChargeCubes > chargeCubeLimit || deployedRequiredChargeCubes > deployedChargeCubeLimit)
                throw new ArgumentException("A charge card can't require more cubes that it is allowed to have.");

            NumChargeCubes = 0;
            ChargeEffectType = chargeEffectType;
            _chargeCommand = GetCommand(chargeEffectType);
            RequiredChargeCubes = requiredChargeCubes;
            ChargeCubeLimit = chargeCubeLimit;
            ChargeCardType = chargeCardType;
            DeployedChargeEffectType = deployedChargeEffectType;
            DeployedRequiredChargeCubes = deployedRequiredChargeCubes;
            DeployedChargeCubeLimit = deployedChargeCubeLimit;
            DeployedChargeCardType = deployedChargeCardType;
        }

        [JsonPropertyOrder(10), JsonConverter(typeof(JsonStringEnumConverter))]
        public ChargeActionType ChargeEffectType { get; }

        [JsonPropertyOrder(11)]
        public int RequiredChargeCubes { get; }

        [JsonPropertyOrder(12)]
        public int ChargeCubeLimit { get; }

        [JsonPropertyOrder(13), JsonConverter(typeof(JsonStringEnumConverter))]
        public ChargeCardType ChargeCardType { get; }

        [JsonPropertyOrder(14), JsonConverter(typeof(JsonStringEnumConverter))]
        public ChargeActionType DeployedChargeEffectType { get; }

        [JsonPropertyOrder(15)]
        public int DeployedRequiredChargeCubes { get; }

        [JsonPropertyOrder(16)]
        public int DeployedChargeCubeLimit { get; }

        [JsonPropertyOrder(17), JsonConverter(typeof(JsonStringEnumConverter))]
        public ChargeCardType DeployedChargeCardType { get; }

        [JsonIgnore]
        public override CardType CardType { get => CardType.Charge; }

        [JsonIgnore]
        public ICardCommand ChargeCommand { get => _chargeCommand; }

        [JsonIgnore]
        public int NumChargeCubes { get; private set; }

        /// <summary>
        /// Adds a charge cube to this card.
        /// </summary>
        public void AddChargeCube()
        {
            if (NumChargeCubes < ChargeCubeLimit)
                NumChargeCubes++;
        }

        /// <summary>
        /// Activates the charge cube effect, if applicable, and updates the given player's resources. Then reduces the amount of charge cubes on this card.
        /// </summary>
        /// <param name="player">The player to receive the resources.</param>
        public void ActivateChargeEffect(Player player)
        {
            if (NumChargeCubes < RequiredChargeCubes)
                return;

            _chargeCommand.Execute(player);
            NumChargeCubes -= RequiredChargeCubes;
        }

        private ICardCommand GetCommand(ChargeActionType chargeActionType)
        {
            switch (chargeActionType)
            {
                case ChargeActionType.AddToSum1:
                    return new AddToSumCommand(1);
                case ChargeActionType.AddToSum2:
                    return new AddToSumCommand(2);
                case ChargeActionType.BuyCardAndAdd3Credits:
                case ChargeActionType.BuyCardAndAdd4Credits:
                case ChargeActionType.BuyCardAndAdd4VictoryPoints:
                case ChargeActionType.BuyCardAndPlaceInAnySector7To12:
                case ChargeActionType.RerollDie:
                case ChargeActionType.Set1DieBeforeRoll:
                case ChargeActionType.Set1DieBeforeRollAndAdd1Credit:
                case ChargeActionType.Set1DieBeforeRollAndAdd4Credits:
                case ChargeActionType.Swap112Sectors:
                case ChargeActionType.Swap211Sectors:
                case ChargeActionType.Swap49Sectors:
                case ChargeActionType.Swap410Sectors:
                case ChargeActionType.Swap58Sectors:
                case ChargeActionType.Swap67Sectors:
                case ChargeActionType.Add3Credits:
                case ChargeActionType.Add20Credits:
                case ChargeActionType.Claim2Level1Cards:
                case ChargeActionType.Claim3Level1Cards:
                case ChargeActionType.Claim1Level2Card:
                case ChargeActionType.Claim2Level2Cards:
                case ChargeActionType.Claim1Level2CardAnd1Level1Card:
                case ChargeActionType.Claim1Level3Card:
                case ChargeActionType.Claim1Level3CardAnd1Level1Card:
                case ChargeActionType.OppLose3VictoryPointsAndBuy1Card:
                case ChargeActionType.OppLose4VictoryPointsAndBuy2Cards:
                case ChargeActionType.Place1ChargeAnywhere:
                case ChargeActionType.Place1ChargeAnywhereAndMove1Charge:
                case ChargeActionType.DoubleSectorRewards:
                case ChargeActionType.ExchangeWithAnyCard:
                case ChargeActionType.InstantVictory:
                default:
                    throw new NotSupportedException($"Invalid action type: {chargeActionType}");
            }
        }

        #region ISerializable methods

        public override string Serialize() => JsonSerializer.Serialize(this);

        public override object? Deserialize(string str) => JsonSerializer.Deserialize<ChargeCard>(str);

        #endregion ISerializable methods

        #region Equatable methods

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

        public override int GetHashCode() => base.GetHashCode() + (int)ChargeEffectType * 17;

        #endregion Equatable methods
    }

    /// <summary>
    /// A colony card is a simple card with only a sector ID and cost.
    /// </summary>
    public sealed class ColonyCard : CardBase, IColonyCard, ISerializable
    {
        [JsonConstructor]
        internal ColonyCard(int sectorID, int cost, int amount) : base(sectorID, cost, amount) { }

        [JsonIgnore]
        public override CardType CardType { get => CardType.Colony; }

        #region ISerializable methods

        public override string Serialize() => JsonSerializer.Serialize(this);

        public override object? Deserialize(string str) => JsonSerializer.Deserialize<ColonyCard>(str);

        #endregion ISerializable methods
    }
}
