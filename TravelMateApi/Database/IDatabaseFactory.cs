using System;
using System.Collections.Generic;
using TravelMateApi.Models;

namespace TravelMateApi.Database
{
    public interface IDatabaseFactory
    {
        void SaveJourneyToDb(DbJourney dbJourney);
        void DeleteJourney(int id);
        DbJourney GetJourneyFromDb(DbJourney journey);
        void SaveLines(IEnumerable<DbLine> lines);
        void SaveJourneyLines(IEnumerable<DbJourneyLine> dbJourneyLines);
        IEnumerable<DbLine> GetLines();
        IEnumerable<string> GetLinesForJourneyId(int journeyId);
        IEnumerable<DbJourney> GetJourneysForUid(string uid);
        DbLine GetLine(string lineId);
        DbAccount GetAccountByUid(string uid);
        void RefreshToken(string uid, string token);
        List<Tuple<string, int, int, string>> GetDisruptionNotificationsDetails();
        List<DbLine> GetJourneyDelayedLines(int journeyId);
        void MarkUsersNotifiedForLine(int journeyId, int lineId);
        void UpdateLineDelayDetails(DbLine inputDbLine);
        void UpdateGoodStatusLineDelayDetails(IEnumerable<int> lineIds);
    }
}