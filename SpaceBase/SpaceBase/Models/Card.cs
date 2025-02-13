namespace SpaceBase.Models
{
    public class Card
    {
        private readonly int _sectorID;
        private readonly int _cost;
        private readonly int _amount;
        private readonly int _secondaryAmount;
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
            Effect = CardActions.GetAction(ActionType.AddGold);
            _amount = 1;
            _secondaryAmount = 1;
            DeployedEffect = CardActions.GetAction(ActionType.AddGold);
            _deployedAmount = 1;
            _deployedSecondaryAmount = 1;
        }

        public Card(int sectorID, int cost, Action<Player, int, int> effect, int amount, int? secondaryAmount,
            Action<Player, int, int> deployedEffect, int deployedAmount, int? secondaryDeployedAmount)
        {
            if (sectorID < Constants.MinSectorID || sectorID > Constants.MaxSectorID)
                throw new NotSupportedException($"The sector must be between {Constants.MinSectorID} and {Constants.MaxSectorID} inclusive.");

            _sectorID = sectorID;
            _cost = cost;
            Effect = effect;
            _amount = amount;
            _secondaryAmount = secondaryAmount ?? -1;
            _deployedAmount = deployedAmount;
            _secondaryAmount = secondaryDeployedAmount ?? -1;
            DeployedEffect = deployedEffect;
        }

        public int SectorID { get => _sectorID; }
        public int Cost { get => _cost; }

        public Action<Player, int, int> Effect;
        public Action<Player, int, int> DeployedEffect;

        /// <summary>
        /// Activates the active effect and updates the given player's resources.
        /// </summary>
        /// <param name="player">The player whose resource to update.</param>
        public void ActivateActiveEffect(Player player)
        {
            Effect.Invoke(player, _amount, _secondaryAmount);
        }

        /// <summary>
        /// Activates the deployed effect and updates the given player's resources.
        /// </summary>
        /// <param name="player">The player whose resource to update.</param>
        public void ActivateDeployedEffect(Player player)
        {
            DeployedEffect.Invoke(player, _deployedAmount, _deployedSecondaryAmount);
        }
    }
}
