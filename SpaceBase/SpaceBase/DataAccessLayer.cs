namespace SpaceBase
{
    internal class DataAccessLayer
    {
        private readonly string _connectionString;

        public DataAccessLayer()
        {
            string? server = Environment.GetEnvironmentVariable(Constants.ServerEnvironmentVariable, EnvironmentVariableTarget.User);
            string? database = Environment.GetEnvironmentVariable(Constants.DatabaseEnvironmentVariable, EnvironmentVariableTarget.User);

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(database))
                throw new InvalidOperationException($"Error loading the server {server} or database {database}.");

            _connectionString = $"{Constants.ServerKey}={server};{Constants.DatabaseKey}={database};";

            // TODO  Get Certificate Authority signed certificate and remove the assignment below
            _connectionString += "Encrypt=False;Trusted_Connection=True";
        }

        /// <summary>
        /// Opens a connection along with error handling.
        /// </summary>
        /// <param name="connection">The connection to open.</param>
        /// <exception cref="Exception">The connection has failed to open.</exception>
        private async Task Connect(SqlConnection connection)
        {
            try
            {
                await connection.OpenAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error connecting to {_connectionString}", ex);
            }
        }

        /// <summary>
        /// Closes a connection along with error handling.
        /// </summary>
        /// <param name="connection">The connection to close.</param>
        private async Task Disconnect(SqlConnection connection)
        {
            try
            {
                await connection.CloseAsync();
            }
            catch (Exception ex)
            {
                // Do not throw here since we are already trying to close the connection.
                Trace.WriteLine($"Error closing the connection to {_connectionString}\n{ex.Message}");
            }
        }

        /// <summary>
        /// Gets the list of cards from the database connection.
        /// </summary>
        /// <returns>The list of cards from the database connection.</returns>
        public async Task<List<ICard>> GetCards()
        {
            List<ICard> cards = [];
            SqlConnection? connection = null;

            string? table = Environment.GetEnvironmentVariable(Constants.CardsTableEnvironmentVariable, EnvironmentVariableTarget.User);
            string queryString = $"SELECT * FROM {table}";

            try
            {
                connection = new(_connectionString);
                await Connect(connection);

                using var command = new SqlCommand(queryString, connection);
                using SqlDataReader reader = command.ExecuteReader();

                while (await reader.ReadAsync())
                {
                    cards.Add(CreateCard(reader));
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                if (ex.InnerException != null)
                    Trace.WriteLine(ex.InnerException.Message);
            }
            finally
            {
                if (connection != null)
                {
                    await Disconnect(connection);
                    connection.Dispose();
                }
            }

            return cards;
        }

        /// <summary>
        /// Creates a card based on the current row in the table.
        /// </summary>
        /// <param name="reader">The iterator over the current row in the table.</param>
        /// <returns>A card based on the table row.</returns>
        private static ICard CreateCard(SqlDataReader reader)
        {
            int id = reader.GetInt32(0);
            int level = reader.GetInt32(1);
            int sectorID = reader.GetInt32(2);
            int cost = reader.GetInt32(3);

            int effectAmount = reader.GetInt32(5);

            if (level != Constants.ColonyCardLevel)
            {
                int effect = reader.GetInt32(4);
                int? secondaryEffectAmount = !reader.IsDBNull(6) ? reader.GetInt32(6) : null;
                int deployedEffect = reader.GetInt32(7);
                int deployedEffectAmount = reader.GetInt32(8);
                int? secondaryDeployedEffectAmount = !reader.IsDBNull(9) ? reader.GetInt32(9) : null;

                if (reader.IsDBNull(10))
                {
                    return CardFactory.CreateStandardCard(id, level, sectorID, cost,
                        (ActionType)effect, effectAmount, secondaryEffectAmount,
                        (ActionType)deployedEffect, deployedEffectAmount, secondaryDeployedEffectAmount);
                }
                else
                {
                    int chargeEffect = reader.GetInt32(10);
                    int requiredChargeCubes = reader.GetInt32(11);
                    int chargeCubeLimit = reader.GetInt32(12);
                    int chargeCardType = reader.GetInt32(13);
                    int deployedChargeEffect = reader.GetInt32(14);
                    int deployedRequiredChargeCubes = reader.GetInt32(15);
                    int deployedChargeCubeLimit = reader.GetInt32(16);
                    int deployedChargeCardType = reader.GetInt32(17);

                    return new ChargeCard(id, level, sectorID, cost,
                        (ActionType)effect, effectAmount, secondaryEffectAmount,
                        (ActionType)deployedEffect, deployedEffectAmount, secondaryDeployedEffectAmount,
                        (ChargeActionType)chargeEffect, requiredChargeCubes, chargeCubeLimit, (ChargeCardType)chargeCardType,
                        (ChargeActionType)deployedChargeEffect, deployedRequiredChargeCubes, deployedChargeCubeLimit, (ChargeCardType)deployedChargeCardType);
                }
            }
            else
            {
                return CardFactory.CreateColonyCard(id, sectorID, cost, effectAmount);
            }

        }

    }
}
