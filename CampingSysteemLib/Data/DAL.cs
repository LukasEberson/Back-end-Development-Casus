using Microsoft.Data.SqlClient;

namespace CampingSystem
{
    public class DAL
    {
        private readonly string _connectionString;

        public Dictionary<int, CampingPlaatsReservering> campingPlaatsReserveringen = [];
        public Dictionary<int, CampingPlaats> campingPlaatsen = [];
        public Dictionary<int, CampingPlaatsType> campingPlaatsTypen = [];

        public DAL()
        {
            _connectionString = "<pleur 'm hiero>";

            if (_connectionString != null)
            {
                this.fetch();
            }
        }

        private int parseInt(SqlDataReader reader, int column)
        {
            return reader.IsDBNull(column) ? -1 : reader.GetInt32(column);
        }

        private void fetch()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    // selecteer tabellen die verbonden zijn via een 1 -> x relatie
                    // zo kunnen alle objecten uit die tabellen worden opgehaald in
                    // 1 sql command ipv meerdere
                    command.CommandText =
                        "SELECT "
                            + "R.ID, R.PlaatsID, R.AantalVolwassenen, R.AantalKinderenOnder7, R.AantalKinderenOnder12, R.AantalHonden, "
                            + "P.ID, P.TypeID, P.Nummer, "
                            + "T.ID "
                        + "FROM "
                            + "CampingPlaatsReservering as R "
                        // gebruik right joins zodat ook entiteiten waar de relatie
                        // leeg is opgehaald worden
                        + "RIGHT JOIN "
                             + "CampingPlaats as P on R.PlaatsID = P.ID "
                        + "RIGHT JOIN "
                             + "CampingPlaatsType as T on P.TypeID = T.ID "
                        + "ORDER BY "
                            // sorteer op omgekeerde volgorde van 1->x relaties
                            // dan hoeft iedere entiteit enkel een keer te worden
                            // gemaakt en niet later opgezocht
                            + "T.ID, P.ID, R.ID";

                    using (var reader = command.ExecuteReader())
                    {
                        CampingPlaatsType? type = null;
                        CampingPlaats? plaats = null;
                        CampingPlaatsReservering? reservering = null;

                        while (reader.Read())
                        {
                            int reserveringId = parseInt(reader, 0);
                            int aantalVolwassenen = parseInt(reader, 2);
                            int aantalKinderenOnder7 = parseInt(reader, 3);
                            int aantalKinderenOnder12 = parseInt(reader, 4);
                            int aantalHonden = parseInt(reader, 5);
                            int plaatsId = parseInt(reader, 6);
                            int plaatsNummer = parseInt(reader, 8);
                            int typeId = parseInt(reader, 9);

                            if (type == null || type.Id != typeId)
                            {
                                if (typeId == -1)
                                {
                                    type = null;
                                }
                                else
                                {
                                    type = new CampingPlaatsType()
                                    {
                                        Id = typeId
                                    };

                                    campingPlaatsTypen.Add(typeId, type);
                                }
                            }
                            if (plaats == null || plaats.Id != plaatsId)
                            {
                                if (plaatsId == -1)
                                {
                                    plaats = null;
                                }
                                else
                                {
                                    plaats = new CampingPlaats()
                                    {
                                        Id = plaatsId,
                                        Type = type,
                                        Nummer = plaatsNummer
                                    };

                                    campingPlaatsen.Add(plaatsId, plaats);
                                    if (type != null)
                                    {
                                        type.Plaatsen.Add(plaats);
                                    }
                                }
                            }
                            if (reservering == null || reservering.Id != reserveringId)
                            {
                                if (reserveringId == -1)
                                {
                                    reservering = null;
                                }
                                else
                                {
                                    reservering = new CampingPlaatsReservering()
                                    {
                                        Id = reserveringId,
                                        Plaats = plaats,
                                        AantalVolwassenen = aantalVolwassenen,
                                        AantalKinderenOnder7 = aantalKinderenOnder7,
                                        AantalKinderenOnder12 = aantalKinderenOnder12,
                                        AantalHonden = aantalHonden
                                    };

                                    campingPlaatsReserveringen.Add(reserveringId, reservering);
                                    if (plaats != null)
                                    {
                                        plaats.Reserveringen.Add(reservering);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public ICollection<CampingPlaatsReservering> GetCampingPlaatsReserveringen()
        {
            return campingPlaatsReserveringen.Values;
        }

        public CampingPlaatsReservering? GetCampingPlaatsReservering(int id)
        {
            return campingPlaatsReserveringen.GetValueOrDefault(id);
        }

        public void CreateCampingPlaatsReservering(CampingPlaatsReservering reservering)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "INSERT INTO "
                            + "CampingPlaatsReservering ( PlaatsID, AantalVolwassenen, AantalKinderenOnder7, AantalKinderenOnder12, AantalHonden ) "
                        + "VALUES "
                            + "( @p, @av, @ako7, @ako12, @ah )";
                    command.Parameters.AddWithValue("@p", reservering.Plaats?.Id);
                    command.Parameters.AddWithValue("@av", reservering.AantalVolwassenen);
                    command.Parameters.AddWithValue("@ako7", reservering.AantalKinderenOnder7);
                    command.Parameters.AddWithValue("@ako12", reservering.AantalKinderenOnder12);
                    command.Parameters.AddWithValue("@ah", reservering.AantalHonden);

                    command.ExecuteNonQuery();
                }
            }

            campingPlaatsReserveringen[reservering.Id] = reservering;
        }

        public void UpdateCampingPlaatsReservering(CampingPlaatsReservering reservering)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "UPDATE "
                            + "CampingPlaatsReservering "
                        + "SET "
                            + "PlaatsID = @p, "
                            + "AantalVolwassenen = @av, "
                            + "AantalKinderenOnder7 = @ako7, "
                            + "AantalKinderenOnder12 = @ako12, "
                            + "AantalHonden = @ah "
                        + "WHERE "
                            + "ID = @id";
                    command.Parameters.AddWithValue("@id", reservering.Id);
                    command.Parameters.AddWithValue("@p", reservering.Plaats?.Id);
                    command.Parameters.AddWithValue("@av", reservering.AantalVolwassenen);
                    command.Parameters.AddWithValue("@ako7", reservering.AantalKinderenOnder7);
                    command.Parameters.AddWithValue("@ako12", reservering.AantalKinderenOnder12);
                    command.Parameters.AddWithValue("@ah", reservering.AantalHonden);
                    
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteCampingPlaatsReservering(CampingPlaatsReservering reservering)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "DELETE FROM "
                            + "CampingPlaatsReservering "
                        + "WHERE "
                            + "ID = @id";
                    command.Parameters.AddWithValue("@id", reservering.Id);
                    
                    command.ExecuteNonQuery();
                }
            }

            campingPlaatsReserveringen.Remove(reservering.Id);
        }

        public ICollection<CampingPlaats> GetCampingPlaatsen()
        {
            return campingPlaatsen.Values;
        }

        public CampingPlaats? GetCampingPlaats(int id)
        {
            return campingPlaatsen.GetValueOrDefault(id);
        }

        public void CreateCampingPlaats(CampingPlaats plaats)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "INSERT INTO "
                            + "CampingPlaats ( TypeID, Nummer ) "
                        + "VALUES "
                            + " ( @t, @n )";
                    command.Parameters.AddWithValue("@t", plaats.Type?.Id);
                    command.Parameters.AddWithValue("@n", plaats.Nummer);

                    command.ExecuteNonQuery();
                }
            }

            campingPlaatsen[plaats.Id] = plaats;
        }

        public void UpdateCampingPlaats(CampingPlaats plaats)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "UPDATE "
                            + "CampingPlaats "
                        + "SET "
                            + "TypeID = @t, "
                            + "Nummer = @n "
                        + "WHERE "
                            + "ID = @id";
                    command.Parameters.AddWithValue("@id", plaats.Id);
                    command.Parameters.AddWithValue("@t", plaats.Type?.Id);
                    command.Parameters.AddWithValue("@n", plaats.Nummer);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteCampingPlaats(CampingPlaats plaats)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "DELETE FROM "
                            + "CampingPlaats "
                        + "WHERE "
                            + "ID = @id";
                    command.Parameters.AddWithValue("@id", plaats.Id);
                    
                    command.ExecuteNonQuery();
                }
            }

            campingPlaatsen.Remove(plaats.Id);
        }

        public ICollection<CampingPlaatsType> GetCampingPlaatsTypen()
        {
            return campingPlaatsTypen.Values;
        }

        public CampingPlaatsType? GetCampingPlaatsType(int id)
        {
            return campingPlaatsTypen.GetValueOrDefault(id);
        }

        public void CreateCampingPlaatsType(CampingPlaatsType type)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "INSERT INTO "
                            + "CampingPlaatsType "
                        + "DEFAULT VALUES";

                    command.ExecuteNonQuery();
                }
            }

            campingPlaatsTypen[type.Id] = type;
        }

        public void UpdateCampingPlaatsType(CampingPlaatsType type)
        {
            /*using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "UPDATE "
                            + "CampingPlaatsType "
                        + "SET "
                        + "WHERE "
                            + "ID = @id";
                    command.Parameters.AddWithValue("@id", type.Id);

                    command.ExecuteNonQuery();
                }
            }*/
        }

        public void DeleteCampingPlaatsType(CampingPlaatsType type)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "DELETE FROM "
                            + "CampingPlaatsType "
                        + "WHERE "
                            + "ID = @id";
                    command.Parameters.AddWithValue("@id", type.Id);
                    
                    command.ExecuteNonQuery();
                }
            }

            campingPlaatsTypen.Remove(type.Id);
        }
    }
}