using System;
using Soapstone.Domain;

namespace Soapstone.WebApi.InputModels
{
    public class PostInputModel
    {
        public Guid UserId { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public static implicit operator Post(PostInputModel inputModel)
        {
            return new Post(inputModel.UserId, inputModel.Message, inputModel.ImageUrl, inputModel.Latitude, inputModel.Longitude);
        }
    }
}