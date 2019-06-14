namespace Soapstone.WebApi.InputModels
{
    public class ChangePasswordInputModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}