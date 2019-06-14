using Soapstone.Domain;

namespace Soapstone.WebApi.InputModels
{
    public class UserInputModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public static implicit operator User(UserInputModel inputModel)
            => new User(inputModel.Username, inputModel.Email, inputModel.Password);
    }
}