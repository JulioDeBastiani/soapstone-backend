namespace Soapstone.WebApi.InputModels
{
    public class PostsPageInputModel
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}