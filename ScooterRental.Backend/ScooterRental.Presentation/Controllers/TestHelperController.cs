namespace ScooterRental.Presentation.Controllers
{
    public class TestPayloadRequest
    {
        public string SerialNumber { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    [Route("api/test")]
    public class TestHelperController : ApiController
    {
        [HttpPost("generate-payload")]
        public ActionResult GeneratePayload([FromBody] TestPayloadRequest request)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var data = new { batteryLevel = 90, latitude = request.Latitude, longitude = request.Longitude, alarm = false };

            var innerJson = JsonSerializer.Serialize(data);
            var stringToSign = $"{innerJson}.{timestamp}";

            var keyBytes = Encoding.UTF8.GetBytes(request.SecretKey);
            using var hmac = new HMACSHA256(keyBytes);
            var signature = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign))).ToLower();

            var finalPayload = new
            {
                SerialNumber = request.SerialNumber,
                Timestamp = timestamp,
                Signature = signature,
                Data = data
            };

            return Ok(finalPayload);
        }    
    }
}
