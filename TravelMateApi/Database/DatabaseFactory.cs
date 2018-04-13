using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TravelMateApi.Journey;
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
                context.Journeys.AddIfNotExists(dbJourney,
                    x => !x.AccountId.Equals(dbJourney.AccountId) && !x.Route.Equals(dbJourney.Route));
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
                        => dbjourneyLine.JourneyId.Equals(journeyLine.JourneyId) &&
                           dbjourneyLine.ModeId.Equals(journeyLine.ModeId));
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

        public IEnumerable<string> GetLinesForJourneyId(int journeyId)
        {
            var lineNames = new List<string>();
            using (var context = new DatabaseContext())
            {
                var lineName = from journeyLine in context.JourneyLines
                    join line in context.Lines on journeyLine.ModeId equals line.Id
                    where journeyLine.JourneyId.Equals(journeyId)
                    select line.Name;
                lineNames.AddRange(lineName);
            }

            return lineNames;
        }

        public IEnumerable<DbJourney> GetJourneysForUid(string uid)
        {
            var allDbJourneys = new List<DbJourney>();
            using (var context = new DatabaseContext())
            {
                var dbJourneys = from dbJourney in context.Journeys
                    join account in context.Accounts on dbJourney.AccountId equals account.Id
                    where account.Uid.Equals(uid)
                    select dbJourney;
                allDbJourneys.AddRange(dbJourneys);
            }

            return allDbJourneys;
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
                    var dbAcc = new DbAccount {Token = token, Uid = uid};
                    context.Accounts.Add(dbAcc);
                    Console.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) +
                                      " - Account Created For User: " + uid);
                }

                context.SaveChanges();
            }
        }

        public List<Tuple<string, int, string>> GetDisruptionNotificationsDetails()
        {
            var accountsList = new List<Tuple<string, int, string>>();
            using (var context = new DatabaseContext())
            {
                var accounts = from dbLine in context.Lines
                    join dbJourneyLine in context.JourneyLines on dbLine.Id equals dbJourneyLine.ModeId
                    join dbJourney in context.Journeys on dbJourneyLine.JourneyId equals dbJourney.Id
                    join dbAccount in context.Accounts on dbJourney.AccountId equals dbAccount.Id
                    where dbLine.IsDelayed.Equals(JourneyStatus.Delayed) && dbLine.UsersNotified.Equals(false.ToString()) &&
                          TimeSpan.Compare(DateTime.Now.TimeOfDay, DateTime.ParseExact(dbJourney.Time, "HH:mm",
                              CultureInfo.InvariantCulture).TimeOfDay) >= 0 &&
                          TimeSpan.Compare(DateTime.Now.TimeOfDay, DateTime.ParseExact(dbJourney.Time, "HH:mm",
                                  CultureInfo.InvariantCulture)
                              .AddMinutes(double.Parse(dbJourney.Period)).TimeOfDay) <= 0
                    select Tuple.Create(dbAccount.Token, dbLine.Id, dbLine.Description);
                accountsList.AddRange(accounts);
            }

            return accountsList;
        }

        public List<DbLine> GetJourneyDelayedLines(int journeyId)
        {
            var lines = new List<DbLine>();
            using (var context = new DatabaseContext())
            {
                var delayed = from dbJourney in context.Journeys
                    join dbJourneyLine in context.JourneyLines on dbJourney.Id equals dbJourneyLine.JourneyId
                    join dbLine in context.Lines on dbJourneyLine.ModeId equals dbLine.Id
                    where dbJourney.Id == journeyId && dbLine.IsDelayed.Equals(JourneyStatus.Delayed)
                    select dbLine;
                lines.AddRange(delayed);
            }

            return lines;
        }

        public void MarkUsersNotifiedForLine(int lineId)
        {
            using (var context = new DatabaseContext())
            {
                var dbLine = context.Lines.FirstOrDefault(line => line.Id == lineId);
                if (dbLine != null)
                {
                    dbLine.UsersNotified = true.ToString();
                    context.SaveChanges();
                }
            }
        }

        public void UpdateLineDelayDetails(DbLine inputDbLine)
        {
            using (var context = new DatabaseContext())
            {
                var result = context.Lines.FirstOrDefault(dbLine => dbLine.Id == inputDbLine.Id);
                if (result != null && result.IsDelayed != inputDbLine.IsDelayed)
                {
                    result.Description = inputDbLine.Description;
                    result.IsDelayed = inputDbLine.IsDelayed;
                    result.UsersNotified = false.ToString();
                }

                context.SaveChanges();
            }
        }

        public void UpdateGoodStatusLineDelayDetails(IEnumerable<int> lineIds)
        {
            using (var context = new DatabaseContext())
            {
                foreach (var line in context.Lines)
                {
                    if (lineIds.Contains(line.Id)) continue;
                    line.Description = "";
                    line.IsDelayed = JourneyStatus.GoodService;
                    line.UsersNotified = false.ToString();
                }

                context.SaveChanges();
            }
        }
    }
}