using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayArt.Core.DTOs
{
    public class PaintedDrawingDTO
    {
        public int Id { get; set; }
        public int DrawingId { get; set; }
        public int UserId { get; set; }
        public string PaintedImageUrl { get; set; }
        public DateTime PaintedAt { get; set; }
    }
}
