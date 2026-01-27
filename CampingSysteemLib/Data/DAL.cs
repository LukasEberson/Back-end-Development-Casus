using CampingSystem;
using Microsoft.Data.SqlClient;

namespace CampingSystem
{
    public class DAL
    {
        private readonly string _connectionString;

        public Dictionary<int, CampingPlaatsReservering> campingPlaatsReserveringen = [];
        public Dictionary<int, CampingPlaats> campingPlaatsen = [];
        public Dictionary<int, CampingPlaatsType> campingPlaatsTypen = [];
        public Dictionary<int, CampingRekening> campingRekeningen = [];
        public Dictionary<int, CampingReservering> campingReserveringen = [];
        public Dictionary<int, CampingPlaatsTarieven> campingPlaatsTarieven = [];


        public DAL()
        {
            _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=CampingSysteemDb;Trusted_Connection=True;TrustServerCertificate=True;";

            if (_connectionString != null)
            {
                this.fetch();
            }
        }

        private int parseInt(SqlDataReader reader, int column)
        {
            return reader.IsDBNull(column) ? -1 : reader.GetInt32(column);
        }

        private string parseString(SqlDataReader reader, int column)
        {
            return reader.IsDBNull(column) ? "" : reader.GetString(column).Trim();
        }

        private DateTime parseDate(SqlDataReader reader, int column)
        {
            return reader.IsDBNull(column) ? DateTime.MinValue : reader.GetDateTime(column);
        }

        private bool parseBool(SqlDataReader reader, int column)
        {
            return !reader.IsDBNull(column) && reader.GetBoolean(column);
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
                            + "R.ID, R.PlaatsID, R.ReserveringID, R.TarievenID, R.AantalVolwassenen, R.AantalKinderenOnder7, R.AantalKinderenOnder12, R.AantalHonden, "
                            + "P.ID, P.TypeID, P.Nummer, "
                            + "T.ID, "
                            + "CR.ID, CR.RekeningID, CR.Naam, CR.Emailadres, CR.Telefoonnummer, CR.BeginDatum, CR.EindDatum, "
                            + "RK.ID, RK.ToeristenBelasting, RK.Korting, RK.Betaald, "
                            + "PT.ID, PT.GeldingVan, PT.GeldigTot, PT.TariefVolwassenen, PT.TariefKinderenOnder7, PT.TariefKinderenOnder12, PT.TariefHonden, PT.TariefElectriciteit "
                        + "FROM "
                            + "CampingPlaatsReservering as R "
                        // gebruik right joins zodat ook entiteiten waar de relatie
                        // leeg is opgehaald worden
                        + "RIGHT JOIN "
                             + "CampingPlaats as P on R.PlaatsID = P.ID "
                        + "RIGHT JOIN "
                             + "CampingPlaatsType as T on P.TypeID = T.ID "
                        + "RIGHT JOIN "
                            + "CampingReservering as CR on R.ReserveringID = CR.ID "
                        + "RIGHT JOIN "
                            + "CampingRekening as RK on CR.RekeningID = RK.ID "
                        + "RIGHT JOIN "
                            + "CampingPlaatsTarieven as PT on R.TatievenID = PT.ID "
                        + "ORDER BY "
                            // sorteer op omgekeerde volgorde van 1->x relaties
                            // dan hoeft iedere entiteit enkel een keer te worden
                            // gemaakt en niet later opgezocht
                            + "T.ID, P.ID, RK.ID, CR.ID, PT, R.ID";



                    using (var reader = command.ExecuteReader())
                    {
                        CampingPlaatsType? type = null;
                        CampingPlaats? plaats = null;
                        CampingPlaatsReservering? plaatsReservering = null;
                        CampingReservering? reservering = null;
                        CampingRekening? rekening = null;
                        CampingPlaatsTarieven? tarieven = null;

                        while (reader.Read())
                        {
                            int plaatsReserveringId = parseInt(reader, 0);
                            int plaatsId = parseInt(reader, 8);
                            int typeId = parseInt(reader, 11);
                            int reserveringId = parseInt(reader, 12);
                            int rekeningId = parseInt(reader, 19);
                            int tarievenId = parseInt(reader, 23);

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
                                    int nummer = parseInt(reader, 10);

                                    // type aflezen VOOR plaats!
                                    plaats = new CampingPlaats()
                                    {
                                        Id = plaatsId,
                                        Type = type,
                                        Nummer = nummer
                                    };

                                    campingPlaatsen.Add(plaatsId, plaats);
                                    if (type != null)
                                    {
                                        type.Plaatsen.Add(plaats);
                                    }
                                }
                            }
                            if (rekening == null || rekening.Id != rekeningId)
                            {
                                if (rekeningId == -1)
                                {
                                    rekening = null;
                                }
                                else
                                {
                                    int toeristenBelasting = parseInt(reader, 20);
                                    int korting = parseInt(reader, 21);
                                    bool betaald = parseBool(reader, 22);

                                    rekening = new CampingRekening()
                                    {
                                        Id = rekeningId,
                                        ToeristenBelasting = toeristenBelasting,
                                        Korting = korting,
                                        Betaald = betaald
                                    };

                                    campingRekeningen.Add(rekeningId, rekening);
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
                                    string naam = parseString(reader, 14);
                                    string emailadres = parseString(reader, 15);
                                    string telefoonnummer = parseString(reader, 16);
                                    DateTime beginDatum = parseDate(reader, 17);
                                    DateTime eindDatum = parseDate(reader, 18);

                                    // rekening aflezen VOOR reservering!
                                    reservering = new CampingReservering()
                                    {
                                        Id = reserveringId,
                                        Rekening = rekening,
                                        Naam = naam,
                                        Emailadres = emailadres,
                                        Telefoonnummer = telefoonnummer,
                                        BeginDatum = beginDatum,
                                        EindDatum = eindDatum
                                    };

                                    campingReserveringen.Add(reserveringId, reservering);
                                    if (rekening != null)
                                    {
                                        rekening.Reserveringen.Add(reservering);
                                    }
                                }
                            }
                            if (tarieven == null || tarieven.Id != tarievenId)
                            {
                                if (tarievenId == -1)
                                {
                                    tarieven = null;
                                }
                                else
                                {
                                    DateTime? geldigVan = parseDate(reader, 24);
                                    DateTime? geldigTot = parseDate(reader, 25);
                                    int tariefVolwassenen = parseInt(reader, 26);
                                    int tariefKinderenOnder7 = parseInt(reader, 27);
                                    int tariefKinderenOnder12 = parseInt(reader, 28);
                                    int tariefHonden = parseInt(reader, 29);
                                    int tariefElectriciteit = parseInt(reader, 30);

                                    // type aflezen VOOR tarieven!
                                    tarieven = new CampingPlaatsTarieven()
                                    {
                                        Id = tarievenId,
                                        Type = type,
                                        GeldigVan = geldigVan,
                                        GeldigTot = geldigTot,
                                        TariefVolwassenen = tariefVolwassenen,
                                        TariefKinderenOnder7 = tariefKinderenOnder7,
                                        TariefKinderenOnder12 = tariefKinderenOnder12,
                                        TariefHonden = tariefHonden,
                                        TariefElectriciteit = tariefElectriciteit
                                    };

                                    campingPlaatsTarieven.Add(tarievenId, tarieven);
                                    if (type != null)
                                    {
                                        type.Tarieven.Add(tarieven);
                                    }
                                }
                            }
                            if (plaatsReservering == null || plaatsReservering.Id != plaatsReserveringId)
                            {
                                if (plaatsReserveringId == -1)
                                {
                                    plaatsReservering = null;
                                }
                                else
                                {
                                    int aantalVolwassenen = parseInt(reader, 4);
                                    int aantalKinderenOnder7 = parseInt(reader, 5);
                                    int aantalKinderenOnder12 = parseInt(reader, 6);
                                    int aantalHonden = parseInt(reader, 7);

                                    // plaats, reservering, en tarieven aflezen VOOR plaats reservering!
                                    plaatsReservering = new CampingPlaatsReservering()
                                    {
                                        Id = plaatsReserveringId,
                                        Reservering = reservering,
                                        Plaats = plaats,
                                        Tarieven = tarieven,
                                        AantalVolwassenen = aantalVolwassenen,
                                        AantalKinderenOnder7 = aantalKinderenOnder7,
                                        AantalKinderenOnder12 = aantalKinderenOnder12,
                                        AantalHonden = aantalHonden
                                    };

                                    campingPlaatsReserveringen.Add(plaatsReserveringId, plaatsReservering);
                                    if (plaats != null)
                                    {
                                        plaats.Reserveringen.Add(plaatsReservering);
                                    }
                                    if (reservering != null)
                                    {
                                        reservering.PlaatsReserveringen.Add(plaatsReservering);
                                    }
                                    if (tarieven != null)
                                    {
                                        tarieven.Reserveringen.Add(plaatsReservering);
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
                    campingPlaatsen[plaats.Id] = plaats;
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

        public ICollection<CampingPlaatsTarieven> GetCampingPlaatsTarieven() => campingPlaatsTarieven.Values;
        public CampingPlaatsTarieven? GetCampingPlaatsTarieven(int id) => campingPlaatsTarieven.GetValueOrDefault(id);

        public void CreateCampingPlaatsTarieven(CampingPlaatsTarieven t)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();

            command.CommandText =
                "INSERT INTO CampingPlaatsTarieven (TypeID, GeldigVan, GeldigTot, TariefVolwassenen, TafiefKinderenOnder7, TariefKinderenOnder12, TariefHonden, TariefElectriciteit) " +
                "VALUES (@typeId, @van, @tot, @tv, @tk7, @tk12, @th, @te); " +
                "SELECT CAST(SCOPE_IDENTITY() as int);";

            command.Parameters.AddWithValue("@typeId", (object?)t.Type?.Id ?? DBNull.Value);
            command.Parameters.AddWithValue("@van", (object?)t.GeldigVan ?? DBNull.Value);
            command.Parameters.AddWithValue("@tot", (object?)t.GeldigTot ?? DBNull.Value);
            command.Parameters.AddWithValue("@tv", t.TariefVolwassenen);
            command.Parameters.AddWithValue("@tk7", t.TariefKinderenOnder7);
            command.Parameters.AddWithValue("@tk12", t.TariefKinderenOnder12);
            command.Parameters.AddWithValue("@th", t.TariefHonden);
            command.Parameters.AddWithValue("@te", t.TariefElectriciteit);

            t.Id = (int)command.ExecuteScalar();
            campingPlaatsTarieven[t.Id] = t;
        }

        public void UpdateCampingPlaatsTarieven(CampingPlaatsTarieven t)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();

            command.CommandText =
                "UPDATE CampingPlaatsTarieven SET " +
                "TypeID=@typeId, GeldigVan=@van, GeldigTot=@tot, TariefVolwassenen=@tv, TafiefKinderenOnder7=@tk7, TariefKinderenOnder12=@tk12, TariefHonden=@th, TariefElectriciteit=@te " +
                "WHERE ID=@id";

            command.Parameters.AddWithValue("@id", t.Id);
            command.Parameters.AddWithValue("@typeId", (object?)t.Type?.Id ?? DBNull.Value);
            command.Parameters.AddWithValue("@van", (object?)t.GeldigVan ?? DBNull.Value);
            command.Parameters.AddWithValue("@tot", (object?)t.GeldigTot ?? DBNull.Value);
            command.Parameters.AddWithValue("@tv", t.TariefVolwassenen);
            command.Parameters.AddWithValue("@tk7", t.TariefKinderenOnder7);
            command.Parameters.AddWithValue("@tk12", t.TariefKinderenOnder12);
            command.Parameters.AddWithValue("@th", t.TariefHonden);
            command.Parameters.AddWithValue("@te", t.TariefElectriciteit);

            command.ExecuteNonQuery();
        }

        public void DeleteCampingPlaatsTarieven(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();

            command.CommandText = "DELETE FROM CampingPlaatsTarieven WHERE ID=@id";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();

            campingPlaatsTarieven.Remove(id);
        }

        public ICollection<CampingReservering> GetCampingReserveringen() => campingReserveringen.Values;
        public CampingReservering? GetCampingReservering(int id) => campingReserveringen.GetValueOrDefault(id);

        public void CreateCampingReservering(CampingReservering r)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();

            command.CommandText =
                "INSERT INTO CampingReservering (RekeningID, Naam, Emailadres, Telefoonnummer, BeginDatum, EindDatum) " +
                "VALUES (@rekId, @naam, @email, @tel, @begin, @eind); " +
                "SELECT CAST(SCOPE_IDENTITY() as int);";

            command.Parameters.AddWithValue("@rekId", (object?)r.Rekening?.Id ?? DBNull.Value);
            command.Parameters.AddWithValue("@naam", (object?)r.Naam ?? DBNull.Value);
            command.Parameters.AddWithValue("@email", (object?)r.Emailadres ?? DBNull.Value);
            command.Parameters.AddWithValue("@tel", (object?)r.Telefoonnummer ?? DBNull.Value);
            command.Parameters.AddWithValue("@begin", (object?)r.BeginDatum ?? DBNull.Value);
            command.Parameters.AddWithValue("@eind", (object?)r.EindDatum ?? DBNull.Value);

            r.Id = (int)command.ExecuteScalar();
            campingReserveringen[r.Id] = r;
        }

        public void UpdateCampingReservering(CampingReservering r)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();

            command.CommandText =
                "UPDATE CampingReservering SET " +
                "RekeningID=@rekId, Naam=@naam, Emailadres=@email, Telefoonnummer=@tel, BeginDatum=@begin, EindDatum=@eind " +
                "WHERE ID=@id";

            command.Parameters.AddWithValue("@id", r.Id);
            command.Parameters.AddWithValue("@rekId", (object?)r.Rekening?.Id ?? DBNull.Value);
            command.Parameters.AddWithValue("@naam", (object?)r.Naam ?? DBNull.Value);
            command.Parameters.AddWithValue("@email", (object?)r.Emailadres ?? DBNull.Value);
            command.Parameters.AddWithValue("@tel", (object?)r.Telefoonnummer ?? DBNull.Value);
            command.Parameters.AddWithValue("@begin", (object?)r.BeginDatum ?? DBNull.Value);
            command.Parameters.AddWithValue("@eind", (object?)r.EindDatum ?? DBNull.Value);

            command.ExecuteNonQuery();
        }

        public void DeleteCampingReservering(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();

            command.CommandText = "DELETE FROM CampingReservering WHERE ID=@id";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();

            campingReserveringen.Remove(id);
        }

        public ICollection<CampingRekening> GetCampingRekeningen()
        {
            return campingRekeningen.Values;
        }

        public CampingRekening? GetCampingRekening(int id)
        {
            return campingRekeningen.GetValueOrDefault(id);
        }

        public void CreateCampingRekening(CampingRekening r)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText =
                "INSERT INTO CampingRekening (ToeristenBelasting, Korting, Betaald) " +
                "VALUES (@tb, @k, @b); " +
                "SELECT CAST(SCOPE_IDENTITY() as int);";

            command.Parameters.AddWithValue("@tb", r.ToeristenBelasting);
            command.Parameters.AddWithValue("@k", r.Korting);
            command.Parameters.AddWithValue("@b", r.Betaald);

            r.Id = (int)command.ExecuteScalar();
            campingRekeningen[r.Id] = r;
        }

        public void UpdateCampingRekening(CampingRekening r)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText =
                "UPDATE CampingRekening SET " +
                "ToeristenBelasting=@tb, Korting=@k, Betaald=@b " +
                "WHERE ID=@id";

            command.Parameters.AddWithValue("@id", r.Id);
            command.Parameters.AddWithValue("@tb", r.ToeristenBelasting);
            command.Parameters.AddWithValue("@k", r.Korting);
            command.Parameters.AddWithValue("@b", r.Betaald);

            command.ExecuteNonQuery();
            campingRekeningen[r.Id] = r;
        }

        public void DeleteCampingRekening(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM CampingRekening WHERE ID=@id";
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
            campingRekeningen.Remove(id);
        }
    }
}