using PlayArt.Core.entities;
using PlayArt.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayArt.Core.Interfaces.IRepositories
{
   public interface IDrawingRepository:IRepository<Drawing>
    {
        List<Drawing> GetDrawingCategory(DrawingCategory? category = null);
    }
}
