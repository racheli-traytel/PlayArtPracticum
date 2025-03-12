namespace PlayArt.Api.Models
{
    public class PaintedDrawingPostModel
    {
        public int DrawingId { get; set; }
        public int UserId { get; set; }
        public string PaintedImageUrl { get; set; } = string.Empty;
    }
}
