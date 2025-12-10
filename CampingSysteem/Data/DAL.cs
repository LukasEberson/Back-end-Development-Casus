using Microsoft.Data.SqlClient;

namespace CampingSystem
{
    public class CampingRepository
    {
        private readonly string _connectionString;

        public CampingRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Haal alle camping plekken op
        public List<CampingPlek> GetAllePlekken()
        {
            var plekken = new List<CampingPlek>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SELECT * FROM CampingPlekken", connection);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                plekken.Add(new CampingPlek
                {
                    Id = reader.GetInt32(0),
                    Nummer = reader.GetString(1),
                    Type = reader.GetString(2),
                    PrijsPerNacht = reader.GetDecimal(3)
                });
            }

            return plekken;
        }

        // Haal beschikbare plekken op voor bepaalde periode
        public List<CampingPlek> GetBeschikbarePlekken(DateTime start, DateTime eind)
        {
            var plekken = new List<CampingPlek>();

            using var connection = new SqlConnection(_connectionString);
            var sql = @"
                SELECT cp.* 
                FROM CampingPlekken cp
                WHERE cp.Id NOT IN (
                    SELECT CampingPlekId 
                    FROM Reserveringen
                    WHERE (StartDatum <= @Eind AND EindDatum >= @Start)
                )";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Start", start);
            command.Parameters.AddWithValue("@Eind", eind);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                plekken.Add(new CampingPlek
                {
                    Id = reader.GetInt32(0),
                    Nummer = reader.GetString(1),
                    Type = reader.GetString(2),
                    PrijsPerNacht = reader.GetDecimal(3)
                });
            }

            return plekken;
        }

        // Maak een nieuwe reservering
        public int MaakReservering(Reservering reservering)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            // Check eerst of de plek beschikbaar is
            var checkSql = @"
                SELECT COUNT(*) 
                FROM Reserveringen
                WHERE CampingPlekId = @CampingPlekId
                AND (StartDatum <= @EindDatum AND EindDatum >= @StartDatum)";

            using var checkCommand = new SqlCommand(checkSql, connection);
            checkCommand.Parameters.AddWithValue("@CampingPlekId", reservering.CampingPlekId);
            checkCommand.Parameters.AddWithValue("@StartDatum", reservering.StartDatum);
            checkCommand.Parameters.AddWithValue("@EindDatum", reservering.EindDatum);

            var aantalConflicten = (int)checkCommand.ExecuteScalar();

            if (aantalConflicten > 0)
            {
                throw new Exception("Deze plek is al gereserveerd voor de gekozen periode");
            }

            // Maak de reservering
            var insertSql = @"
                INSERT INTO Reserveringen 
                (CampingPlekId, KlantNaam, KlantEmail, StartDatum, EindDatum, TotaalPrijs)
                VALUES 
                (@CampingPlekId, @KlantNaam, @KlantEmail, @StartDatum, @EindDatum, @TotaalPrijs);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            using var insertCommand = new SqlCommand(insertSql, connection);
            insertCommand.Parameters.AddWithValue("@CampingPlekId", reservering.CampingPlekId);
            insertCommand.Parameters.AddWithValue("@KlantNaam", reservering.KlantNaam);
            insertCommand.Parameters.AddWithValue("@KlantEmail", reservering.KlantEmail);
            insertCommand.Parameters.AddWithValue("@StartDatum", reservering.StartDatum);
            insertCommand.Parameters.AddWithValue("@EindDatum", reservering.EindDatum);
            insertCommand.Parameters.AddWithValue("@TotaalPrijs", reservering.TotaalPrijs);

            return (int)insertCommand.ExecuteScalar();
        }

        // Bereken totaalprijs
        public decimal BerekenTotaalPrijs(int campingPlekId, DateTime start, DateTime eind)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SELECT PrijsPerNacht FROM CampingPlekken WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", campingPlekId);

            connection.Open();
            var prijsPerNacht = (decimal)command.ExecuteScalar();

            int aantalNachten = (eind - start).Days;
            return prijsPerNacht * aantalNachten;
        }
    }
}