using DTO.Enums;

namespace DTO.UserDtos
{
    public class UserPublicModelDto
    {
        public string Name { get; set; }
        public DateOnly BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
