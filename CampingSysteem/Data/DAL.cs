using System.Data.Common;
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

        public List<CampingPlaatsReservering> GetCampingPlaatsReserveringen()
        {
            List<CampingPlaatsReservering> reserveringen = new List<CampingPlaatsReservering>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT "
                            + "R.ID, R.PlaatsID, R.AantalVolwassenen, R.AantalKinderenOnder7, R.AantalKinderenOnder12, R.AantalHonden, "
                            + "P.ID, P.TypeID, P.Nummer, "
                            + "T.ID "
                        + "FROM "
                            + "CampingPlaatsReservering as R "
                        + "INNER JOIN "
                             + "CampingPlaats as P on R.PlaatsID = P.ID "
                        + "INNER JOIN "
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
                            int reserveringId = reader.GetInt32(0);
                            int aantalVolwassenen = reader.GetInt32(2);
                            int aantalKinderenOnder7 = reader.GetInt32(3);
                            int aantalKinderenOnder12 = reader.GetInt32(4);
                            int aantalHonden = reader.GetInt32(5);
                            int plaatsId = reader.GetInt32(6);
                            int plaatsNummer = reader.GetInt32(8);
                            int typeId = reader.GetInt32(9);

                            if (type == null || type.Id != typeId)
                            {
                                type = new CampingPlaatsType()
                                {
                                    Id = typeId
                                };
                            }
                            if (plaats == null || plaats.Id != plaatsId)
                            {
                                plaats = new CampingPlaats()
                                {
                                    Id = plaatsId,
                                    Type = type,
                                    Nummer = plaatsNummer
                                };

                                type.Plaatsen.Add(plaats);
                            }
                            if (reservering == null || reservering.Id != reserveringId)
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

                                plaats.Reserveringen.Add(reservering);
                                reserveringen.Add(reservering);
                            }
                        }
                    }
                }
            }

            return reserveringen;
        }

        public CampingPlaatsReservering? GetCampingPlaatsReservering(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT "
                            + "R.ID, R.PlaatsID, R.AantalVolwassenen, R.AantalKinderenOnder7, R.AantalKinderenOnder12, R.AantalHonden, "
                            + "P.ID, P.TypeID, P.Nummer, "
                            + "T.ID "
                        + "FROM "
                            + "CampingPlaatsReservering as R "
                        + "INNER JOIN "
                             + "CampingPlaats as P on R.PlaatsID = P.ID "
                        + "INNER JOIN "
                             + "CampingPlaatsType as T on P.TypeID = T.ID "
                        + "WHERE "
                            + "R.ID = @id";
                    command.Parameters.AddWithValue("@id", id);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }
                        else
                        {
                            int reserveringId = reader.GetInt32(0);
                            int aantalVolwassenen = reader.GetInt32(2);
                            int aantalKinderenOnder7 = reader.GetInt32(3);
                            int aantalKinderenOnder12 = reader.GetInt32(4);
                            int aantalHonden = reader.GetInt32(5);
                            int plaatsId = reader.GetInt32(6);
                            int plaatsNummer = reader.GetInt32(8);
                            int typeId = reader.GetInt32(9);

                            CampingPlaatsType type = new CampingPlaatsType()
                            {
                                Id = typeId
                            };
                            CampingPlaats plaats = new CampingPlaats()
                            {
                                Id = plaatsId,
                                Type = type,
                                Nummer = plaatsNummer
                            };
                            CampingPlaatsReservering reservering = new CampingPlaatsReservering()
                            {
                                Id = reserveringId,
                                Plaats = plaats,
                                AantalVolwassenen = aantalVolwassenen,
                                AantalKinderenOnder7 = aantalKinderenOnder7,
                                AantalKinderenOnder12 = aantalKinderenOnder12,
                                AantalHonden = aantalHonden
                            };

                            type.Plaatsen.Add(plaats);
                            plaats.Reserveringen.Add(reservering);

                            return reservering;
                        }
                    }
                }
            }
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
        }

        public List<CampingPlaats> GetCampingPlaatsen()
        {
            List<CampingPlaats> plaatsen = new List<CampingPlaats>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT "
                            + "R.ID, R.PlaatsID, R.AantalVolwassenen, R.AantalKinderenOnder7, R.AantalKinderenOnder12, R.AantalHonden, "
                            + "P.ID, P.TypeID, P.Nummer, "
                            + "T.ID "
                        + "FROM "
                            + "CampingPlaats as P "
                        + "INNER JOIN "
                             + "CampingPlaatsType as T on P.TypeID = T.ID "
                        + "INNER JOIN "
                             + "CampingPlaatsReservering as R on R.PlaatsID = P.ID "
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
                            int reserveringId = reader.GetInt32(0);
                            int aantalVolwassenen = reader.GetInt32(2);
                            int aantalKinderenOnder7 = reader.GetInt32(3);
                            int aantalKinderenOnder12 = reader.GetInt32(4);
                            int aantalHonden = reader.GetInt32(5);
                            int plaatsId = reader.GetInt32(6);
                            int plaatsNummer = reader.GetInt32(8);
                            int typeId = reader.GetInt32(9);

                            if (type == null || type.Id != typeId)
                            {
                                type = new CampingPlaatsType()
                                {
                                    Id = typeId
                                };
                            }
                            if (plaats == null || plaats.Id != plaatsId)
                            {
                                plaats = new CampingPlaats()
                                {
                                    Id = plaatsId,
                                    Type = type,
                                    Nummer = plaatsNummer
                                };

                                type.Plaatsen.Add(plaats);
                                plaatsen.Add(plaats);
                            }
                            if (reservering == null || reservering.Id != reserveringId)
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

                                plaats.Reserveringen.Add(reservering);
                            }
                        }
                    }
                }
            }

            return plaatsen;
        }

        public CampingPlaats? GetCampingPlaats(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT "
                            + "R.ID, R.PlaatsID, R.AantalVolwassenen, R.AantalKinderenOnder7, R.AantalKinderenOnder12, R.AantalHonden, "
                            + "P.ID, P.TypeID, P.Nummer, "
                            + "T.ID "
                        + "FROM "
                            + "CampingPlaats as P "
                        + "INNER JOIN "
                             + "CampingPlaatsType as T on P.TypeID = T.ID "
                        + "INNER JOIN "
                             + "CampingPlaatsReservering as R on R.PlaatsID = P.ID "
                        + "WHERE "
                            + "P.ID = @id";
                    command.Parameters.AddWithValue("@id", id);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }
                        else
                        {
                            int reserveringId = reader.GetInt32(0);
                            int aantalVolwassenen = reader.GetInt32(2);
                            int aantalKinderenOnder7 = reader.GetInt32(3);
                            int aantalKinderenOnder12 = reader.GetInt32(4);
                            int aantalHonden = reader.GetInt32(5);
                            int plaatsId = reader.GetInt32(6);
                            int plaatsNummer = reader.GetInt32(8);
                            int typeId = reader.GetInt32(9);

                            CampingPlaatsType type = new CampingPlaatsType()
                            {
                                Id = typeId
                            };
                            CampingPlaats plaats = new CampingPlaats()
                            {
                                Id = plaatsId,
                                Type = type,
                                Nummer = plaatsNummer
                            };
                            CampingPlaatsReservering reservering = new CampingPlaatsReservering()
                            {
                                Id = reserveringId,
                                Plaats = plaats,
                                AantalVolwassenen = aantalVolwassenen,
                                AantalKinderenOnder7 = aantalKinderenOnder7,
                                AantalKinderenOnder12 = aantalKinderenOnder12,
                                AantalHonden = aantalHonden
                            };

                            type.Plaatsen.Add(plaats);
                            plaats.Reserveringen.Add(reservering);

                            return plaats;
                        }
                    }
                }
            }
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
        }

        public List<CampingPlaatsType> GetCampingPlaatsTypen()
        {
            List<CampingPlaatsType> typen = new List<CampingPlaatsType>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT "
                            + "R.ID, R.PlaatsID, R.AantalVolwassenen, R.AantalKinderenOnder7, R.AantalKinderenOnder12, R.AantalHonden, "
                            + "P.ID, P.TypeID, P.Nummer, "
                            + "T.ID "
                        + "FROM "
                            + "CampingPlaatsType as T "
                        + "INNER JOIN "
                             + "CampingPlaats as P on P.TypeID = T.ID "
                        + "INNER JOIN "
                             + "CampingPlaatsReservering as R on R.PlaatsID = P.ID "
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
                            int reserveringId = reader.GetInt32(0);
                            int aantalVolwassenen = reader.GetInt32(2);
                            int aantalKinderenOnder7 = reader.GetInt32(3);
                            int aantalKinderenOnder12 = reader.GetInt32(4);
                            int aantalHonden = reader.GetInt32(5);
                            int plaatsId = reader.GetInt32(6);
                            int plaatsNummer = reader.GetInt32(8);
                            int typeId = reader.GetInt32(9);

                            if (type == null || type.Id != typeId)
                            {
                                type = new CampingPlaatsType()
                                {
                                    Id = typeId
                                };

                                typen.Add(type);
                            }
                            if (plaats == null || plaats.Id != plaatsId)
                            {
                                plaats = new CampingPlaats()
                                {
                                    Id = plaatsId,
                                    Type = type,
                                    Nummer = plaatsNummer
                                };

                                type.Plaatsen.Add(plaats);
                            }
                            if (reservering == null || reservering.Id != reserveringId)
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

                                plaats.Reserveringen.Add(reservering);
                            }
                        }
                    }
                }
            }

            return typen;
        }

        public CampingPlaatsType? GetCampingPlaatsType(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT "
                            + "R.ID, R.PlaatsID, R.AantalVolwassenen, R.AantalKinderenOnder7, R.AantalKinderenOnder12, R.AantalHonden, "
                            + "P.ID, P.TypeID, P.Nummer, "
                            + "T.ID "
                        + "FROM "
                            + "CampingPlaatsType as T "
                        + "INNER JOIN "
                             + "CampingPlaats as P on P.TypeID = T.ID "
                        + "INNER JOIN "
                             + "CampingPlaatsReservering as R on R.PlaatsID = P.ID "
                        + "WHERE "
                            + "T.ID = @id";
                    command.Parameters.AddWithValue("@id", id);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }
                        else
                        {
                            int reserveringId = reader.GetInt32(0);
                            int aantalVolwassenen = reader.GetInt32(2);
                            int aantalKinderenOnder7 = reader.GetInt32(3);
                            int aantalKinderenOnder12 = reader.GetInt32(4);
                            int aantalHonden = reader.GetInt32(5);
                            int plaatsId = reader.GetInt32(6);
                            int plaatsNummer = reader.GetInt32(8);
                            int typeId = reader.GetInt32(9);

                            CampingPlaatsType type = new CampingPlaatsType()
                            {
                                Id = typeId
                            };
                            CampingPlaats plaats = new CampingPlaats()
                            {
                                Id = plaatsId,
                                Type = type,
                                Nummer = plaatsNummer
                            };
                            CampingPlaatsReservering reservering = new CampingPlaatsReservering()
                            {
                                Id = reserveringId,
                                Plaats = plaats,
                                AantalVolwassenen = aantalVolwassenen,
                                AantalKinderenOnder7 = aantalKinderenOnder7,
                                AantalKinderenOnder12 = aantalKinderenOnder12,
                                AantalHonden = aantalHonden
                            };

                            type.Plaatsen.Add(plaats);
                            plaats.Reserveringen.Add(reservering);

                            return type;
                        }
                    }
                }
            }
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
        }

        public void UpdateCampingPlaatsType(CampingPlaatsType type)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    /*command.CommandText =
                        "UPDATE "
                            + "CampingPlaatsType "
                        + "SET "
                        + "WHERE "
                            + "ID = @id";
                    command.Parameters.AddWithValue("@id", type.Id);

                    command.ExecuteNonQuery();*/
                }
            }
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
        }
    }
}