namespace MiniHotel.Application.Exceptions
{
    public class BookingCreationException : Exception
    {
        public BookingCreationException(string message)
        : base(message)
        {
        }

        public BookingCreationException(string message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
