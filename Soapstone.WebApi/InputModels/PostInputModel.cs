using System;
using Microsoft.AspNetCore.Http;
using Soapstone.Domain;

namespace Soapstone.WebApi.InputModels
{
    public class PostInputModel
    {
        public string Message { get; set; }
        public string ImageUrl { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}