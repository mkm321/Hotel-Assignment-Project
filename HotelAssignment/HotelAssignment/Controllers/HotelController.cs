using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HotelAssignment.Model;
namespace HotelAssignment.Controllers
{
    [Produces("application/json")]
    [Route("api/Hotel")]

    public class HotelController : Controller
    {
        private static List<Hotel> _HotelList = new List<Hotel>(new[]{
            new Hotel(){id=1,name="Hyatt",numberOfAvailableRooms=5,address="viman nagar",cityCode=414001},
            new Hotel(){id=2,name="Novotel",numberOfAvailableRooms=5,address="viman nagar",cityCode=414001},
            new Hotel(){id=3,name="Hyatt Regency",numberOfAvailableRooms=5,address="viman nagar",cityCode=414001},
            new Hotel(){id=4,name="Hyatt Regency",numberOfAvailableRooms=5,address="viman nagar",cityCode=414001},
            new Hotel(){id=5,name="Hyatt Regency",numberOfAvailableRooms=5,address="viman nagar",cityCode=414001}
        });
        [HttpGet]
        public List<Hotel> Get()
        {
            return _HotelList;
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Show showObj = new Show();
            try
            {
               
                var hotel = _HotelList.SingleOrDefault(_HotelList => _HotelList.id == id);
                if (hotel != null)
                {
                    showObj.hotelObj = hotel;
                    showObj.responseObj.statusCode = 200;
                    showObj.responseObj.status = "Success";
                    showObj.responseObj.statusMessage = "Your request is successful";
                    return Ok(showObj);
                }
                else
                {
                    throw new Exception("File not found");
                }
            }
            catch(Exception e)
            {
                showObj.hotelObj = null;
                showObj.responseObj.statusCode = 404;
                showObj.responseObj.statusMessage = e.Message;
                showObj.responseObj.status = "Failure";
                return Ok(showObj);
            }
        }
        [HttpPost]
        public IActionResult Post([FromBody] Hotel hotel)
        {
            Show showObj = new Show();
            try
            {
                _HotelList.Add(hotel);
                showObj.hotelObj = hotel;
                showObj.responseObj.status = "Success";
                showObj.responseObj.statusCode = 200;
                showObj.responseObj.statusMessage = "New Hotel Added Successfully";
                return Ok(showObj);
            }
            catch(Exception e)
            {
                showObj.hotelObj = null;
                showObj.responseObj.statusCode = 500;
                showObj.responseObj.statusMessage = e.Message;
                showObj.responseObj.status = "Failure";
                return Ok(showObj);
            }
        }
        [HttpPut("{id}")]

        public IActionResult Put(int id, [FromBody] int rooms)
        {
            Show showObj = new Show();
            try
            {
                int index = _HotelList.FindIndex(_HotelList => _HotelList.id == id);
                if(index == -1)
                {
                    throw new Exception("ID Not Found");
                }
                if (_HotelList[index].numberOfAvailableRooms < rooms)
                {
                    throw new Exception("Not Enough Rooms");
                }
                _HotelList[index].numberOfAvailableRooms -= rooms;
                showObj.hotelObj = _HotelList[index];
                showObj.responseObj.status = "Success";
                showObj.responseObj.statusCode = 200;
                showObj.responseObj.statusMessage = "Booked";
                return Ok(showObj);
            }
            catch(Exception e)
            {
                showObj.hotelObj = null;
                showObj.responseObj.statusCode = 404;
                showObj.responseObj.statusMessage = e.Message;
                showObj.responseObj.status = "Failure";
                return Ok(showObj);
            }
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Show showObj = new Show();
            try
            {
                int index = _HotelList.FindIndex(_HotelList => _HotelList.id == id);
                if (index == -1)
                {
                    throw new Exception("File not Found");
                }
                var hotel = _HotelList.ElementAt(index);
                _HotelList.RemoveAt(index);
                showObj.hotelObj = hotel;
                showObj.responseObj.status = "Success";
                showObj.responseObj.statusCode = 200;
                showObj.responseObj.statusMessage = "Deleted";
                return Ok(showObj);
            }
            catch(Exception e)
            {
                showObj.hotelObj = null;
                showObj.responseObj.statusCode = 404;
                showObj.responseObj.statusMessage = e.Message;
                showObj.responseObj.status = "Failure";
                return Ok(showObj);
            }
            
        }
        [HttpPost]
        [Route(api / hotel)]
        public List<string> HotelsFromGoogleApi([FromBody] string location)
        {
            var hotels = new List<string>();
            using (var client = new WebClient())
            {
                string url = "https://maps.googleapis.com/maps/api/place/autocomplete/json?input=" + city + "&types=geocode&language=fr&key=AIzaSyBHQlon-Trj64v6ZNjc2UteDu6kVBSIJQA";
                var jsonPrediction = client.DownloadString(url);
                var jsonObjectPrediction = JsonConvert.DeserializeObject<ListRootObject>(jsonPrediction);
                var result = jsonObjectPrediction.search.Select(c => c.details).ToList();
                
                var locationService = new GoogleLocationService();
                var point = locationService.GetLatLongFromAddress(result[0]);
                var latitude = point.Latitude;
                var longitude = point.Longitude;

                //Find Hotels in 20Km Radius
                string uri = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + 18.5204 + "," + 73.8567 + "&radius=20000&type=hotel&&key=AIzaSyBHQlon-Trj64v6ZNjc2UteDu6kVBSIJQA";
                var json = client.DownloadString(uri).ToString();
                var jsonObject = JsonConvert.DeserializeObject<ListHotelscs>(json);
                hotels = jsonObject.results.Select(h => h.Name).ToList();

            }
            return hotels;
        }

    }
}