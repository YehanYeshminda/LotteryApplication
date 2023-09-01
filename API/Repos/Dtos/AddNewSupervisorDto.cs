namespace API.Repos.Dtos
{
    public class AssignSupervisorDto
    {
        public int AssignedIntoUser { get; set; }
        public int SupervisorId { get; set; }
        public string Coupon { get; set; }
        public AuthDto AuthDto { get; set; }
    }
}
