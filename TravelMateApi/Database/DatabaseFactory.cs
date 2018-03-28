using System;
using System.Collections.Generic;
using System.Globalization;
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
                context.Journeys.AddIfNotExists(dbJourney, x => !x.AccountId.Equals(dbJourney.AccountId) && !x.Route.Equals(dbJourney.Route));
                context.SaveChanges();
            }
        }

        public DbJourney GetJourneyFromDb(DbJourney journey)
        {
            using (var context = new DatabaseContext())
            {
                return context.Journeys.FirstOrDefault(dbJourney => dbJourney == journey);
            }
        }

        public void SaveLines(IEnumerable<DbLine> lines)
        {
            using (var context = new DatabaseContext())
            {
                // Creates the database if not exists
                context.Database.EnsureCreated();
                foreach (var line in lines)
                {
                    context.Lines.AddIfNotExists(line, dbLine => dbLine.Name.Equals(line.Name));
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
                    context.JourneyLines.AddIfNotExists(journeyLine, dbjourneyLine 
                        => dbjourneyLine.JourneyId.Equals(journeyLine.JourneyId) && dbjourneyLine.ModeId.Equals(journeyLine.ModeId));
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

        public DbLine GetLine(string lineId)
        {
            var dbLine = new DbLine();
            using (var context = new DatabaseContext())
            {
                if (context.Lines != null && context.Lines.Any())
                {
                    dbLine = context.Lines.FirstOrDefault(line => line.Name.Equals(lineId));
                }
            }
            return dbLine;
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
                            if (!contextJourneyLine.ModeId.Equals(line.LineId)) continue;
                            line.JourneyId = contextJourneyLine.JourneyId;
                            disruptions.Add(line);
                        }
                    }
                }
            }

            return disruptions;
        }

        public DbAccount GetAccountByJourneyId(int journeyId)
        {
            var dbAccount = new DbAccount();
            using (var context = new DatabaseContext())
            {
                var dbJourney = new DbJourney();
                if (context.Accounts != null && context.Accounts.Any())
                {
                    dbJourney = context.Journeys.FirstOrDefault(journey => journey.Id == journeyId);
                }
                if (dbJourney != null && context.Accounts != null && context.Accounts.Any())
                {
                    dbAccount = context.Accounts.FirstOrDefault(account => account.Id.Equals(dbJourney.AccountId));
                }
            }

            return dbAccount;
        }

        public DbAccount GetAccountByUid(string uid)
        {
            var dbAccount = new DbAccount();
            using (var context = new DatabaseContext())
            {
                if (context.Accounts != null && context.Accounts.Any())
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
                    Console.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) +
                                      " - Token Refreshed For User: " + uid);
                }
                else
                {
                    var dbAcc = new DbAccount { Token = token, Uid = uid };
                    context.Accounts.Add(dbAcc);
                    Console.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) +
                                      " - Account Created For User: " + uid);
                }
                context.SaveChanges();
            }
        }
    }
}
