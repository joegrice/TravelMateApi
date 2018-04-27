using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using TravelMateApi.Connection;
using TravelMateApi.Journey;
using TravelMateApi.Models;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TravelMateApiTests
{
    [TestClass]
    public class SearchJourneyTest
    {
        private string _startLocation;
        private string _endLocation;
        private GJourney _journey;
        private IApiConnect _apiConnect;

        private void SetUp()
        {
            _startLocation = "KT48LJ";
            _endLocation = "One New Change, London";

            _journey = new GJourneyBuilder().WithId(0).WithStart(_startLocation)
                .WithEnd(_endLocation).AddRoute("Get Northern Line to Bank", "Northern")
                .WithDisruptedLine(1, "Northern", "Severe Delayed on the Northern Line", JourneyStatus.Delayed).Build();

            _apiConnect = Substitute.For<IApiConnect>();
            var url = UrlFactory.GetGoogleJourneys(_startLocation, _endLocation);
            var journeyJson = JsonConvert.SerializeObject(_journey);
            _apiConnect.GetJson(url).Returns(journeyJson);
        }

        [TestMethod]
        public void TestJourneyValues()
        {
            SetUp();
            var searchJourney = new SearchJourney(_apiConnect, _startLocation, _endLocation);
            var searchResult = searchJourney.Search();

            Assert.AreEqual(searchResult.id, 0);
            Assert.AreEqual(searchResult.from, _startLocation);
            Assert.AreEqual(searchResult.to, _endLocation);
            Assert.AreEqual(searchResult.disruptedLines.Length, 1);
            Assert.AreEqual(searchResult.status, searchResult.disruptedLines[0].IsDelayed);
        }

        [TestMethod]
        public void TestRouteValues()
        {
            SetUp();
            var searchJourney = new SearchJourney(_apiConnect, _startLocation, _endLocation);
            var searchResult = searchJourney.Search();

            Assert.AreEqual(searchResult.routes.Length, 1);
            Assert.AreEqual(searchResult.routes[0].legs.Length, 1);
            Assert.AreEqual(searchResult.routes[0].legs[0].start_address, _startLocation);
            Assert.AreEqual(searchResult.routes[0].legs[0].end_address, _endLocation);
        }

        [TestMethod]
        public void TestStepValues()
        {
            SetUp();
            var searchJourney = new SearchJourney(_apiConnect, _startLocation, _endLocation);
            var searchResult = searchJourney.Search();

            Assert.AreEqual(searchResult.routes[0].legs[0].steps.Length, 1);
            Assert.AreEqual(searchResult.routes[0].legs[0].steps[0].html_instructions, "Get Northern Line to Bank");
            Assert.AreEqual(searchResult.routes[0].legs[0].steps[0].transit_details.line.name, "Northern");
            Assert.AreEqual(searchResult.routes[0].legs[0].steps[0].transit_details.line.short_name, "Northern" + "Short");
        }
    }
}