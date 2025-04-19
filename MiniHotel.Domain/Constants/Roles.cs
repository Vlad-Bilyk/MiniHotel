namespace MiniHotel.Domain.Constants
{
    /// <summary>
    /// Defines role names used across the application for authorization.
    /// </summary>
    public static class Roles
    {
        public const string Client = "Client";
        public const string Receptionist = "Receptionist";
        public const string Manager = "Manager";

        public const string AdminRoles = Manager + "," + Receptionist;
    }
}
