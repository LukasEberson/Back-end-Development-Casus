using Microsoft.Data.SqlClient;

namespace CampingSystem
{
    public class DAL
    {
        private readonly string _connectionString;

        public DAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Haal alle camping plekken op
        public List<CampingPlaats> GetAllePlekken()
        {
            var plekken = new List<CampingPlaats>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SELECT * FROM CampingPlaatsken", connection);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                plekken.Add(new CampingPlaats
                {
                    Id = reader.GetInt32(0),
                    Nummer = reader.GetInt32(1),
                    Type = reader.GetString(2),
                    PrijsPerNacht = reader.GetDecimal(3)
                });
            }

            return plekken;
        }

        // Haal beschikbare plekken op voor bepaalde periode
        public List<CampingPlaats> GetBeschikbarePlekken(DateTime start, DateTime eind)
        {
            var plekken = new List<CampingPlaats>();

            using var connection = new SqlConnection(_connectionString);
            var sql = @"
                SELECT cp.* 
                FROM CampingPlaatsken cp
                WHERE cp.Id NOT IN (
                    SELECT CampingPlaatsId 
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
                plekken.Add(new CampingPlaats
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
        public int MaakReservering(CampingPlaatsReservering reservering)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            // Check eerst of de plek beschikbaar is
            var checkSql = @"
                SELECT COUNT(*) 
                FROM Reserveringen
                WHERE CampingPlaatsId = @CampingPlaatsId
                AND (StartDatum <= @EindDatum AND EindDatum >= @StartDatum)";

            using var checkCommand = new SqlCommand(checkSql, connection);
            checkCommand.Parameters.AddWithValue("@CampingPlaatsId", reservering.CampingPlaatsId);
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
                (CampingPlaatsId, KlantNaam, KlantEmail, StartDatum, EindDatum, TotaalPrijs)
                VALUES 
                (@CampingPlaatsId, @KlantNaam, @KlantEmail, @StartDatum, @EindDatum, @TotaalPrijs);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            using var insertCommand = new SqlCommand(insertSql, connection);
            insertCommand.Parameters.AddWithValue("@CampingPlaatsId", reservering.CampingPlaatsId);
            insertCommand.Parameters.AddWithValue("@KlantNaam", reservering.KlantNaam);
            insertCommand.Parameters.AddWithValue("@KlantEmail", reservering.KlantEmail);
            insertCommand.Parameters.AddWithValue("@StartDatum", reservering.StartDatum);
            insertCommand.Parameters.AddWithValue("@EindDatum", reservering.EindDatum);
            insertCommand.Parameters.AddWithValue("@TotaalPrijs", reservering.TotaalPrijs);

            return (int)insertCommand.ExecuteScalar();
        }

        // Bereken totaalprijs
        public decimal BerekenTotaalPrijs(int CampingPlaatsId, DateTime start, DateTime eind)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SELECT PrijsPerNacht FROM CampingPlaatsken WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", CampingPlaatsId);

            connection.Open();
            var prijsPerNacht = (decimal)command.ExecuteScalar();

            int aantalNachten = (eind - start).Days;
            return prijsPerNacht * aantalNachten;
        }
    }
}