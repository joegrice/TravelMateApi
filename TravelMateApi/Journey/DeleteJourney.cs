using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TravelMateApi.Database;
using TravelMateApi.Models;

namespace TravelMateApi.Journey
{
    public class DeleteJourney
    {
        private readonly string _id;

        public DeleteJourney(string id)
        {
            _id = id;
        }

        public void Delete()
        {
            var databaseFactory = new DatabaseFactory();
            var intId = int.Parse(_id);
            databaseFactory.DeleteJourney(intId);
        }
    }
}