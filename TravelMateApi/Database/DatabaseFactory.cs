using System.Collections.Generic;
using System.Linq;
using TravelMateApi.Models;

namespace TravelMateApi.Database
{
    public class DatabaseFactory
    {
        public void SaveJourneyToDb(DbJourney dbJourney)
        {
            using (var context = new DatabaseContext())
            {
                // Creates the database if not exists
                context.Database.EnsureCreated();
                context.Journeys.AddIfNotExists(dbJourney, x => x.Uid.Equals(dbJourney.Uid) && x.Position.Equals(dbJourney.Position));
                context.SaveChanges();
            }
        }

        public void SaveLines(IEnumerable<DbLine> dbLines)
        {
            using (var context = new DatabaseContext())
            {
                // Creates the database if not exists
                context.Database.EnsureCreated();
                foreach (var dbLine in dbLines)
                {
                    context.Lines.AddIfNotExists(dbLine);
                }
                context.SaveChanges();
            }
        }

        public void SaveJourneyLines(IEnumerable<DbJourneyLine> dbJourneyLines)
        {
            using (var context = new DatabaseContext())
            {
                // Creates the database if not exists
                context.Database.EnsureCreated();
                foreach (var journeyLine in dbJourneyLines)
                {
                    context.JourneyLines.AddIfNotExists(journeyLine);
                }
                context.SaveChanges();
            }
        }

        public IEnumerable<DbLine> GetLines()
        {
            IEnumerable<DbLine> dbLines = new List<DbLine>();
            using (var context = new DatabaseContext())
            {
                if (context.Lines != null && context.Lines.Any())
                {
                    dbLines = context.Lines.ToList();
                }
            }

            return dbLines;
        }

        public IEnumerable<RealTimeDisruption> GetDisruptionTokens(IEnumerable<RealTimeDisruption> lines)
        {
            var disruptions = new List<RealTimeDisruption>();
            using (var context = new DatabaseContext())
            {
                if (context.JourneyLines != null && context.JourneyLines.Any())
                {
                    foreach (var line in lines)
                    {
                        //dbJourneyLines.AddRange(context.JourneyLines.Where(journeyLine => journeyLine.ModeId.Equals(line.ModeId)).ToList());
                        foreach (var contextJourneyLine in context.JourneyLines)
                        {
                            if (contextJourneyLine.ModeId.Equals(line.ModeId))
                            {
                                line.Uid = contextJourneyLine.Uid;
                                disruptions.Add(line);
                            }
                        }
                    }
                }
            }

            return disruptions;
        }

        public DbAccount GetAccountByUid(string uid)
        {
            var dbAccount = new DbAccount();
            using (var context = new DatabaseContext())
            {
                if (context.Lines != null && context.Lines.Any())
                {
                    dbAccount = context.Accounts.FirstOrDefault(account => account.Uid.Equals(uid));
                }
            }

            return dbAccount;
        }

        public void RefreshToken(string uid, string token)
        {
            using (var context = new DatabaseContext())
            {
                // Creates the database if not exists
                context.Database.EnsureCreated();
                var account = context.Accounts.FirstOrDefault(bdAccount => bdAccount.Uid.Equals(uid));
                if (account != null)
                {
                    account.Token = token;
                }
                else
                {
                    var dbAcc = new DbAccount {Token = token, Uid = uid};
                    context.Accounts.Add(dbAcc);
                }
                context.SaveChanges();
            }
        }
    }
}
