using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using TravelMateApi.Connection;
using TravelMateApi.Journey;
using TravelMateApi.Models;

namespace TravelMateApiTests
{
    [TestClass]
    public class SearchJourneyTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            const string startLocation = "KT48LJ";
            const string endLocation = "One New Change, London";

            var journey = new GJourneyBuilder().WithId(0).WithStart(startLocation)
                .WithEnd(endLocation).AddRoute("Get Northern Line to Bank", "Northern")
                .WithDisruptedLine(1, "Northern", "Severe Delayed on the Northern Line", JourneyStatus.Delayed).Build();

            var apiConnect = Substitute.For<IApiConnect>();
            var url = UrlFactory.GetGoogleJourneys(startLocation, endLocation);
            var journeyJson = JsonConvert.SerializeObject(journey);
            apiConnect.GetJson(url).Returns(journeyJson);

            
            var searchJourney = new SearchJourney(apiConnect, startLocation, endLocation);
            
            Assert.AreEqual(searchJourney.Search(), journey);
        }
    }
}
