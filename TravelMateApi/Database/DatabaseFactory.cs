using System.Collections.Generic;
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
    }
}
